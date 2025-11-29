namespace Strada.Modules.Screen
{
    /// <summary>
    /// Tags for categorizing screens. Useful for batch operations
    /// like hiding all popups or unloading all dialogs.
    /// </summary>
    public enum ScreenTag
    {
        /// <summary>
        /// Default tag for uncategorized screens.
        /// </summary>
        Default,

        /// <summary>
        /// Popup screens - typically modal overlays.
        /// </summary>
        Popup,

        /// <summary>
        /// Overlay screens - non-modal overlays on top of content.
        /// </summary>
        Overlay,

        /// <summary>
        /// Dialog screens - confirmation or input dialogs.
        /// </summary>
        Dialog,

        /// <summary>
        /// HUD screens - heads-up display elements.
        /// </summary>
        HUD,

        /// <summary>
        /// Menu screens - main menu, pause menu, etc.
        /// </summary>
        Menu,

        /// <summary>
        /// Loading screens - shown during loading operations.
        /// </summary>
        Loading,

        /// <summary>
        /// Tutorial screens - onboarding and help content.
        /// </summary>
        Tutorial
    }
}
