using System;

namespace Rille.Hass.AppStarter.RawModels
{
    /// <summary>
    /// Deserialized as-is from Hass
    /// </summary>
    internal class HassEventRawModel
    {
        internal EventRaw @event { get; set; }
        internal string type { get; set; }
        internal int id { get; set; }
    }

    internal class EventRaw
    {
        internal string event_type { get; set; }
        internal DateTime? time_fired { get; set; }
        internal string origin { get; set; }
        internal DataRaw data { get; set; }
    }

    internal class DataRaw
    {
        internal StateRaw new_state { get; set; }
        internal StateRaw old_state { get; set; }
        internal string entity_id { get; set; }
    }

    internal class StateRaw
    {
        internal DateTime? last_changed { get; set; }
        internal object attributes { get; set; }
        internal string entity_id { get; set; }
        internal string state { get; set; }
        internal DateTime? last_updated { get; set; }
    }

    internal class AttributesRaw
    {
        internal DateTime? last_triggered { get; set; }
        internal string friendly_name { get; set; }
        internal string id { get; set; }
    }
    
}
