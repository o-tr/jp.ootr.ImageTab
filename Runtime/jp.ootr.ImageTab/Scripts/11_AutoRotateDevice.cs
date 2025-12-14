using jp.ootr.ImageDeviceController.CommonDevice;
using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Enums;
using VRC.SDKBase;

namespace jp.ootr.ImageTab
{
    public class AutoRotateDevice : UIAnimatorHandler
    {
        private const float UIAnimationDuration = 0.25f;

        [SerializeField][Range(0.01f, 1f)] public float arWatchInterval = 0.2f;

        [Header("初期設定")][SerializeField] internal TabletDirection initialDirection = TabletDirection.Bottom;

        [Header("回転検知用")][SerializeField] private Transform arAnchorTop;

        [SerializeField] private Transform arAnchorBottom;
        [SerializeField] private Transform arAnchorLeft;
        [SerializeField] private Transform arAnchorRight;

        [Header("アニメーション用定数")] private readonly int _animatorDirection = Animator.StringToHash("Direction");

        private readonly int _animatorLockRotation = Animator.StringToHash("LockRotation");

        private readonly string[] _autoRotateDevicePrefix = { "AutoRotateDevice" };
        [UdonSynced] private TabletDirection _arDirection = TabletDirection.Bottom;
        private bool _arIsHolding;

        [Header("同期用")][UdonSynced] private bool _arIsLockRotate;

        private bool _arIsLockRotateLocal;
        private TabletDirection _arLocalDirection = 0;

        protected override void Start()
        {
            base.Start();
            if (Networking.IsOwner(gameObject))
            {
                _arDirection = initialDirection;
                RequestSerialization();
                ApplyDirectionToAnimator();
            }
        }

        public override void OnPickup()
        {
            _arIsHolding = true;
            SendCustomEventDelayedSeconds(nameof(WatchObjectTransform), arWatchInterval);
        }

        public override void OnDrop()
        {
            _arIsHolding = false;
        }

        public virtual void WatchObjectTransform()
        {
            if (!_arIsHolding) return;
            SendCustomEventDelayedSeconds(nameof(WatchObjectTransform), arWatchInterval);
            UpdateTabletDirection();
        }

        protected virtual void UpdateTabletDirection()
        {
            if (_arIsLockRotate) return;

            var top = arAnchorTop.transform.position.y;
            var bottom = arAnchorBottom.transform.position.y;
            var left = arAnchorLeft.transform.position.y;
            var right = arAnchorRight.transform.position.y;

            var min = Mathf.Min(top, bottom, left, right);
            var max = Mathf.Max(top, bottom, left, right);

            if (max - min < 0.05f) return;
            if (top < bottom && top < left && top < right)
                _arDirection = TabletDirection.Top;
            else if (bottom < top && bottom < left && bottom < right)
                _arDirection = TabletDirection.Bottom;
            else if (left < top && left < bottom && left < right)
                _arDirection = TabletDirection.Left;
            else if (right < top && right < bottom && right < left) _arDirection = TabletDirection.Right;
            if (_arLocalDirection == _arDirection) return;
            Sync();
        }

        protected virtual void ApplyDirectionToAnimator()
        {
            if (_arLocalDirection == _arDirection) return;
            ConsoleDebug($"direction changed: {_arLocalDirection} -> {_arDirection}", _autoRotateDevicePrefix);
            _arLocalDirection = _arDirection;
            animator.SetInteger(_animatorDirection, (int)_arDirection);
            SendCustomEventDelayedSeconds(nameof(OnDirectionChanged), UIAnimationDuration, EventTiming.LateUpdate);
        }

        protected virtual void ApplyRotateLock()
        {
            if (_arIsLockRotateLocal != _arIsLockRotate)
            {
                _arIsLockRotateLocal = _arIsLockRotate;
                animator.SetBool(_animatorLockRotation, _arIsLockRotate);
                if (!_arIsLockRotate) UpdateTabletDirection();
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
            _arIsLockRotate = !_arIsLockRotate;
            Sync();
        }

        public virtual void OnDirectionChanged()
        {
        }
    }
}
