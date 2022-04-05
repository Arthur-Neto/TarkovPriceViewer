using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using TarkovPriceViewer.Infrastructure.Constants;
using TarkovPriceViewer.Infrastructure.Entities;

namespace TarkovPriceViewer.Infrastructure.Services
{
    public interface IBallisticService
    {
        Task<Dictionary<string, Ballistic>> GetBallisticsAsync(CancellationToken cancellationToken = default);
    }

    public class BallisticService : IBallisticService
    {
        private readonly ILogger<BallisticService> _logger;
        private readonly IMemoryCache _memoryCache;

        public BallisticService(
            ILogger<BallisticService> logger,
            IMemoryCache memoryCache
        )
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<Dictionary<string, Ballistic>> GetBallisticsAsync(CancellationToken cancellationToken = default)
        {
            var dicBallistic = await _memoryCache.GetOrCreateAsync(
                "Ballistics",
                async cacheEntry =>
                {
                    var dicBallistic = new Dictionary<string, Ballistic>();
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();

                        using var httpClient = new HttpClient();
                        using (var response = await httpClient.GetAsync($"{Links.WIKI}Ballistics"))
                        {
                            using var content = response.Content;
                            var json = await content.ReadAsStringAsync(cancellationToken);

                            doc.LoadHtml(json);
                        }

                        var nodeTm = doc.DocumentNode.SelectSingleNode("//table[@id='trkballtable']");
                        HtmlAgilityPack.HtmlNodeCollection? nodes = null;
                        HtmlAgilityPack.HtmlNodeCollection? subNodes = null;
                        if (nodeTm != null)
                        {
                            nodeTm = nodeTm.SelectSingleNode(".//tbody");
                            if (nodeTm != null)
                            {
                                nodes = nodeTm.SelectNodes(".//tr");
                                if (nodes != null)
                                {
                                    var subBallisticList = new List<Ballistic>();
                                    foreach (var node in nodes)
                                    {
                                        if (!node.GetAttributeValue("id", "").Equals(""))
                                        {
                                            subBallisticList = new List<Ballistic>();
                                        }

                                        subNodes = node.SelectNodes(".//td");
                                        if (subNodes != null && subNodes.Count >= 15)
                                        {
                                            var first = subNodes[0].GetAttributeValue("rowspan", 1) == 1 ? 0 : 1;
                                            if (subNodes[0].InnerText.Trim().Equals("40x46 mm"))
                                            {
                                                first = 1;
                                            }
                                            var name = subNodes[first++].InnerText.Trim();
                                            var special = "";
                                            if (name.EndsWith(" S T"))
                                            {
                                                name = new Regex("(S T)$").Replace(name, "");
                                                special = @"Subsonic & Tracer";
                                            }
                                            if (name.EndsWith(" T"))
                                            {
                                                name = new Regex("T$").Replace(name, "");
                                                special = @"Tracer";
                                            }
                                            if (name.EndsWith(" S"))
                                            {
                                                name = new Regex("S$").Replace(name, "");
                                                special = @"Subsonic";
                                            }
                                            if (name.EndsWith(" S T FM"))
                                            {
                                                name = new Regex("(S T FM)$").Replace(name, "");
                                                special = @"Subsonic & Tracer";
                                            }
                                            if (name.EndsWith(" T FM"))
                                            {
                                                name = new Regex("T FM$").Replace(name, "");
                                                special = @"Tracer";
                                            }
                                            if (name.EndsWith(" S FM"))
                                            {
                                                name = new Regex("S FM$").Replace(name, "");
                                                special = @"Subsonic";
                                            }
                                            if (name.EndsWith(" FM"))//must be last
                                            {
                                                name = new Regex("FM$").Replace(name, "");
                                                special = @"";
                                            }

                                            name = name.Replace("*", "").Trim();
                                            var damage = subNodes[first++].InnerText.Trim();
                                            if (damage.Contains("x"))
                                            {
                                                var splitDamage = damage.Split('x');
                                                var multiplier = 1;
                                                try
                                                {
                                                    foreach (var d in splitDamage)
                                                    {
                                                        multiplier *= int.Parse(d);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    _logger.LogError(ex, "Exception on parsing ballistics");
                                                }
                                                damage += " = " + multiplier;
                                            }
                                            var ballistic = new Ballistic(
                                                name, damage,
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                subNodes[first++].InnerText.Trim(),
                                                special, subBallisticList
                                            );
                                            subBallisticList.Add(ballistic);
                                            dicBallistic.Add(name, ballistic);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error with Ballistics");
                        Thread.Sleep(3000);
                    }

                    return dicBallistic;
                }
            );

            _logger.LogInformation("Finish to get ballistics");

            return dicBallistic;
        }
    }
}
