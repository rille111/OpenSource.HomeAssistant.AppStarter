using System.Threading.Tasks;
using NLog;
using Rille.Hass.AppStarter;

namespace HomeAssistant.ConsoleExample
{
    class Program
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        static void Main()
        {
            var prog = new Program();
            var t = Task.Run(prog.RunAppStarter);
            t.ConfigureAwait(false).GetAwaiter().GetResult();

            System.Console.WriteLine("\n-- Press any key to exit --");
            System.Console.ReadKey();
        }

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
    }
}
