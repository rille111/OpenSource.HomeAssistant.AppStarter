using System.Threading.Tasks;
using NLog;
using Rille.Hass.AppStarter;
using Rille.Hass.AppStarter.Helpers;
using Rille.Hass.AppStarter.Models.Events;
using Rille.Hass.AppStarter.WebServices;

namespace HassLab.Console.HassApps
{
    public class XiButtonsDimmerApp : IHassApp
    {
        public string TriggeredByEntities { get; set; } = "binary_sensor.switch_158d00016d75aa, binary_sensor.switch_158d0001a654b8";
        public bool IsExecuting { get; set; }

        private readonly ILogger _logger;
        private readonly IHassWebApiServiceProxy _hassApiProxy;
        private readonly LightsHelper _lightsHelper;

        // Dependencies (IoC or Factories not supported yet, but would be nice with a factory that you can override to work with whatever)
        public XiButtonsDimmerApp()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _hassApiProxy = new HassWebApiServiceProxy("http://192.168.0.201:8123/api");
            _lightsHelper = new LightsHelper(_hassApiProxy);
        }

        public async Task ExecuteAsync(EventData e, string rawData)
        {

            _logger.Info($"Executing {nameof(XiButtonsDimmerApp)} for [{e.EntityId}]");

            var newBrightness = 0;
            var currentBrightness = await _lightsHelper.GetCurrentBrightnessFor("light.dimmer_vardagsrum_level");

            if (e.ClickData.ClickType == "single" || e.ClickData.ClickType == "double")
            {
                if (e.EntityId == "binary_sensor.switch_158d00016d75aa")
                    newBrightness = _lightsHelper.IncreaseBrightness(currentBrightness);
                else
                    newBrightness = _lightsHelper.DecreaseBrightness(currentBrightness);
            }
            else
            {
                if (e.EntityId == "binary_sensor.switch_158d00016d75aa")
                    newBrightness = _lightsHelper.IncreaseBrightness(255);
                else
                    newBrightness = _lightsHelper.DecreaseBrightness(0);
            }

            await _lightsHelper.TurnOnLightFor("light.dimmer_vardagsrum_level", newBrightness);
            await _lightsHelper.TurnOnLightFor("light.dimmer_minihall_level", newBrightness);
        }
    }
}