using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab.ImageTab
{
    public class UIDeviceList : UIHistory
    {
        public virtual void OnDeviceButtonClicked()
        {
            if (!DeviceListButtonToggles.HasChecked(out var offset)) return;
            var index = (int)DeviceListButtonSliders[offset].value;
            if (index < 0 || index >= Devices.Length) return;
            var device = Devices[index];
            if (device == null) return;
            if (!device.IsCastableDevice()) return;
            CastImageToDevice(index);
        }

        protected virtual void CastImageToDevice(int index)
        {
        }

        public virtual void CastImageToAllScreen()
        {
            foreach (var device in Devices)
            {
                if (device == null) continue;
                if (!device.IsCastableDevice()) return;
                if (device.GetDeviceId() == DeviceId) continue;
                CastImageToDevice(device.GetDeviceId());
            }
        }

        public virtual void ShowScreenNameOnScreen()
        {
            foreach (var device in Devices)
            {
                if (device == null) continue;
                if (!device.IsCastableDevice()) return;
                if (device.GetDeviceId() == DeviceId) continue;
                device.ShowScreenName();
            }
        }
    }
}