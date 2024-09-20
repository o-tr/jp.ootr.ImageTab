#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using jp.ootr.common;
using jp.ootr.ImageDeviceController.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.SDKBase.Editor.BuildPipeline;
using Object = UnityEngine.Object;

namespace jp.ootr.ImageTab.Editor
{
    [CustomEditor(typeof(ImageTab))]
    public class ImageTabEditor : CommonDeviceEditor
    {
        [SerializeField] private StyleSheet imageTabStyle;
        private SerializedProperty _arWatchInterval;
        private SerializedProperty _uiBookmarkNames;
        private SerializedProperty _uiBookmarkUrls;
        private SerializedProperty _uiHistoryDisabled;
        private SerializedProperty _isObjectSyncEnabled;
        private SerializedProperty _isPickupEnabled;
        
        private List<int> _bookmarkIndex = new List<int>(); 
        
        protected override string GetScriptName()
        {
            return "ImageTab";
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Root.styleSheets.Add(imageTabStyle);
            _arWatchInterval = serializedObject.FindProperty("arWatchInterval");
            _uiBookmarkNames = serializedObject.FindProperty("uIBookmarkNames");
            _uiBookmarkUrls = serializedObject.FindProperty("uIBookmarkUrls");
            _uiHistoryDisabled = serializedObject.FindProperty("uIHistoryDisabled");
            _isObjectSyncEnabled = serializedObject.FindProperty("isObjectSyncEnabled");
            _isPickupEnabled = serializedObject.FindProperty("isPickupEnabled");
            
            _bookmarkIndex.Clear();
            for (int i = 0; i < _uiBookmarkUrls.arraySize; i++)
            {
                _bookmarkIndex.Add(i);
            }
        }

        protected override VisualElement GetContentTk()
        {
            var container = new VisualElement();
            container.AddToClassList("container");
            container.Add(GetArWatchInterval());
            container.Add(GetDisableHistory());
            container.Add(GetObjectSyncEnabled());
            container.Add(GetPickupEnabled());
            container.Add(GetBookmark());

            return container;
        }

        private VisualElement GetArWatchInterval()
        {
            var field = new PropertyField()
            {
                bindingPath = "arWatchInterval",
                label = "Rotation Check Interval",
            };
            return field;
        }
        
        private VisualElement GetDisableHistory()
        {
            var field = new Toggle("Disable History")
            {
                bindingPath = "uIHistoryDisabled"
            };
            return field;
        }
        
        private VisualElement GetObjectSyncEnabled()
        {
            var field = new Toggle("Enable Object Sync")
            {
                bindingPath = "isObjectSyncEnabled"
            };
            field.RegisterValueChangedCallback(evt =>
            {
                ImageTabUtils.UpdateObjectSync((ImageTab)target);
            });
            return field;
        }
        
        private VisualElement GetPickupEnabled()
        {
            var field = new Toggle("Enable Pickup")
            {
                bindingPath = "isPickupEnabled"
            };
            field.RegisterValueChangedCallback(evt =>
            {
                ImageTabUtils.UpdatePickup((ImageTab)target);
            });
            return field;
        }

        private VisualElement GetBookmark()
        {
            var script = (ImageTab)target;
            var container = new VisualElement();
            var newSize = Mathf.Max(script.uIBookmarkNames.Length, script.uIBookmarkUrls.Length);
            if (script.uIBookmarkNames.Length != newSize || script.uIBookmarkUrls.Length != newSize)
            {
                _uiBookmarkNames.arraySize = newSize;
                _uiBookmarkUrls.arraySize = newSize;
            }

            var list = new ListView
            {
                style = { height = 200 },
                itemsSource = _bookmarkIndex,
                reorderable = true,
                showBorder = true,
                showFoldoutHeader = true,
                headerTitle = "Bookmarks",
                showAddRemoveFooter = true,
                reorderMode = ListViewReorderMode.Animated,
            };
            container.Add(list);
            list.itemsAdded += (index) =>
            {
                newSize++;
                serializedObject.Update();
                _uiBookmarkNames.arraySize = newSize;
                _uiBookmarkUrls.arraySize = newSize;
                serializedObject.ApplyModifiedProperties();
                _bookmarkIndex[index.First()] = newSize-1;
            };
            list.itemsRemoved += (index) =>
            {
                newSize--;
                serializedObject.Update();
                _uiBookmarkNames.DeleteArrayElementAtIndex(index.First());
                _uiBookmarkUrls.DeleteArrayElementAtIndex(index.First());
                serializedObject.ApplyModifiedProperties();
            };
            
            list.itemIndexChanged += (oldIndex, newIndex) =>
            {
                (_bookmarkIndex[oldIndex], _bookmarkIndex[newIndex]) = (_bookmarkIndex[newIndex], _bookmarkIndex[oldIndex]);
                serializedObject.Update();
                _uiBookmarkNames.MoveArrayElement(oldIndex, newIndex);
                _uiBookmarkUrls.MoveArrayElement(oldIndex, newIndex);
                serializedObject.ApplyModifiedProperties();
            };
            
            list.makeItem = () => new VisualElement();
            list.bindItem = (e, i) =>
            {
                e.Clear();
                var row = new VisualElement();
                row.AddToClassList("table-row");
                var nameField = new TextField("Name")
                {
                    bindingPath = $"uIBookmarkNames.Array.data[{i}]",
                };
                nameField.Bind(serializedObject);
                nameField.AddToClassList("bookmark-field");
                nameField.AddToClassList("name");
                row.Add(nameField);
                var urlField = new TextField("URL")
                {
                    bindingPath = $"uIBookmarkUrls.Array.data[{i}]"
                };
                urlField.Bind(serializedObject);
                urlField.AddToClassList("bookmark-field");
                urlField.AddToClassList("url");
                row.Add(urlField);
                e.Add(row);
            };
            
            return container;
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
            var imageTab = ComponentUtils.GetAllComponents<ImageTab>();
            foreach (var tab in imageTab)
            {
                ImageTabUtils.UpdateObjectSync(tab);
                ImageTabUtils.UpdatePickup(tab);
                ImageTabUtils.UpdateVRCUrlStore(tab);
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
                ImageTabUtils.UpdateVRCUrlStore(tab);
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
            so.FindProperty("m_Enabled").boolValue = script.isPickupEnabled;
            so.ApplyModifiedProperties();
        }

        public static void UpdateVRCUrlStore(ImageTab script)
        {
            var so = new SerializedObject(script);
            so.Update();
            var urls = so.FindProperty("uIToStoreUrls");
            urls.arraySize = script.uIBookmarkUrls.Length;
            for (int i = 0; i < script.uIBookmarkUrls.Length; i++)
            {
                var vrcUrl = urls.GetArrayElementAtIndex(i);
                vrcUrl.FindPropertyRelative("url").stringValue = script.uIBookmarkUrls[i];
            }
            so.ApplyModifiedProperties();
        }
    }
}
#endif
