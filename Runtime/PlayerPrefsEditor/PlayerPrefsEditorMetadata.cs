using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YummyDev.PlayerPrefsTool
{
    internal static class PlayerPrefsEditorMetadata
    {
        public const string PrefsKeyPrefix = "__uapp_playerprefs_editor.";

        public static bool HasString(string key)
        {
#if UNITY_EDITOR
            return EditorPrefs.HasKey(ProjectKey(key));
#else
            return PlayerPrefs.HasKey(ProjectKey(key));
#endif
        }

        public static string GetString(string key, string fallback = "")
        {
#if UNITY_EDITOR
            return EditorPrefs.GetString(ProjectKey(key), fallback);
#else
            return PlayerPrefs.GetString(ProjectKey(key), fallback);
#endif
        }

        public static void SetString(string key, string value)
        {
#if UNITY_EDITOR
            EditorPrefs.SetString(ProjectKey(key), value ?? "");
#else
            PlayerPrefs.SetString(ProjectKey(key), value ?? "");
            PlayerPrefs.Save();
#endif
        }

        public static float GetFloat(string key, float fallback)
        {
#if UNITY_EDITOR
            return EditorPrefs.GetFloat(ProjectKey(key), fallback);
#else
            return PlayerPrefs.GetFloat(ProjectKey(key), fallback);
#endif
        }

        public static void SetFloat(string key, float value)
        {
#if UNITY_EDITOR
            EditorPrefs.SetFloat(ProjectKey(key), value);
#else
            PlayerPrefs.SetFloat(ProjectKey(key), value);
            PlayerPrefs.Save();
#endif
        }

        private static string ProjectKey(string key) =>
            PrefsKeyPrefix + key + "." + Application.identifier;
    }
}
