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
        [SerializeField] public BoxCollider pickupCollider;
        [SerializeField] public bool isObjectSyncEnabled = true;
        [SerializeField] public bool isPickupEnabled = true;
        
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

        public override void LoadImage(string sourceUrl, string fileUrl, bool shouldPushHistory = false)
        {
            _shouldPushHistory = shouldPushHistory;
            _syncSource = sourceUrl;
            _syncFileName = fileUrl;
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
            controller.UnloadSource(this, _localSource);
            _localSource = _syncSource;
            _localFileName = _syncFileName;
            _localFileName.ParseFileName(out var type, out var options);
            
            LLIFetchImage(_localSource, type, options);
        }

        public void ClearUrl()
        {
            inputField.SetUrl(VRCUrl.Empty);
        }

        public override void OnSourceLoadSuccess(string sourceUrl, string[] fileUrls)
        {
            if (sourceUrl != _localSource) return;
            base.OnSourceLoadSuccess(sourceUrl, fileUrls);
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

        public override void OnSourceLoadFailed(LoadError error)
        {
            base.OnSourceLoadFailed(error);
            SetLoading(false);
        }

        public override bool IsCastableDevice()
        {
            return true;
        }
    }
}
