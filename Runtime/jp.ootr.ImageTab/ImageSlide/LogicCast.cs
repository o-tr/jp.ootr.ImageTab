namespace jp.ootr.ImageTab.ImageSlide
{
    public class LogicCast : UIAnimationHandler
    {
        protected virtual void CastImageToSelectedDevice(string source, string fileName)
        {
            for (var i = 0; i < DeviceListButtonToggles.Length; i++)
            {
                var toggle = DeviceListButtonToggles[i];
                if (toggle == null) continue;
                if (!toggle.isOn) continue;
                var device = Devices[i];
                if (device == null) continue;
                device.LoadImage(source, fileName);
            }
        }
    }
}