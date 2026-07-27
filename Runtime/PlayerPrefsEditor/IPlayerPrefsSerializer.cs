using System.Collections.Generic;

namespace YummyDev.PlayerPrefsTool
{
    /// <summary>
    /// Converts a collection of <see cref="PlayerPrefStore"/> entries to and from
    /// a serialized string representation.
    /// </summary>
    public interface IPlayerPrefsSerializer
    {
        /// <summary>Serializes <paramref name="prefs"/> to a string.</summary>
        string Serialize(List<PlayerPrefStore> prefs);

        /// <summary>Serializes PlayerPrefs grouped by UI group name.</summary>
        string SerializeGroups(Dictionary<string, List<PlayerPrefStore>> groups);

        /// <summary>
        /// Deserializes a string previously produced by <see cref="Serialize"/>.
        /// Returns an empty list when the data contains no valid entries.
        /// </summary>
        List<PlayerPrefStore> Deserialize(string data);
    }
}

