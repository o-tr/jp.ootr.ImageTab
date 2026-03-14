using System.Text.RegularExpressions;
using jp.ootr.ImageDeviceController;
using TMPro;
using UnityEngine;

namespace jp.ootr.ImageTab
{
    public class UIError : LogicHistory
    {
        [SerializeField] private TextMeshProUGUI uIErrorTitle;
        [SerializeField] private TextMeshProUGUI uIErrorMessage;
        [SerializeField] private string proxyBaseUrl;
        [SerializeField] private TextMeshProUGUI uIDimensionErrorTitle;
        [SerializeField] private TextMeshProUGUI uIDimensionErrorMessage;
        [SerializeField] private TMP_InputField uIDimensionErrorInput;
        [SerializeField] private GameObject uiDimensionProxyTosInput;

        protected virtual string GetCurrentSourceUrl() => "";

        public override void OnSourceLoadFailed(LoadError error)
        {
            base.OnSourceLoadFailed(error);
            if (error == LoadError.MaximumDimensionExceeded)
            {
                ShowDimensionError(GetCurrentSourceUrl());
                return;
            }
            ShowError(error);
        }

        protected void ShowError(LoadError error)
        {
            error.ParseMessage(out var title, out var message);
            uIErrorTitle.text = $"<color=#ff0000><sprite name=\"o_alert\" color=\"#ff0000\">{title}</color>";
            uIErrorMessage.text = message;
            OpenErrorModal();
        }

        protected void ShowDimensionError(string sourceUrl)
        {
            uIDimensionErrorTitle.text = __("error.dimension.title");
            if (TryGetResizedUrl(sourceUrl, out var resizedUrl))
            {
                uIDimensionErrorMessage.text = __("error.dimension.cdn.message");
                uIDimensionErrorInput.text = resizedUrl;
                uiDimensionProxyTosInput.SetActive(false);
            }
            else
            {
                uIDimensionErrorMessage.text = __("error.dimension.proxy.message");
                uIDimensionErrorInput.text = string.IsNullOrEmpty(proxyBaseUrl)
                    ? sourceUrl
                    : $"{proxyBaseUrl}{sourceUrl}";
                uiDimensionProxyTosInput.SetActive(true);
            }
            OpenDimensionErrorModal();
        }

        private bool TryGetResizedUrl(string sourceUrl, out string resizedUrl)
        {
            resizedUrl = "";
            if (string.IsNullOrEmpty(sourceUrl)) return false;

            if (sourceUrl.Contains("cdn.discordapp.com") || sourceUrl.Contains("media.discordapp.net"))
            {
                return TransformDiscordUrl(sourceUrl, out resizedUrl);
            }

            if (sourceUrl.Contains("pbs.twimg.com"))
            {
                return TransformTwitterUrl(sourceUrl, out resizedUrl);
            }

            return false;
        }

        private bool TransformDiscordUrl(string url, out string resizedUrl)
        {
            resizedUrl = "";
            var widthMatch = Regex.Match(url, @"[?&]width=(\d+)");
            var heightMatch = Regex.Match(url, @"[?&]height=(\d+)");

            if (!widthMatch.Success || !heightMatch.Success) return false;

            if (!int.TryParse(widthMatch.Groups[1].Value, out var width) ||
                !int.TryParse(heightMatch.Groups[1].Value, out var height)) return false;
            var ratio = (float)width / height;

            int newWidth, newHeight;
            if (width >= height)
            {
                newWidth = 2048;
                newHeight = Mathf.Max(1, (int)(2048 / ratio));
            }
            else
            {
                newHeight = 2048;
                newWidth = Mathf.Max(1, (int)(2048 * ratio));
            }

            url = Regex.Replace(url, @"(?<=[?&])width=\d+", $"width={newWidth}");
            url = Regex.Replace(url, @"(?<=[?&])height=\d+", $"height={newHeight}");
            resizedUrl = url;
            return true;
        }

        private bool TransformTwitterUrl(string url, out string resizedUrl)
        {
            resizedUrl = "";
            var nameMatch = Regex.Match(url, @"[?&]name=([^&]*)");
            if (nameMatch.Success)
            {
                var currentName = nameMatch.Groups[1].Value;
                if (currentName == "large" || currentName == "medium" || currentName == "small" || currentName == "thumb")
                    return false;
                resizedUrl = Regex.Replace(url, @"([?&]name=)[^&]*", "${1}large");
                return true;
            }
            var separator = url.Contains("?") ? "&" : "?";
            resizedUrl = $"{url}{separator}name=large";
            return true;
        }
    }
}
