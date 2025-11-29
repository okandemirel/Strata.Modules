using System;
using System.Collections.Generic;
using Strada.Core.Patterns;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Manages active screens and the passive pool for screen reuse.
    /// </summary>
    public class ScreenRuntimeModel : Model, IScreenRuntimeModel
    {
        // Passive pool: ScreenType -> List of pooled screens
        private readonly Dictionary<Type, List<IScreenBody>> _passivePool = new();

        // Active screens by manager: ManagerId -> (ScreenType -> Screen)
        private readonly Dictionary<int, Dictionary<Type, IScreenBody>> _activeByManager = new();

        // Active screens by layer: ManagerId -> (LayerIndex -> Screen)
        private readonly Dictionary<int, Dictionary<int, IScreenBody>> _activeByLayer = new();

        // Active screens by tag: ManagerId -> (Tag -> List of screens)
        private readonly Dictionary<int, Dictionary<ScreenTag, List<IScreenBody>>> _activeByTag = new();

        // Parent transform for pooled screens
        private Transform _poolParent;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            // Create pool parent
            var poolObject = new GameObject("[ScreenModule] Pool");
            UnityEngine.Object.DontDestroyOnLoad(poolObject);
            poolObject.SetActive(false);
            _poolParent = poolObject.transform;
        }

        protected override void OnDispose()
        {
            ClearAll();

            if (_poolParent != null)
            {
                UnityEngine.Object.Destroy(_poolParent.gameObject);
                _poolParent = null;
            }

            base.OnDispose();
        }

        #region Passive Pool

        public void AddToPassivePool(IScreenBody screen)
        {
            if (screen == null)
                return;

            var screenType = screen.Data?.ScreenType ?? screen.GetType();

            if (!_passivePool.TryGetValue(screenType, out var list))
            {
                list = new List<IScreenBody>();
                _passivePool[screenType] = list;
            }

            if (!list.Contains(screen))
            {
                list.Add(screen);
                screen.AddState(ScreenState.InPool);
                screen.RemoveState(ScreenState.InUse);

                // Parent to pool
                if (screen.GameObject != null && _poolParent != null)
                {
                    screen.GameObject.transform.SetParent(_poolParent, false);
                    screen.GameObject.SetActive(false);
                }
            }
        }

        public void RemoveFromPassivePool(IScreenBody screen)
        {
            if (screen == null)
                return;

            var screenType = screen.Data?.ScreenType ?? screen.GetType();

            if (_passivePool.TryGetValue(screenType, out var list))
            {
                list.Remove(screen);
                screen.RemoveState(ScreenState.InPool);
            }
        }

        public bool TryGetFromPool<T>(out T screen) where T : class, IScreenBody
        {
            screen = null;

            if (_passivePool.TryGetValue(typeof(T), out var list) && list.Count > 0)
            {
                var pooledScreen = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);

                pooledScreen.RemoveState(ScreenState.InPool);
                screen = pooledScreen as T;
                return screen != null;
            }

            return false;
        }

        public bool TryGetFromPool(Type screenType, out IScreenBody screen)
        {
            screen = null;

            if (_passivePool.TryGetValue(screenType, out var list) && list.Count > 0)
            {
                screen = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                screen.RemoveState(ScreenState.InPool);
                return true;
            }

            return false;
        }

        #endregion

        #region Active Pools

        public void AddToActive(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var data = screen.Data;
            var managerId = data.ManagerId;
            var screenType = data.ScreenType ?? screen.GetType();
            var layerIndex = data.LayerIndex;
            var tag = data.Tag;

            // By manager + type
            if (!_activeByManager.TryGetValue(managerId, out var managerDict))
            {
                managerDict = new Dictionary<Type, IScreenBody>();
                _activeByManager[managerId] = managerDict;
            }
            managerDict[screenType] = screen;

            // By layer
            if (!_activeByLayer.TryGetValue(managerId, out var layerDict))
            {
                layerDict = new Dictionary<int, IScreenBody>();
                _activeByLayer[managerId] = layerDict;
            }
            layerDict[layerIndex] = screen;

            // By tag
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

            screen.AddState(ScreenState.InUse);
        }

        public void RemoveFromActive(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var data = screen.Data;
            var managerId = data.ManagerId;
            var screenType = data.ScreenType ?? screen.GetType();
            var layerIndex = data.LayerIndex;
            var tag = data.Tag;

            // Remove from manager dict
            if (_activeByManager.TryGetValue(managerId, out var managerDict))
            {
                managerDict.Remove(screenType);
            }

            // Remove from layer dict
            if (_activeByLayer.TryGetValue(managerId, out var layerDict))
            {
                if (layerDict.TryGetValue(layerIndex, out var layerScreen) && layerScreen == screen)
                {
                    layerDict.Remove(layerIndex);
                }
            }

            // Remove from tag list
            if (_activeByTag.TryGetValue(managerId, out var tagDict))
            {
                if (tagDict.TryGetValue(tag, out var tagList))
                {
                    tagList.Remove(screen);
                }
            }

            screen.RemoveState(ScreenState.InUse);
        }

        #endregion

        #region Queries

        public bool IsLayerOccupied(int layerIndex, int managerId, out IScreenBody occupant)
        {
            occupant = null;

            if (_activeByLayer.TryGetValue(managerId, out var layerDict))
            {
                return layerDict.TryGetValue(layerIndex, out occupant);
            }

            return false;
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

        public List<IScreenBody> GetAllPooledScreens()
        {
            var result = new List<IScreenBody>();

            foreach (var list in _passivePool.Values)
            {
                result.AddRange(list);
            }

            return result;
        }

        #endregion

        public void ClearAll()
        {
            // Destroy all pooled screens
            foreach (var list in _passivePool.Values)
            {
                foreach (var screen in list)
                {
                    if (screen?.GameObject != null)
                    {
                        UnityEngine.Object.Destroy(screen.GameObject);
                    }
                }
                list.Clear();
            }
            _passivePool.Clear();

            // Clear active pools (don't destroy - they might be in use)
            _activeByManager.Clear();
            _activeByLayer.Clear();
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
