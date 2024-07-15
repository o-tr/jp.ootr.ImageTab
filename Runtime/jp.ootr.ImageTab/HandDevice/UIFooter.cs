using jp.ootr.common;
using jp.ootr.ImageDeviceController;
using jp.ootr.ImageDeviceController.CommonDevice;
using UnityEngine;

namespace jp.ootr.ImageTab.HandDevice
{
    public class UIFooter : UIDeviceList
    {
        [SerializeField] protected Transform uIFooter;

        public override void InitController(DeviceController controller, int deviceId, CommonDevice[] devices)
        {
            base.InitController(controller, deviceId, devices);
            UpdateFooter();
        }

        public override void OnDirectionChanged()
        {
            base.OnDirectionChanged();
            UpdateFooter();
        }

        public virtual void UpdateFooter()
        {
            uIFooter.ToFillChildren(FillDirection.Horizontal, 0, 2);
        }
    }
}