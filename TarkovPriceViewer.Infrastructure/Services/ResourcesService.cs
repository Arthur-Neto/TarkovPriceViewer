using Microsoft.Extensions.Localization;

namespace TarkovPriceViewer.Infrastructure.Services
{
    public interface IResourcesService
    {
        string GetString(string key);
    }

    public class ResourcesService : IResourcesService
    {
        private readonly IStringLocalizer<ResourcesService> _stringLocalizer;

        public ResourcesService(IStringLocalizer<ResourcesService> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public string GetString(string key)
        {
            return _stringLocalizer.GetString(key);
        }
    }
}
