# Strada Screen Module

A comprehensive UI screen management system for Unity, built on the Strada framework. Provides screen lifecycle management, navigation history, object pooling, and flexible animation support.

## Features

- **Multiple Loading Strategies**: DirectPrefab, Resources, or Addressables
- **Object Pooling**: Reusable screen instances for better performance
- **Navigation History**: Back button support with screen history tracking
- **Flexible Animations**: IScreenAnimator interface, Unity Animator, or virtual methods
- **Multiple Managers**: Support for different UI contexts (HUD, Popups, Overlays)
- **Safe Area Support**: Device-aware layout adjustments for notches and rounded corners
- **Fluent Builder API**: Clean, chainable screen opening syntax
- **Custom Editors**: Inspector tools for easy configuration

## Installation

1. Ensure you have the Strada Core package installed
2. Add the ScreenModule package to your project
3. Install Unity Addressables package (required dependency)

### Dependencies

- `com.strada.core` >= 1.0.0
- `com.unity.addressables` >= 1.19.0

## Quick Start

### 1. Create a Screen

```csharp
using Strada.Modules.Screen;

public class MainMenuScreen : ScreenBody
{
    public override void OnSetup(params object[] parameters)
    {
        // Called when screen is shown with parameters
    }

    public override void OnShow()
    {
        // Called when screen becomes visible
    }

    public override void OnHide()
    {
        // Called when screen is hidden
    }
}
```

### 2. Create a Screen Config

1. Right-click in Project window
2. Select **Create > Strada > Screen > Screen Config**
3. Configure:
   - **Type Name**: Full type name (e.g., `MyGame.UI.MainMenuScreen`)
   - **Load Type**: Choose DirectPrefab, Resource, or Addressable
   - **Prefab/Path/Key**: Based on load type
   - **Default Layer**: Which layer to show on
   - **Poolable**: Enable for frequently used screens

### 3. Setup Screen Manager

1. Create a Canvas in your scene
2. Add child GameObjects for each layer (e.g., "Background", "Main", "Popup", "Overlay")
3. Add `ScreenLayer` component to each layer object
4. Add `ScreenManager` component to the Canvas
5. Assign layers and screen configs in the inspector

### 4. Register the Module

```csharp
// In your GameBootstrapper or module initialization
screenModuleConfig.Install(containerBuilder);

// After container is built
screenModuleConfig.Initialize(container);
```

### 5. Open Screens

```csharp
public class GameController : MonoBehaviour
{
    [Inject] private IScreenService _screenService;

    public void ShowMainMenu()
    {
        // Simple open
        _screenService.Open<MainMenuScreen>();

        // With fluent builder
        _screenService.Open<MainMenuScreen>()
            .OnLayer(1)
            .WithParams("param1", 42)
            .AddToHistory()
            .Execute();
    }

    public void GoBack()
    {
        _screenService.GoBack();
    }
}
```

## Architecture

### Core Components

| Component | Description |
|-----------|-------------|
| `ScreenBody` | Abstract base class for all screens |
| `ScreenView` | Simple concrete implementation |
| `ScreenLayer` | UI layer container (RectTransform) |
| `ScreenManager` | Scene component that registers layers and configs |
| `ScreenConfig` | ScriptableObject defining screen settings |

### Services

| Service | Description |
|---------|-------------|
| `IScreenService` | Main facade for screen operations |
| `IScreenBuilderService` | Fluent builder for screen opening |
| `ScreenLoadService` | Handles prefab loading |
| `ScreenShowService` | Manages show animations |
| `ScreenHideService` | Manages hide animations |
| `ScreenUnloadService` | Handles screen destruction |
| `ScreenHistoryService` | Navigation history management |

### Models

| Model | Description |
|-------|-------------|
| `IScreenConfigModel` | Stores screen configurations |
| `IScreenRuntimeModel` | Manages active and pooled screens |

## Screen Lifecycle

```
Load -> Setup -> Show -> [Active] -> Hide -> Pool/Unload
```

### States (Flags)

- `None` - Default state
- `Loading` - Being loaded from source
- `InPool` - In passive pool, ready to reuse
- `InUse` - Currently active
- `InShowAnimation` - Playing show animation
- `InHideAnimation` - Playing hide animation
- `Unloading` - Being destroyed

