using System;
using System.Collections.Generic;
using Strada.Core.Patterns;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Manages active screens and the passive pool for screen reuse.
    /// Uses composition with specialized trackers for better separation of concerns.
    /// </summary>
    public class ScreenRuntimeModel : Model, IScreenRuntimeModel
    {
        private readonly ScreenPoolManager _poolManager = new();
        private readonly ActiveScreenRegistry _activeRegistry = new();
        private readonly ScreenLayerTracker _layerTracker = new();
        private readonly ScreenTagIndex _tagIndex = new();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _poolManager.Initialize();
        }

        protected override void OnDispose()
        {
            ClearAll();
            _poolManager.Dispose();
            base.OnDispose();
        }

        #region Passive Pool

        public void AddToPassivePool(IScreenBody screen)
        {
            _poolManager.AddToPool(screen);
        }

        public void RemoveFromPassivePool(IScreenBody screen)
        {
            _poolManager.RemoveFromPool(screen);
        }

        public bool TryGetFromPool<T>(out T screen) where T : class, IScreenBody
        {
            return _poolManager.TryGetFromPool(out screen);
        }

        public bool TryGetFromPool(Type screenType, out IScreenBody screen)
        {
            return _poolManager.TryGetFromPool(screenType, out screen);
        }

        #endregion

        #region Active Pools

        public void AddToActive(IScreenBody screen)
        {
            _activeRegistry.Add(screen);
            _layerTracker.Add(screen);
            _tagIndex.Add(screen);
        }

        public void RemoveFromActive(IScreenBody screen)
        {
            _activeRegistry.Remove(screen);
            _layerTracker.Remove(screen);
            _tagIndex.Remove(screen);
        }

        #endregion

        #region Queries

        public bool IsLayerOccupied(int layerIndex, int managerId, out IScreenBody occupant)
        {
            return _layerTracker.IsLayerOccupied(layerIndex, managerId, out occupant);
        }

        public bool IsScreenActive(Type screenType, int managerId, out IScreenBody screen)
        {
            return _activeRegistry.IsScreenActive(screenType, managerId, out screen);
        }

        public bool IsScreenActive<T>(int managerId, out T screen) where T : class, IScreenBody
        {
            return _activeRegistry.IsScreenActive(managerId, out screen);
        }

        public List<IScreenBody> GetAllActiveScreens()
        {
            return _activeRegistry.GetAllActiveScreens();
        }

        public List<IScreenBody> GetActiveScreensByManager(int managerId)
        {
            return _activeRegistry.GetActiveScreensByManager(managerId);
        }

        public List<IScreenBody> GetActiveScreensByTag(ScreenTag tag, int managerId)
        {
            return _tagIndex.GetActiveScreensByTag(tag, managerId);
        }

        public List<IScreenBody> GetAllPooledScreens()
        {
            return _poolManager.GetAllPooledScreens();
        }

        #endregion

        public void ClearAll()
        {
            _poolManager.Clear();
            _activeRegistry.Clear();
            _layerTracker.Clear();
            _tagIndex.Clear();
        }
    }
}
