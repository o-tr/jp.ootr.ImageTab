using UnityEngine;

namespace jp.ootr.ImageTab.ImageTab
{
    public class UIFooter : UIBookmark
    {
        [SerializeField] protected GameObject uICastModalButton;

        public override void InitController()
        {
            var castableDeviceCount = 0;
            foreach (var device in devices)
            {
                if (device == null) continue;
                if (device.IsCastableDevice()) castableDeviceCount++;
            }

            //自分が含まれるので2以上
            uICastModalButton.SetActive(castableDeviceCount > 1);
            base.InitController();
        }
    }
}