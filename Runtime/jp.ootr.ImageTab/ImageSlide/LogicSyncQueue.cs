using System;
using jp.ootr.ImageDeviceController;
using UnityEngine;
using static jp.ootr.common.ArrayUtils;

namespace jp.ootr.ImageTab.ImageSlide
{
    public class LogicSyncQueue : UIError
    {
        protected readonly int AnimatorIsSyncing = Animator.StringToHash("IsSyncing");
        protected readonly int SyncProcessInterval = 10;
        protected string[] CurrentQueuedFileNames = new string[0];
        protected int CurrentQueuedIndex;
        protected int CurrentQueuedLength;

        protected string[] CurrentQueuedSources = new string[0];
        protected ImageSlideSyncAction CurrentQueuedSyncAction;

        protected bool IsQueueProcessing;
        protected string[][] QueuedFileNames = new string[0][];
        protected int[] QueuedIndex = new int[0];
        protected int[] QueuedLength = new int[0];

        protected string QueuedSource = "";
        protected string[][] QueuedSources = new string[0][];
        protected ImageSlideSyncAction[] QueuedSyncActions = new ImageSlideSyncAction[0];

        protected virtual void AddSyncQueue(ImageSlideSyncAction action, string[] source, string[] fileName, int length,
            int index)
        {
            QueuedSources = QueuedSources.Append(source);
            QueuedFileNames = QueuedFileNames.Append(fileName);
            QueuedLength = QueuedLength.Append(length);
            QueuedIndex = QueuedIndex.Append(index);
            QueuedSyncActions = QueuedSyncActions.Append(action);

            ConsoleDebug(
                $"[AddSyncQueue] add {action} queue, {QueuedSources.Length} queues remain, processing: {IsQueueProcessing}");
            if (IsQueueProcessing) return;
            IsQueueProcessing = true;
            animator.SetBool(AnimatorIsSyncing, true);
            SetNextQueue();
        }

        public virtual void SetNextQueue()
        {
            ConsoleDebug($"[SetNextQueue] remaining {QueuedSources.Length} queues");
            if (QueuedSources.Length == 0)
            {
                IsQueueProcessing = false;
                animator.SetBool(AnimatorIsSyncing, false);
                return;
            }

            if (QueuedFileNames.Length == 0)
            {
                ConsoleError("[SetNextQueue] QueuedFileNames is empty");
                return;
            }

            if (QueuedLength.Length == 0)
            {
                ConsoleError("[SetNextQueue] QueuedLength is empty");
                return;
            }

            if (QueuedIndex.Length == 0)
            {
                ConsoleError("[SetNextQueue] QueuedIndex is empty");
                return;
            }

            if (QueuedSyncActions.Length == 0)
            {
                ConsoleError("[SetNextQueue] QueuedSyncActions is empty");
                return;
            }

            QueuedSources = QueuedSources.__Shift(out CurrentQueuedSources);
            QueuedFileNames = QueuedFileNames.__Shift(out CurrentQueuedFileNames);
            QueuedLength = QueuedLength.__Shift(out CurrentQueuedLength);
            QueuedIndex = QueuedIndex.__Shift(out CurrentQueuedIndex);
            QueuedSyncActions = QueuedSyncActions.__Shift(out CurrentQueuedSyncAction);
            ProcessCurrentQueue();
        }

        protected virtual void ProcessCurrentQueue()
        {
            ConsoleDebug($"[ProcessCurrentQueue] action: {CurrentQueuedSyncAction}");
            switch (CurrentQueuedSyncAction)
            {
                case ImageSlideSyncAction.AddImage:
                    AddUrlInternal(CurrentQueuedSources[0], URLType.Image);
                    break;
                case ImageSlideSyncAction.AddTextZip:
                    AddUrlInternal($"zip://{CurrentQueuedSources[0].Substring(8)}", URLType.TextZip);
                    break;
                case ImageSlideSyncAction.AddVideo:
                    AddUrlInternal(CurrentQueuedSources[0], URLType.Video);
                    break;
                case ImageSlideSyncAction.RemoveUrl:
                    RemoveUrlInternal(CurrentQueuedIndex);
                    break;
                case ImageSlideSyncAction.SyncAll:
                    ReloadSources(CurrentQueuedSources, CurrentQueuedFileNames);
                    break;
                case ImageSlideSyncAction.RemoveUnusedFiles:
                    RemoveUnusedFilesInternal(CurrentQueuedSources, CurrentQueuedFileNames);
                    break;
                case ImageSlideSyncAction.SeekTo:
                    SeekToInternal(CurrentQueuedIndex);
                    SendCustomEventDelayedFrames(nameof(SetNextQueue), SyncProcessInterval);
                    break;
                default:
                    ConsoleDebug($"{CurrentQueuedSyncAction}");
                    SendCustomEventDelayedFrames(nameof(SetNextQueue), SyncProcessInterval);
                    break;
            }
        }

        protected void AddUrlInternal(string source, URLType type)
        {
            AddImage(source, source);
            AddLoadingSource(source);
            OnUrlsUpdated();
            QueuedSource = source;
            LLIFetchImage(source, type);
        }

