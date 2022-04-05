using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
using TarkovPriceChecker;
using TarkovPriceViewer.Infrastructure.Settings;

namespace TarkovPriceViewer
{
    public class Startup
    {
        public static readonly string VERSION = Assembly.GetEntryAssembly().GetName().Version.ToString();

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

            Program.GetItemList();

            var services = new ServiceCollection();

            Program.ConfigureServices(services);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.Configure<AppSettings>(options => configuration.Bind(options));

            using var serviceProvider = services.BuildServiceProvider();
            if (configuration.GetValue<bool>("MinimizetoTrayWhenStartup"))
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
