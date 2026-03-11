# PDM - Preprocessor Directive Manager

PDM (Preprocessor Directive Manager) is a Unity Editor tool that automatically defines C# preprocessor directives based on the presence of packages, namespaces, or classes in your project. It is part of the [YJack framework](https://github.com/YvesAlbuquerque).

## Overview

When developing Unity projects with optional dependencies (e.g., Oculus VR, Universal Render Pipeline, LiteNetLib), you often need to wrap code in `#if` preprocessor directives to avoid compilation errors when a package is not installed. PDM automates the management of these directives by:

1. Scanning loaded assemblies for specified namespaces, classes, or UPM packages
2. Automatically updating scripting define symbols via `PlayerSettings.SetScriptingDefineSymbols` with `NamedBuildTarget`
3. Recompilation is triggered automatically by Unity when defines change

## Installation

### Via Unity Package Manager (Git URL)

1. Open Unity and go to **Window > Package Manager**
2. Click the **+** button and select **Add package from git URL...**
3. Enter: `https://github.com/YvesAlbuquerque/PDM_Core.git`
4. Click **Add**

### Manual Installation

1. Clone or download this repository
2. Copy the contents into your Unity project's `Packages/com.ygamedev.pdm/` directory

## Usage

### Lookup Types

PDM supports three domain types for detection:

| Domain Type | Description | Example Lookup Value |
|---|---|---|
| **Namespace** | Checks if a namespace exists in any loaded assembly | `Unity.Netcode` |
| **Class** | Checks if a class name exists in any loaded assembly | `OVRManager` |
| **Package** | Checks if a UPM package is installed by package name | `com.unity.render-pipelines.universal` |

> **Tip:** For UPM packages, prefer the `Package` type over `Namespace` — it's more reliable since it checks the package registry directly via `PackageInfo.FindForAssetPath` rather than scanning assembly types.

### Default Configuration

PDM comes with a pre-configured `PreprocessorDirectiveDefiner` asset that includes common directive mappings. The default directives include:

| Namespace/Class | Directive Defined | Description |
|---|---|---|
| `UnityStandardAssets.ImageEffects` | `STANDARD_IMAGE_EFFECTS_EXIST` | Standard image effects |
| `OVRManager` | `USING_OVR` | Oculus VR integration |
| `UnityEngine.Analytics` | `ANALYTICS` | Unity Analytics |
| `Unity.RemoteConfig` | `REMOTECONFIG` | Remote Config service |
| `LiteNetLib` | `LITENETLIB` | LiteNetLib networking |
| `Unity.Netcode` | `NETCODE` | Netcode for GameObjects |
| `Unity.Netcode.Transports.UNET` | `UNET_TRANSPORT` | UNET Transport |
| `Unity.Services.Core` | `UNITY_SERVICES` | Unity Services |
| `UnityEngine.Rendering.Universal` | `URP` | Universal Render Pipeline |
| `Unity.EditorCoroutines` | `EDITORCOROUTINES` | Editor Coroutines |
| `InstaLOD` | `ENABLE_INSTALOD` | InstaLOD integration |
| `Unity.Sentis` | `UNITY_SENTIS` | Unity Sentis AI |

### Custom Configuration

To add your own directive mappings:

1. Select the `PreprocessorDirectiveDefiner` asset in your project
2. Add entries to the **Look Up Code** array:
   - **Domain Type**: Choose `Namespace`, `Class`, or `Package`
   - **If Exist**: The namespace, class name, or UPM package name to look for
   - **Define**: The preprocessor directive to define when found

### How It Works

- PDM runs automatically when the Unity Editor loads (via `[InitializeOnLoadMethod]`)
- It also re-runs whenever packages are registered or unregistered via the UPM
- Detected directives are set via `PlayerSettings.SetScriptingDefineSymbols` using `NamedBuildTarget`, making them visible in **Project Settings > Player > Scripting Define Symbols**
- You can manually trigger a refresh using the **Apply Directives** context menu on the asset
- PDM re-runs on domain reload (including after build target switches), so defines are kept up to date

### Using Directives in Code

Once PDM defines a directive, you can use it in your C# code:

```csharp
#if USING_OVR
    // Oculus VR specific code
    OVRManager.instance.Initialize();
#endif

#if URP
    // Universal Render Pipeline specific code
    var renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
#endif
```

## Optional: Odin Inspector Support

If you have [Odin Inspector](https://odininspector.com/) installed and the `ODIN_INSPECTOR` define is set, the DirectiveDefiner asset will display an **Apply** button in the Inspector for easy manual refresh.

## Migration from v1.1.x

PDM v1.2.0 replaces the `csc.rsp` file approach with Unity's `PlayerSettings.SetScriptingDefineSymbols` API:

- **Defines are now visible** in Project Settings > Player > Scripting Define Symbols
- **No more generated files** — the `Assets/csc.rsp` file is no longer written. You can safely remove any PDM-generated `csc.rsp` from your project.
- **Minimum Unity version** is now 2021.3 LTS (required for the `NamedBuildTarget` API)
- **New `Package` domain type** allows direct UPM package detection by name

## Requirements

- Unity 2021.3 LTS or later

## License

BSD 2.0 - See [LICENSE.md](LICENSE.md) for details.

## Author

Created by **Yves J. Albuquerque**