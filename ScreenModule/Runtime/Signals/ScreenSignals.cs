using System;
using System.Collections.Generic;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Signal for registering a screen manager.
    /// </summary>
    public readonly struct RegisterScreenManagerSignal
    {
        public readonly int ManagerId;
        public readonly IReadOnlyList<ScreenLayer> Layers;
        public readonly IReadOnlyList<ScreenConfig> Configs;

        public RegisterScreenManagerSignal(int managerId, IReadOnlyList<ScreenLayer> layers, IReadOnlyList<ScreenConfig> configs)
        {
            ManagerId = managerId;
            Layers = layers;
            Configs = configs;
        }
    }

    /// <summary>
    /// Signal for unregistering a screen manager.
    /// </summary>
    public readonly struct UnregisterScreenManagerSignal
    {
        public readonly int ManagerId;

        public UnregisterScreenManagerSignal(int managerId)
        {
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Signal for opening a screen.
    /// </summary>
    public readonly struct OpenScreenSignal
    {
        public readonly Type ScreenType;
        public readonly int ManagerId;
        public readonly int LayerIndex;
        public readonly bool ForceOpen;
        public readonly object[] Parameters;
        public readonly bool AddToHistory;

        public OpenScreenSignal(Type screenType, int managerId, int layerIndex, bool forceOpen, object[] parameters, bool addToHistory)
        {
            ScreenType = screenType;
            ManagerId = managerId;
            LayerIndex = layerIndex;
            ForceOpen = forceOpen;
            Parameters = parameters;
            AddToHistory = addToHistory;
        }
    }

    /// <summary>
    /// Signal for hiding a screen.
    /// </summary>
    public readonly struct HideScreenSignal
    {
        public readonly IScreenBody Screen;
        public readonly bool Immediate;

        public HideScreenSignal(IScreenBody screen, bool immediate)
        {
            Screen = screen;
            Immediate = immediate;
        }
    }

    /// <summary>
    /// Signal for hiding all screens with a tag.
    /// </summary>
    public readonly struct HideScreensByTagSignal
    {
        public readonly ScreenTag Tag;
        public readonly int ManagerId;
        public readonly bool Immediate;

        public HideScreensByTagSignal(ScreenTag tag, int managerId, bool immediate)
        {
            Tag = tag;
            ManagerId = managerId;
            Immediate = immediate;
        }
    }

    /// <summary>
    /// Signal for unloading a screen.
    /// </summary>
    public readonly struct UnloadScreenSignal
    {
        public readonly IScreenBody Screen;
        public readonly bool Immediate;

        public UnloadScreenSignal(IScreenBody screen, bool immediate)
        {
            Screen = screen;
            Immediate = immediate;
        }
    }

    /// <summary>
    /// Signal for navigating back in history.
    /// </summary>
    public readonly struct GoBackSignal
    {
        public readonly int ManagerId;

        public GoBackSignal(int managerId)
        {
            ManagerId = managerId;
        }
    }

    /// <summary>
    /// Signal for clearing navigation history.
    /// </summary>
    public readonly struct ClearHistorySignal
    {
        public readonly int ManagerId;

        public ClearHistorySignal(int managerId)
        {
            ManagerId = managerId;
        }
    }
}
