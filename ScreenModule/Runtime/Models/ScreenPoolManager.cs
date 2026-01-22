using System;
using System.Collections.Generic;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Manages the passive pool of screen instances for reuse.
    /// </summary>
    internal sealed class ScreenPoolManager : IDisposable
    {
        private readonly Dictionary<Type, List<IScreenBody>> _passivePool = new();
        private Transform _poolParent;

        public void Initialize()
        {
            var poolObject = new GameObject("[ScreenModule] Pool");
            UnityEngine.Object.DontDestroyOnLoad(poolObject);
            poolObject.SetActive(false);
            _poolParent = poolObject.transform;
        }

        public void AddToPool(IScreenBody screen)
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

                if (screen.GameObject != null && _poolParent != null)
                {
                    screen.GameObject.transform.SetParent(_poolParent, false);
                    screen.GameObject.SetActive(false);
                }
            }
        }

        public void RemoveFromPool(IScreenBody screen)
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

        public List<IScreenBody> GetAllPooledScreens()
        {
            var result = new List<IScreenBody>();

            foreach (var list in _passivePool.Values)
            {
                result.AddRange(list);
            }

            return result;
        }

        public void Clear()
        {
            foreach (var list in _passivePool.Values)
            {
                foreach (var screen in list)
                {
                    if (screen is MonoBehaviour mb && mb != null)
                    {
                        UnityEngine.Object.Destroy(mb.gameObject);
                    }
                }
                list.Clear();
            }
            _passivePool.Clear();
        }

        public void Dispose()
        {
            Clear();

            if (_poolParent != null)
            {
                UnityEngine.Object.Destroy(_poolParent.gameObject);
                _poolParent = null;
            }
        }
    }
}
