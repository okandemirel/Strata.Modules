using System.Collections.Generic;

namespace Strada.Modules.Screen
{
    /// <summary>
    /// Tracks screen layer occupation.
    /// </summary>
    internal sealed class ScreenLayerTracker
    {
        private readonly Dictionary<int, Dictionary<int, IScreenBody>> _activeByLayer = new();

        public void Add(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var data = screen.Data;
            var managerId = data.ManagerId;
            var layerIndex = data.LayerIndex;

            if (!_activeByLayer.TryGetValue(managerId, out var layerDict))
            {
                layerDict = new Dictionary<int, IScreenBody>();
                _activeByLayer[managerId] = layerDict;
            }
            layerDict[layerIndex] = screen;
        }

        public void Remove(IScreenBody screen)
        {
            if (screen?.Data == null)
                return;

            var data = screen.Data;
            var managerId = data.ManagerId;
            var layerIndex = data.LayerIndex;

            if (_activeByLayer.TryGetValue(managerId, out var layerDict))
            {
                if (layerDict.TryGetValue(layerIndex, out var layerScreen) && layerScreen == screen)
                {
                    layerDict.Remove(layerIndex);
                }
            }
        }

        public bool IsLayerOccupied(int layerIndex, int managerId, out IScreenBody occupant)
        {
            occupant = null;

            if (_activeByLayer.TryGetValue(managerId, out var layerDict))
            {
                return layerDict.TryGetValue(layerIndex, out occupant);
            }

            return false;
        }

        public void Clear()
        {
            _activeByLayer.Clear();
        }
    }
}
