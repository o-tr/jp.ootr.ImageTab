using UnityEngine;

namespace jp.ootr.ImageTab.ImageTab
{
    public class UIAnimatorHandler : LogicHistory
    {
        protected readonly int AnimatorCastModalState = Animator.StringToHash("CastModalState");
        protected readonly int AnimatorErrorModalState = Animator.StringToHash("ErrorModalState");
        protected readonly int AnimatorLibraryModalState = Animator.StringToHash("LibraryModalState");
        protected readonly int AnimatorSettingsModalState = Animator.StringToHash("SettingsModalState");

        public virtual void CloseCastModal()
        {
            animator.SetInteger(AnimatorCastModalState, 0);
        }

        public virtual void OpenCastModal()
        {
            animator.SetInteger(AnimatorCastModalState, 1);
        }

        public virtual void CloseLibraryModal()
        {
            animator.SetInteger(AnimatorLibraryModalState, 0);
        }

        public virtual void OpenLibraryModal()
        {
            animator.SetInteger(AnimatorLibraryModalState, 1);
        }

        public virtual void OpenLibraryModalHistoryPage()
        {
            animator.SetInteger(AnimatorLibraryModalState, 2);
        }

        public virtual void OpenLibraryModalBookmarkPage()
        {
            animator.SetInteger(AnimatorLibraryModalState, 3);
        }

        public virtual void OpenHistoryModal()
        {
            animator.SetInteger(AnimatorLibraryModalState, 4);
        }

        public virtual void CloseSettingsModal()
        {
            animator.SetInteger(AnimatorSettingsModalState, 0);
        }

        public virtual void OpenSettingsModal()
        {
            animator.SetInteger(AnimatorSettingsModalState, 1);
        }

        public virtual void OpenSettingsModalAboutPage()
        {
            animator.SetInteger(AnimatorSettingsModalState, 2);
        }

        public virtual void OpenSettingsModalLicensePage()
        {
            animator.SetInteger(AnimatorSettingsModalState, 3);
        }

        public virtual void CloseErrorModal()
        {
            animator.SetInteger(AnimatorErrorModalState, 0);
        }

        public virtual void OpenErrorModal()
        {
            animator.SetInteger(AnimatorErrorModalState, 1);
        }
    }
}