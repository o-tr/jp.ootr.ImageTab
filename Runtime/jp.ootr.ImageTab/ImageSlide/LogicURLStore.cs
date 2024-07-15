using System;
using static jp.ootr.common.ArrayUtils;

namespace jp.ootr.ImageTab.ImageSlide
{
    public class LogicURLStore : LogicURLState
    {
        protected int CurrentIndex = 0;
        protected string[] FileNames = new string[0];
        protected string[] Sources = new string[0];

        protected virtual void AddImage(string source, string fileName)
        {
            Sources = Sources.Append(source);
            FileNames = FileNames.Append(fileName);
        }

        protected virtual void RemoveImage(string source, string fileName)
        {
            if (!HasImage(source, fileName, out var index)) return;
            RemoveImage(index);
        }

        protected virtual void RemoveImage(int index)
        {
            if (index < 0 || index >= Sources.Length) return;
            Sources = Sources.Remove(index);
            FileNames = FileNames.Remove(index);
        }

        protected virtual void ReplaceImages(int index, string[] tmpFileNames)
        {
            if (index < 0 || index >= Sources.Length)
            {
                ConsoleError($"[ReplaceImages] index out of range: {index}, sources length: {Sources.Length}");
                return;
            }

            var sources = new string[Sources.Length + tmpFileNames.Length - 1];
            Array.Copy(Sources, sources, index);
            for (var i = index; i < index + tmpFileNames.Length; i++) sources[i] = Sources[index];
            Array.Copy(Sources, index + 1, sources, index + tmpFileNames.Length, Sources.Length - index - 1);
            Sources = sources;
            FileNames = FileNames.Replace(tmpFileNames, index);
        }

        protected virtual bool HasImage(string source, string fileName, out int index)
        {
            for (var i = 0; i < Sources.Length; i++)
                if (Sources[i] == source && FileNames[i] == fileName)
                {
                    index = i;
                    return true;
                }

            index = -1;
            return false;
        }
    }
}