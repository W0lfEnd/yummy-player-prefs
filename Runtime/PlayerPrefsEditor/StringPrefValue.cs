using System.Globalization;
using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    public sealed class StringPrefValue : PrefValue
    {
        public string Value { get; set; } = "";

        public StringPrefValue(string value = "") { Value = value ?? ""; }

        public override string TypeId          => "string";
        public override string TypeDisplayName => "String";
        public override string StringValue     => Value;

        public override bool TrySetFromString(string raw)
        {
            Value = raw ?? "";
            return true; // strings are always valid
        }

        public override void WriteToPlayerPrefs(string key) => PlayerPrefs.SetString(key, Value);
        public override PrefValue Clone()                   => new StringPrefValue(Value);

        public override bool ValueEquals(PrefValue other) =>
            other is StringPrefValue sp && sp.Value == Value;

        public override PrefValue ConvertTo(string targetTypeId) => targetTypeId switch
        {
            "integer" => new IntPrefValue(
                int.TryParse(Value, out var iv) ? iv : 0),
            "real" => new FloatPrefValue(
                float.TryParse(Value, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var fv) ? fv : 0f),
            _ => Clone(),
        };
    }
}