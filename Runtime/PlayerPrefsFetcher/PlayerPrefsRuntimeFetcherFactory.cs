using System;
using System.Collections.Generic;
using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    public static class PlayerPrefsRuntimeFetcherFactory
    {
        public static IPlayerPrefsRuntimeFetcher Create()
        {
#if UNITY_EDITOR_WIN
            return new PlayerPrefsRuntimeFetcherWindowsEditor();
#elif UNITY_EDITOR_OSX
            return new PlayerPrefsRuntimeFetcherMacOSEditor();
#elif UNITY_STANDALONE_WIN
            return new PlayerPrefsRuntimeFetcherWindows();
#elif UNITY_STANDALONE_OSX
            return new PlayerPrefsRuntimeFetcherMacOS();
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            return new PlayerPrefsRuntimeFetcherLinux();
#elif UNITY_ANDROID
            return new PlayerPrefsRuntimeFetcherAndroid();
#elif UNITY_IOS
            return new PlayerPrefsRuntimeFetcherIOS();
#else
            return new PlayerPrefsRuntimeFetcherUnavailable();
#endif
        }

        private sealed class PlayerPrefsRuntimeFetcherUnavailable : IPlayerPrefsRuntimeFetcher
        {
            public Dictionary<string, object> GetAllPlayerPrefs()
            {
                Debug.LogWarning("[PlayerPrefsRuntime] No PlayerPrefs fetcher is available for this platform.");
                return new Dictionary<string, object>(StringComparer.Ordinal);
            }
        }
    }
}
