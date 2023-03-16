using UnityEngine;
using System.Collections.Generic;
using TWorld.Utility;

namespace TWorld
{
    [ExecuteAlways]
    public class TerrainWorld : MonoBehaviour
    {
        private static TerrainWorld _Ins;
        public static TerrainWorld Ins
        {
            get
            {
                if (_Ins == null)
                {
                    _Ins = GameObject.FindObjectOfType<TerrainWorld>();
                }
                return _Ins;
            }
        }

        public int SizeX;
        public int SizeZ;

        public TerrainChunk[,] chunks;

        [HideInInspector] public int HeightmapResolution, AlphamapResolution, Width, Length, Height;

        public bool IsInit { get; private set; }
        public GameObject Terrains { get; private set; }
        public GameObject Stamps { get; private set; }
        public float ResolutionSizeScale { get; private set; }

        private bool NeedFlush { get; set; }

        private List<TerrainChunk> needFlushChunks = new List<TerrainChunk>();

        public void Flush()
        {
            NeedFlush = true;
        }


        private void OnEnable()
        {
            Terrains = transform.Find("Terrains").gameObject;
            Stamps = transform.Find("Stamps").gameObject;
            CreateTerrains();
        }

        private void LateUpdate()
        {
            if (NeedFlush)
                FlushTerrainHeight();
        }

        public void CreateTerrains()
        {
            ClearTerrains();
            ResolutionSizeScale = Width * 1f / (HeightmapResolution - 1);
            chunks = new TerrainChunk[SizeX, SizeZ];
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeZ; j++)
                {
                    chunks[i, j] = CreateTerrain(i, j);
                }
            }
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeZ; j++)
                {
                    TerrainChunk left = i == 0 ? null : chunks[i - 1, j];
                    TerrainChunk top = j == SizeZ - 1 ? null : chunks[i, j + 1];
                    TerrainChunk right = i == SizeX - 1 ? null : chunks[i + 1, j];
                    TerrainChunk bottom = j == 0 ? null : chunks[i, j - 1];
                    chunks[i, j].SetNeighbors(left, top, right, bottom);
                }
            }
            IsInit = true;
            FlushTerrainHeight(true);
        }

        public void ClearTerrains()
        {
            while (Terrains.transform.childCount != 0)
            {
                GameObject.DestroyImmediate(Terrains.transform.GetChild(0).gameObject);
            }
            needFlushChunks.Clear();
            var stamps = Stamps.GetComponentsInChildren<HeightStamp>();
            foreach (var stamp in stamps)
            {
                stamp.affectChunks.Clear();
            }
            IsInit = false;
        }

        private TerrainChunk CreateTerrain(int x, int z)
        {
            var settings = new TerrainChunkSettings(HeightmapResolution, AlphamapResolution, Width, Height, Length);
            var terrainChunk = new TerrainChunk(x, z);
            terrainChunk.CreateTerrain(settings);
            terrainChunk.gameObject.name = x + "_" + z;
            terrainChunk.gameObject.transform.SetParent(Terrains.transform);
            return terrainChunk;
        }

        private void FlushTerrainHeight(bool forceFlush = false)
        {
            if (!IsInit)
            {
                return;
            }
            needFlushChunks.Clear();
            var stamps = Stamps.GetComponentsInChildren<HeightStamp>();
            foreach (var stamp in stamps)
            {
                if (!forceFlush && !stamp.IsChange) continue;
                foreach (var affectChunk in stamp.affectChunks)
                {
                    affectChunk.stampData.Remove(stamp);
                }
                needFlushChunks.AddRange(stamp.affectChunks);
                stamp.affectChunks.Clear();
                var pos = stamp.GetMinMaxPos();
                var data = stamp.GetData();
                if (data == null)
                {
                    continue;
                }
                FillData(stamp, (int)(pos.Item1.x / ResolutionSizeScale), (int)(pos.Item1.z / ResolutionSizeScale), data, ref stamp.affectChunks);
                stamp.IsChange = false;
            }
            foreach (var chunk in needFlushChunks)
            {
                chunk.Flush();
            }
        }

        private void FillData(HeightStamp stamp, int xBase, int yBase, float[,] heights, ref List<TerrainChunk> affectChunks)
        {
            var heightmapSize = HeightmapResolution - 1;
            var xIndex = xBase / heightmapSize;
            var yIndex = yBase / heightmapSize;
            //索引起点超出右上, 直接返回
            if (xIndex >= SizeX || yIndex >= SizeZ)
            {
                return;
            }
            //索引起点在地形左下
            if (xBase < 0 || yBase < 0)
            {
                var length_0 = heights.GetLength(0);
                var length_1 = heights.GetLength(1);
                //超出范围
                if (-yBase >= length_0 || -xBase >= length_1)
                {
                    return;
                }
                var newxBase = 0;
                var newyBase = 0;
                if (xBase < 0)
                {
                    newxBase = -xBase;
                    xBase = 0;
                }
                if (yBase < 0)
                {
                    newyBase = -yBase;
                    yBase = 0;
                }
                var newLength_0 = length_0 - newyBase;
                var newLength_1 = length_1 - newxBase;
                heights = heights.GetPart(newyBase, newxBase, newLength_0, newLength_1);
                xIndex = xBase / heightmapSize;
                yIndex = yBase / heightmapSize;

            }
            SetHeightMap(chunks[xIndex, yIndex], stamp, heights, xBase % heightmapSize, yBase % heightmapSize, ref affectChunks);
        }

        /// <summary>
        /// 跨多块地形设置Terrain的HeightMap
        /// </summary>
        /// <param name="terrain">目标地形</param>
        /// <param name="heights">高度图</param>
        /// <param name="xBase">X起点</param>
        /// <param name="yBase">Y起点</param>
        private void SetHeightMap(TerrainChunk chunk, HeightStamp stamp, float[,] heights, int xBase, int yBase, ref List<TerrainChunk> affectChunks)
        {
            if (chunk?.Terrain == null)
            {
                return;
            }
            TerrainData terrainData = chunk.Terrain.terrainData;
            int length_1 = heights.GetLength(1);
            int length_0 = heights.GetLength(0);

            int differX = xBase + length_1 - (terrainData.heightmapResolution - 1);
            int differY = yBase + length_0 - (terrainData.heightmapResolution - 1);

            affectChunks.Add(chunk);
            // 根据溢出情况对数据做处理
            if (differX <= 0 && differY <= 0) // 无溢出
            {
                SetSingleHeightMap(chunk, stamp, heights, xBase, yBase);
            }
            else if (differX > 0 && differY <= 0) // 右溢出
            {
                SetSingleHeightMap(chunk, stamp, heights.GetPart(0, 0, length_0, length_1 - differX + 1), xBase, yBase);  // 最后的 +1是为了和右边的地图拼接

                SetHeightMap(chunk.Right, stamp, heights.GetPart(0, length_1 - differX, length_0, differX), 0, yBase, ref affectChunks);
            }
            else if (differX <= 0 && differY > 0) // 上溢出
            {
                SetSingleHeightMap(chunk, stamp, heights.GetPart(0, 0, length_0 - differY + 1, length_1), xBase, yBase);  // 最后的 +1是为了和上边的地图拼接

                SetHeightMap(chunk.Top, stamp, heights.GetPart(length_0 - differY, 0, differY, length_1), xBase, 0, ref affectChunks);
            }
            else  // 右上均溢出
            {
                SetSingleHeightMap(chunk, stamp, heights.GetPart(0, 0, length_0 - differY + 1, length_1 - differX + 1), xBase, yBase);  // 最后的 +1是为了和上边及右边的地图拼接

                SetHeightMap(chunk.Right, stamp, heights.GetPart(0, length_1 - differX, length_0 - differY + 1, differX), 0, yBase, ref affectChunks);
                SetHeightMap(chunk.Top, stamp, heights.GetPart(length_0 - differY, 0, differY, length_1 - differX + 1), xBase, 0, ref affectChunks);
                SetHeightMap(chunk.Top?.Right, stamp, heights.GetPart(length_0 - differY, length_1 - differX, differY, differX), 0, 0, ref affectChunks);
            }
        }

        /// <summary>
        /// 设置单块地图的高度图
        /// </summary>
        private void SetSingleHeightMap(TerrainChunk chunk, HeightStamp stamp, float[,] heights, int xBase, int yBase)
        {
            chunk.SetStampData(stamp, xBase, yBase, heights);
            needFlushChunks.Add(chunk);
        }
    }

}

