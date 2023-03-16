using System.Collections.Generic;
using UnityEngine;
namespace TWorld
{
    public class TerrainChunk
    {
        public GameObject gameObject;
        public int X { get; private set; }
        public int Z { get; private set; }
        public Terrain Terrain { get; set; }


        public TerrainChunk Left { get; private set; }
        public TerrainChunk Top { get; private set; }
        public TerrainChunk Right { get; private set; }
        public TerrainChunk Bottom { get; private set; }


        public Dictionary<HeightStamp, TerrainFlushData> stampData = new Dictionary<HeightStamp, TerrainFlushData>();

        public TerrainChunk() { }
        public TerrainChunk(int x, int z)
        {
            X = x;
            Z = z;
        }

        public void SetNeighbors(TerrainChunk left = null, TerrainChunk top = null, TerrainChunk right = null, TerrainChunk bottom = null)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            Terrain.SetNeighbors(left?.Terrain, top?.Terrain, right?.Terrain, bottom?.Terrain);
        }

        public Terrain CreateTerrain(TerrainChunkSettings Settings)
        {
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = Settings.HeightmapResolution;
            terrainData.alphamapResolution = Settings.AlphamapResolution;
            terrainData.size = new Vector3(Settings.Width, Settings.Height, Settings.Width);
            gameObject = Terrain.CreateTerrainGameObject(terrainData);
            gameObject.transform.position = new Vector3(X * Settings.Width, 0, Z * Settings.Width);
            Terrain = gameObject.GetComponent<Terrain>();
            return Terrain;
        }

        public TerrainFlushData SetStampData(HeightStamp stamp, int xBase, int yBase, float[,] height)
        {
            var data = new TerrainFlushData(xBase, yBase, height);
            stampData[stamp] = data;
            return data;
        }

        public void Flush()
        {
            var source = new float[TerrainWorld.Ins.HeightmapResolution, TerrainWorld.Ins.HeightmapResolution];
            foreach (var kv in stampData)
            {
                var stamp = kv.Key;
                var flushData = kv.Value;
                var heights = flushData.heights;
                var length_0 = heights.GetLength(0);
                var length_1 = heights.GetLength(1);
                for (int i = 0; i < length_1; i++)
                {
                    for (int j = 0; j < length_0; j++)
                    {
                        source[flushData.yBase + j, flushData.xBase + i] = Mathf.Max(source[flushData.yBase + j, flushData.xBase + i], heights[j, i]);
                    }
                }
            }
            Terrain.terrainData.SetHeightsDelayLOD(0, 0, source);
            Terrain.terrainData.SyncHeightmap();
        }
    }

    public class TerrainFlushData
    {
        public int xBase;
        public int yBase;
        public float[,] heights;

        public TerrainFlushData(int xBase, int yBase, float[,] heights)
        {
            this.xBase = xBase;
            this.yBase = yBase;
            this.heights = heights;
        }
    }

    public class TerrainChunkSettings
    {
        public int HeightmapResolution { get; private set; }
        public int AlphamapResolution { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Length { get; private set; }
        public TerrainChunkSettings() { }

        public TerrainChunkSettings(int heightmapResolution, int alphamapResolution, int width, int height, int length)
        {
            HeightmapResolution = heightmapResolution;
            AlphamapResolution = alphamapResolution;
            Width = width;
            Height = height;
            Length = length;
        }
    }
}