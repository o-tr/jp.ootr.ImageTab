using jp.ootr.ImageDeviceController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace jp.ootr.ImageTab
{
    public class UIError : UIAnimatorHandler
    {
        [SerializeField] private TextMeshProUGUI uIErrorTitle;
        [SerializeField] private TextMeshProUGUI uIErrorMessage;

        public override void OnSourceLoadFailed(LoadError error)
        {
            base.OnSourceLoadFailed(error);
            ShowError(error);
        }
        
        protected void ShowError(LoadError error)
        {
            error.ParseMessage(out var title, out var message);
            uIErrorTitle.text =$"<color=#ff0000><sprite name=\"o_alert\" color=\"#ff0000\">{title}</color>";
            uIErrorMessage.text = message;
            OpenErrorModal();
        }
    }
}
