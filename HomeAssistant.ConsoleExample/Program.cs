using System.Threading.Tasks;
using HomeAssistant.AppStarter;
using NLog;

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

            // Add your password, if any, for Home Assistant here
            var apiPassword = "";
            // Add the address to your Home Assistant instance here
            var hassUrl = "ip-address:port";

            var appRunner = new HassAppsRunner($"ws://{hassUrl}/api/websocket", apiPassword);

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
