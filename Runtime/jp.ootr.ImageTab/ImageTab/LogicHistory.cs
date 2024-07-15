using static jp.ootr.common.ArrayUtils;

namespace jp.ootr.ImageTab.ImageTab
{
    public class LogicHistory : UIDeviceList
    {
        protected string[] HistoryFileNames = new string[0];
        protected string[] HistoryUrls = new string[0];

        protected virtual void PushHistory(string url, string fileName)
        {
            HistoryUrls = HistoryUrls.Append(url);
            HistoryFileNames = HistoryFileNames.Append(fileName);
            UpdateHistoryUI(HistoryUrls, HistoryFileNames);
        }

        protected override void OnRemoveHistory(int index)
        {
            HistoryUrls = HistoryUrls.Remove(index);
            HistoryFileNames = HistoryFileNames.Remove(index);
            UpdateHistoryUI(HistoryUrls, HistoryFileNames);
        }

        protected override string[] GetHistoryByIndex(int index)
        {
            if (HistoryUrls.Length <= index) return new string[0];
            return new[]
            {
                HistoryUrls[index],
                HistoryFileNames[index]
            };
        }
    }
}