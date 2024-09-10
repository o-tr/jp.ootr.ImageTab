#if UNITY_EDITOR
using System;
using jp.ootr.common;
using jp.ootr.ImageDeviceController.Editor;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace jp.ootr.ImageTab.Editor
{
    [CustomEditor(typeof(ImageTab))]
    public class ImageTabEditor : CommonDeviceEditor
    {
        private SerializedProperty _arWatchInterval;
        private SerializedProperty _uiBookmarkNames;
        private SerializedProperty _uiBookmarkUrls;
        private SerializedProperty _uiHistoryDisabled;

        public override void OnEnable()
        {
            base.OnEnable();
            _arWatchInterval = serializedObject.FindProperty("arWatchInterval");
            _uiBookmarkNames = serializedObject.FindProperty("uIBookmarkNames");
            _uiBookmarkUrls = serializedObject.FindProperty("uIBookmarkUrls");
            _uiHistoryDisabled = serializedObject.FindProperty("uIHistoryDisabled");
        }

        protected override void ShowContent()
        {
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_arWatchInterval, new GUIContent("Rotation Check Interval"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_uiHistoryDisabled, new GUIContent("Disable History"));
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.Space();
            BuildBookmark((ImageTab)target);
        }

        protected override void ShowScriptName()
        {
            EditorGUILayout.LabelField("ImageTab", EditorStyle.UiTitle);
        }

        private void BuildBookmark(ImageTab script)
        {
            var newSize = Mathf.Max(script.uIBookmarkNames.Length, script.uIBookmarkUrls.Length);
            if (script.uIBookmarkNames.Length != newSize || script.uIBookmarkUrls.Length != newSize)
            {
                _uiBookmarkNames.arraySize = newSize;
                _uiBookmarkUrls.arraySize = newSize;
            }

            EditorGUILayout.LabelField("Bookmarks", EditorStyles.boldLabel);
            serializedObject.Update();

            for (var i = 0; i < newSize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.PropertyField(_uiBookmarkNames.GetArrayElementAtIndex(i), GUIContent.none);
                EditorGUILayout.PropertyField(_uiBookmarkUrls.GetArrayElementAtIndex(i), GUIContent.none);
                
                
                if (GUILayout.Button("Remove"))
                {
                    newSize--;
                    _uiBookmarkNames.DeleteArrayElementAtIndex(i);
                    _uiBookmarkUrls.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Element"))
            {
                newSize++;
                _uiBookmarkNames.arraySize = newSize;
                _uiBookmarkUrls.arraySize = newSize;
            }

            if (GUILayout.Button("Remove Last Element"))
                if (newSize > 0)
                {
                    newSize--;
                    _uiBookmarkNames.arraySize = newSize;
                    _uiBookmarkUrls.arraySize = newSize;
                }

            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
