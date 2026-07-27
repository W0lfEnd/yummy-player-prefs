#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    public sealed class PlayerPrefsRuntimeFetcherLinux : IPlayerPrefsRuntimeFetcher
    {
        public Dictionary<string, object> GetAllPlayerPrefs()
        {
            var prefs = new Dictionary<string, object>(StringComparer.Ordinal);

            try
            {
                string companyName = string.IsNullOrWhiteSpace(Application.companyName) ? "UnityDefaultCompany" : Application.companyName;
                string productName = string.IsNullOrWhiteSpace(Application.productName) ? "UnnamedProduct" : Application.productName;
                string home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string configRoot = Path.Combine(home, ".config", "unity3d", companyName);

                foreach (string path in GetCandidatePaths(configRoot, productName))
                {
                    if (!File.Exists(path))
                        continue;

                    Debug.Log($"[PlayerPrefsRuntime] Fetching Linux PlayerPrefs from: {path}");
                    ParsePrefsFile(path, prefs);
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[PlayerPrefsRuntime] Error fetching PlayerPrefs on Linux: {e.Message}\n{e.StackTrace}");
            }

            return prefs;
        }

        private static IEnumerable<string> GetCandidatePaths(string configRoot, string productName)
        {
            yield return Path.Combine(configRoot, productName);
            yield return Path.Combine(configRoot, productName, "prefs");
            yield return Path.Combine(configRoot, productName + ".prefs");
        }

        private static void ParsePrefsFile(string path, Dictionary<string, object> prefs)
        {
            string data = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(data))
                return;

            if (data.TrimStart().StartsWith("<", StringComparison.Ordinal))
            {
                ParseXmlPrefs(data, prefs);
                return;
            }

            ParseLinePrefs(data, prefs);
        }

        private static void ParseXmlPrefs(string xml, Dictionary<string, object> prefs)
        {
            var document = new XmlDocument { XmlResolver = null };
            document.LoadXml(xml);

            foreach (XmlNode node in document.SelectNodes("//*[@name or @key]"))
            {
                string key = node.Attributes?["name"]?.Value ?? node.Attributes?["key"]?.Value;
                if (string.IsNullOrEmpty(key))
                    continue;

                string type = node.Attributes?["type"]?.Value;
                string raw = node.Attributes?["value"]?.Value ?? node.InnerText;
                prefs[key] = ParseTypedValue(type, raw);
            }
        }

        private static void ParseLinePrefs(string data, Dictionary<string, object> prefs)
        {
            using (var reader = new StringReader(data))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int separator = line.IndexOf('=');
                    if (separator <= 0)
                        continue;

                    string key = line.Substring(0, separator).Trim();
                    string raw = line.Substring(separator + 1).Trim();
                    if (!string.IsNullOrEmpty(key))
                        prefs[key] = ParseTypedValue(null, raw);
                }
            }
        }

        private static object ParseTypedValue(string type, string raw)
        {
            if (string.Equals(type, "int", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(type, "integer", StringComparison.OrdinalIgnoreCase))
            {
                return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue)
                    ? intValue
                    : 0;
            }

            if (string.Equals(type, "float", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(type, "real", StringComparison.OrdinalIgnoreCase))
            {
                return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue)
                    ? floatValue
                    : 0f;
            }

            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedInt))
                return parsedInt;

            if (float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsedFloat))
                return parsedFloat;

            return raw ?? "";
        }
    }
}
#endif
