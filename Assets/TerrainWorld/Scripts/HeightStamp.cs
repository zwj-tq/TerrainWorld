using System.Collections.Generic;
using UnityEngine;
namespace TWorld
{
    [ExecuteAlways]
    public class HeightStamp : MonoBehaviour
    {

        public Texture2D heightStamp;


        private Vector3 lastScale;
        private Vector3 lastPosition;

        public float Width { get; set; }
        public float Height { get; set; }
        public float Length { get; set; }
        public bool IsChange { get; set; }

        [ReadOnly] public Texture2D dataTexture2D;
        [HideInInspector] public List<TerrainChunk> affectChunks = new List<TerrainChunk>();

        void Update()
        {
            if (heightStamp == null) return;
            UpdateData();
            UpdateTexture();
        }

        public void UpdateData()
        {
            transform.localScale = Vector3.Max(transform.localScale, Vector3.zero);
            var scale = TerrainWorld.Ins.ResolutionSizeScale;
            Width = transform.localScale.x / scale;
            Height = transform.localScale.y;
            Length = transform.localScale.z / scale;
        }

        private void UpdateTexture()
        {
            if (dataTexture2D == null)
            {
                dataTexture2D = new Texture2D(0, 0);
                GetScaleTexture();
            }
            if (transform.localScale != lastScale)
            {
                GetScaleTexture();
                TerrainWorld.Ins.Flush();
                IsChange = true;
                lastScale = transform.localScale;
            }
            if (transform.position != lastPosition)
            {
                TerrainWorld.Ins.Flush();
                IsChange = true;
                lastPosition = transform.position;
            }
        }

        public (Vector3, Vector3) GetMinMaxPos()
        {
            var scaleRadius = transform.localScale / 2;
            scaleRadius.y = 0;
            return (transform.position - scaleRadius, transform.position + scaleRadius);
        }

        public float[,] GetData()
        {
            if (dataTexture2D == null)
            {
                return null;
            }
            int width = dataTexture2D.width;
            int length = dataTexture2D.height;
            var arr = new float[length, width];
            if (width == 0 || length == 0)
            {
                return arr;
            }
            var colors = dataTexture2D.GetPixels();
            var heightScale = Height / TerrainWorld.Ins.Height;
            for (int i = 0, index = 0; i < length; i++)
            {
                index = i * width;
                for (int j = 0; j < width; j++)
                {
                    arr[i, j] = colors[index + j].r * heightScale;
                }
            }
            return arr;
        }

        public Texture2D GetScaleTexture()
        {
            var width = (int)Width;
            var length = (int)Length;

            dataTexture2D.Resize(width, length, TextureFormat.ARGB32, false);
            if (width == 0 || length == 0) return dataTexture2D;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    dataTexture2D.SetPixel(i, j, heightStamp.GetPixelBilinear((float)i / width, (float)j / length));
                }
            }
            dataTexture2D.Apply();
            return dataTexture2D;
        }

    }
}