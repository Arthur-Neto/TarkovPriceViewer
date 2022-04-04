using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using TarkovPriceChecker;

namespace TarkovPriceViewer
{
    public static class Program
    {
        private static MainForm main = null;
        public static Dictionary<string, string> settings = new Dictionary<string, string>();
        public static readonly List<Item> itemlist = new List<Item>();
        public static readonly Dictionary<string, Ballistic> blist = new Dictionary<string, Ballistic>();
        public static readonly Color[] BEColor = new Color[] { ColorTranslator.FromHtml("#B32425"),
            ColorTranslator.FromHtml("#DD3333"),
            ColorTranslator.FromHtml("#EB6C0D"),
            ColorTranslator.FromHtml("#AC6600"),
            ColorTranslator.FromHtml("#FB9C0E"),
            ColorTranslator.FromHtml("#006400"),
            ColorTranslator.FromHtml("#009900") };
        public static readonly HashSet<string> BEType = new HashSet<string> { "Round", "Slug", "Buckshot", "Grenade launcher cartridge" };
        public static readonly string setting_path = @"settings.json";
        public static readonly string appname = "EscapeFromTarkov";
        public static readonly string loading = "Loading...";
        public static readonly string notfound = "Item Name Not Found.";
        public static readonly string noflea = "Item not Found on Flea.";
        public static readonly string notfinishloading = "Wait for Loading Data. Please Check Your Internet, and Check Tarkov Wiki Site.";
        public static readonly string presscomparekey = "Please Press Compare Key.";
        public static bool finishloadingballistics = false;
        public static readonly string wiki = "https://escapefromtarkov.fandom.com/wiki/";
        public static readonly string tarkovmarket = "https://tarkov-market.com/item/";
        public static readonly string official = "https://www.escapefromtarkov.com/";
        public static readonly string github = "https://github.com/hwangshkr/TarkovPriceViewer";
        public static readonly string checkupdate = "https://github.com/hwangshkr/TarkovPriceViewer/raw/main/README.md";
        public static readonly char rouble = '₽';
        public static readonly char dollar = '$';
        public static readonly char euro = '€';
        public static readonly char[] splitcur = new char[] { rouble, dollar, euro };
        public static readonly Regex inraid_filter = new Regex(@"in raid");
        public static readonly Regex money_filter = new Regex(@"([\d,]+[₽\$€]|[₽\$€][\d,]+)");

        [STAThread]
        private static void Main()
        {
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
                    Debug.WriteLine(ex.Message);
                }
            }
            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(20, 20);
            var task = Task.Factory.StartNew(() => getBallistics());
            LoadSettings();
            getItemList();
            main = new MainForm();
            if (Convert.ToBoolean(settings["MinimizetoTrayWhenStartup"]))
            {
                Application.Run();
            }
            else
            {
                Application.Run(main);
            }
        }

        private static void getItemList()
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
                    itemlist.Add(item);
                }
            }
            Debug.WriteLine("itemlist Count : " + itemlist.Count);
        }

        public static void LoadSettings()
        {
            try
            {
                if (!File.Exists(setting_path))
                {
                    File.Create(setting_path).Dispose();
                }
                var text = File.ReadAllText(setting_path);
                try
                {
                    settings = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
                }
                catch (JsonException je)
                {
                    Debug.WriteLine(je.Message);
                    text = "{}";
                    settings = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
                }
                settings.Remove("Version");//force
                settings.Add("Version", "v1.16");//force
                if (!settings.TryGetValue("MinimizetoTrayWhenStartup", out var st))
                {
                    settings.Add("MinimizetoTrayWhenStartup", "false");
                }
                if (!settings.TryGetValue("CloseOverlayWhenMouseMoved", out st))
                {
                    settings.Add("CloseOverlayWhenMouseMoved", "true");
                }
                if (!settings.TryGetValue("RandomItem", out st))
                {
                    settings.Add("RandomItem", "false");//false
                }
                if (!settings.TryGetValue("ShowOverlay_Key", out st))
                {
                    settings.Add("ShowOverlay_Key", "120");
                }
                if (!settings.TryGetValue("HideOverlay_Key", out st))
                {
                    settings.Add("HideOverlay_Key", "121");
                }
                if (!settings.TryGetValue("CompareOverlay_Key", out st))
                {
                    settings.Add("CompareOverlay_Key", "119");
                }
                if (!settings.TryGetValue("Overlay_Transparent", out st))
                {
                    settings.Add("Overlay_Transparent", "80");
                }
                if (!settings.TryGetValue("Show_Last_Price", out st))
                {
                    settings.Add("Show_Last_Price", "true");
                }
                if (!settings.TryGetValue("Show_Day_Price", out st))
                {
                    settings.Add("Show_Day_Price", "true");
                }
                if (!settings.TryGetValue("Show_Week_Price", out st))
                {
                    settings.Add("Show_Week_Price", "true");
                }
                if (!settings.TryGetValue("Sell_to_Trader", out st))
                {
                    settings.Add("Sell_to_Trader", "true");
                }
                if (!settings.TryGetValue("Buy_From_Trader", out st))
                {
                    settings.Add("Buy_From_Trader", "true");
                }
                if (!settings.TryGetValue("Needs", out st))
                {
                    settings.Add("Needs", "true");
                }
                if (!settings.TryGetValue("Barters_and_Crafts", out st))
                {
                    settings.Add("Barters_and_Crafts", "true");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public static void SaveSettings()
        {
            try
            {
                if (!File.Exists(setting_path))
                {
                    File.Create(setting_path).Dispose();
                }
                var jsonString = JsonSerializer.Serialize(settings);
                File.WriteAllText(setting_path, jsonString.Replace(",", ",\n"));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static void getBallistics()
        {
            while (!finishloadingballistics)
            {
                try
                {
                    using (var wc = new TPVWebClient())
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        Debug.WriteLine(wiki + "Ballistics");
                        doc.LoadHtml(wc.DownloadString(wiki + "Ballistics"));
                        var node_tm = doc.DocumentNode.SelectSingleNode("//table[@id='trkballtable']");
                        HtmlAgilityPack.HtmlNodeCollection nodes = null;
                        HtmlAgilityPack.HtmlNodeCollection sub_nodes = null;
                        if (node_tm != null)
                        {
                            node_tm = node_tm.SelectSingleNode(".//tbody");
                            if (node_tm != null)
                            {
                                nodes = node_tm.SelectNodes(".//tr");
                                if (nodes != null)
                                {
                                    blist.Clear();
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
                                            blist.Add(name, b);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finishloadingballistics = true;
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