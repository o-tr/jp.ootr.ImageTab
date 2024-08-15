using UnityEngine;
using UnityEngine.UI;

namespace jp.ootr.ImageTab
{
    public class UIDeviceName : UIFooter
    {
        [SerializeField] private InputField uIDeviceNameInputField;

        public override void InitController()
        {
            base.InitController();
            uIDeviceNameInputField.text = deviceName;
        }
    }
}