using System.Globalization;
using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    // =========================================================================
    // Abstract base
    // =========================================================================

    /// <summary>
    /// Replaces the old <c>PrefType</c> enum and all associated switch statements.
    /// Each concrete subclass owns a specific PlayerPrefs type and all operations
    /// that depend on it (parsing, saving, converting, comparing).
    /// </summary>
    public abstract class PrefValue
    {
        // ── Type identity ─────────────────────────────────────────────────────

        /// <summary>Registry / plist type id: "integer" | "real" | "string".</summary>
        public abstract string TypeId { get; }

        /// <summary>Short display name used in the editor UI: "Int" | "Float" | "String".</summary>
        public abstract string TypeDisplayName { get; }

        // ── Value ─────────────────────────────────────────────────────────────

        /// <summary>Current value rendered as a string (used for display and Value-column sorting).</summary>
        public abstract string StringValue { get; }

        // ── Mutation ──────────────────────────────────────────────────────────

        /// <summary>
        /// Tries to parse <paramref name="raw"/> into this value's native type.
        /// Returns <c>true</c> and mutates <c>this</c> on success;
        /// returns <c>false</c> and leaves <c>this</c> unchanged on failure.
        /// </summary>
        public abstract bool TrySetFromString(string raw);

        /// <summary>Persists the value to <see cref="PlayerPrefs"/> under <paramref name="key"/>.</summary>
        public abstract void WriteToPlayerPrefs(string key);

        // ── Structural ────────────────────────────────────────────────────────

        /// <summary>Deep copy with the same type and value.</summary>
        public abstract PrefValue Clone();

        /// <summary>Returns <c>true</c> when <paramref name="other"/> has the same type AND value.</summary>
        public abstract bool ValueEquals(PrefValue other);

        /// <summary>
        /// Returns a <em>new</em> <see cref="PrefValue"/> whose type is
        /// <paramref name="targetTypeId"/>, converting the current numeric/string
        /// content as accurately as possible (lossy for numeric truncation).
        /// </summary>
        public abstract PrefValue ConvertTo(string targetTypeId);

        // ── Static helpers ────────────────────────────────────────────────────

        public static readonly string[] AllTypeIds          = { "integer", "real", "string" };
        public static readonly string[] AllTypeDisplayNames = { "Int",     "Float", "String" };

        public static string TypeIdToDisplay(string typeId) => typeId switch
        {
            "integer" => "Int",
            "real"    => "Float",
            _         => "String",
        };

        public static string DisplayToTypeId(string display) => display switch
        {
            "Int"   => "integer",
            "Float" => "real",
            _       => "string",
        };

        /// <summary>Factory — creates a <see cref="PrefValue"/> from a type id and a raw string.</summary>
        public static PrefValue Create(string typeId, string raw)
        {
            switch (typeId)
            {
                case "integer":
                    int.TryParse(raw, out var iv);
                    return new IntPrefValue(iv);

                case "real":
                    float.TryParse(raw, NumberStyles.Float,
                        CultureInfo.InvariantCulture, out var fv);
                    return new FloatPrefValue(fv);

                default:
                    return new StringPrefValue(raw ?? "");
            }
        }
    }
}

