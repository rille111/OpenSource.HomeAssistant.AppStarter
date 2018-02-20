using System.Threading.Tasks;
using HomeAssistant.AppStarter.Models.Events;

namespace HomeAssistant.AppStarter
{
    public interface IHassApp
    {
        /// <summary>
        /// HassAppRunner uses this in order to avoid simultaneous executions. If you for example spam a button and <see cref="ExecuteAsync"/> already is running, it won't be executed.
        /// Future improvement would be to tag the app to decide how concurrency is handled. For example [DisallowConcurrentExecutions] like Quartz.net has it.
        /// </summary>
        bool IsExecuting { get; set; }
        /// <summary>
        /// The entity identifier(s) that we listen on. Separate multiple id's with comma, and/or use wildcard *.
        /// Examples: 'automation.wake_up*' or 'automation.wake_up_1, automation.wake_up_2' or 'sensor.my_sensor'
        /// </summary>
        string TriggeredByEntities { get; set; }
        /// <summary>
        /// Implement this with your own code.
        /// </summary>
        /// <param name="e">The json from Hass is parsed for the most common properties.</param>
        /// <param name="rawData">The raw untouched data containing the entire websocket message. It's json so you can parse it yourself.</param>
        /// <returns></returns>
        Task ExecuteAsync(EventData e, string rawData);
    }
}
