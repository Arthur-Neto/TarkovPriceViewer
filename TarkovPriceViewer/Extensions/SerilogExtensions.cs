using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace TarkovPriceViewer.Extensions
{
    public static class SerilogExtensions
    {
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
    }
}
