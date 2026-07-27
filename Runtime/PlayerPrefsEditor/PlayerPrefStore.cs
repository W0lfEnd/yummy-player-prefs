using System.Globalization;

namespace YummyDev.PlayerPrefsTool
{
    /// <summary>
    /// Holds the key name, current value and baseline (initial) value for one
    /// PlayerPref entry.  All type-specific logic is delegated to the
    /// <see cref="PrefValue"/> hierarchy – no switches, no enums here.
    /// </summary>
    public class PlayerPrefStore
    {
        // ── Fields ────────────────────────────────────────────────────────────

        public string   name;
        public bool     isMarkedForDelete;

        /// <summary>True when the entry was created in this editor session and not yet saved.</summary>
        public bool     isNew;

        /// <summary>Current (possibly unsaved) value.</summary>
        public PrefValue value;

        /// <summary>Value at the time of the last Save – baseline for change detection.</summary>
        public PrefValue initial;

        private string _cachedSearchNameSource;
        private string _cachedSearchName;
        private string _cachedSearchValueSource;
        private string _cachedSearchValue;

        // ── Derived ──────────────────────────────────────────────────────────

        public string StringValue => value.StringValue;
        public string StringType  => value.TypeId;

        public string SearchName
        {
            get
            {
                string current = name ?? "";
                if (_cachedSearchNameSource != current)
                {
                    _cachedSearchNameSource = current;
                    _cachedSearchName       = current.ToLowerInvariant();
                }
                return _cachedSearchName;
            }
        }

        public string SearchValue
        {
            get
            {
                string current = StringValue ?? "";
                if (_cachedSearchValueSource != current)
                {
                    _cachedSearchValueSource = current;
                    _cachedSearchValue       = current.ToLowerInvariant();
                }
                return _cachedSearchValue;
            }
        }

        /// <summary>
        /// True when the current value differs from the last-saved baseline.
        /// New entries always return false (they show as "new", not "edited").
        /// </summary>
        public bool Changed => !isNew && !value.ValueEquals(initial);

        // ── Constructors ──────────────────────────────────────────────────────

        public PlayerPrefStore(string name, PrefValue initialValue)
        {
            this.name = name;
            value     = initialValue;
            initial   = initialValue.Clone();
        }

        /// <summary>Convenience factory: build from a type-id string and a raw value string.</summary>
        public static PlayerPrefStore FromTypeString(string name, string typeId, string raw)
            => new PlayerPrefStore(name, PrefValue.Create(typeId, raw));

        // ── Operations ────────────────────────────────────────────────────────

        /// <summary>Restores value to the last-saved baseline.</summary>
        public void Reset() => value = initial.Clone();

        /// <summary>
        /// Commits the current value as the new baseline and clears the
        /// <see cref="isNew"/> flag (called after a successful PlayerPrefs.Save).
        /// </summary>
        public void Save()
        {
            isNew   = false;
            initial = value.Clone();
        }
    }
}
