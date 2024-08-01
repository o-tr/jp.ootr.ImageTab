using UnityEngine;
using UnityEngine.UI;
using VRC.Udon.Common.Enums;
using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab.HandDevice
{
    public class UIDeviceList : AutoRotateDevice
    {
        [SerializeField] protected RectTransform uIDeviceListContainer;
        [SerializeField] protected GameObject uIOriginalDeviceListButton;
        [SerializeField] protected Sprite uIDeviceScreenIcon;
        [SerializeField] protected Sprite uIDeviceTabletIcon;
        protected Slider[] DeviceListButtonSliders = new Slider[0];

        protected Toggle[] DeviceListButtonToggles = new Toggle[0];

        public override void InitController()
        {
            base.InitController();
            SendCustomEventDelayedFrames(nameof(UpdateDeviceList), 1, EventTiming.LateUpdate);
        }

        public virtual void UpdateDeviceList()
        {
            DeviceListButtonToggles = new Toggle[devices.Length];
            DeviceListButtonSliders = new Slider[devices.Length];
            for (var i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                if (device == null || device.deviceUuid == deviceUuid) continue;
                if (!device.IsCastableDevice()) continue;
                CreateButton(i, device.GetName(), uIOriginalDeviceListButton, out var button, out var animator,
                    out var inputField, out var slider, out var toggle);
                DeviceListButtonToggles[i] = toggle;
                DeviceListButtonSliders[i] = slider;
                switch (device.GetClassName())
                {
                    case "jp.ootr.ImageTab.scripts.ImageTab.ImageTab":
                        button.transform.Find("Image").GetComponent<Image>().sprite = uIDeviceTabletIcon;
                        break;
                    default:
                        button.transform.Find("Image").GetComponent<Image>().sprite = uIDeviceScreenIcon;
                        break;
                }
            }

            uIDeviceListContainer.ToListChildren(adjustHeight:true);
        }
    }
}