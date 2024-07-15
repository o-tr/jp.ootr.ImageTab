using jp.ootr.ImageDeviceController;
using jp.ootr.ImageDeviceController.CommonDevice;
using UnityEngine;

namespace jp.ootr.ImageTab.ImageTab
{
    public class UIFooter : UIBookmark
    {
        [SerializeField] protected GameObject uICastModalButton;

        public override void InitController(DeviceController controller, int deviceId, CommonDevice[] devices)
        {
            var castableDeviceCount = 0;
            foreach (var device in devices)
            {
                if (device == null) continue;
                if (device.IsCastableDevice()) castableDeviceCount++;
            }

            //自分が含まれるので2以上
            uICastModalButton.SetActive(castableDeviceCount > 1);
            base.InitController(controller, deviceId, devices);
        }
    }
}