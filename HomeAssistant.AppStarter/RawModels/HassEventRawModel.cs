using Newtonsoft.Json;
using System;

namespace HomeAssistant.AppStarter.RawModels
{
    /// <summary>
    /// Deserialized as-is from Hass
    /// </summary>
    internal class HassEventRawModel
    {
        [JsonProperty("event")]
        internal EventRaw @event { get; set; }
        [JsonProperty("type")]
        internal string type { get; set; }
        [JsonProperty("id")]
        internal int id { get; set; }
    }

    internal class EventRaw
    {
        [JsonProperty("event_type")]
        internal string event_type { get; set; }
        [JsonProperty("time_fired")]
        internal DateTime? time_fired { get; set; }
        [JsonProperty("origin")]
        internal string origin { get; set; }
        [JsonProperty("data")]
        internal DataRaw data { get; set; }
    }

    internal class DataRaw
    {
        [JsonProperty("new_state")]
        internal StateRaw new_state { get; set; }
        [JsonProperty("old_state")]
        internal StateRaw old_state { get; set; }
        [JsonProperty("entity_id")]
        internal string entity_id { get; set; }
    }

    internal class StateRaw
    {
        [JsonProperty("last_changed")]
        internal DateTime? last_changed { get; set; }
        [JsonProperty("attributes")]
        internal object attributes { get; set; }
        [JsonProperty("entity_id")]
        internal string entity_id { get; set; }
        [JsonProperty("state")]
        internal string state { get; set; }
        [JsonProperty("last_updated")]
        internal DateTime? last_updated { get; set; }
    }

    internal class AttributesRaw
    {
        [JsonProperty("last_triggered")]
        internal DateTime? last_triggered { get; set; }
        [JsonProperty("friendly_name")]
        internal string friendly_name { get; set; }
        [JsonProperty("id")]
        internal string id { get; set; }
    }
    
}
