using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using TarkovPriceViewer.Infrastructure.JsonWriter;
using TarkovPriceViewer.Infrastructure.Settings;

namespace TarkovPriceViewer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SettingsOptions>(options => configuration.GetSection("Settings").Bind(options));

            services.ConfigureWritable<SettingsOptions>(configuration.GetSection("Settings"));
        }

        public static void RegisterLogger(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("serilogsettings.json");

            var configuration = builder.Build();

            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
        }

        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection section,
            string file = "appsettings.json") where T : class, new()
        {
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var environment = provider.GetService<IHostEnvironment>();
                var options = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment, options, section.Key, file);
            });
        }
    }
}
