using jp.ootr.common;
using jp.ootr.ImageDeviceController;
using jp.ootr.ImageDeviceController.CommonDevice;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace jp.ootr.ImageTab
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ImageTab : UIError
    {
        [SerializeField] private VRCUrlInputField inputField;

        [SerializeField] private RawImage image;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;

        [SerializeField] public GameObject rootGameObject;
        [SerializeField] public BoxCollider pickupCollider;
        [SerializeField] public bool isObjectSyncEnabled = true;
        [SerializeField] public bool isPickupEnabled = true;

        private bool _isInitialized;

        private readonly string[] _imageTabPrefixes = { "ImageTab" };

        private int _animatorIsLoading = Animator.StringToHash("IsLoading");
        private int _animatorShowSplash = Animator.StringToHash("ShowSplashScreen");

        private bool _isLoading;
        private string _localFileName = "";

        private string _localSource = "";
        private string _previousSource = "";
        private string _previousFileName = "";
        private string _queuedSourceUrl = "";
        private string _queuedFileName = "";
        [UdonSynced] private bool _shouldPushHistory = true;
        [UdonSynced] private string _syncFileName = "";

        [UdonSynced] private string _syncSource = "";

        public override string GetClassName()
        {
            return "jp.ootr.ImageTab.ImageTab.ImageTab";
        }

        public override string GetDisplayName()
        {
            return "ImageTab";
        }

        protected override void Start()
        {
            base.Start();
            _animatorIsLoading = Animator.StringToHash("IsLoading");
            _animatorShowSplash = Animator.StringToHash("ShowSplashScreen");
        }

        public void OnUrlEndEdit()
        {
            var url = inputField.GetUrl();
            var urlStr = url.ToString();

            if (urlStr.IsNullOrEmpty() || _syncSource == url.ToString()) return;
            if (!urlStr.IsValidUrl(out var error))
            {
                ShowError(error);
                return;
            }

            controller.UsAddUrl(url);
            var fileUrl = $"{controller.PROTOCOL_IMAGE}://{url.ToString().Substring(8)}";

            if (_isLoading)
            {
                // 読み込み中の場合はバッファリング（上書き）
                _queuedSourceUrl = urlStr;
                _queuedFileName = fileUrl;
                ConsoleInfo($"[OnUrlEndEdit] queued URL while loading: {urlStr} / {fileUrl}", _imageTabPrefixes);
                return;
            }

            LoadImage(urlStr, fileUrl, true);
        }

        public override void LoadImage(string sourceUrl, string fileUrl, bool shouldPushHistory = false)
        {
            ConsoleDebug($"LoadImage: {sourceUrl} / {fileUrl}", _imageTabPrefixes);
            _shouldPushHistory = shouldPushHistory;
            _syncSource = sourceUrl;
            _syncFileName = fileUrl;
            SetLoading(true);
            Sync();
        }

        public override void _OnDeserialization()
        {
            base._OnDeserialization();
            if ((_localSource == _syncSource && _localFileName == _syncFileName) || _syncSource.IsNullOrEmpty())
            {
                SetLoading(false);
                return;
            }

            SetLoading(true);
            // 古い画像の情報を保存（新しい画像の読み込み成功後に解放するため）
            ConsoleDebug($"[_OnDeserialization] saving previous image: {_localSource} / {_localFileName}", _imageTabPrefixes);
            _previousSource = _localSource;
            _previousFileName = _localFileName;
            _localSource = _syncSource;
            _localFileName = _syncFileName;
            ConsoleInfo($"[_OnDeserialization] loading new image: {_localSource} / {_localFileName}, previous: {_previousSource} / {_previousFileName}", _imageTabPrefixes);
            _localFileName.ParseFileName(out var type, out var options);

            LLIFetchImage(_localSource, type, options);
        }

        public void ClearUrl()
        {
            inputField.SetUrl(VRCUrl.Empty);
        }

        private void LoadQueuedImage()
        {
            if (_queuedSourceUrl.IsNullOrEmpty() || _queuedFileName.IsNullOrEmpty())
            {
                SetLoading(false);
                return;
            }

            var queuedSourceUrl = _queuedSourceUrl;
            var queuedFileName = _queuedFileName;
            _queuedSourceUrl = "";
            _queuedFileName = "";

            ConsoleInfo($"[LoadQueuedImage] loading queued image: {queuedSourceUrl} / {queuedFileName}", _imageTabPrefixes);
            LoadImage(queuedSourceUrl, queuedFileName, true);
        }

        public override void OnSourceLoadSuccess(string sourceUrl, string[] fileUrls)
        {
            // _localSource または _syncSource のいずれかと一致するか確認
            if (sourceUrl != _localSource && sourceUrl != _syncSource)
            {
                ConsoleDebug($"[OnSourceLoadSuccess] source mismatch: {sourceUrl} != {_localSource} and != {_syncSource}, ignoring", _imageTabPrefixes);
                // キューを処理するために base.OnSourceLoadSuccess を呼び出す
                base.OnSourceLoadSuccess(sourceUrl, fileUrls);
                return;
            }

            // 一致するソースが見つかった場合、_localSource と _localFileName を更新
            if (sourceUrl == _syncSource)
            {
                _localSource = _syncSource;
                _localFileName = _syncFileName;
                ConsoleDebug($"[OnSourceLoadSuccess] updated _localSource to match _syncSource: {_localSource} / {_localFileName}", _imageTabPrefixes);
            }

            ConsoleInfo($"[OnSourceLoadSuccess] source loaded: {sourceUrl}, files: {string.Join(", ", fileUrls)}", _imageTabPrefixes);
            base.OnSourceLoadSuccess(sourceUrl, fileUrls);
            if (_shouldPushHistory)
            {
                PushHistory(_localSource, _localFileName);
                _shouldPushHistory = false;
            }

            ConsoleDebug($"[OnSourceLoadSuccess] calling LoadFile: {_localSource} / {_localFileName}", _imageTabPrefixes);
            controller.LoadFile(this, _localSource, _localFileName, 100);
        }

        public override void OnFileLoadSuccess(string source, string fileUrl, string channel)
        {
            // _localSource または _syncSource のいずれかと一致するか確認
            var isLocalMatch = source == _localSource && fileUrl == _localFileName;
            var isSyncMatch = source == _syncSource && fileUrl == _syncFileName;

            if (!isLocalMatch && !isSyncMatch)
            {
                ConsoleDebug($"[OnFileLoadSuccess] source/file mismatch: {source}/{fileUrl} != ({_localSource}/{_localFileName} or {_syncSource}/{_syncFileName}), ignoring", _imageTabPrefixes);
                return;
            }

            // 一致するソースが見つかった場合、_localSource と _localFileName を更新
            if (isSyncMatch && !isLocalMatch)
            {
                _localSource = _syncSource;
                _localFileName = _syncFileName;
                ConsoleDebug($"[OnFileLoadSuccess] updated _localSource to match _syncSource: {_localSource} / {_localFileName}", _imageTabPrefixes);
            }

            ConsoleInfo($"[OnFileLoadSuccess] file loaded: {source} / {fileUrl}, channel: {channel}", _imageTabPrefixes);
            base.OnFileLoadSuccess(source, fileUrl, channel);
            inputField.SetUrl(controller.UsGetUrl(_localSource));
            ConsoleDebug($"[OnFileLoadSuccess] getting texture: {_localSource} / {_localFileName}", _imageTabPrefixes);
            var texture = controller.CcGetTexture(_localSource, _localFileName);
            if (texture != null)
            {
                ConsoleDebug($"[OnFileLoadSuccess] texture obtained: {texture.width}x{texture.height}", _imageTabPrefixes);
                image.texture = texture;
                aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;
            }
            else
            {
                ConsoleWarn($"[OnFileLoadSuccess] texture is null: {_localSource} / {_localFileName}", _imageTabPrefixes);
            }

            // 新しい画像のテクスチャ取得後に、古い画像を解放
            if (!_previousSource.IsNullOrEmpty() && !_previousFileName.IsNullOrEmpty())
            {
                ConsoleInfo($"[OnFileLoadSuccess] releasing previous image: {_previousSource} / {_previousFileName}", _imageTabPrefixes);
                controller.CcReleaseTexture(_previousSource, _previousFileName);
                controller.UnloadSource(this, _previousSource);
                _previousSource = "";
                _previousFileName = "";
                ConsoleDebug($"[OnFileLoadSuccess] previous image released", _imageTabPrefixes);
            }
            else
            {
                ConsoleDebug($"[OnFileLoadSuccess] no previous image to release (previousSource: '{_previousSource}', previousFileName: '{_previousFileName}')", _imageTabPrefixes);
            }

            // バッファリングされたURLがある場合は、それを読み込む
            if (!_queuedSourceUrl.IsNullOrEmpty() && !_queuedFileName.IsNullOrEmpty())
            {
                ConsoleInfo($"[OnFileLoadSuccess] queued URL found, loading: {_queuedSourceUrl} / {_queuedFileName}", _imageTabPrefixes);
                LoadQueuedImage();
                return;
            }

            SetLoading(false);
            if (_isInitialized) return;
            _isInitialized = true;
            animator.SetBool(_animatorShowSplash, false);
        }

        protected virtual void SetLoading(bool loading)
        {
            _isLoading = loading;
            animator.SetBool(_animatorIsLoading, loading);
        }

        protected override void CastImageToDevice(CommonDevice device)
        {
            if (!device.IsCastableDevice()) return;
            device.LoadImage(_localSource, _localFileName);
        }

        public override void OnSourceLoadFailed(LoadError error)
        {
            ConsoleError($"[OnSourceLoadFailed] source load failed: {error}, current source: {_localSource}", _imageTabPrefixes);
            base.OnSourceLoadFailed(error);

            // Clean up previous image references
            if (!_previousSource.IsNullOrEmpty() && !_previousFileName.IsNullOrEmpty())
            {
                ConsoleInfo($"[OnSourceLoadFailed] releasing previous image: {_previousSource} / {_previousFileName}", _imageTabPrefixes);
                controller.CcReleaseTexture(_previousSource, _previousFileName);
                controller.UnloadSource(this, _previousSource);
                _previousSource = "";
                _previousFileName = "";
            }

            // Process queued URL if present
            if (!_queuedSourceUrl.IsNullOrEmpty() && !_queuedFileName.IsNullOrEmpty())
            {
                ConsoleInfo($"[OnSourceLoadFailed] queued URL found, loading: {_queuedSourceUrl} / {_queuedFileName}", _imageTabPrefixes);
                LoadQueuedImage();
                return;
            }

            SetLoading(false);
        }

        public override void OnFileLoadError(string source, string fileUrl, string channel, LoadError error)
        {
            // _localSource または _syncSource のいずれかと一致するか確認
            var isLocalMatch = source == _localSource && fileUrl == _localFileName;
            var isSyncMatch = source == _syncSource && fileUrl == _syncFileName;

            if (!isLocalMatch && !isSyncMatch)
            {
                ConsoleDebug($"[OnFileLoadError] source/file mismatch: {source}/{fileUrl} != ({_localSource}/{_localFileName} or {_syncSource}/{_syncFileName}), ignoring", _imageTabPrefixes);
                return;
            }

            // 一致するソースが見つかった場合、_localSource と _localFileName を更新
            if (isSyncMatch && !isLocalMatch)
            {
                _localSource = _syncSource;
                _localFileName = _syncFileName;
                ConsoleDebug($"[OnFileLoadError] updated _localSource to match _syncSource: {_localSource} / {_localFileName}", _imageTabPrefixes);
            }

            ConsoleError($"[OnFileLoadError] file load error: {source} / {fileUrl}, channel: {channel}, error: {error}", _imageTabPrefixes);
            base.OnFileLoadError(source, fileUrl, channel, error);

            // Clean up previous image references
            if (!_previousSource.IsNullOrEmpty() && !_previousFileName.IsNullOrEmpty())
            {
                ConsoleInfo($"[OnFileLoadError] releasing previous image: {_previousSource} / {_previousFileName}", _imageTabPrefixes);
                controller.CcReleaseTexture(_previousSource, _previousFileName);
                controller.UnloadSource(this, _previousSource);
                _previousSource = "";
                _previousFileName = "";
            }

            // バッファリングされたURLがある場合は、それを読み込む
            if (!_queuedSourceUrl.IsNullOrEmpty() && !_queuedFileName.IsNullOrEmpty())
            {
                ConsoleInfo($"[OnFileLoadError] queued URL found, loading: {_queuedSourceUrl} / {_queuedFileName}", _imageTabPrefixes);
                LoadQueuedImage();
                return;
            }

            SetLoading(false);
        }

        public override bool IsCastableDevice()
        {
            return true;
        }
    }
}
