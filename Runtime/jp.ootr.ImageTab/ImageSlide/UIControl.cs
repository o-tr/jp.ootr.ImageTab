using jp.ootr.common;
using jp.ootr.ImageDeviceController;
using jp.ootr.ImageDeviceController.CommonDevice;
using UnityEngine;
using UnityEngine.UI;

namespace jp.ootr.ImageTab.ImageSlide
{
    public class UIControl : UIUrlList
    {
        [SerializeField] protected Transform uIControlVertical;
        [SerializeField] protected Transform uIControlHorizontal;

        [SerializeField] protected Texture2D uIBlankSprite;

        [SerializeField] protected RawImage uICurrentImage;
        [SerializeField] protected AspectRatioFitter uICurrentAspectRatioFitter;
        [SerializeField] protected RawImage uINextImage;
        [SerializeField] protected AspectRatioFitter uINextAspectRatioFitter;
        [SerializeField] protected RawImage uIPreviousImage;
        [SerializeField] protected AspectRatioFitter uIPreviousAspectRatioFitter;

        public override void InitController(DeviceController controller, int deviceId, CommonDevice[] devices)
        {
            base.InitController(controller, deviceId, devices);
            UpdateControlLayout();
        }

        public override void OnDirectionChanged()
        {
            base.OnDirectionChanged();
            UpdateControlLayout();
        }

        protected virtual void UpdateControlLayout()
        {
            uIControlVertical.ToFillChildren(FillDirection.Vertical, 32, 32);
            uIControlHorizontal.ToFillChildren(FillDirection.Horizontal, 32, 32);
        }

        public override void OnUrlsUpdated()
        {
            base.OnUrlsUpdated();
            UpdateControlImages();
        }

        protected virtual void UpdateControlImages()
        {
            if (Sources.Length == 0 || CurrentIndex < 0 || CurrentIndex >= Sources.Length)
            {
                uICurrentImage.texture = uIBlankSprite;
            }
            else
            {
                var texture = Controller.CcGetTexture(Sources[CurrentIndex], FileNames[CurrentIndex]);
                if (texture == null)
                {
                    uICurrentImage.texture = uIBlankSprite;
                }
                else
                {
                    uICurrentImage.texture = texture;
                    uICurrentAspectRatioFitter.aspectRatio = texture.width / (float)texture.height;
                }
            }

            if (Sources.Length == 0 || CurrentIndex == 0 || CurrentIndex >= Sources.Length)
            {
                uIPreviousImage.texture = uIBlankSprite;
            }
            else
            {
                var texture = Controller.CcGetTexture(Sources[CurrentIndex - 1], FileNames[CurrentIndex - 1]);
                if (texture == null)
                {
                    uIPreviousImage.texture = uIBlankSprite;
                }
                else
                {
                    uIPreviousImage.texture = texture;
                    uIPreviousAspectRatioFitter.aspectRatio = texture.width / (float)texture.height;
                }
            }

            if (Sources.Length == 0 || CurrentIndex >= Sources.Length - 1)
            {
                uINextImage.texture = uIBlankSprite;
            }
            else
            {
                var texture = Controller.CcGetTexture(Sources[CurrentIndex + 1], FileNames[CurrentIndex + 1]);
                if (texture == null)
                {
                    uINextImage.texture = uIBlankSprite;
                }
                else
                {
                    uINextImage.texture = texture;
                    uINextAspectRatioFitter.aspectRatio = texture.width / (float)texture.height;
                }
            }
        }

        protected virtual void SeekTo(int index)
        {
            SyncAction = ImageSlideSyncAction.SeekTo;
            SyncIndex = index;
            CastImageToSelectedDevices(index);
            Sync();
        }

        public virtual void OnCurrent()
        {
            if (Sources.Length == 0) return;
            SeekTo(CurrentIndex);
        }

        public virtual void OnNext()
        {
            if (Sources.Length == 0 || CurrentIndex >= Sources.Length - 1) return;
            CurrentIndex++;
            SeekTo(CurrentIndex);
        }

        public virtual void OnPrevious()
        {
            if (Sources.Length == 0 || CurrentIndex <= 0) return;
            CurrentIndex--;
            SeekTo(CurrentIndex);
        }

        protected override void SeekToInternal(int index)
        {
            CurrentIndex = index;
            UpdateControlImages();
            base.SeekToInternal(index);
        }

        protected virtual void CastImageToSelectedDevices(int index)
        {
            for (var i = 0; i < DeviceListButtonToggles.Length; i++)
            {
                var toggle = DeviceListButtonToggles[i];
                if (toggle == null || !toggle.isOn) continue;
                var deviceIndex = (int)DeviceListButtonSliders[i].value;
                var device = Devices[deviceIndex];
                var source = Sources[index];
                var fileName = FileNames[index];
                device.LoadImage(source, fileName);
            }
        }
    }
}