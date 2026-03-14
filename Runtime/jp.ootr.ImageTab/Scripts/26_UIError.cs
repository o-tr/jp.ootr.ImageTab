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
            LoadError.MaximumDimensionExceeded.ParseMessage(out var title, out var message);
            uIDimensionErrorTitle.text = title;
            uIDimensionErrorMessage.text = message;
            uIDimensionErrorInput.text = $"{proxyBaseUrl}{sourceUrl}";
            OpenDimensionErrorModal();
        }
    }
}
