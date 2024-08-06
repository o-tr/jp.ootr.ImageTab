using jp.ootr.common;
using UnityEngine;
using UnityEngine.UI;
using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab.ImageTab
{
    public class UIHistory : UIFooter
    {
        [SerializeField] protected GameObject uIOriginalHistoryButton;
        protected InputField[] UIHistoryButtonInputFields = new InputField[0];
        protected GameObject[] UIHistoryButtons = new GameObject[0];
        protected Toggle[] UIHistoryButtonToggles = new Toggle[0];

        public virtual void UpdateHistoryUI(string[] urls, string[] filenames)
        {
            if (urls.Length != filenames.Length)
            {
                ConsoleError("[UpdateHistoryUI] History URLとHistory Nameの数が一致しません");
                return;
            }

            if (UIHistoryButtons.Length < urls.Length)
            {
                var originalLength = UIHistoryButtons.Length;
                UIHistoryButtons = UIHistoryButtons.Resize(urls.Length);
                UIHistoryButtonToggles = UIHistoryButtonToggles.Resize(urls.Length);
                UIHistoryButtonInputFields = UIHistoryButtonInputFields.Resize(urls.Length);
                for (var i = originalLength; i < urls.Length; i++)
                {
                    CreateButton(i, filenames[i], uIOriginalHistoryButton, out var button, out var void2,
                        out var inputField, out var void4, out var toggle);
                    UIHistoryButtons[i] = button;
                    UIHistoryButtonToggles[i] = toggle;
                    UIHistoryButtonInputFields[i] = inputField;
                }
            }
            else if (UIHistoryButtons.Length > urls.Length)
            {
                for (var i = urls.Length; i < UIHistoryButtons.Length; i++) Destroy(UIHistoryButtons[i]);
                UIHistoryButtons = UIHistoryButtons.Resize(urls.Length);
                UIHistoryButtonToggles = UIHistoryButtonToggles.Resize(urls.Length);
                UIHistoryButtonInputFields = UIHistoryButtonInputFields.Resize(urls.Length);
            }

            for (var i = 0; i < UIHistoryButtons.Length; i++)
            {
                if (UIHistoryButtonInputFields[i].text != filenames[i])
                    UIHistoryButtonInputFields[i].text = filenames[i];
                if (!UIHistoryButtonToggles[i].isOn) UIHistoryButtonToggles[i].isOn = false;
            }

            uIOriginalHistoryButton.transform.parent.ToListChildrenVertical(reverse: true, adjustHeight: true);
        }

        public virtual void OnHistoryListClicked()
        {
            if (!UIHistoryButtonToggles.HasChecked(out var index)) return;
            var source = GetHistoryByIndex(index);
            if (source.Length == 0) return;
            LoadImage(source[0], source[1]);
        }

        public virtual void RemoveHistory()
        {
            if (!UIHistoryButtonToggles.HasChecked(out var index)) return;
            OnRemoveHistory(index);
        }

        protected virtual void OnRemoveHistory(int index)
        {
        }

        protected virtual string[] GetHistoryByIndex(int index)
        {
            return new string[0];
        }
    }
}