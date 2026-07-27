using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    public sealed class IntPrefValue : PrefValue
    {
        public int Value { get; set; }

        public IntPrefValue(int value = 0) { Value = value; }

        public override string TypeId          => "integer";
        public override string TypeDisplayName => "Int";
        public override string StringValue     => Value.ToString();

        public override bool TrySetFromString(string raw)
        {
            if (!int.TryParse(raw, out var v)) return false;
            Value = v;
            return true;
        }

        public override void WriteToPlayerPrefs(string key) => PlayerPrefs.SetInt(key, Value);
        public override PrefValue Clone()                   => new IntPrefValue(Value);

        public override bool ValueEquals(PrefValue other) =>
            other is IntPrefValue ip && ip.Value == Value;

        public override PrefValue ConvertTo(string targetTypeId) => targetTypeId switch
        {
            "integer" => Clone(),
            "real"    => new FloatPrefValue(Value),
            _         => new StringPrefValue(Value.ToString()),
        };
    }
}