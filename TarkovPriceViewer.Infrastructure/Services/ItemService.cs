using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TarkovPriceViewer.Infrastructure.Entities;

namespace TarkovPriceViewer.Infrastructure.Services
{
    public interface IItemService
    {
        Task<List<Item>> GetItemListAsync(CancellationToken cancellationToken = default);
    }

    public class ItemService : IItemService
    {
        private readonly ILogger<ItemService> _logger;
        private readonly IMemoryCache _memoryCache;

        public ItemService(
            ILogger<ItemService> logger,
            IMemoryCache memoryCache
        )
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<List<Item>> GetItemListAsync(CancellationToken cancellationToken = default)
        {
            var itemList = await _memoryCache.GetOrCreateAsync(
                "Items",
                async cacheEntry =>
                {
                    var itemList = new List<Item>();
                    string[]? textValue = null;
                    if (File.Exists(@"Resources\itemlist.txt"))
                    {
                        textValue = await File.ReadAllLinesAsync(@"Resources\itemlist.txt", cancellationToken);
                    }

                    if (textValue != null && textValue.Length > 0)
                    {
                        for (var i = 0; i < textValue.Length; i++) // Ignore 1,2 Line
                        {
                            var spl = textValue[i].Split('\t');
                            var item = new Item
                            {
                                NameDisplay = spl[0].Trim(),
                                NameDisplay2 = spl[2].Trim()
                            };
                            item.NameCompare = item.NameDisplay.ToLower().ToCharArray();
                            item.NameCompare2 = item.NameDisplay2.ToLower().ToCharArray();
                            item.MarketAddress = spl[1].Replace(" ", "_").Trim();
                            item.WikiAddress = spl[0].Replace(" ", "_").Trim();
                            itemList.Add(item);
                        }
                    }

                    return itemList;
                }
            );

            _logger.LogInformation("ItemList Count: {ItemCount}", itemList.Count());

            return itemList;
        }
    }
}
