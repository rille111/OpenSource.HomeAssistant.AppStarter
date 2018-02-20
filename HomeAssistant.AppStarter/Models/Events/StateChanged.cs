using System.Collections.Generic;

namespace Rille.Hass.AppStarter.Models.Events
{
    public class StateChanged
    {
        public string NewState { get; set; }
        public string OldState { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
    }
}
