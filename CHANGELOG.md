Changelog
=========

[1.0.2] - 2026-07-27
--------------------
* **Fixed**
    * Regenerate the editor PlayerPrefs folder GUID to avoid conflicts with options-provider.

[1.0.1] - 2026-07-27
--------------------
* **Fixed**
    * Compile the editor window wrapper in the same assembly as the shared PlayerPrefs editor view.

[1.0.0] - 2026-07-27
--------------------
* **Added**
    * Initial Yummy Player Prefs package.
    * PlayerPrefs editor window with filtering, editing, grouping, favorites, snapshots, JSON import/export, and bulk actions.
    * Runtime UI Toolkit debug panel for inspecting PlayerPrefs in builds.
    * Platform-aware runtime fetchers for Editor, Windows, macOS, Linux, Android, and iOS.
    * PlayerPrefsProviderBase for strongly typed PlayerPrefs-backed settings classes.
    * DeferredPlayerPrefsSaver for delayed PlayerPrefs.Save() flushing.
