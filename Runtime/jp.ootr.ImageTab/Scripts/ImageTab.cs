using jp.ootr.common;
using jp.ootr.ImageDeviceController;
using jp.ootr.ImageDeviceController.CommonDevice;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace jp.ootr.ImageTab
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ImageTab : UIError
    {
        [SerializeField] private VRCUrlInputField inputField;

        [SerializeField] private RawImage image;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;
        
        [SerializeField] public GameObject rootGameObject;
        [SerializeField] public bool isObjectSyncEnabled = true;
        
        private readonly int _animatorIsLoading = Animator.StringToHash("IsLoading");

        private bool _isLoading;
        private string _localFileName = "";

        private string _localSource = "";
        [UdonSynced] private bool _shouldPushHistory = true;
        [UdonSynced] private string _syncFileName = "";

        [UdonSynced] private string _syncSource = "";

        public override string GetClassName()
        {
            return "jp.ootr.ImageTab.ImageTab.ImageTab";
        }

        public override string GetDisplayName()
        {
            return "ImageTab";
        }

        public void OnUrlEndEdit()
        {
            var url = inputField.GetUrl();
            var urlStr = url.ToString();

            if (_isLoading || urlStr.IsNullOrEmpty() || _syncSource == url.ToString()) return;
            if (!urlStr.IsValidUrl(out var error))
            {
                ShowError(error);
                return;
            }

            controller.UsAddUrl(url);

            LoadImage(urlStr, urlStr, true);
        }

        public override void LoadImage(string source, string fileName, bool shouldPushHistory = false)
        {
            _shouldPushHistory = shouldPushHistory;
            _syncSource = source;
            _syncFileName = fileName;
            SetLoading(true);
            Sync();
        }

        public override void _OnDeserialization()
        {
            base._OnDeserialization();
            if ((_localSource == _syncSource && _localFileName == _syncFileName) || _syncSource.IsNullOrEmpty())
            {
                SetLoading(false);
                return;
            }

            SetLoading(true);
            controller.CcReleaseTexture(_localSource, _localFileName);
            controller.UnloadFilesFromUrl(this, _localSource);
            _localSource = _syncSource;
            _localFileName = _syncFileName;
            LLIFetchImage(_localSource, _syncSource == _syncFileName ? URLType.Image : URLType.TextZip);
        }

        public void ClearUrl()
        {
            inputField.SetUrl(VRCUrl.Empty);
        }

        public override void OnFilesLoadSuccess(string source, string[] fileNames)
        {
            if (source != _localSource) return;
            base.OnFilesLoadSuccess(source, fileNames);
            if (_shouldPushHistory)
            {
                PushHistory(_localSource, _localFileName);
                _shouldPushHistory = false;
            }

            inputField.SetUrl(controller.UsGetUrl(_localSource));
            var texture = controller.CcGetTexture(_localSource, _localFileName);
            if (!texture) return;
            image.texture = texture;
            aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
            SetLoading(false);
        }

        protected virtual void SetLoading(bool loading)
        {
            _isLoading = loading;
            animator.SetBool(_animatorIsLoading, loading);
        }

        protected override void CastImageToDevice(CommonDevice device)
        {
            if (!device.IsCastableDevice()) return;
            device.LoadImage(_localSource, _localFileName);
        }

        public override void OnFilesLoadFailed(LoadError error)
        {
            base.OnFilesLoadFailed(error);
            SetLoading(false);
        }

        public override bool IsCastableDevice()
        {
            return true;
        }
    }
}
