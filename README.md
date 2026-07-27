# Yummy Player Prefs

Yummy Player Prefs is a Unity package for inspecting and editing `PlayerPrefs` while developing, debugging, and testing games or apps. It includes a full editor window, a runtime UI Toolkit panel, JSON import/export, snapshots, grouping, and small helper APIs for PlayerPrefs-backed settings.

## Features

- Editor window for viewing all available `PlayerPrefs` keys.
- Add, edit, restore, duplicate-check, delete, and bulk-delete preferences.
- Filter by key, type, and value.
- Group keys into custom tabs, mark favorites, and hide ignored keys.
- Save and load local snapshots from `Application.persistentDataPath`.
- Export and import PlayerPrefs as JSON, including selected groups.
- Runtime UI Toolkit panel for debug builds and device testing.
- Platform-aware PlayerPrefs fetchers for Editor, Windows, macOS, Linux, Android, and iOS.
- `DeferredPlayerPrefsSaver` for batching `PlayerPrefs.Save()` calls.
- `PlayerPrefsProviderBase` for strongly typed PlayerPrefs-backed settings classes.

## Installation

Install from Unity Package Manager with this Git URL:

```text
https://github.com/W0lfEnd/yummy-player-prefs.git#v1.0.3
```

Or add it directly to `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.yummydev.player-prefs": "https://github.com/W0lfEnd/yummy-player-prefs.git#v1.0.3"
  }
}
```

The package depends on `com.unity.nuget.newtonsoft-json`, which is declared in `package.json`.

## Usage

Open the editor window from:

```text
Tools > Yummy Player Prefs > Editor
```

Generate test data from:

```text
Tools > Yummy Player Prefs > Test Data > Generate 1 000 entries
```

To create a runtime debug panel:

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using YummyDev.PlayerPrefsTool;

public sealed class PlayerPrefsDebugPanelBootstrap : MonoBehaviour
{
    [SerializeField] private PanelSettings panelSettings;

    private void Awake()
    {
        var settings = panelSettings != null
            ? panelSettings
            : PlayerPrefsEditorRuntimePanel.CreateDefaultPanelSettings();

        var panel = PlayerPrefsEditorRuntimePanel.Create(settings);
        DontDestroyOnLoad(panel.gameObject);
    }
}
```

Use the runtime panel only in development, QA, or debug builds unless your product intentionally exposes PlayerPrefs editing to users.

## Package Info

- Package id: `com.yummydev.player-prefs`
- Display name: `Yummy Player Prefs`
- Namespace: `YummyDev.PlayerPrefsTool`
- Runtime assembly: `Yummy.PlayerPrefs`
- Editor assembly: `Yummy.PlayerPrefs.Editor`
- Unity version: `2021.3` or newer
- License: MIT
