using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using TarkovPriceChecker;

namespace TarkovPriceViewer
{
    public static class Program
    {
        public static Dictionary<string, string> Settings = new Dictionary<string, string>();
        public static readonly List<Item> ItemList = new List<Item>();
        public static readonly Dictionary<string, Ballistic> DicBallistic = new Dictionary<string, Ballistic>();
        public static readonly Color[] BEColor = new Color[] {
            ColorTranslator.FromHtml("#B32425"),
            ColorTranslator.FromHtml("#DD3333"),
            ColorTranslator.FromHtml("#EB6C0D"),
            ColorTranslator.FromHtml("#AC6600"),
            ColorTranslator.FromHtml("#FB9C0E"),
            ColorTranslator.FromHtml("#006400"),
            ColorTranslator.FromHtml("#009900")
        };
        public static readonly HashSet<string> BEType = new HashSet<string> { "Round", "Slug", "Buckshot", "Grenade launcher cartridge" };
        public static readonly string SettingsPath = @"settings.json";
        public static readonly string AppName = "EscapeFromTarkov";
        public static readonly string Loading = "Loading...";
        public static readonly string NotFound = "Item Name Not Found.";
        public static readonly string NoFlea = "Item not Found on Flea.";
        public static readonly string NotFinishLoading = "Wait for Loading Data. Please Check Your Internet, and Check Tarkov Wiki Site.";
        public static readonly string PressCompareKey = "Please Press Compare Key.";
        public static bool FinishLoadingBallistics = false;
        public static readonly string WikiLink = "https://escapefromtarkov.fandom.com/wiki/";
        public static readonly string TarkovMarketLink = "https://tarkov-market.com/item/";
        public static readonly string OfficialLink = "https://www.escapefromtarkov.com/";
        public static readonly string GithubLink = "https://github.com/hwangshkr/TarkovPriceViewer";
        public static readonly string CheckUpdateLink = "https://github.com/hwangshkr/TarkovPriceViewer/raw/main/README.md";
        public static readonly char RoubleChar = '₽';
        public static readonly char DolarChar = '$';
        public static readonly char EuroChar = '€';
        public static readonly char[] SplitCur = new char[] { RoubleChar, DolarChar, EuroChar };
        public static readonly Regex InRaidRegex = new Regex(@"in raid");
        public static readonly Regex MoneyRegex = new Regex(@"([\d,]+[₽\$€]|[₽\$€][\d,]+)");

        private static MainForm? _main = null;

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

            var task = Task.Factory.StartNew(() => GetBallistics());

            LoadSettings();
            GetItemList();

            _main = new MainForm();

            if (Convert.ToBoolean(Settings["MinimizetoTrayWhenStartup"]))
            {
                Application.Run();
            }
            else
            {
                Application.Run(_main);
            }
        }

        private static void GetItemList()
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

        public static void LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    File.Create(SettingsPath).Dispose();
                }
                var text = File.ReadAllText(SettingsPath);
                try
                {
                    Settings = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
                }
                catch (JsonException je)
                {
                    Debug.WriteLine(je.Message);
                    text = "{}";
                    Settings = JsonSerializer.Deserialize<Dictionary<string, string>>(text);
                }
                Settings.Remove("Version");//force
                Settings.Add("Version", "v1.16");//force
                if (!Settings.TryGetValue("MinimizetoTrayWhenStartup", out var st))
                {
                    Settings.Add("MinimizetoTrayWhenStartup", "false");
                }
                if (!Settings.TryGetValue("CloseOverlayWhenMouseMoved", out st))
                {
                    Settings.Add("CloseOverlayWhenMouseMoved", "true");
                }
                if (!Settings.TryGetValue("RandomItem", out st))
                {
                    Settings.Add("RandomItem", "false");//false
                }
                if (!Settings.TryGetValue("ShowOverlay_Key", out st))
                {
                    Settings.Add("ShowOverlay_Key", "120");
                }
                if (!Settings.TryGetValue("HideOverlay_Key", out st))
                {
                    Settings.Add("HideOverlay_Key", "121");
                }
                if (!Settings.TryGetValue("CompareOverlay_Key", out st))
                {
                    Settings.Add("CompareOverlay_Key", "119");
                }
                if (!Settings.TryGetValue("Overlay_Transparent", out st))
                {
                    Settings.Add("Overlay_Transparent", "80");
                }
                if (!Settings.TryGetValue("Show_Last_Price", out st))
                {
                    Settings.Add("Show_Last_Price", "true");
                }
                if (!Settings.TryGetValue("Show_Day_Price", out st))
                {
                    Settings.Add("Show_Day_Price", "true");
                }
                if (!Settings.TryGetValue("Show_Week_Price", out st))
                {
                    Settings.Add("Show_Week_Price", "true");
                }
                if (!Settings.TryGetValue("Sell_to_Trader", out st))
                {
                    Settings.Add("Sell_to_Trader", "true");
                }
                if (!Settings.TryGetValue("Buy_From_Trader", out st))
                {
                    Settings.Add("Buy_From_Trader", "true");
                }
                if (!Settings.TryGetValue("Needs", out st))
                {
                    Settings.Add("Needs", "true");
                }
                if (!Settings.TryGetValue("Barters_and_Crafts", out st))
                {
                    Settings.Add("Barters_and_Crafts", "true");
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
        }

        private static void GetBallistics()
        {
            while (!FinishLoadingBallistics)
            {
                try
                {
                    using (var wc = new TPVWebClient())
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        Debug.WriteLine(WikiLink + "Ballistics");
                        doc.LoadHtml(wc.DownloadString(WikiLink + "Ballistics"));
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