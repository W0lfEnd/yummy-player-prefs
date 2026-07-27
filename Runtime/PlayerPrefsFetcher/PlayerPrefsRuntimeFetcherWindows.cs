#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace YummyDev.PlayerPrefsTool
{
    public class PlayerPrefsRuntimeFetcherWindows : IPlayerPrefsRuntimeFetcher
    {
        public Dictionary<string, object> GetAllPlayerPrefs()
        {
            try
            {
                string companyName = string.IsNullOrWhiteSpace(Application.companyName) ? "UnityDefaultCompany" : Application.companyName;
                string productName = string.IsNullOrWhiteSpace(Application.productName) ? "UnnamedProduct" : Application.productName;
                string registryPath = $@"Software\{companyName}\{productName}";

                Debug.Log($"[PlayerPrefsRuntime] Fetching Windows PlayerPrefs from registry path: {registryPath}");
                return PlayerPrefsRuntimeWindowsRegistryReader.ReadPlayerPrefs(registryPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"[PlayerPrefsRuntime] Error fetching PlayerPrefs on Windows: {e.Message}\n{e.StackTrace}");
            }

            return new Dictionary<string, object>(StringComparer.Ordinal);
        }
    }
}
#endif
