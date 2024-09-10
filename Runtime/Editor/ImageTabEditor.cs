#if UNITY_EDITOR
using System;
using jp.ootr.common;
using jp.ootr.ImageDeviceController.Editor;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;
using Object = UnityEngine.Object;

namespace jp.ootr.ImageTab.Editor
{
    [CustomEditor(typeof(ImageTab))]
    public class ImageTabEditor : CommonDeviceEditor
    {
        private SerializedProperty _arWatchInterval;
        private SerializedProperty _uiBookmarkNames;
        private SerializedProperty _uiBookmarkUrls;
        private SerializedProperty _uiHistoryDisabled;
        private SerializedProperty _isObjectSyncEnabled;
        private SerializedProperty _isPickupEnabled;

        public override void OnEnable()
        {
            base.OnEnable();
            _arWatchInterval = serializedObject.FindProperty("arWatchInterval");
            _uiBookmarkNames = serializedObject.FindProperty("uIBookmarkNames");
            _uiBookmarkUrls = serializedObject.FindProperty("uIBookmarkUrls");
            _uiHistoryDisabled = serializedObject.FindProperty("uIHistoryDisabled");
            _isObjectSyncEnabled = serializedObject.FindProperty("isObjectSyncEnabled");
            _isPickupEnabled = serializedObject.FindProperty("isPickupEnabled");
        }

        protected override void ShowContent()
        {
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_arWatchInterval, new GUIContent("Rotation Check Interval"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_uiHistoryDisabled, new GUIContent("Disable History"));
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_isObjectSyncEnabled, new GUIContent("Enable Object Sync"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_isPickupEnabled, new GUIContent("Enable Pickup"));
            if (EditorGUI.EndChangeCheck())
            {
                ImageTabUtils.UpdateObjectSync((ImageTab)target);
                ImageTabUtils.UpdatePickup((ImageTab)target);
            }
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
    
    [InitializeOnLoad]
    public class PlayModeNotifier
    {
        static PlayModeNotifier()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var imageTab = ComponentUtils.GetAllComponents<ImageTab>();
                foreach (var tab in imageTab)
                {
                    ImageTabUtils.UpdateObjectSync(tab);
                    ImageTabUtils.UpdatePickup(tab);
                }
            }
        }
    }
    
    public class SetObjectReferences : UnityEditor.Editor, IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => 12;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            var imageTab = ComponentUtils.GetAllComponents<ImageTab>();
            foreach (var tab in imageTab)
            {
                ImageTabUtils.UpdateObjectSync(tab);
                ImageTabUtils.UpdatePickup(tab);
            }

            return true;
        }
    }

    public static class ImageTabUtils
    {
        public static void UpdateObjectSync(ImageTab script)
        {
            var currentSyncObj = script.rootGameObject.GetComponent<VRCObjectSync>();
            if (script.isObjectSyncEnabled)
            {
                if (currentSyncObj == null) script.rootGameObject.AddComponent<VRCObjectSync>();
            }
            else
            {
                if (currentSyncObj != null) Object.DestroyImmediate(currentSyncObj);
            }
        }
        
        public static void UpdatePickup(ImageTab script)
        {
            var so = new SerializedObject(script.pickupCollider);
            so.Update();
            so.FindProperty("m_Enabled").boolValue = !script.isPickupEnabled;
            so.ApplyModifiedProperties();
        }
    }
}
#endif
