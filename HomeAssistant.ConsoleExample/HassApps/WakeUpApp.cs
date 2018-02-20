using System.Threading.Tasks;
using NLog;
using Rille.Hass.AppStarter;
using Rille.Hass.AppStarter.Models.Events;
using Rille.Hass.AppStarter.WebServices;

namespace HassLab.Console.HassApps
{
    public class WakeUpApp : IHassApp
    {
        public string TriggeredByEntities { get; set; } = "automation.wakeup_*"; // <-- yes, wildcards are supported!

        // Dependencies (IoC or Factories not supported right now)
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly HassWebApiServiceProxy _hassApiProxy = new HassWebApiServiceProxy("http://192.168.0.201:8123/api");

        public async Task ExecuteAsync(EventData e, string rawData)
        {
            if (e.StateChangeData.OldState != "on" || e.StateChangeData.NewState != "on")
                return; // A trigger for an automation has both these states set to "on" by some reason.

            _logger.Info($"Executing {nameof(WakeUpApp)} for [{e.EntityId}]");

            // Turn on all lamps
            await TurnOnLightFor("light.dimmer_vardagsrum_level", 155);
            await TurnOnLightFor("light.dimmer_minihall_level", 155);
            await TurnOnLightFor("light.dimmer_hall_level", 155);
            await TurnOnLightFor("light.led_sovrum_tak_level", 255);
        }

        public bool IsExecuting { get; set; }

        private async Task TurnOnLightFor(string entity_id, int brightness)
        {
            await _hassApiProxy.CallHassService("light", "turn_on", new {entity_id, brightness});
        }
    }
}