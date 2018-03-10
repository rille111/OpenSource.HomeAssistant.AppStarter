using System.Threading.Tasks;
using HomeAssistant.AppStarter;
using HomeAssistant.AppStarter.Helpers;
using HomeAssistant.AppStarter.Models.Events;
using HomeAssistant.AppStarter.WebServices;
using NLog;

namespace HassLab.Console.HassApps
{
    public class XiButtonsDimmerApp : IHassApp
    {
        public string TriggeredByEntities { get; set; } = "input_boolean.cleaning_day";
        public bool IsExecuting { get; set; }

        private readonly ILogger _logger;
        private readonly IHassWebApiServiceProxy _hassApiProxy;
        private readonly LightsHelper _lightsHelper;

        // Add your password, if any, for Home Assistant here
        private readonly string ApiPassword = "";
        // Add the address to your Home Assistant instance here
        private readonly string WebApiBaseUrl = "http://ip-address:port/";

        // Dependencies (IoC or Factories not supported yet, but would be nice with a factory that you can override to work with whatever)
        public XiButtonsDimmerApp()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _hassApiProxy = new HassWebApiServiceProxy(WebApiBaseUrl, ApiPassword);
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