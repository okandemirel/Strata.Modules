using System.Collections.Generic;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Indexes screens by tag for efficient queries.
    /// </summary>
    internal sealed class ScreenTagIndex
    {
        private readonly Dictionary<int, Dictionary<ScreenTag, List<IScreenBody>>> _activeByTag = new();

        public void Add(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var data = screen.Data;
            var managerId = data.ManagerId;
            var tag = data.Tag;

            if (!_activeByTag.TryGetValue(managerId, out var tagDict))
            {
                tagDict = new Dictionary<ScreenTag, List<IScreenBody>>();
                _activeByTag[managerId] = tagDict;
            }
            if (!tagDict.TryGetValue(tag, out var tagList))
            {
                tagList = new List<IScreenBody>();
                tagDict[tag] = tagList;
            }
            if (!tagList.Contains(screen))
            {
                tagList.Add(screen);
            }
        }

        public void Remove(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var data = screen.Data;
            var managerId = data.ManagerId;
            var tag = data.Tag;

            if (_activeByTag.TryGetValue(managerId, out var tagDict))
            {
                if (tagDict.TryGetValue(tag, out var tagList))
                {
                    tagList.Remove(screen);
                }
            }
        }

        public List<IScreenBody> GetActiveScreensByTag(ScreenTag tag, int managerId)
        {
            if (_activeByTag.TryGetValue(managerId, out var tagDict))
            {
                if (tagDict.TryGetValue(tag, out var list))
                {
                    return new List<IScreenBody>(list);
                }
            }

            return new List<IScreenBody>();
        }

        public void Clear()
        {
            foreach (var tagDict in _activeByTag.Values)
            {
                foreach (var list in tagDict.Values)
                {
                    list.Clear();
                }
            }
            _activeByTag.Clear();
        }
    }
}
