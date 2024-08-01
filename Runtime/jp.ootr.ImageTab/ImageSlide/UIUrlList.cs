using jp.ootr.common;
using UnityEngine;
using UnityEngine.UI;
using VRC.Udon.Common.Enums;
using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab.ImageSlide
{
    public class UIUrlList : LogicLoadUrls
    {
        [SerializeField] protected Transform uIUrlList;
        [SerializeField] protected GameObject uIUrlOriginalButton;
        protected readonly int AnimatorError = Animator.StringToHash("IsError");

        protected readonly int AnimatorLoadProgress = Animator.StringToHash("LoadProgress");
        protected Animator[] UIUrlListAnimators = new Animator[0];
        protected GameObject[] UIUrlListButtons = new GameObject[0];
        protected InputField[] UIUrlListInputFields = new InputField[0];

        protected Slider[] UIUrlListSliders = new Slider[0];
        protected Toggle[] UIUrlListToggles = new Toggle[0];

        public override void OnUrlsUpdated()
        {
            ConsoleDebug($"[OnUrlsUpdated] source length: {Sources.Length}");
            if (UIUrlListButtons.Length < Sources.Length)
            {
                var originalLength = UIUrlListButtons.Length;
                UIUrlListButtons = UIUrlListButtons.Resize(Sources.Length);
                UIUrlListSliders = UIUrlListSliders.Resize(Sources.Length);
                UIUrlListInputFields = UIUrlListInputFields.Resize(Sources.Length);
                UIUrlListToggles = UIUrlListToggles.Resize(Sources.Length);
                UIUrlListAnimators = UIUrlListAnimators.Resize(Sources.Length);
                for (var i = originalLength; i < Sources.Length; i++)
                {
                    CreateButton(i, FileNames[i], uIUrlOriginalButton, out var button, out var tmpAnimator,
                        out var inputField, out var slider, out var checkbox);
                    UIUrlListButtons[i] = button;
                    UIUrlListSliders[i] = slider;
                    UIUrlListInputFields[i] = inputField;
                    UIUrlListToggles[i] = checkbox;
                    UIUrlListAnimators[i] = tmpAnimator;
                }
            }

            if (UIUrlListButtons.Length > Sources.Length)
            {
                for (var i = Sources.Length; i < UIUrlListButtons.Length; i++) Destroy(UIUrlListButtons[i]);
                UIUrlListButtons = UIUrlListButtons.Resize(Sources.Length);
                UIUrlListSliders = UIUrlListSliders.Resize(Sources.Length);
                UIUrlListInputFields = UIUrlListInputFields.Resize(Sources.Length);
                UIUrlListToggles = UIUrlListToggles.Resize(Sources.Length);
                UIUrlListAnimators = UIUrlListAnimators.Resize(Sources.Length);
            }

            for (var i = 0; i < UIUrlListButtons.Length; i++)
            {
                if (UIUrlListInputFields[i].text != FileNames[i]) UIUrlListInputFields[i].text = FileNames[i];
                UIUrlListSliders[i].value = i;
                if (UIUrlListToggles[i].isOn) UIUrlListToggles[i].isOn = false;
                if (HasLoadingProgress(Sources[i], out var progress))
                    UIUrlListAnimators[i].SetFloat(AnimatorLoadProgress, progress);
                else
                    UIUrlListAnimators[i].SetFloat(AnimatorLoadProgress, 2);

                var isError = HasFailedSource(Sources[i], out var index);
                UIUrlListAnimators[i].SetBool(AnimatorError, isError);
            }

            uIUrlList.ToListChildren(adjustHeight:true);
        }

        public override void OnFileLoadProgress(string source, float progress)
        {
            base.OnFileLoadProgress(source, progress);
            if (!HasImage(source, source, out var index) &&
                !HasImage(source, $"zip://{source.Substring(8)}", out index)) return;
            if (index < 0 || UIUrlListAnimators.Length <= index || UIUrlListAnimators[index] == null) return;
            UIUrlListAnimators[index].SetFloat(AnimatorLoadProgress, progress);
        }

        public virtual void OnDeleteButtonClick()
        {
            if (!UIUrlListToggles.HasChecked(UIUrlListSliders, out var index) || IsQueueProcessing) return;
            ConsoleDebug($"[OnDeleteButtonClick] index: {index}");
            RemoveUrl(index);
        }

        public override void ShowPageList()
        {
            base.ShowPageList();
            SendCustomEventDelayedFrames(nameof(OnUrlsUpdated), 1, EventTiming.LateUpdate);
        }
    }
}