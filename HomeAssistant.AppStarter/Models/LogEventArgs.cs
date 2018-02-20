using System;

namespace HomeAssistant.AppStarter.Models
{
    /// <summary>
    /// <see cref="HassAppsRunner"/> outputs this as an object to all Log events
    /// </summary>
    public class LogEventArgs
    {
        public string Text { get; set; }
        public Exception Exception { get; set; }
    }
}