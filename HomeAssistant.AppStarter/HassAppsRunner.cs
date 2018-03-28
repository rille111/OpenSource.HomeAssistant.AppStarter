using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeAssistant.AppStarter.Extensions;
using HomeAssistant.AppStarter.Models;
using HomeAssistant.AppStarter.Models.Events;
using HomeAssistant.AppStarter.Models.WebsocketCommands;
using HomeAssistant.AppStarter.RawModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace HomeAssistant.AppStarter
{
    // TODO: Implement IDisposable to call Stop()
    /// <summary>
    /// Connects to a websocket from Hass to catch events. See https://home-assistant.io/developers/websocket_api/
    /// </summary>
    public class HassAppsRunner
    {
        // Events
        public event EventHandler<LogEventArgs> TraceOutput;
        public event EventHandler<LogEventArgs> DebugOutput;
        public event EventHandler<LogEventArgs> InfoOutput;
        public event EventHandler<LogEventArgs> WarnOutput;
        public event EventHandler<LogEventArgs> ErrorOutput;

        // Fields
        private readonly string _hassWebsocketUri;
        private readonly string _apiPasswordQuery;
        private bool _inited;
        /// <summary>
        /// TODO: This lib needs to be .NET Core
        /// </summary>
        private WebSocket _ws;

        // Props
        public HashSet<string> EncounteredEntityIdsWithoutSubscription { get; set; } = new HashSet<string>();
        private Dictionary<string, List<IHassApp>> _apps;
        private bool _started;

        // Ctor
        /// <param name="hassWebsocketUri">Example: 'ws://192.168.0.168:8123/api/websocket' </param>
        public HassAppsRunner(string hassWebsocketUri)
        {
            _hassWebsocketUri = hassWebsocketUri;
        }
        // Ctor
        /// <param name="hassWebsocketUri">Example: 'ws://192.168.0.168:8123/api/websocket' </param>
        /// <param name="apiPassword"></param>
        public HassAppsRunner(string hassWebsocketUri, string apiPassword)
        {
            _hassWebsocketUri = hassWebsocketUri;
            _apiPasswordQuery = $"?api_password={apiPassword}";
        }

        // Public 

        /// <summary>
        /// Start a websocket and connect, subscribe to hass state_changed events
        /// </summary>
        public void Start()
        {
            if (!_inited)
                Initialize();
            if (_started)
                throw new InvalidOperationException($"{nameof(HassAppsRunner)} already started!");

            if(string.IsNullOrEmpty(_apiPasswordQuery))
            {
                _ws = new WebSocket(_hassWebsocketUri);
            }
            else
            {
                _ws = new WebSocket($"{_hassWebsocketUri}{_apiPasswordQuery}");
            }
            _ws.Log.Output += OnWebsocketLog;
            _ws.OnError += OnError;
            _ws.OnMessage += OnMessage;
            _ws.Connect();

            SubscribeToEvents();

            _started = true;
        }

        /// <summary>
        /// Disconnect and dispose websocket, kill subscriptions and let go of IHassApp instances
        /// </summary>
        public void Stop()
        {
            if (_ws == null)
                return;

            if (_ws.ReadyState == WebSocketState.Connecting || _ws.ReadyState == WebSocketState.Open)
                _ws.Close();

            _ws = null;
            _started = false;
        }

        // Private 

        /// <summary>
        /// * Scans assembly for implementations of <see cref="IHassApp"/>
        /// * Instantiates and connects them by their EntityId ids that they are listening to
        /// </summary>
        private void Initialize()
        {
            var apps = ScanAssemblyForHassApps();

            _apps = new Dictionary<string, List<IHassApp>>();

            foreach (var app in apps)
            {
                // TODO: Replace this to support ctor injection.
                var instance = (IHassApp)Activator.CreateInstance(app);

                // Clean up the filter
                instance.TriggeredByEntities = instance.TriggeredByEntities.ToLowerInvariant().Replace(" ", "");

                // Empty or null not allowed
                if (string.IsNullOrEmpty(instance.TriggeredByEntities))
                    throw new ArgumentNullException($"{nameof(IHassApp.TriggeredByEntities)} must not be null or empty!");

                // Support for comma-delimited entity id's
                var entityIdentifiers = instance.TriggeredByEntities.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var entId in entityIdentifiers)
                {
                    if (!_apps.ContainsKey(entId))
                    {
                        _apps.Add(entId, new List<IHassApp>());
                    }
                    _apps[entId].Add(instance);
                }
            }

            _inited = true;
        }

        private IEnumerable<Type> ScanAssemblyForHassApps()
        {
            var type = typeof(IHassApp);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p))
                    .Where(p => p.IsClass)
                ;

            return types;
        }

        private void SubscribeToEvents()
        {
            var cmd1 = new SubscribeToEventsCommand(EventType.state_changed, 1);
            var cmd2 = new SubscribeToEventsCommand(EventType.click, 2);
            _ws.Send(JsonConvert.SerializeObject(cmd1));
            _ws.Send(JsonConvert.SerializeObject(cmd2));
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (!e.IsText && !e.Data.IsValidJson())
                return;

            var json = JToken.Parse(e.Data);

            if (json.IsAuthMessage())
            {
                InfoOutput?.Invoke(this, new LogEventArgs { Text = $"Authorize message: {e.Data.ToPrettyJson()}" });
                return;
            }

            if (json.IsResult())
            {
                // Isn't an event, log and exit.
                DebugOutput?.Invoke(this, new LogEventArgs { Text = $"Result message: {e.Data.ToPrettyJson()}" });
                return;
            }

            if (!json.IsEvent())
            {
                // Isn't an event! And event's are what we're working with.
                WarnOutput?.Invoke(this, new LogEventArgs { Text = $"Unsupported message (not an 'event'): {e.Data.ToPrettyJson()}" });
                return;
            }

            var entId = json.ExtractEntityId().ToLowerInvariant();
            var matchedApps = _apps.FindApps(entId);

            if (matchedApps.Count == 0)
            {
                // No matched apps, log and exit.
                if (EncounteredEntityIdsWithoutSubscription.Add(entId))
                    TraceOutput?.Invoke(this, new LogEventArgs { Text = $"First time encounter of message with an EntityId that we're not listening on: {entId}" });
                return;
            }

            // Found matched apps! Log and determine which type
            InfoOutput?.Invoke(this, new LogEventArgs { Text = e.Data.ToPrettyJson() });
            var eventData = new EventData { EntityId = entId };

            if (json.IsClickEvent())
            {
                eventData.ClickData = new Click { ClickType = (string)json["event"]["data"]["click_type"] };
            }

            if (json.IsStateChangeEvent())
            {
                //entity_boolean doesn't have a "last_triggered" attribute.
                if (!entId.Contains("input_boolean."))
                {
                    if (!json.HasNewStateWithLastTriggered())
                        return; // Irrelevant event, we need new states that has "last time triggered" otherwise it might be an event provoked by reloading Hass. Unsure about this.

                }
                if (!json.IsTheMostRelevantStateChangeMessage())
                    return; // Is most probably a 'duped' event, throw it away ..
                if (!json.HasNewState())
                    return; // Irrelevant event, we need new states only ..

                var rawGraph = JsonConvert.DeserializeObject<HassEventRawModel>(e.Data);
                var stateChange = new StateChanged();
                stateChange.NewState = rawGraph.@event.data.new_state?.state;
                stateChange.OldState = rawGraph.@event.data.old_state?.state;
                stateChange.Attributes = JsonConvert.DeserializeObject<Dictionary<string, object>>((rawGraph.@event.data.new_state ?? rawGraph.@event.data.old_state ?? new StateRaw()).attributes.ToString());
                eventData.StateChangeData = stateChange;
            }

            foreach (var hassApp in matchedApps.Where(p => p.IsExecuting == false))
            {
                hassApp.IsExecuting = true;
                hassApp
                    .ExecuteAsync(eventData, e.Data)
                    .ContinueWith(p =>
                    {
                        // Only on exception: Raise event..
                        var ex = p.Exception?.InnerExceptions?.FirstOrDefault() ?? p.Exception;
                        ErrorOutput?.Invoke(this, new LogEventArgs { Text = ex?.Message, Exception = ex });
                    }, TaskContinuationOptions.OnlyOnFaulted)
                    .ContinueWith(p =>
                    {
                        hassApp.IsExecuting = false;
                    }, TaskContinuationOptions.None);
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            ErrorOutput?.Invoke(this, new LogEventArgs { Text = e.Message, Exception = e.Exception });
        }

        private void OnWebsocketLog(LogData data, string arg2)
        {
            TraceOutput?.Invoke(this, new LogEventArgs { Text = data.Message + " [arg2]" });
        }
    }
}