// /// <summary>
// /// 跨多块地形返回Terrain的HeightMap的一部分
// /// </summary>
// /// <param name="terrain">目标地形</param>
// /// <param name="xBase">检索HeightMap时的X索引起点</param>
// /// <param name="yBase">检索HeightMap时的Y索引起点</param>
// /// <param name="width">在X轴上的检索长度</param>
// /// <param name="height">在Y轴上的检索长度</param>
// /// <returns></returns>
// private float[,] GetHeightMap(TerrainChunk chunk, int xBase = 0, int yBase = 0, int width = 0, int height = 0)
// {
//     // 如果后四个均为默认参数，则直接返回当前地形的整个高度图
//     if (xBase + yBase + width + height == 0)
//     {
//         return chunk.Terrain.terrainData.GetHeights(xBase, yBase, HeightmapResolution, HeightmapResolution);
//     }

//     TerrainData terrainData = chunk.Terrain.terrainData;
//     int differX = xBase + width - (terrainData.heightmapResolution - 1);   // 右溢出量级
//     int differY = yBase + height - (terrainData.heightmapResolution - 1);  // 上溢出量级

//     // 根据数据溢出情况做处理
//     float[,] ret;
//     if (differX <= 0 && differY <= 0)  // 无溢出
//     {
//         ret = chunk.Terrain.terrainData.GetHeights(xBase, yBase, width, height);
//     }
//     else if (differX > 0 && differY <= 0) // 右边溢出
//     {
//         ret = chunk.Terrain.terrainData.GetHeights(xBase, yBase, width - differX, height);
//         float[,] right = chunk.Terrain.Right()?.terrainData.GetHeights(0, yBase, differX, height);
//         if (right != null)
//             ret = ret.Concat1(right);
//     }
//     else if (differX <= 0 && differY > 0)  // 上边溢出
//     {
//         ret = chunk.Terrain.terrainData.GetHeights(xBase, yBase, width, height - differY);
//         float[,] up = chunk.Terrain.Top()?.terrainData.GetHeights(xBase, 0, width, differY);
//         if (up != null)
//             ret = ret.Concat0(up);
//     }
//     else // 上右均溢出
//     {
//         ret = chunk.Terrain.terrainData.GetHeights(xBase, yBase, width - differX, height - differY);

//         float[,] right = chunk.Terrain.Right()?.terrainData.GetHeights(0, yBase, differX, height - differY);
//         float[,] up = chunk.Terrain.Top()?.terrainData.GetHeights(xBase, 0, width - differX, differY);
//         float[,] upRight = chunk.Terrain.Right()?.Top()?.terrainData.GetHeights(0, 0, differX, differY);

//         if (right != null)
//             ret = ret.Concat1(right);
//         if (upRight != null)
//             ret = ret.Concat0(up.Concat1(upRight));
//     }

//     return ret;
// }
