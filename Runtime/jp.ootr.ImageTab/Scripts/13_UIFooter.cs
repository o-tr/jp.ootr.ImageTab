using jp.ootr.common;
using UnityEngine;

namespace jp.ootr.ImageTab
{
    public class UIFooter : LogicDeviceList
    {
        [SerializeField] protected Transform uIFooter;
        [SerializeField] private GameObject uICastModalButton;

        public override void InitController()
        {
            base.InitController();
            UpdateFooterButton();
            UpdateFooter();
        }

        public override void OnDirectionChanged()
        {
            base.OnDirectionChanged();
            UpdateFooter();
        }

        public virtual void UpdateFooter()
        {
            uIFooter.ToFillChildrenHorizontal(0, 2);
        }

        private void UpdateFooterButton()
        {
            var castableDeviceCount = 0;
            foreach (var device in devices)
            {
                if (device == null) continue;
                if (device.IsCastableDevice()) castableDeviceCount++;
            }

            //自分が含まれるので2以上
            uICastModalButton.SetActive(castableDeviceCount > 1);
            base.InitController();
        }
    }
}
