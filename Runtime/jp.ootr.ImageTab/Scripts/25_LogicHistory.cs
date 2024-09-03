using static jp.ootr.common.ArrayUtils;

namespace jp.ootr.ImageTab
{
    public class LogicHistory : UIDeviceList
    {
        private string[] _historyFileNames = new string[0];
        private string[] _historyUrls = new string[0];

        protected virtual void PushHistory(string url, string fileName)
        {
            _historyUrls = _historyUrls.Append(url);
            _historyFileNames = _historyFileNames.Append(fileName);
            UpdateHistoryUI(_historyUrls, _historyFileNames);
        }

        protected override void OnRemoveHistory(int index)
        {
            _historyUrls = _historyUrls.Remove(index);
            _historyFileNames = _historyFileNames.Remove(index);
            UpdateHistoryUI(_historyUrls, _historyFileNames);
        }

        protected override string[] GetHistoryByIndex(int index)
        {
            if (_historyUrls.Length <= index) return new string[0];
            return new[]
            {
                _historyUrls[index],
                _historyFileNames[index]
            };
        }
    }
}
