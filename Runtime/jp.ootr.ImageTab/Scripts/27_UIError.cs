using jp.ootr.ImageDeviceController;
using UnityEngine;
using UnityEngine.UI;

namespace jp.ootr.ImageTab
{
    public class UIError : UIAnimatorHandler
    {
        [SerializeField] private InputField uIErrorTitle;
        [SerializeField] private InputField uIErrorMessage;

        public override void OnSourceLoadFailed(LoadError error)
        {
            base.OnSourceLoadFailed(error);
            ShowError(error);
        }
        
        protected void ShowError(LoadError error)
        {
            error.ParseMessage(out var title, out var message);
            uIErrorTitle.text = title;
            uIErrorMessage.text = message;
            OpenErrorModal();
        }
    }
}
