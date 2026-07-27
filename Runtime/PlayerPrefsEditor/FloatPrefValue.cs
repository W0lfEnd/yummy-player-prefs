using System.Globalization;
using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    public sealed class FloatPrefValue : PrefValue
    {
        public float Value { get; set; }

        public FloatPrefValue(float value = 0f) { Value = value; }

        public override string TypeId          => "real";
        public override string TypeDisplayName => "Float";
        public override string StringValue     =>
            Value.ToString("G", CultureInfo.InvariantCulture);

        public override bool TrySetFromString(string raw)
        {
            if (!float.TryParse(raw, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var v)) return false;
            Value = v;
            return true;
        }

        public override void WriteToPlayerPrefs(string key) => PlayerPrefs.SetFloat(key, Value);
        public override PrefValue Clone()                   => new FloatPrefValue(Value);

        public override bool ValueEquals(PrefValue other) =>
            other is FloatPrefValue fp && fp.Value == Value;

        public override PrefValue ConvertTo(string targetTypeId) => targetTypeId switch
        {
            "integer" => new IntPrefValue((int)Value),
            "real"    => Clone(),
            _         => new StringPrefValue(StringValue),
        };
    }
}