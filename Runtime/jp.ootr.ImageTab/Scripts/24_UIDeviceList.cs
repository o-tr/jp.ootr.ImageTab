using jp.ootr.ImageDeviceController.CommonDevice;
using VRC.SDKBase;
using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab
{
    public class UIDeviceList : UIHistory
    {
        public virtual void OnDeviceButtonClicked()
        {
            if (!DeviceListButtonToggles.HasChecked(out var offset)) return;
            var index = (int)DeviceListButtonSliders[offset].value;
            if (index < 0 || index >= devices.Length) return;
            var device = devices[index];
            if (!Utilities.IsValid(device)) return;
            if (!device.IsCastableDevice()) return;
            CastImageToDevice(device);
        }

        protected virtual void CastImageToDevice(CommonDevice device)
        {
        }

        public virtual void CastImageToAllScreen()
        {
            foreach (var device in devices)
            {
                if (!Utilities.IsValid(device)) continue;
                if (!device.IsCastableDevice()) return;
                if (device.GetDeviceUuid() == deviceUuid) continue;
                CastImageToDevice(device);
            }
        }

        public virtual void ShowScreenNameOnScreen()
        {
            foreach (var device in devices)
            {
                if (!Utilities.IsValid(device)) continue;
                if (!device.IsCastableDevice()) return;
                if (device.GetDeviceUuid() == deviceUuid) continue;
                device.ShowScreenName();
            }
        }
    }
}
