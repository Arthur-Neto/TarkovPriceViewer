using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using TarkovPriceChecker;

namespace TarkovPriceViewer
{
    public class Startup
    {
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            foreach (var process in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                if (process.Id == Process.GetCurrentProcess().Id)
                {
                    continue;
                }
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(20, 20);

            var task = Task.Factory.StartNew(() => Program.GetBallistics());

            Program.LoadSettings();
            Program.GetItemList();

            var services = new ServiceCollection();

            Program.ConfigureServices(services);

            using var serviceProvider = services.BuildServiceProvider();
            if (Convert.ToBoolean(Program.Settings["MinimizetoTrayWhenStartup"]))
            {
                Application.Run();
            }
            else
            {
                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
        }
    }
}
