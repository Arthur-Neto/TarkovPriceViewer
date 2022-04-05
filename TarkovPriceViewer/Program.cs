using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using TarkovPriceViewer.Extensions;
using TarkovPriceViewer.Forms;
using TarkovPriceViewer.Infrastructure.Services;

namespace TarkovPriceViewer
{
    public static class Program
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.RegisterLogger();

            services.AddMemoryCache();

            services.AddScoped<MainForm>();
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

        /*
        public static void SaveSettings()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    File.Create(SettingsPath).Dispose();
                }
                var jsonString = JsonSerializer.Serialize(Settings);
                File.WriteAllText(SettingsPath, jsonString.Replace(",", ",\n"));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }*/
    }
}