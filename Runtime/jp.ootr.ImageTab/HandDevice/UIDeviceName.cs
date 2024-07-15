using jp.ootr.ImageDeviceController;
using jp.ootr.ImageDeviceController.CommonDevice;
using UnityEngine;
using UnityEngine.UI;

namespace jp.ootr.ImageTab.HandDevice
{
    public class UIDeviceName : UIFooter
    {
        [SerializeField] protected InputField uIDeviceNameInputField;

        public override void InitController(DeviceController controller, int deviceId, CommonDevice[] devices)
        {
            base.InitController(controller, deviceId, devices);
            uIDeviceNameInputField.text = deviceName;
        }
    }
}