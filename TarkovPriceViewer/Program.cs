using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;
using TarkovPriceViewer.Forms;
using WindowsFormsLifetime;

namespace TarkovPriceViewer
{
    public static class Program
    {
        public static readonly string VERSION = Assembly.GetEntryAssembly().GetName().Version.ToString();

        [STAThread]
        private static void Main(string[] args)
        {
            // ??
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

            CreateHostBuilder(args)
                .UseEnvironment("Production")
                .Build()
                .Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsFormsLifetime<MainForm>(options =>
                {
                    options.HighDpiMode = HighDpiMode.SystemAware;
                    options.EnableVisualStyles = true;
                    options.CompatibleTextRenderingDefault = false;
                    options.SuppressStatusMessages = false;
                    options.EnableConsoleShutdown = true;
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigureServices();
                });
        }
    }
}
