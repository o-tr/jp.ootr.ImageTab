using jp.ootr.ImageTab.HandDevice;
using UnityEngine;

namespace jp.ootr.ImageTab.ImageSlide
{
    public class UIAnimationHandler : UIDeviceName
    {
        protected readonly int AnimatorErrorModalState = Animator.StringToHash("ErrorModalState");
        protected readonly int AnimatorInfoModalState = Animator.StringToHash("InfoModalState");
        protected readonly int AnimatorPageState = Animator.StringToHash("PageState");

        protected bool IsInfoModalOpen;

        public virtual void ShowPageList()
        {
            animator.SetInteger(AnimatorPageState, 0);
        }

        public virtual void ShowPageScreens()
        {
            animator.SetInteger(AnimatorPageState, 1);
        }

        public virtual void ShowPageControl()
        {
            animator.SetInteger(AnimatorPageState, 2);
        }

        public virtual void ShowPageSettings()
        {
            animator.SetInteger(AnimatorPageState, 3);
        }

        public virtual void ShowPageSettingsAbout()
        {
            animator.SetInteger(AnimatorPageState, 4);
        }

        public virtual void ShowPageSettingsAboutLicense()
        {
            animator.SetInteger(AnimatorPageState, 5);
        }

        public virtual void CloseErrorModal()
        {
            animator.SetInteger(AnimatorErrorModalState, 0);
        }

        public virtual void OpenErrorModal()
        {
            animator.SetInteger(AnimatorErrorModalState, 1);
        }

        public virtual void ToggleInfoModal()
        {
            IsInfoModalOpen = !IsInfoModalOpen;
            animator.SetInteger(AnimatorInfoModalState, IsInfoModalOpen ? 1 : 0);
        }
    }
}