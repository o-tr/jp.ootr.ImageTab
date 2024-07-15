using jp.ootr.ImageDeviceController;
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace jp.ootr.ImageTab.ImageSlide
{
    public class LogicLoadUrls : LogicSyncQueue
    {
        [UdonSynced] protected ImageSlideSyncAction SyncAction = ImageSlideSyncAction.None;
        [UdonSynced] protected string[] SyncFileNames = new string[0];
        [UdonSynced] protected int SyncIndex = -1;
        [UdonSynced] protected int SyncLength;
        [UdonSynced] protected string[] SyncSources = new string[0];

        public virtual void AddUrl(VRCUrl url, URLType type, string urlStr = null)
        {
            Controller.UsAddUrl(url);
            SyncAction = type.ToSyncAction();
            SyncSources = new[]
            {
                urlStr ?? url.ToString()
            };
            SyncLength = Sources.Length;
            Sync();
        }

        public virtual void RemoveUrl(int index)
        {
            if (index < 0 || index >= Sources.Length) return;
            SyncAction = ImageSlideSyncAction.RemoveUrl;
            SyncIndex = index;
            SyncLength = Sources.Length;
            Sync();
        }

        public virtual void RequestSyncAllToOwner()
        {
            SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(SyncAll));
        }

        public virtual void SyncAll()
        {
            SyncAction = ImageSlideSyncAction.SyncAll;
            SyncSources = Sources;
            SyncFileNames = FileNames;
            Sync();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            SyncAll();
        }

        public override void _OnDeserialization()
        {
            AddSyncQueue(SyncAction, SyncSources, SyncFileNames, SyncLength, SyncIndex);
            base._OnDeserialization();
            SyncAction = ImageSlideSyncAction.None;
        }
    }
}