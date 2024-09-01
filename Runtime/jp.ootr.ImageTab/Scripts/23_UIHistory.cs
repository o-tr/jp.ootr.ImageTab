using jp.ootr.common;
using UnityEngine;
using UnityEngine.UI;
using static jp.ootr.common.UI;

namespace jp.ootr.ImageTab
{
    public class UIHistory : UIBookmark
    {
        [SerializeField] private GameObject uIOriginalHistoryButton;
        private InputField[] _uiHistoryButtonInputFields = new InputField[0];
        private GameObject[] _uiHistoryButtons = new GameObject[0];
        private Toggle[] _uiHistoryButtonToggles = new Toggle[0];

        private readonly string[] _uiHistoryPrefix = new[] {"UIHistory"};
        
        public virtual void UpdateHistoryUI(string[] urls, string[] filenames)
        {
            if (urls.Length != filenames.Length)
            {
                ConsoleError($"history urls and filenames length mismatch: {urls.Length} != {filenames.Length}", _uiHistoryPrefix);
                return;
            }

            if (_uiHistoryButtons.Length < urls.Length)
            {
                var originalLength = _uiHistoryButtons.Length;
                _uiHistoryButtons = _uiHistoryButtons.Resize(urls.Length);
                _uiHistoryButtonToggles = _uiHistoryButtonToggles.Resize(urls.Length);
                _uiHistoryButtonInputFields = _uiHistoryButtonInputFields.Resize(urls.Length);
                for (var i = originalLength; i < urls.Length; i++)
                {
                    CreateButton(i, filenames[i], uIOriginalHistoryButton, out var button, out var void2,
                        out var inputField, out var void4, out var toggle);
                    _uiHistoryButtons[i] = button;
                    _uiHistoryButtonToggles[i] = toggle;
                    _uiHistoryButtonInputFields[i] = inputField;
                }
            }
            else if (_uiHistoryButtons.Length > urls.Length)
            {
                for (var i = urls.Length; i < _uiHistoryButtons.Length; i++) Destroy(_uiHistoryButtons[i]);
                _uiHistoryButtons = _uiHistoryButtons.Resize(urls.Length);
                _uiHistoryButtonToggles = _uiHistoryButtonToggles.Resize(urls.Length);
                _uiHistoryButtonInputFields = _uiHistoryButtonInputFields.Resize(urls.Length);
            }

            for (var i = 0; i < _uiHistoryButtons.Length; i++)
            {
                if (_uiHistoryButtonInputFields[i].text != filenames[i])
                    _uiHistoryButtonInputFields[i].text = filenames[i];
                if (!_uiHistoryButtonToggles[i].isOn) _uiHistoryButtonToggles[i].isOn = false;
            }

            uIOriginalHistoryButton.transform.parent.ToListChildrenVertical(reverse: true, adjustHeight: true);
        }

        public virtual void OnHistoryListClicked()
        {
            if (!_uiHistoryButtonToggles.HasChecked(out var index)) return;
            var source = GetHistoryByIndex(index);
            if (source.Length == 0) return;
            LoadImage(source[0], source[1]);
        }

        public virtual void RemoveHistory()
        {
            if (!_uiHistoryButtonToggles.HasChecked(out var index)) return;
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