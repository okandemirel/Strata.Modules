using Strada.Core.DI;
using Strada.Core.Modules;
using UnityEngine;

namespace Strada.Modules.Screen
{
    [CreateAssetMenu(fileName = "ScreenModuleConfig", menuName = "Strada/Modules/Screen Module Config")]
    public class ScreenModuleConfig : ModuleConfig
    {
        protected override void Configure(IModuleBuilder builder)
        {
            builder
                .Register<IScreenConfigModel, ScreenConfigModel>()
                .Register<IScreenRuntimeModel, ScreenRuntimeModel>()
                .Register<IScreenService, ScreenService>()
                .Register<IScreenBuilderService, ScreenBuilderService>()
                .Register<ScreenLoadService>()
                .Register<ScreenShowService>()
                .Register<ScreenHideService>()
                .Register<ScreenUnloadService>()
                .Register<ScreenSetupService>()
                .Register<ScreenCheckService>()
                .Register<ScreenHistoryService>();
        }

        public override void Initialize(IServiceLocator services)
        {
            var container = services.Get<IContainer>();

            var configModel = services.Get<IScreenConfigModel>();
            var runtimeModel = services.Get<IScreenRuntimeModel>();

            if (configModel is ScreenConfigModel cm)
            {
                InjectionProcessor.Inject(cm, container);
                cm.Initialize();
            }

            if (runtimeModel is ScreenRuntimeModel rm)
            {
                InjectionProcessor.Inject(rm, container);
                rm.Initialize();
            }

            InitializeService(services.Get<IScreenService>(), container);
            InitializeService(services.Get<IScreenBuilderService>(), container);
            InitializeService(services.Get<ScreenLoadService>(), container);
            InitializeService(services.Get<ScreenShowService>(), container);
            InitializeService(services.Get<ScreenHideService>(), container);
            InitializeService(services.Get<ScreenUnloadService>(), container);
            InitializeService(services.Get<ScreenSetupService>(), container);
            InitializeService(services.Get<ScreenCheckService>(), container);
            InitializeService(services.Get<ScreenHistoryService>(), container);

            RegisterSceneManagers(configModel);
        }

        private void InitializeService(object service, IContainer container)
        {
            if (service == null) return;

            InjectionProcessor.Inject(service, container);

            if (service is Strada.Core.Patterns.Service s)
            {
                s.Initialize();
            }
        }

        private void RegisterSceneManagers(IScreenConfigModel configModel)
        {
            var managers = Object.FindObjectsByType<ScreenManager>(FindObjectsSortMode.None);

            foreach (var manager in managers)
            {
                RegisterManager(manager, configModel);
            }

            Debug.Log($"[ScreenModule] Registered {managers.Length} manager(s) from scene");
        }

        private void RegisterManager(ScreenManager manager, IScreenConfigModel configModel)
        {
            var managerData = new ScreenManagerData(manager.ManagerId);
            managerData.Layers.AddRange(manager.Layers);

            configModel.RegisterManager(managerData);

            int registeredCount = 0;
            foreach (var config in manager.Configs)
            {
                if (config == null)
                {
                    Debug.LogWarning("[ScreenModule] Null config in manager configs list");
                    continue;
                }

                if (config.ResolveType())
                {
                    configModel.RegisterConfig(manager.ManagerId, config.ScreenType, config);
                    registeredCount++;
                    Debug.Log($"[ScreenModule] Registered config for {config.ScreenType.Name}");
                }
                else
                {
                    Debug.LogWarning($"[ScreenModule] Failed to resolve type for config '{config.name}'. Ensure DirectPrefab has an IScreenBody component.");
                }
            }

            Debug.Log($"[ScreenModule] Manager {manager.ManagerId} registered with {manager.LayerCount} layers and {registeredCount}/{manager.Configs.Count} configs");
        }
    }
}
