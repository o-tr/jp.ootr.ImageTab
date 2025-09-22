using TMPro;
using UnityEngine;

namespace jp.ootr.ImageTab
{
    public class UIDeviceName : UIFooter
    {
        [SerializeField] private TextMeshProUGUI UIDeviceNameText;

        public override void InitController()
        {
            base.InitController();
            UIDeviceNameText.text= $"<size=50%>DeviceName</size>\n{deviceName}";
        }
    }
}
