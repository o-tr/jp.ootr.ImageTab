using jp.ootr.ImageDeviceController;

namespace jp.ootr.ImageTab
{
    public enum TabletDirection
    {
        Bottom,
        Right,
        Left,
        Top
    }

    public enum ImageSlideSyncAction
    {
        AddImage,
        AddTextZip,
        AddVideo,
        SyncAll,
        RemoveUrl,
        SeekTo,
        None,
        RemoveUnusedFiles
    }

    public static class ImageSlideSyncActionExtensions
    {
        public static ImageSlideSyncAction ToSyncAction(this URLType type)
        {
            switch (type)
            {
                case URLType.Image:
                    return ImageSlideSyncAction.AddImage;
                case URLType.TextZip:
                    return ImageSlideSyncAction.AddTextZip;
                case URLType.Video:
                    return ImageSlideSyncAction.AddVideo;
                default:
                    return ImageSlideSyncAction.None;
            }
        }
    }
}
