using System;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Event raised when a screen is loaded into memory.
    /// </summary>
    public readonly struct ScreenLoadedEvent
    {
        public readonly IScreenBody Screen;
        public readonly Type ScreenType;
        public readonly int ManagerId;

        public ScreenLoadedEvent(IScreenBody screen, Type screenType, int managerId)
        {
            Screen = screen;
            ScreenType = screenType;
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Event raised when a screen is shown.
    /// </summary>
    public readonly struct ScreenShownEvent
    {
        public readonly IScreenBody Screen;
        public readonly Type ScreenType;
        public readonly int ManagerId;
        public readonly int LayerIndex;

        public ScreenShownEvent(IScreenBody screen, Type screenType, int managerId, int layerIndex)
        {
            Screen = screen;
            ScreenType = screenType;
            ManagerId = managerId;
            LayerIndex = layerIndex;
        }
    }

    /// <summary>
    /// Event raised when a screen's show animation completes.
    /// </summary>
    public readonly struct ScreenShowAnimationCompleteEvent
    {
        public readonly IScreenBody Screen;
        public readonly int ManagerId;

        public ScreenShowAnimationCompleteEvent(IScreenBody screen, int managerId)
        {
            Screen = screen;
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Event raised when a screen is hidden.
    /// </summary>
    public readonly struct ScreenHiddenEvent
    {
        public readonly IScreenBody Screen;
        public readonly Type ScreenType;
        public readonly int ManagerId;

        public ScreenHiddenEvent(IScreenBody screen, Type screenType, int managerId)
        {
            Screen = screen;
            ScreenType = screenType;
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Event raised when a screen's hide animation completes.
    /// </summary>
    public readonly struct ScreenHideAnimationCompleteEvent
    {
        public readonly IScreenBody Screen;
        public readonly int ManagerId;

        public ScreenHideAnimationCompleteEvent(IScreenBody screen, int managerId)
        {
            Screen = screen;
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Event raised when a screen is unloaded and destroyed.
    /// </summary>
    public readonly struct ScreenUnloadedEvent
    {
        public readonly Type ScreenType;
        public readonly int ManagerId;

        public ScreenUnloadedEvent(Type screenType, int managerId)
        {
            ScreenType = screenType;
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Event raised when a screen manager is registered.
    /// </summary>
    public readonly struct ScreenManagerRegisteredEvent
    {
        public readonly int ManagerId;
        public readonly int LayerCount;
        public readonly int ConfigCount;

        public ScreenManagerRegisteredEvent(int managerId, int layerCount, int configCount)
        {
            ManagerId = managerId;
            LayerCount = layerCount;
            ConfigCount = configCount;
        }
    }

    /// <summary>
    /// Event raised when a screen manager is unregistered.
    /// </summary>
    public readonly struct ScreenManagerUnregisteredEvent
    {
        public readonly int ManagerId;

        public ScreenManagerUnregisteredEvent(int managerId)
        {
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Event raised when back navigation is performed.
    /// </summary>
    public readonly struct ScreenBackNavigationEvent
    {
        public readonly IScreenBody FromScreen;
        public readonly IScreenBody ToScreen;
        public readonly int ManagerId;

        public ScreenBackNavigationEvent(IScreenBody fromScreen, IScreenBody toScreen, int managerId)
        {
            FromScreen = fromScreen;
            ToScreen = toScreen;
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Event raised when navigation history is cleared.
    /// </summary>
    public readonly struct ScreenHistoryClearedEvent
    {
        public readonly int ManagerId;
        public readonly int ClearedCount;

        public ScreenHistoryClearedEvent(int managerId, int clearedCount)
        {
            ManagerId = managerId;
            ClearedCount = clearedCount;
        }
    }
}
