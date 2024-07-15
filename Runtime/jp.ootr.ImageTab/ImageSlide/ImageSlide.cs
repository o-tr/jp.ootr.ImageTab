using jp.ootr.common;
using jp.ootr.ImageDeviceController;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace jp.ootr.ImageTab.ImageSlide
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ImageSlide : UIControl
    {
        [SerializeField] protected VRCUrlInputField imageInputField;
        [SerializeField] protected VRCUrlInputField zipInputField;

        public override string GetClassName()
        {
            return "jp.ootr.ImageTab.scripts.ImageSlide.ImageSlide";
        }

        public virtual void OnUrlEndEdit()
        {
            var imageUrl = imageInputField.GetUrl();
            var zipUrl = zipInputField.GetUrl();
            imageInputField.SetUrl(VRCUrl.Empty);
            zipInputField.SetUrl(VRCUrl.Empty);
            var isImageUrlEmpty = imageUrl.ToString().IsNullOrEmpty();
            var isZipUrlEmpty = zipUrl.ToString().IsNullOrEmpty();
            if (isImageUrlEmpty && isZipUrlEmpty) return;
            var url = isImageUrlEmpty ? zipUrl : imageUrl;
            var urlStr = url.ToString();
            if (!urlStr.IsValidUrl(out var error))
            {
                ShowErrorModal(error);
                return;
            }

            if (Sources.Has(urlStr))
            {
                ShowErrorModal(LoadError.DuplicateURL);
                return;
            }

            Controller.UsAddUrl(url);

            AddUrl(url, isImageUrlEmpty ? URLType.TextZip : URLType.Image);
        }

        public override bool IsCastableDevice()
        {
            return false;
        }
    }
}