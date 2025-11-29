using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Strada.Core.DI.Attributes;
using Strada.Core.Patterns;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Service for loading screen prefabs.
    /// </summary>
    public class ScreenLoadService : Service
    {
        [Inject] private IScreenConfigModel _configModel;
        [Inject] private IScreenRuntimeModel _runtimeModel;

        private readonly Dictionary<ScreenConfig, AsyncOperationHandle<GameObject>> _addressableHandles = new();

        protected override void OnDispose()
        {
            foreach (var handle in _addressableHandles.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _addressableHandles.Clear();

            base.OnDispose();
        }

        /// <summary>
        /// Loads a screen based on its configuration.
        /// </summary>
        /// <param name="config">The screen configuration.</param>
        /// <returns>Task that completes with the loaded screen.</returns>
        public async Task<IScreenBody> LoadScreenAsync(ScreenConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (_configModel.IsConfigLoaded(config, out var existingScreen))
            {
                return existingScreen;
            }

            GameObject prefab;
            GameObject instance;

            switch (config.LoadType)
            {
                case ScreenLoadType.DirectPrefab:
                    prefab = config.DirectPrefab;
                    if (prefab == null)
                        throw new InvalidOperationException($"DirectPrefab is null for screen config: {config.name}");
                    instance = UnityEngine.Object.Instantiate(prefab);
                    break;

                case ScreenLoadType.Resource:
                    prefab = await LoadFromResourcesAsync(config.ResourcePath);
                    if (prefab == null)
                        throw new InvalidOperationException($"Failed to load screen from Resources: {config.ResourcePath}");
                    instance = UnityEngine.Object.Instantiate(prefab);
                    break;

                case ScreenLoadType.Addressable:
                    instance = await LoadFromAddressablesAsync(config);
                    if (instance == null)
                        throw new InvalidOperationException($"Failed to load screen from Addressables: {config.AddressableKey}");
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unknown load type: {config.LoadType}");
            }

            var screenBody = instance.GetComponent<IScreenBody>();
            if (screenBody == null)
            {
                UnityEngine.Object.Destroy(instance);
                throw new InvalidOperationException($"Loaded prefab does not have IScreenBody component: {config.name}");
            }

            var screenData = new ScreenData();
            config.CopyToData(screenData);
            screenBody.Data = screenData;

            _configModel.MarkConfigLoaded(config, screenBody);

            screenBody.AddState(ScreenState.InPool);
            _runtimeModel.AddToPassivePool(screenBody);

            return screenBody;
        }

        /// <summary>
        /// Loads a screen of a specific type.
        /// </summary>
        /// <typeparam name="T">The screen type.</typeparam>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>Task that completes with the loaded screen.</returns>
        public async Task<T> LoadScreenAsync<T>(int managerId) where T : class, IScreenBody
        {
            var config = _configModel.GetConfig(managerId, typeof(T));
            if (config == null)
                throw new InvalidOperationException($"No config found for screen type {typeof(T).Name} on manager {managerId}");

            var screen = await LoadScreenAsync(config);
            return screen as T;
        }

        /// <summary>
        /// Unloads a screen, releasing resources.
        /// </summary>
        /// <param name="screen">The screen to unload.</param>
        /// <param name="config">The screen configuration.</param>
        public void UnloadScreen(IScreenBody screen, ScreenConfig config)
        {
            if (screen == null)
                return;

            _runtimeModel.RemoveFromPassivePool(screen);
            _runtimeModel.RemoveFromActive(screen);

            if (config != null)
            {
                _configModel.MarkConfigUnloaded(config);

                if (config.LoadType == ScreenLoadType.Addressable)
                {
                    ReleaseAddressableHandle(config);
                }
            }

            if (screen.GameObject != null)
            {
                UnityEngine.Object.Destroy(screen.GameObject);
            }
        }

        /// <summary>
        /// Releases an addressable handle for a config.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private void ReleaseAddressableHandle(ScreenConfig config)
        {
            if (_addressableHandles.TryGetValue(config, out var handle))
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
                _addressableHandles.Remove(config);
            }
        }

        /// <summary>
        /// Loads a prefab from Resources asynchronously.
        /// </summary>
        private async Task<GameObject> LoadFromResourcesAsync(string path)
        {
            var request = Resources.LoadAsync<GameObject>(path);

            while (!request.isDone)
            {
                await Task.Yield();
            }

            return request.asset as GameObject;
        }

        /// <summary>
        /// Loads a prefab from Addressables and instantiates it.
        /// </summary>
        private async Task<GameObject> LoadFromAddressablesAsync(ScreenConfig config)
        {
            var handle = Addressables.InstantiateAsync(config.AddressableKey);
            _addressableHandles[config] = handle;

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }

            _addressableHandles.Remove(config);
            return null;
        }
    }
}
