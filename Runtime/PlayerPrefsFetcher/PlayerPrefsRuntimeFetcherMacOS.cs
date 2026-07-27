#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
namespace YummyDev.PlayerPrefsTool
{
    public sealed class PlayerPrefsRuntimeFetcherMacOS : PlayerPrefsRuntimeFetcherMacOSFileSystem
    {
    }
}
#endif
