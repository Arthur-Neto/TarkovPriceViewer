using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TarkovPriceViewer.Extensions;
using TarkovPriceViewer.Forms;
using TarkovPriceViewer.Infrastructure.Services;

namespace TarkovPriceViewer
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            services.RegisterLogger();

            services.ConfigureSettings(configuration);

            services.AddMemoryCache();

            services.AddScoped<InfoOverlay>();
            services.AddScoped<CompareOverlay>();
            services.AddScoped<KeyPressCheck>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IBallisticService, BallisticService>();
        }
    }
}