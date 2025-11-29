using System;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Event raised when a screen is loaded into memory.
    /// </summary>
    public struct ScreenLoadedEvent
    {
        public IScreenBody Screen;
        public Type ScreenType;
        public int ManagerId;
    }

    /// <summary>
    /// Event raised when a screen is shown.
    /// </summary>
    public struct ScreenShownEvent
    {
        public IScreenBody Screen;
        public Type ScreenType;
        public int ManagerId;
        public int LayerIndex;
    }

    /// <summary>
    /// Event raised when a screen's show animation completes.
    /// </summary>
    public struct ScreenShowAnimationCompleteEvent
    {
        public IScreenBody Screen;
        public int ManagerId;
    }

    /// <summary>
    /// Event raised when a screen is hidden.
    /// </summary>
    public struct ScreenHiddenEvent
    {
        public IScreenBody Screen;
        public Type ScreenType;
        public int ManagerId;
    }

    /// <summary>
    /// Event raised when a screen's hide animation completes.
    /// </summary>
    public struct ScreenHideAnimationCompleteEvent
    {
        public IScreenBody Screen;
        public int ManagerId;
    }

    /// <summary>
    /// Event raised when a screen is unloaded and destroyed.
    /// </summary>
    public struct ScreenUnloadedEvent
    {
        public Type ScreenType;
        public int ManagerId;
    }

    /// <summary>
    /// Event raised when a screen manager is registered.
    /// </summary>
    public struct ScreenManagerRegisteredEvent
    {
        public int ManagerId;
        public int LayerCount;
        public int ConfigCount;
    }

    /// <summary>
    /// Event raised when a screen manager is unregistered.
    /// </summary>
    public struct ScreenManagerUnregisteredEvent
    {
        public int ManagerId;
    }

    /// <summary>
    /// Event raised when back navigation is performed.
    /// </summary>
    public struct ScreenBackNavigationEvent
    {
        public IScreenBody FromScreen;
        public IScreenBody ToScreen;
        public int ManagerId;
    }

    /// <summary>
    /// Event raised when navigation history is cleared.
    /// </summary>
    public struct ScreenHistoryClearedEvent
    {
        public int ManagerId;
        public int ClearedCount;
    }
}
