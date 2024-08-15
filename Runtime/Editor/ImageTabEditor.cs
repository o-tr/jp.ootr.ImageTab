using System;
using jp.ootr.common;
using jp.ootr.ImageDeviceController.Editor;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace jp.ootr.ImageTab.Editor
{
    [CustomEditor(typeof(ImageTab.ImageTab))]
    public class ImageTabEditor : CommonDeviceEditor
    {
        private SerializedProperty _arWatchInterval;

        public override void OnEnable()
        {
            base.OnEnable();
            _arWatchInterval = serializedObject.FindProperty("ARWatchInterval");
        }

        protected override void ShowContent()
        {
            EditorGUILayout.Space();
            var so = new SerializedObject(target);
            EditorGUILayout.PropertyField(_arWatchInterval, new GUIContent("Rotation Check Interval"));
            so.ApplyModifiedProperties();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            BuildBookmark((ImageTab.ImageTab)target);
            if (!EditorGUI.EndChangeCheck()) return;
            EditorUtility.SetDirty(target);
        }

        protected override void ShowScriptName()
        {
            EditorGUILayout.LabelField("ImageTab", EditorStyle.UiTitle);
        }

        private void BuildBookmark(ImageTab.ImageTab script)
        {
            var newSize = Mathf.Max(script.uIBookmarkNames.Length, script.uIBookmarkUrls.Length);
            if (script.uIBookmarkNames.Length != newSize || script.uIBookmarkUrls.Length != newSize)
            {
                Array.Resize(ref script.uIBookmarkNames, newSize);
                Array.Resize(ref script.uIBookmarkUrls, newSize);
            }

            EditorGUILayout.LabelField("Bookmarks", EditorStyles.boldLabel);

            for (var i = 0; i < newSize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                script.uIBookmarkNames[i] = EditorGUILayout.TextField(script.uIBookmarkNames[i]);
                script.uIBookmarkUrls[i] = new VRCUrl(EditorGUILayout.TextField(script.uIBookmarkUrls[i].ToString()));

                if (GUILayout.Button("Remove"))
                {
                    newSize--;
                    var tmpNames = new string[newSize];
                    var tmpUrls = new VRCUrl[newSize];
                    Array.Copy(script.uIBookmarkNames, tmpNames, i);
                    Array.Copy(script.uIBookmarkNames, i + 1, tmpNames, i, newSize - i);
                    Array.Copy(script.uIBookmarkUrls, tmpUrls, i);
                    Array.Copy(script.uIBookmarkUrls, i + 1, tmpUrls, i, newSize - i);
                    script.uIBookmarkNames = tmpNames;
                    script.uIBookmarkUrls = tmpUrls;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Element"))
            {
                newSize++;
                Array.Resize(ref script.uIBookmarkNames, newSize);
                Array.Resize(ref script.uIBookmarkUrls, newSize);
            }

            if (GUILayout.Button("Remove Last Element"))
                if (newSize > 0)
                {
                    newSize--;
                    Array.Resize(ref script.uIBookmarkNames, newSize);
                    Array.Resize(ref script.uIBookmarkUrls, newSize);
                }

            EditorGUILayout.EndHorizontal();
        }
    }
}