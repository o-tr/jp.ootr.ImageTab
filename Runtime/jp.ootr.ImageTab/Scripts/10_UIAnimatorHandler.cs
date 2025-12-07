using jp.ootr.ImageDeviceController.CommonDevice;
using UnityEngine;

namespace jp.ootr.ImageTab
{
    public class UIAnimatorHandler : CommonDevice
    {
        private readonly int _animatorCastModalState = Animator.StringToHash("CastModalState");
        private readonly int _animatorErrorModalState = Animator.StringToHash("ErrorModalState");
        private readonly int _animatorLibraryModalState = Animator.StringToHash("LibraryModalState");
        private readonly int _animatorSettingsModalState = Animator.StringToHash("SettingsModalState");

        public virtual void CloseCastModal()
        {
            animator.SetInteger(_animatorCastModalState, 0);
        }

        public virtual void OpenCastModal()
        {
            animator.SetInteger(_animatorCastModalState, 1);
        }

        public virtual void CloseLibraryModal()
        {
            animator.SetInteger(_animatorLibraryModalState, 0);
        }

        public virtual void OpenLibraryModal()
        {
            animator.SetInteger(_animatorLibraryModalState, 1);
        }

        public virtual void OpenLibraryModalHistoryPage()
        {
            animator.SetInteger(_animatorLibraryModalState, 2);
        }

        public virtual void OpenLibraryModalBookmarkPage()
        {
            animator.SetInteger(_animatorLibraryModalState, 3);
        }

        public virtual void OpenHistoryModal()
        {
            animator.SetInteger(_animatorLibraryModalState, 4);
        }

        public virtual void OpenBookmarkModal()
        {
            animator.SetInteger(_animatorLibraryModalState, 5);
        }

        public virtual void CloseSettingsModal()
        {
            animator.SetInteger(_animatorSettingsModalState, 0);
        }

        public virtual void OpenSettingsModal()
        {
            animator.SetInteger(_animatorSettingsModalState, 1);
        }

        public virtual void CloseErrorModal()
        {
            animator.SetInteger(_animatorErrorModalState, 0);
        }

        public virtual void OpenErrorModal()
        {
            animator.SetInteger(_animatorErrorModalState, 1);
        }
    }
}
