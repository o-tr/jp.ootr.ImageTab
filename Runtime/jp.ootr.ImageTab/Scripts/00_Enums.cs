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
        public static ImageSlideSyncAction ToSyncAction(this SourceType type)
        {
            switch (type)
            {
                case SourceType.Image:
                    return ImageSlideSyncAction.AddImage;
                case SourceType.TextZip:
                    return ImageSlideSyncAction.AddTextZip;
                case SourceType.Video:
                    return ImageSlideSyncAction.AddVideo;
                default:
                    return ImageSlideSyncAction.None;
            }
        }
    }
}
