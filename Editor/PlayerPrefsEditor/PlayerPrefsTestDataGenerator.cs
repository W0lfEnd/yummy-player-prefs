using UnityEditor;
using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    /// <summary>
    /// Stand-alone utility for generating and removing synthetic PlayerPrefs entries.
    /// Completely independent from <see cref="PlayerPrefsEditor"/> — can be invoked
    /// via the menu or called programmatically from tests / other editor tooling.
    /// </summary>
    public static class PlayerPrefsTestDataGenerator
    {
        public const int    DefaultCount = 1000;
        public const string KeyPrefix   = "__test__";

        private static readonly string[] Categories =
        {
            "player", "settings", "cache", "game",
            "ui",     "audio",   "network", "analytics",
        };

        // ── Menu items ────────────────────────────────────────────────────────

        [MenuItem("Tools/Yummy Player Prefs/Test Data/Generate 1 000 entries", false, 100)]
        public static void GenerateViaMenu() => Generate(DefaultCount, confirmDialog: true);

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Writes <paramref name="count"/> PlayerPrefs entries and calls
        /// <see cref="PlayerPrefs.Save"/>.
        /// Keys follow the pattern <c>__test__{category}.entry_NNNN</c>.
        /// </summary>
        public static void Generate(int count = DefaultCount, bool confirmDialog = false)
        {
            if (confirmDialog && !EditorUtility.DisplayDialog(
                    "Generate Test Data",
                    $"Write {count} PlayerPrefs entries prefixed with \"{KeyPrefix}\"?\n" +
                    "Existing test entries with the same keys will be overwritten.",
                    "Generate", "Cancel"))
                return;
            
            for (int i = 0; i < count; i++)
            {
                string cat = Categories[i % Categories.Length];
                string key = $"{KeyPrefix}{cat}.entry_{Random.Range(0, 100_000_000)}";

                switch (i % 3)
                {
                    case 0:
                        PlayerPrefs.SetInt(key, Random.Range(0, 100_000));
                        break;
                    case 1:
                        PlayerPrefs.SetFloat(key, (float)(Random.value * 1000.0));
                        break;
                    default:
                        PlayerPrefs.SetString(key, $"value_{i}_{cat}");
                        break;
                }
            }

            PlayerPrefs.Save();
        }
    }
}

