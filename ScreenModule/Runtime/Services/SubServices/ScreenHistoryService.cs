using System.Collections.Generic;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Service for managing screen history.
    /// </summary>
    public class ScreenHistoryService : Service
    {
        private readonly Dictionary<int, Stack<ScreenHistoryEntry>> _historyByManager = new();

        protected override void OnDispose()
        {
            ClearAll();
            base.OnDispose();
        }

        /// <summary>
        /// Pushes a screen onto the history stack.
        /// </summary>
        /// <param name="screen">The screen to add to history.</param>
        public void Push(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            if (!screen.Data.AddToHistory)
                return;

            var managerId = screen.Data.ManagerId;

            if (!_historyByManager.TryGetValue(managerId, out var stack))
            {
                stack = new Stack<ScreenHistoryEntry>();
                _historyByManager[managerId] = stack;
            }

            stack.Push(new ScreenHistoryEntry(screen));
        }

        /// <summary>
        /// Pops the most recent history entry for a manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="entry">The popped entry if found.</param>
        /// <returns>True if an entry was popped.</returns>
        public bool TryPop(int managerId, out ScreenHistoryEntry entry)
        {
            entry = null;

            if (!_historyByManager.TryGetValue(managerId, out var stack) || stack.Count == 0)
                return false;

            entry = stack.Pop();
            return true;
        }

        /// <summary>
        /// Peeks at the most recent history entry without removing it.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <param name="entry">The peeked entry if found.</param>
        /// <returns>True if an entry exists.</returns>
        public bool TryPeek(int managerId, out ScreenHistoryEntry entry)
        {
            entry = null;

            if (!_historyByManager.TryGetValue(managerId, out var stack) || stack.Count == 0)
                return false;

            entry = stack.Peek();
            return true;
        }

        /// <summary>
        /// Checks if there is history available for back navigation.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>True if history exists.</returns>
        public bool CanGoBack(int managerId)
        {
            return _historyByManager.TryGetValue(managerId, out var stack) && stack.Count > 0;
        }

        /// <summary>
        /// Gets the number of history entries for a manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>Number of entries.</returns>
        public int GetCount(int managerId)
        {
            return _historyByManager.TryGetValue(managerId, out var stack) ? stack.Count : 0;
        }

        /// <summary>
        /// Clears history for a specific manager.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        public void Clear(int managerId)
        {
            if (_historyByManager.TryGetValue(managerId, out var stack))
            {
                stack.Clear();
            }
        }

        /// <summary>
        /// Clears all history for all managers.
        /// </summary>
        public void ClearAll()
        {
            foreach (var stack in _historyByManager.Values)
            {
                stack.Clear();
            }
            _historyByManager.Clear();
        }

        /// <summary>
        /// Gets all history entries for a manager (most recent first).
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>List of history entries.</returns>
        public List<ScreenHistoryEntry> GetHistory(int managerId)
        {
            if (_historyByManager.TryGetValue(managerId, out var stack))
            {
                return new List<ScreenHistoryEntry>(stack);
            }
            return new List<ScreenHistoryEntry>();
        }
    }
}
