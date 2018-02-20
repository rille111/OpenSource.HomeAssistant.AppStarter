# Hass AppStarter

## Purpose

Home Assistant 'Hass' is a home automation system running on Unix-systems and AppDaemon is used along with it to further empower developers to write Python-based automations and apps, working with Hass.

I don't excel at either Unix or Python but I still enjoy Home Assistant and want to use .NET to accomplish things I couldn't with Hass-AppDaemon.
Hence the birth of this project, and being inspired by AppDaemon I want to enable myself and others to use their existing .NET knowledge to write app-like implementations based on Hass.

It is easy, just nuget-install Rille.Hass.AppStarter and follow the steps and you'll be going in no time!

## Installation

* By nuget
`Install-Package HomeAssistant.AppStarter`
* By downloading this library.

## Usage - Create apps

* Create a class that implements IHassApp
* Filling out the various properties (see code for examples)
* Implement ExecuteAsync() with your code

## Usage - Configure & Run 

Super easy! Better show with code:
``` CSharp
        public async Task RunAppStarter()
        {
            await Task.Delay(0);

            var appRunner = new HassAppsRunner("ws://192.168.0.201:8123/api/websocket");

            appRunner.TraceOutput += (sender, args) => _logger.Trace(args.Exception, args.Text);
            appRunner.DebugOutput += (sender, args) => _logger.Debug(args.Exception, args.Text);
            appRunner.WarnOutput += (sender, args) => _logger.Warn(args.Exception, args.Text);
            appRunner.InfoOutput += (sender, args) => _logger.Info(args.Exception, args.Text);
            appRunner.ErrorOutput += (sender, args) => _logger.Error(args.Exception, args.Text);

            appRunner.Start();

            System.Console.WriteLine($"\n-- Connected. Press any key to exit --");
            System.Console.ReadKey();
            appRunner.Stop();
        }
```

And an app!:
```
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
```

## Engine

Since Hass exposes a Websocket API, this lib will subscribe to all state_changed events via this Websocket, and connect your written apps with those events.

## TODO

* Since it's in alpha development it will crash! Feel free to contribute. :)