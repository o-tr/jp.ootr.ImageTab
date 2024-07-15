using jp.ootr.ImageDeviceController.CommonDevice;
using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Enums;

namespace jp.ootr.ImageTab.HandDevice
{
    public class AutoRotateDevice : CommonDevice
    {
        [Header("同期間隔")] [SerializeField] [Range(0.01f, 1f)]
        public float ARWatchInterval = 0.2f;

        [Header("回転検知用")] [SerializeField] protected Transform ARAnchorTop;

        [SerializeField] protected Transform ARAnchorBottom;
        [SerializeField] protected Transform ARAnchorLeft;
        [SerializeField] protected Transform ARAnchorRight;

        [Header("アニメーション用定数")] protected readonly int AnimatorDirection = Animator.StringToHash("Direction");

        protected readonly int AnimatorLockRotation = Animator.StringToHash("LockRotation");

        protected readonly float UIAnimationDuration = 0.25f;
        [UdonSynced] protected TabletDirection ARDirection = TabletDirection.Bottom;
        protected bool ARIsHolding;

        [Header("同期用")] [UdonSynced] protected bool ARIsLockRotate;

        protected bool ARIsLockRotateLocal;
        protected TabletDirection ARLocalDirection = 0;

        public override void OnPickup()
        {
            ARIsHolding = true;
            SendCustomEventDelayedSeconds(nameof(WatchObjectTransform), ARWatchInterval);
        }

        public override void OnDrop()
        {
            ARIsHolding = false;
        }

        public virtual void WatchObjectTransform()
        {
            if (!ARIsHolding) return;
            SendCustomEventDelayedSeconds(nameof(WatchObjectTransform), ARWatchInterval);
            UpdateTabletDirection();
        }

        protected virtual void UpdateTabletDirection()
        {
            if (ARIsLockRotate) return;

            var top = ARAnchorTop.transform.position.y;
            var bottom = ARAnchorBottom.transform.position.y;
            var left = ARAnchorLeft.transform.position.y;
            var right = ARAnchorRight.transform.position.y;

            var min = Mathf.Min(top, bottom, left, right);
            var max = Mathf.Max(top, bottom, left, right);

            if (max - min < 0.05f) return;
            if (top < bottom && top < left && top < right)
                ARDirection = TabletDirection.Top;
            else if (bottom < top && bottom < left && bottom < right)
                ARDirection = TabletDirection.Bottom;
            else if (left < top && left < bottom && left < right)
                ARDirection = TabletDirection.Left;
            else if (right < top && right < bottom && right < left) ARDirection = TabletDirection.Right;
            if (ARLocalDirection == ARDirection) return;
            Sync();
        }

        protected virtual void ApplyDirectionToAnimator()
        {
            if (ARLocalDirection == ARDirection) return;
            ConsoleDebug($"[ApplyDirectionToAnimator] current: {ARLocalDirection}, new: {ARDirection}");
            ARLocalDirection = ARDirection;
            animator.SetInteger(AnimatorDirection, (int)ARDirection);
            SendCustomEventDelayedSeconds(nameof(OnDirectionChanged), UIAnimationDuration, EventTiming.LateUpdate);
        }

        protected virtual void ApplyRotateLock()
        {
            if (ARIsLockRotateLocal != ARIsLockRotate)
            {
                ARIsLockRotateLocal = ARIsLockRotate;
                animator.SetBool(AnimatorLockRotation, ARIsLockRotate);
                if (!ARIsLockRotate) UpdateTabletDirection();
            }
        }

        public override void _OnDeserialization()
        {
            ApplyRotateLock();
            ApplyDirectionToAnimator();
            base._OnDeserialization();
        }

        public void ToggleAutoRotate()
        {
            ARIsLockRotate = !ARIsLockRotate;
            Sync();
        }

        public virtual void OnDirectionChanged()
        {
        }
    }
}