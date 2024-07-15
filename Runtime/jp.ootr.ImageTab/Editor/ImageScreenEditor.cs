using jp.ootr.common;
using jp.ootr.ImageTab.ImageScreen;
using UnityEditor;

namespace jp.ootr.imagetab.Editor
{
    [CustomEditor(typeof(ImageScreen))]
    public class ImageScreenEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (ImageScreen)target;

            EditorGUILayout.LabelField("ImageScreen", EditorStyle.UiTitle);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Device Name");
            script.deviceName = EditorGUILayout.TextField(script.deviceName);

            EditorUtility.SetDirty(script);
        }
    }
}