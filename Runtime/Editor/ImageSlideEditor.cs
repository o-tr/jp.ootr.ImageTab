using jp.ootr.common;
using UnityEditor;

namespace jp.ootr.ImageTab.Editor
{
    [CustomEditor(typeof(ImageSlide.ImageSlide))]
    public class ImageSlideEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var script = (ImageSlide.ImageSlide)target;

            EditorGUILayout.LabelField("ImageSlide", EditorStyle.UiTitle);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Device Name");
            script.deviceName = EditorGUILayout.TextField(script.deviceName);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Watch Interval");
            script.ARWatchInterval = EditorGUILayout.Slider(script.ARWatchInterval, 0.01f, 1f);

            EditorUtility.SetDirty(script);
        }
    }
}