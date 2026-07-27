using System;
using System.Collections.Generic;
using System.Globalization;

namespace YummyDev.PlayerPrefsTool
{
    internal static class PlayerPrefsRuntimeStoreConverter
    {
        public static List<PlayerPrefStore> ReadAll(IPlayerPrefsRuntimeFetcher fetcher)
        {
            var result = new List<PlayerPrefStore>();
            if (fetcher == null)
                return result;

            Dictionary<string, object> prefs = fetcher.GetAllPlayerPrefs()
                ?? new Dictionary<string, object>(StringComparer.Ordinal);

            foreach (var pair in prefs)
            {
                if (string.IsNullOrEmpty(pair.Key) || IsEditorMetadataKey(pair.Key))
                    continue;

                result.Add(ToStore(pair.Key, pair.Value));
            }

            return result;
        }

        public static bool IsEditorMetadataKey(string key) =>
            !string.IsNullOrEmpty(key) &&
            key.StartsWith(PlayerPrefsEditorMetadata.PrefsKeyPrefix, StringComparison.Ordinal);

        private static PlayerPrefStore ToStore(string key, object value)
        {
            switch (value)
            {
                case int intValue:
                    return PlayerPrefStore.FromTypeString(key, "integer", intValue.ToString(CultureInfo.InvariantCulture));
                case long longValue when longValue >= int.MinValue && longValue <= int.MaxValue:
                    return PlayerPrefStore.FromTypeString(key, "integer", longValue.ToString(CultureInfo.InvariantCulture));
                case float floatValue:
                    return PlayerPrefStore.FromTypeString(key, "real", floatValue.ToString("G", CultureInfo.InvariantCulture));
                case double doubleValue:
                    return PlayerPrefStore.FromTypeString(key, "real", doubleValue.ToString("G", CultureInfo.InvariantCulture));
                case bool boolValue:
                    return PlayerPrefStore.FromTypeString(key, "integer", boolValue ? "1" : "0");
                case byte[] bytes:
                    return PlayerPrefStore.FromTypeString(key, "string", Convert.ToBase64String(bytes));
                default:
                    return PlayerPrefStore.FromTypeString(key, "string", Convert.ToString(value, CultureInfo.InvariantCulture) ?? "");
            }
        }
    }
}
