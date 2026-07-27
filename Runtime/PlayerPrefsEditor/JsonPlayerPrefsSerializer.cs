using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace YummyDev.PlayerPrefsTool
{
    /// <summary>
    /// Implements <see cref="IPlayerPrefsSerializer"/> using indented JSON
    /// via Newtonsoft.Json (Unity Plastic / com.unity.nuget.newtonsoft-json).
    /// </summary>
    internal sealed class JsonPlayerPrefsSerializer : IPlayerPrefsSerializer
    {
        // ─── JSON DTO ─────────────────────────────────────────────────────────

        private class PlayerPrefDto
        {
            [JsonProperty("key")]   public string Key   { get; set; }
            [JsonProperty("type")]  public string Type  { get; set; }
            [JsonProperty("value")] public string Value { get; set; }
        }

        private class PlayerPrefsGroupDto
        {
            [JsonProperty("groups")] public Dictionary<string, List<PlayerPrefDto>> Groups { get; set; }
        }

        // ─── IPlayerPrefsSerializer ───────────────────────────────────────────

        public string Serialize(List<PlayerPrefStore> prefs)
        {
            var dtos = prefs.Select(p => new PlayerPrefDto
            {
                Key   = p.name,
                Type  = p.value.TypeId,
                Value = p.StringValue,
            }).ToList();

            return JsonConvert.SerializeObject(dtos, Formatting.Indented);
        }

        public string SerializeGroups(Dictionary<string, List<PlayerPrefStore>> groups)
        {
            var dto = new PlayerPrefsGroupDto
            {
                Groups = groups.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.Select(p => new PlayerPrefDto
                    {
                        Key   = p.name,
                        Type  = p.value.TypeId,
                        Value = p.StringValue,
                    }).ToList())
            };

            return JsonConvert.SerializeObject(dto, Formatting.Indented);
        }

        public List<PlayerPrefStore> Deserialize(string data)
        {
            List<PlayerPrefDto> dtos;

            if ((data ?? "").TrimStart().StartsWith("{"))
            {
                var grouped = JsonConvert.DeserializeObject<PlayerPrefsGroupDto>(data);
                dtos = grouped?.Groups?
                    .SelectMany(pair => pair.Value ?? new List<PlayerPrefDto>())
                    .ToList() ?? new List<PlayerPrefDto>();
            }
            else
            {
                dtos = JsonConvert.DeserializeObject<List<PlayerPrefDto>>(data)
                       ?? new List<PlayerPrefDto>();
            }

            return dtos
                .Where(d => d.Key != null && d.Type != null && d.Value != null)
                .Select(d => PlayerPrefStore.FromTypeString(d.Key, d.Type, d.Value))
                .ToList();
        }
    }
}

