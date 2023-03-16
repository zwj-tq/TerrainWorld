using UnityEngine;
using UnityEditor;
namespace TWorld.Editor
{
    [CustomEditor(typeof(TerrainWorld))]
    public class TerrainWorldEditor : UnityEditor.Editor
    {
        private new TerrainWorld target;
        private void OnEnable()
        {
            target = base.target as TerrainWorld;
        }
        private string[] displayResolutions = new string[] { "65", "129", "257", "513", "1025", "2049", "4097", "8193" };
        private int[] valueResolutions = new int[] { 65, 129, 257, 513, 1025, 2049, 4097, 8193 };
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Height & Alpha MapResolution");
            target.HeightmapResolution = target.AlphamapResolution = EditorGUILayout.IntPopup(target.HeightmapResolution, displayResolutions, valueResolutions);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Width X Height Size");
            target.Height = target.Length = target.Width = EditorGUILayout.IntField(target.Width);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("创建terrain"))
            {
                target.CreateTerrains();
            }
            if (GUILayout.Button("清除terrain"))
            {
                target.ClearTerrains();
            }
        }

    }
}