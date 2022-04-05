using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using TarkovPriceChecker;
using TarkovPriceViewer.Extensions;
using TarkovPriceViewer.Infrastructure.Constants;

namespace TarkovPriceViewer
{
    public static class Program
    {
        public static bool FinishLoadingBallistics = false;

        public static readonly List<Item> ItemList = new List<Item>();
        public static readonly Dictionary<string, Ballistic> DicBallistic = new Dictionary<string, Ballistic>();
        public static readonly HashSet<string> BEType = new HashSet<string> { "Round", "Slug", "Buckshot", "Grenade launcher cartridge" };

        public static void ConfigureServices(IServiceCollection services)
        {
            services.RegisterLogger();

            services.AddScoped<MainForm>();
            services.AddScoped<InfoOverlay>();
            services.AddScoped<CompareOverlay>();
            services.AddScoped<KeyPressCheck>();

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

        public static void GetItemList()
        {
            string[] textValue = null;
            if (File.Exists(@"Resources\itemlist.txt"))
            {
                textValue = File.ReadAllLines(@"Resources\itemlist.txt");
            }
            if (textValue != null && textValue.Length > 0)
            {
                for (var i = 0; i < textValue.Length; i++)//ignore 1,2 Line
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
                    ItemList.Add(item);
                }
            }
            Debug.WriteLine("itemlist Count : " + ItemList.Count);
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

        public static void GetBallistics()
        {
            while (!FinishLoadingBallistics)
            {
                try
                {
                    var doc = new HtmlAgilityPack.HtmlDocument();

                    using var httpClient = new HttpClient();
                    using (var response = httpClient.GetAsync($"{Links.WIKI}Ballistics").ConfigureAwait(false).GetAwaiter().GetResult())
                    {
                        using var content = response.Content;
                        var json = content.ReadAsStringAsync().Result;

                        doc.LoadHtml(json);
                    }

                    Debug.WriteLine(Links.WIKI + "Ballistics");

                    var node_tm = doc.DocumentNode.SelectSingleNode("//table[@id='trkballtable']");
                    HtmlAgilityPack.HtmlNodeCollection? nodes = null;
                    HtmlAgilityPack.HtmlNodeCollection? sub_nodes = null;
                    if (node_tm != null)
                    {
                        node_tm = node_tm.SelectSingleNode(".//tbody");
                        if (node_tm != null)
                        {
                            nodes = node_tm.SelectNodes(".//tr");
                            if (nodes != null)
                            {
                                DicBallistic.Clear();
                                var sub_blist = new List<Ballistic>();
                                foreach (var node in nodes)
                                {
                                    if (!node.GetAttributeValue("id", "").Equals(""))
                                    {
                                        sub_blist = new List<Ballistic>();
                                    }
                                    sub_nodes = node.SelectNodes(".//td");
                                    if (sub_nodes != null && sub_nodes.Count >= 15)
                                    {
                                        var first = sub_nodes[0].GetAttributeValue("rowspan", 1) == 1 ? 0 : 1;
                                        if (sub_nodes[0].InnerText.Trim().Equals("40x46 mm"))
                                        {
                                            first = 1;
                                        }
                                        var name = sub_nodes[first++].InnerText.Trim();
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
                                        var damage = sub_nodes[first++].InnerText.Trim();
                                        if (damage.Contains("x"))
                                        {
                                            var temp_d = damage.Split('x');
                                            var mul = 1;
                                            try
                                            {
                                                foreach (var d in temp_d)
                                                {
                                                    mul *= int.Parse(d);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(ex.Message);
                                            }
                                            damage += " = " + mul;
                                        }
                                        var b = new Ballistic(
                                            name
                                            , damage
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , sub_nodes[first++].InnerText.Trim()
                                            , special
                                            , sub_blist
                                            );
                                        sub_blist.Add(b);
                                        DicBallistic.Add(name, b);
                                    }
                                }
                            }
                        }
                    }
                    FinishLoadingBallistics = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("error with ballistics : " + e.Message);
                    Thread.Sleep(3000);
                }
            }
            Debug.WriteLine("finish to get ballistics.");
        }
    }
}