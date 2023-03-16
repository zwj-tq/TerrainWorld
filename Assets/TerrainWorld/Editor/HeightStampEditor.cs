using UnityEngine;
using UnityEditor;
namespace TWorld.Editor
{

    [CustomEditor(typeof(HeightStamp))]
    public class HeightStampEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/Terrain World/Height Stamp")]
        public static void CreateHeightStamp()
        {
            GameObject go = new GameObject();
            go.name = "Stamp";
            go.AddComponent<HeightStamp>();
            go.transform.SetParent(TerrainWorld.Ins.Stamps.transform);
            Selection.activeGameObject = go;
        }

        public new HeightStamp target;
        private void OnEnable()
        {
            target = base.target as HeightStamp;

        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        public override bool HasPreviewGUI()
        {
            return target.heightStamp != null;
        }
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            GUI.Box(r, target.dataTexture2D);
        }

        private void OnSceneGUI()
        {
            var item = target.GetMinMaxPos();
            Handles.DrawPolyLine(item.Item1, new Vector3(item.Item1.x, 0, item.Item2.z), item.Item2, new Vector3(item.Item2.x, 0, item.Item1.z), item.Item1);
        }

    }
}