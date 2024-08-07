using jp.ootr.ImageTab.HandDevice;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Enums;
using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab.ImageTab
{
    public class UIBookmark : UIDeviceName
    {
        [SerializeField] public VRCUrl[] uIBookmarkUrls = new VRCUrl[0];
        [SerializeField] public string[] uIBookmarkNames = new string[0];
        [SerializeField] protected GameObject uIOriginalBookmarkButton;

        protected readonly int AnimatorHasBookmark = Animator.StringToHash("HasBookmark");
        protected Toggle[] UIBookmarkButtonToggles = new Toggle[0];

        public override void InitController()
        {
            base.InitController();
            StoreBookmarkUrls();
            UpdateBookmark();
        }

        protected virtual void StoreBookmarkUrls()
        {
            if (uIBookmarkUrls.Length == 0) return;
            foreach (var url in uIBookmarkUrls) controller.UsAddUrlLocal(url);
        }

        public virtual void UpdateBookmark()
        {
            if (uIBookmarkUrls.Length != uIBookmarkNames.Length)
            {
                ConsoleError("[UpdateBookmark] Bookmark URLとBookmark Nameの数が一致しません");
                return;
            }

            var isBookmarkEmpty = uIBookmarkUrls.Length == 0;
            animator.SetBool(AnimatorHasBookmark, !isBookmarkEmpty);
            if (isBookmarkEmpty) return;

            UIBookmarkButtonToggles = new Toggle[uIBookmarkUrls.Length];
            for (var i = 0; i < uIBookmarkUrls.Length; i++)
            {
                var itemUrl = uIBookmarkUrls[i];
                var itemName = uIBookmarkNames[i];
                if (itemUrl == null || itemName == null) continue;
                CreateButton(i, itemName, uIOriginalBookmarkButton, out var void1, out var void2, out var void3,
                    out var void4, out var toggle);
                UIBookmarkButtonToggles[i] = toggle;
            }

            uIOriginalBookmarkButton.transform.parent.ToListChildrenVertical(adjustHeight: true);

            SendCustomEventDelayedFrames(nameof(UpdateFooter), 0, EventTiming.LateUpdate);
        }

        public void OnBookmarkListClicked()
        {
            if (!UIBookmarkButtonToggles.HasChecked(out var index)) return;
            var url = uIBookmarkUrls[index];
            if (url == null) return;
            controller.UsAddUrl(url);
            var urlStr = url.ToString();
            LoadImage(urlStr, urlStr, true);
        }
    }
}