        protected void RemoveUrlInternal(int index)
        {
            if (index < 0 || index >= Sources.Length)
            {
                ConsoleError($"[RemoveUrlInternal] invalid index: {index}, source length: {Sources.Length}");
                SendCustomEventDelayedFrames(nameof(SetNextQueue), 10);
                return;
            }

            if (CurrentIndex > Sources.Length - 2 && CurrentIndex > 0) SeekToInternal(Sources.Length - 2);
            var source = Sources[index];
            controller.CcReleaseTexture(source, FileNames[index]);
            RemoveImage(index);
            if (HasFailedSource(source, out var failedIndex)) RemoveFailedSource(failedIndex);
            if (HasLoadingSource(source, out var loadingIndex)) RemoveLoadingSource(loadingIndex);
            OnUrlsUpdated();
            SendCustomEventDelayedFrames(nameof(SetNextQueue), SyncProcessInterval);
        }

        protected virtual void SeekToInternal(int index)
        {
        }

        protected virtual void ReloadSources(string[] sources, string[] files)
        {
            var tmpIndex = 0;
            var tmpActions = new ImageSlideSyncAction[sources.Length + 1];
            var tmpSources = new string[sources.Length + 1][];
            var tmpFiles = new string[sources.Length + 1][];
            var tmpLengths = new int[sources.Length + 1];
            var tmpIndexes = new int[sources.Length + 1];

            var toLoadSources = new string[sources.Length];

            for (var i = Sources.Length - 1; i >= 0; i--)
            {
                if (Array.IndexOf(sources, Sources[i]) >= 0) continue;
                tmpActions[tmpIndex] = ImageSlideSyncAction.RemoveUrl;
                tmpIndexes[tmpIndex] = i;
                tmpIndex++;
            }

            for (var i = 0; i < sources.Length; i++)
                if (Array.IndexOf(Sources, sources[i]) < 0 && Array.IndexOf(toLoadSources, sources[i]) < 0)
                {
                    tmpActions[tmpIndex] = sources[i] == files[i]
                        ? ImageSlideSyncAction.AddImage
                        : ImageSlideSyncAction.AddTextZip;
                    tmpSources[tmpIndex] = new[] { sources[i] };
                    tmpLengths[tmpIndex] = sources.Length;
                    tmpIndexes[tmpIndex] = i;
                    toLoadSources[i] = sources[i];
                    tmpIndex++;
                }

            tmpActions[tmpIndex] = ImageSlideSyncAction.RemoveUnusedFiles;
            tmpSources[tmpIndex] = sources;
            tmpFiles[tmpIndex] = files;

            QueuedSyncActions = QueuedSyncActions.Insert(tmpActions, 0, tmpIndex);
            QueuedSources = QueuedSources.Insert(tmpSources, 0, tmpIndex);
            QueuedFileNames = QueuedFileNames.Insert(tmpFiles, 0, tmpIndex);
            QueuedLength = QueuedLength.Insert(tmpLengths, 0, tmpIndex);
            QueuedIndex = QueuedIndex.Insert(tmpIndexes, 0, tmpIndex);
            ConsoleDebug(
                $"[ReloadSources] source length:{sources.Length}, file length: {files.Length}, inserted queue length: {tmpIndex}, total queue length: {QueuedSources.Length}");

            SendCustomEventDelayedFrames(nameof(SetNextQueue), SyncProcessInterval);
        }

        protected virtual void RemoveUnusedFilesInternal(string[] sources, string[] fileNames)
        {
            for (var i = sources.Length - 1; i <= 0; i--)
            {
                var source = sources[i];
                var fileName = fileNames[i];
                var isUsed = false;
                for (var j = 0; j < CurrentQueuedSources.Length; j++)
                {
                    if (CurrentQueuedSources[j] != source || CurrentQueuedFileNames[j] != fileName) continue;
                    isUsed = true;
                }

                if (isUsed) continue;
                controller.CcReleaseTexture(source, fileName);
                RemoveImage(i);
            }
        }

        public override void OnFilesLoadSuccess(string source, string[] fileNames)
        {
            base.OnFilesLoadSuccess(source, fileNames);
            if (source != QueuedSource) return;
            if (!HasImage(source, source, out var index) &&
                !HasImage(source, $"zip://{source.Substring(8)}", out index)) return;
            ReplaceImages(index, fileNames);
            RemoveLoadingSource(source);
            OnUrlsUpdated();
            SendCustomEventDelayedFrames(nameof(SetNextQueue), SyncProcessInterval);
        }

        public override void OnFilesLoadFailed(LoadError error)
        {
            base.OnFilesLoadFailed(error);
            RemoveLoadingSource(QueuedSource);
            AddFailedSource(QueuedSource);
            ShowErrorModal(error);
            OnUrlsUpdated();
            SendCustomEventDelayedFrames(nameof(SetNextQueue), SyncProcessInterval);
        }

        public virtual void OnUrlsUpdated()
        {
        }
    }
}