## Animation Options

### Option 1: Virtual Methods (Simple)

```csharp
public class MyScreen : ScreenBody
{
    protected override void PlayShowAnimation()
    {
        // Your animation code
        transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.3f)
            .setOnComplete(ShowAnimationFinished);
    }

    protected override void PlayHideAnimation()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.2f)
            .setOnComplete(HideAnimationFinished);
    }
}
```

### Option 2: Unity Animator

Add an `Animator` component with states named "Show" and "Hide". Call `ShowAnimationFinished()` and `HideAnimationFinished()` via Animation Events.

### Option 3: Custom Animator (Most Flexible)

```csharp
public class MyCustomAnimator : IScreenAnimator
{
    public void PlayShow(IScreenBody screen, Action onComplete)
    {
        // DOTween, custom tweening, etc.
        onComplete?.Invoke();
    }

    public void PlayHide(IScreenBody screen, Action onComplete)
    {
        onComplete?.Invoke();
    }
}

public class MyScreen : ScreenBody
{
    private MyCustomAnimator _animator = new();
    protected override IScreenAnimator CustomAnimator => _animator;
}
```

## Loading Strategies

### DirectPrefab
Assign prefab directly in ScreenConfig. Best for prototyping.

### Resources
Load from Resources folder by path. Good for small projects.

```
ResourcePath: "UI/Screens/MainMenu"
```

### Addressables (Recommended)
Load via Addressables system. Best for production.

```
AddressableKey: "screen_mainmenu"
```

## Multiple Managers

Use different manager IDs for separate UI contexts:

```csharp
// Manager 0: Main UI
_screenService.Open<MainMenuScreen>()
    .OnManager(0)
    .Execute();

// Manager 1: HUD
_screenService.Open<HUDScreen>()
    .OnManager(1)
    .Execute();
```

## Events

Subscribe to screen events via EventBus:

```csharp
// Available events
ScreenLoadedEvent
ScreenShownEvent
ScreenShowAnimationCompleteEvent
ScreenHiddenEvent
ScreenHideAnimationCompleteEvent
ScreenUnloadedEvent
ScreenManagerRegisteredEvent
ScreenManagerUnregisteredEvent
ScreenBackNavigationEvent
ScreenHistoryClearedEvent
```

## API Reference

### IScreenService

```csharp
// Open screens
IScreenBuilderService Open<T>() where T : IScreenBody;
IScreenBuilderService Open(Type screenType);

// Hide screens
void Hide(IScreenBody screen, bool immediate = false);
void HideByTag(ScreenTag tag, int managerId = 0, bool immediate = false);

// Unload screens
void Unload(IScreenBody screen, bool immediate = false);

// Navigation
void GoBack(int managerId = 0);
void ClearHistory(int managerId = 0);

// Queries
IScreenBody GetScreen<T>(int managerId = 0);
IScreenBody GetActiveScreen(int layerIndex, int managerId = 0);
bool IsScreenActive<T>(int managerId = 0);
```

### IScreenBuilderService (Fluent API)

```csharp
_screenService.Open<MyScreen>()
    .OnManager(0)           // Target manager ID
    .OnLayer(1)             // Target layer index
    .WithParams(arg1, arg2) // Setup parameters
    .ForceOpen()            // Open even if already shown
    .AddToHistory()         // Add to navigation history
    .Execute();             // Execute the open operation
```

## Best Practices

1. **Use Addressables** for production builds
2. **Enable pooling** for frequently shown screens (popups, dialogs)
3. **Use screen tags** to group related screens for batch operations
4. **Keep screen prefabs lightweight** - load heavy data on demand
5. **Use layers** to manage z-order (Background < Main < Popup < Overlay)

## Troubleshooting

### Screen type not found
- Ensure the full type name is correct (including namespace)
- Check that the type inherits from `ScreenBody`
- Verify the assembly containing the type is referenced

### Screen not showing
- Check that `ScreenManager` is in the scene and registered
- Verify the layer index exists in the manager
- Check console for loading errors

### Animations not playing
- For Animator: ensure "Show"/"Hide" states exist
- Call `ShowAnimationFinished()`/`HideAnimationFinished()` when done
- Check that `CustomAnimator` is returned if using IScreenAnimator

## License

Part of the Strada Framework. See main package for license details.
