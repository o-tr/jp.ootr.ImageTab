using jp.ootr.common;
using jp.ootr.ImageDeviceController;
using jp.ootr.ImageDeviceController.CommonDevice;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace jp.ootr.ImageTab.ImageTab
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ImageTab : UIError
    {
        [SerializeField] protected VRCUrlInputField inputField;

        [SerializeField] protected RawImage image;
        [SerializeField] protected AspectRatioFitter aspectRatioFitter;
        protected readonly int AnimatorIsLoading = Animator.StringToHash("IsLoading");

        protected bool IsLoading;
        protected string LocalFileName = "";

        protected string LocalSource = "";
        [UdonSynced] protected bool ShouldPushHistory = true;
        [UdonSynced] protected string SyncFileName = "";

        [UdonSynced] protected string SyncSource = "";

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

            if (IsLoading || urlStr.IsNullOrEmpty() || SyncSource == url.ToString()) return;
            if (!urlStr.IsValidUrl(out var error))
            {
                OnFilesLoadFailed(error);
                return;
            }

            controller.UsAddUrl(url);

            LoadImage(urlStr, urlStr, true);
        }

        public override void LoadImage(string source, string fileName, bool shouldPushHistory = false)
        {
            ShouldPushHistory = shouldPushHistory;
            SyncSource = source;
            SyncFileName = fileName;
            SetLoading(true);
            Sync();
        }

        public override void _OnDeserialization()
        {
            base._OnDeserialization();
            if ((LocalSource == SyncSource && LocalFileName == SyncFileName) || SyncSource.IsNullOrEmpty())
            {
                SetLoading(false);
                return;
            }

            SetLoading(true);
            controller.CcReleaseTexture(LocalSource, LocalFileName);
            controller.UnloadFilesFromUrl((IControlledDevice)this, LocalSource);
            LocalSource = SyncSource;
            LocalFileName = SyncFileName;
            LLIFetchImage(LocalSource, SyncSource == SyncFileName ? URLType.Image : URLType.TextZip);
        }

        public void ClearUrl()
        {
            inputField.SetUrl(VRCUrl.Empty);
        }

        public override void OnFilesLoadSuccess(string source, string[] fileNames)
        {
            if (source != LocalSource) return;
            base.OnFilesLoadSuccess(source, fileNames);
            if (ShouldPushHistory)
            {
                PushHistory(LocalSource, LocalFileName);
                ShouldPushHistory = false;
            }

            inputField.SetUrl(controller.UsGetUrl(LocalSource));
            var texture = controller.CcGetTexture(LocalSource, LocalFileName);
            if (!texture) return;
            image.texture = texture;
            aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
            SetLoading(false);
        }

        protected virtual void SetLoading(bool loading)
        {
            IsLoading = loading;
            animator.SetBool(AnimatorIsLoading, loading);
        }

        protected override void CastImageToDevice(CommonDevice device)
        {
            if (!device.IsCastableDevice()) return;
            device.LoadImage(LocalSource, LocalFileName);
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