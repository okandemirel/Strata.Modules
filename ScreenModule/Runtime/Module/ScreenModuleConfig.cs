using System;
using Strada.Core.DI;
using UnityEngine;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Module configuration for the Screen Module.
    /// Register this in your GameBootstrapperConfig to enable screen management.
    /// </summary>
    [CreateAssetMenu(fileName = "ScreenModuleConfig", menuName = "Strada/Modules/Screen Module Config")]
    public class ScreenModuleConfig : ScriptableObject
    {
        [Header("Module Settings")]
        [Tooltip("Priority for module initialization (lower = earlier)")]
        [SerializeField] private int _priority = 50;

        /// <summary>
        /// The module name.
        /// </summary>
        public string ModuleName => "ScreenModule";

        /// <summary>
        /// Module initialization priority.
        /// </summary>
        public int Priority => _priority;

        /// <summary>
        /// Registers all screen module dependencies with the container builder.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        public void Install(IContainerBuilder builder)
        {
            builder.Register<IScreenConfigModel, ScreenConfigModel>(Lifetime.Singleton);
            builder.Register<IScreenRuntimeModel, ScreenRuntimeModel>(Lifetime.Singleton);

            builder.Register<IScreenService, ScreenService>(Lifetime.Singleton);
            builder.Register<IScreenBuilderService, ScreenBuilderService>(Lifetime.Singleton);

            builder.Register<ScreenLoadService>(Lifetime.Singleton);
            builder.Register<ScreenShowService>(Lifetime.Singleton);
            builder.Register<ScreenHideService>(Lifetime.Singleton);
            builder.Register<ScreenUnloadService>(Lifetime.Singleton);
            builder.Register<ScreenSetupService>(Lifetime.Singleton);
            builder.Register<ScreenCheckService>(Lifetime.Singleton);
            builder.Register<ScreenHistoryService>(Lifetime.Singleton);
        }

        /// <summary>
        /// Initializes the screen module after container is built.
        /// </summary>
        /// <param name="container">The built container.</param>
        public void Initialize(IContainer container)
        {
            var configModel = container.Resolve<IScreenConfigModel>();
            var runtimeModel = container.Resolve<IScreenRuntimeModel>();

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

            var screenService = container.Resolve<IScreenService>();
            var builderService = container.Resolve<IScreenBuilderService>();

            InitializeService(screenService, container);
            InitializeService(builderService, container);

            InitializeService(container.Resolve<ScreenLoadService>(), container);
            InitializeService(container.Resolve<ScreenShowService>(), container);
            InitializeService(container.Resolve<ScreenHideService>(), container);
            InitializeService(container.Resolve<ScreenUnloadService>(), container);
            InitializeService(container.Resolve<ScreenSetupService>(), container);
            InitializeService(container.Resolve<ScreenCheckService>(), container);
            InitializeService(container.Resolve<ScreenHistoryService>(), container);

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
