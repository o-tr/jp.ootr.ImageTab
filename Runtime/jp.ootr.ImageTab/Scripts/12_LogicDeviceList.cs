﻿using jp.ootr.common;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Enums;
using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab
{
    public class LogicDeviceList : AutoRotateDevice
    {
        [SerializeField] private RectTransform uIDeviceListContainer;
        [SerializeField] private GameObject uIOriginalDeviceListButton;
        [SerializeField] private Sprite uIDeviceScreenIcon;
        [SerializeField] private Sprite uIDeviceTabletIcon;
        protected Slider[] DeviceListButtonSliders = new Slider[0];

        protected Toggle[] DeviceListButtonToggles = new Toggle[0];

        public override void InitController()
        {
            base.InitController();
            SendCustomEventDelayedFrames(nameof(UpdateDeviceList), 1, EventTiming.LateUpdate);
        }

        public virtual void UpdateDeviceList()
        {
            uIDeviceListContainer.ClearChildren();
            
            DeviceListButtonToggles = new Toggle[devices.Length];
            DeviceListButtonSliders = new Slider[devices.Length];
            for (var i = 0; i < devices.Length; i++)
            {
                var device = devices[i];
                if (!Utilities.IsValid(device) || device.deviceUuid == deviceUuid) continue;
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

                button.name = device.deviceUuid;
            }

            uIDeviceListContainer.ToListChildrenVertical(0,0,true);
        }
    }
}
