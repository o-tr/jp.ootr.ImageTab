using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Enums;
using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab
{
    public class UIBookmark : UIDeviceName
    {
        [SerializeField] public VRCUrl[] uIBookmarkUrls = new VRCUrl[0];
        [SerializeField] public string[] uIBookmarkNames = new string[0];
        [SerializeField] private GameObject uIOriginalBookmarkButton;

        private readonly int _animatorHasBookmark = Animator.StringToHash("HasBookmark");

        private readonly string[] _uiBookmarkPrefix = { "UIBookmark" };
        private Toggle[] _uiBookmarkButtonToggles = new Toggle[0];

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

        protected virtual void UpdateBookmark()
        {
            if (uIBookmarkUrls.Length != uIBookmarkNames.Length)
            {
                ConsoleError(
                    $"bookmark urls and names length mismatch: {uIBookmarkUrls.Length} != {uIBookmarkNames.Length}",
                    _uiBookmarkPrefix);
                return;
            }

            var isBookmarkEmpty = uIBookmarkUrls.Length == 0;
            animator.SetBool(_animatorHasBookmark, !isBookmarkEmpty);
            if (isBookmarkEmpty) return;

            _uiBookmarkButtonToggles = new Toggle[uIBookmarkUrls.Length];
            for (var i = 0; i < uIBookmarkUrls.Length; i++)
            {
                var itemUrl = uIBookmarkUrls[i];
                var itemName = uIBookmarkNames[i];
                if (itemUrl == null || itemName == null) continue;
                CreateButton(i, itemName, uIOriginalBookmarkButton, out var void1, out var void2, out var void3,
                    out var void4, out var toggle);
                _uiBookmarkButtonToggles[i] = toggle;
            }

            uIOriginalBookmarkButton.transform.parent.ToListChildrenVertical(0,0,true);

            SendCustomEventDelayedFrames(nameof(UpdateFooter), 0, EventTiming.LateUpdate);
        }

        public void OnBookmarkListClicked()
        {
            if (!_uiBookmarkButtonToggles.HasChecked(out var index)) return;
            var url = uIBookmarkUrls[index];
            if (url == null) return;
            controller.UsAddUrl(url);
            var urlStr = url.ToString();
            LoadImage(urlStr, urlStr, true);
        }
    }
}
