namespace Strada.Modules.Screen
{
    /// <summary>
    /// Defines how a screen prefab should be loaded.
    /// </summary>
    public enum ScreenLoadType
    {
        /// <summary>
        /// Direct prefab reference - fastest, no async loading required.
        /// Use for screens that are always needed or already in memory.
        /// </summary>
        DirectPrefab,

        /// <summary>
        /// Load from Resources folder using Resources.LoadAsync.
        /// Suitable for smaller projects or legacy setups.
        /// </summary>
        Resource,

        /// <summary>
        /// Load using Unity Addressables system.
        /// Recommended for larger projects with asset bundles.
        /// </summary>
        Addressable
    }
}
