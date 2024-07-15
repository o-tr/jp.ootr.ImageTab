using static jp.ootr.common.ArrayUtils;

namespace jp.ootr.ImageTab.ImageSlide
{
    public class LogicURLState : LogicCast
    {
        protected string[] FailedSources = new string[0];
        protected string[] LoadedSources = new string[0];
        protected int[] LoadingProgress = new int[0];
        protected string[] LoadingSources = new string[0];

        protected virtual void AddLoadingSource(string source)
        {
            LoadingSources = LoadingSources.Append(source);
            LoadingProgress = LoadingProgress.Append(-1);
        }

        protected virtual bool HasLoadingSource(string source, out int index)
        {
            return LoadingSources.Has(source, out index);
        }

        protected virtual bool HasLoadingProgress(string source, out int progress)
        {
            progress = -1;
            if (!HasLoadingSource(source, out var index)) return false;
            progress = LoadingProgress[index];
            return true;
        }

        protected virtual void SetLoadingProgress(string source, int progress)
        {
            if (!HasLoadingSource(source, out var index))
            {
                ConsoleWarn($"[SetLoadingProgress] source not found: {source}");
                return;
            }

            LoadingProgress[index] = progress;
        }

        protected virtual void AddLoadedSource(string source)
        {
            LoadedSources = LoadedSources.Append(source);
        }

        protected virtual bool HasLoadedSource(string source, out int index)
        {
            return LoadedSources.Has(source, out index);
        }

        protected virtual void RemoveLoadedSource(string source)
        {
            if (!HasLoadedSource(source, out var index))
            {
                ConsoleWarn($"[RemoveLoadedSource] source not found: {source}");
                return;
            }

            RemoveLoadedSource(index);
        }

        protected virtual void RemoveLoadedSource(int index)
        {
            if (index < 0 || index >= LoadedSources.Length)
            {
                ConsoleWarn(
                    $"[RemoveLoadedSource] index out of range: {index}, loaded sources length: {LoadedSources.Length}");
                return;
            }

            LoadingSources = LoadedSources.Remove(index);
        }

        protected virtual void RemoveLoadingSource(string source)
        {
            if (!HasLoadingSource(source, out var index))
            {
                ConsoleWarn($"[RemoveLoadingSource] source not found: {source}");
                return;
            }

            RemoveLoadingSource(index);
        }

        protected virtual void RemoveLoadingSource(int index)
        {
            if (index < 0 || index >= LoadingSources.Length)
            {
                ConsoleWarn(
                    $"[RemoveLoadingSource] index out of range: {index}, loading sources length: {LoadingSources.Length}");
                return;
            }

            LoadingSources = LoadingSources.Remove(index);
            LoadingProgress = LoadingProgress.Remove(index);
        }

        protected virtual void AddFailedSource(string source)
        {
            FailedSources = FailedSources.Append(source);
        }

        protected virtual bool HasFailedSource(string source, out int index)
        {
            return FailedSources.Has(source, out index);
        }

        protected virtual void RemoveFailedSource(string source)
        {
            if (!HasFailedSource(source, out var index)) return;
            RemoveFailedSource(index);
        }

        protected virtual void RemoveFailedSource(int index)
        {
            if (index < 0 || index >= FailedSources.Length)
            {
                ConsoleWarn(
                    $"[RemoveFailedSource] index out of range: {index}, failed sources length: {FailedSources.Length}");
                return;
            }

            FailedSources = FailedSources.Remove(index);
        }
    }
}