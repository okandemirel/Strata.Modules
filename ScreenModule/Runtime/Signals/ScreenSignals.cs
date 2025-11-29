using System;
using System.Collections.Generic;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Signal for registering a screen manager.
    /// </summary>
    public struct RegisterScreenManagerSignal
    {
        public int ManagerId;
        public List<ScreenLayer> Layers;
        public List<ScreenConfig> Configs;
    }

    /// <summary>
    /// Signal for unregistering a screen manager.
    /// </summary>
    public struct UnregisterScreenManagerSignal
    {
        public int ManagerId;
    }

    /// <summary>
    /// Signal for opening a screen.
    /// </summary>
    public struct OpenScreenSignal
    {
        public Type ScreenType;
        public int ManagerId;
        public int LayerIndex;
        public bool ForceOpen;
        public object[] Parameters;
        public bool AddToHistory;
    }

    /// <summary>
    /// Signal for hiding a screen.
    /// </summary>
    public struct HideScreenSignal
    {
        public IScreenBody Screen;
        public bool Immediate;
    }

    /// <summary>
    /// Signal for hiding all screens with a tag.
    /// </summary>
    public struct HideScreensByTagSignal
    {
        public ScreenTag Tag;
        public int ManagerId;
        public bool Immediate;
    }

    /// <summary>
    /// Signal for unloading a screen.
    /// </summary>
    public struct UnloadScreenSignal
    {
        public IScreenBody Screen;
        public bool Immediate;
    }

    /// <summary>
    /// Signal for navigating back in history.
    /// </summary>
    public struct GoBackSignal
    {
        public int ManagerId;
    }

    /// <summary>
    /// Signal for clearing navigation history.
    /// </summary>
    public struct ClearHistorySignal
    {
        public int ManagerId;
    }
}
