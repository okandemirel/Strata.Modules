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

            ScreenManager.OnManagerRegistered += manager => OnManagerRegistered(manager, container);
            ScreenManager.OnManagerUnregistered += manager => OnManagerUnregistered(manager, container);
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

        private void OnManagerRegistered(ScreenManager manager, IContainer container)
        {
            var configModel = container.Resolve<IScreenConfigModel>();

            var managerData = new ScreenManagerData(manager.ManagerId);
            managerData.Layers.AddRange(manager.Layers);

            configModel.RegisterManager(managerData);

            foreach (var config in manager.Configs)
            {
                if (config != null && config.ResolveType())
                {
                    configModel.RegisterConfig(manager.ManagerId, config.ScreenType, config);
                }
            }

            Debug.Log($"[ScreenModule] Manager {manager.ManagerId} registered with {manager.LayerCount} layers and {manager.Configs.Count} configs");
        }

        private void OnManagerUnregistered(ScreenManager manager, IContainer container)
        {
            var configModel = container.Resolve<IScreenConfigModel>();
            configModel.UnregisterManager(manager.ManagerId);

            Debug.Log($"[ScreenModule] Manager {manager.ManagerId} unregistered");
        }
    }
}
