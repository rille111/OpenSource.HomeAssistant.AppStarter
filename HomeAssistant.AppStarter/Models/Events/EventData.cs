namespace HomeAssistant.AppStarter.Models.Events
{
    public class EventData
    {
        public string EntityId { get; set; }
        public Click ClickData { get; set; }
        public StateChanged StateChangeData { get; set; }
    }
}
