using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
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

            services.AddLocalization(o => o.ResourcesPath = "Properties/Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                new CultureInfo("en-US"),
            };
                options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }
    }
}