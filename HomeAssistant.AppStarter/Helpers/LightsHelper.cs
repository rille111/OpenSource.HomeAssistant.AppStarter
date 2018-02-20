using System.Threading.Tasks;
using HomeAssistant.AppStarter.WebServices;
using Newtonsoft.Json.Linq;

namespace HomeAssistant.AppStarter.Helpers
{
    /// <summary>
    /// Maybe this helper should be in the HassAppsRunner, for ease of use for consumers?
    /// </summary>
    public class LightsHelper
    {
        private readonly IHassWebApiServiceProxy _hassApiProxy;


        public LightsHelper(IHassWebApiServiceProxy hassApiProxy1)
        {
            _hassApiProxy = hassApiProxy1;
        }

        public int IncreaseBrightness(int currentBrightness)
        {
            var newBrightness = currentBrightness += 25;
            if (newBrightness > 255)
                newBrightness = 255;
            return newBrightness;
        }

        public int DecreaseBrightness(int currentBrightness)
        {
            var newBrightness = currentBrightness -= 25;
            if (newBrightness < 0)
                newBrightness = 0;
            return newBrightness;
        }


        public async Task<int> GetCurrentBrightnessFor(string entity_id)
        {
            var jsonString = await _hassApiProxy.GetHassEntityStateAsJson(entity_id);
            var jsonToken = JToken.Parse(jsonString);
            if ((string) jsonToken["state"] == "off")
                return 0;

            return (int)jsonToken["attributes"]["brightness"];
        }

        public async Task TurnOnLightFor(string entity_id, int brightness)
        {
            await _hassApiProxy.CallHassService("light", "turn_on", new { entity_id, brightness });
        }

    }
}