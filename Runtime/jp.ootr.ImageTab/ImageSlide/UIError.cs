using jp.ootr.ImageDeviceController;
using UnityEngine;
using UnityEngine.UI;

namespace jp.ootr.ImageTab.ImageSlide
{
    public class UIError : LogicURLStore
    {
        [SerializeField] protected InputField uIErrorTitle;
        [SerializeField] protected InputField uIErrorMessage;

        protected void ShowErrorModal(LoadError error)
        {
            error.ParseMessage(out var title, out var message);
            uIErrorTitle.text = title;
            uIErrorMessage.text = message;
            OpenErrorModal();
        }
    }
}