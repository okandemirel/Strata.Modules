using System;
using System.Collections.Generic;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Tracks active screens by manager and type.
    /// </summary>
    internal sealed class ActiveScreenRegistry
    {
        private readonly Dictionary<int, Dictionary<Type, IScreenBody>> _activeByManager = new();

        public void Add(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var data = screen.Data;
            var managerId = data.ManagerId;
            var screenType = data.ScreenType ?? screen.GetType();

            if (!_activeByManager.TryGetValue(managerId, out var managerDict))
            {
                managerDict = new Dictionary<Type, IScreenBody>();
                _activeByManager[managerId] = managerDict;
            }
            managerDict[screenType] = screen;

            screen.AddState(ScreenState.InUse);
        }

        public void Remove(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var data = screen.Data;
            var managerId = data.ManagerId;
            var screenType = data.ScreenType ?? screen.GetType();

            if (_activeByManager.TryGetValue(managerId, out var managerDict))
            {
                managerDict.Remove(screenType);
            }

            screen.RemoveState(ScreenState.InUse);
        }

        public bool IsScreenActive(Type screenType, int managerId, out IScreenBody screen)
        {
            screen = null;

            if (_activeByManager.TryGetValue(managerId, out var managerDict))
            {
                return managerDict.TryGetValue(screenType, out screen);
            }

            return false;
        }

        public bool IsScreenActive<T>(int managerId, out T screen) where T : class, IScreenBody
        {
            screen = null;

            if (IsScreenActive(typeof(T), managerId, out var foundScreen))
            {
                screen = foundScreen as T;
                return screen != null;
            }

            return false;
        }

        public List<IScreenBody> GetAllActiveScreens()
        {
            var result = new List<IScreenBody>();

            foreach (var managerDict in _activeByManager.Values)
            {
                result.AddRange(managerDict.Values);
            }

            return result;
        }

        public List<IScreenBody> GetActiveScreensByManager(int managerId)
        {
            if (_activeByManager.TryGetValue(managerId, out var managerDict))
            {
                return new List<IScreenBody>(managerDict.Values);
            }

            return new List<IScreenBody>();
        }

        public void Clear()
        {
            _activeByManager.Clear();
        }
    }
}
