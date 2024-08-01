using jp.ootr.ImageDeviceController;
using jp.ootr.ImageDeviceController.CommonDevice;
using UnityEngine;
using UnityEngine.UI;

namespace jp.ootr.ImageTab.HandDevice
{
    public class UIDeviceName : UIFooter
    {
        [SerializeField] protected InputField uIDeviceNameInputField;

        public override void InitController()
        {
            base.InitController();
            uIDeviceNameInputField.text = deviceName;
        }
    }
}