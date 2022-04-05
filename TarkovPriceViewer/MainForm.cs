using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TarkovPriceViewer;
using TarkovPriceViewer.Infrastructure.Constants;
using TarkovPriceViewer.Properties;
using Tesseract;

namespace TarkovPriceChecker
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO Dummy);

        private static readonly IntPtr _hInstance = LoadLibrary("User32");

        private struct WINDOWPLACEMENT
        {
            public int Length;
            public int Flags;
            public ShowWindowCommands ShowCmd;
            public System.Drawing.Point PtMinPosition;
            public System.Drawing.Point PtMaxPosition;
            public Rectangle RcNormalPosition;
        }

        private enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        private struct LASTINPUTINFO
        {
            public uint CbSize;
            public uint DwTime;
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYUP = 0x101;
        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE = 0x200;

        private static int _nFlags = 0x0;
        private static long _pressTime = 0;

        private static bool _isInfoClosed = true;
        private static bool _isCompareClosed = true;

        private static InfoOverlay? _overlayInfo = null;
        private static CompareOverlay? _overlayCompare = null;
        private static LowLevelProc? _procKeyboard = null;
        private static LowLevelProc? _procMouse = null;
        private static Control? _pressKeyControl = null;

        private static IntPtr _hhookKeyboard = IntPtr.Zero;
        private static IntPtr _hhookMouse = IntPtr.Zero;

        private static System.Drawing.Point _point = new System.Drawing.Point(0, 0);

        private static CancellationTokenSource _cancellationTokenInfo = new CancellationTokenSource();
        private static CancellationTokenSource _cancellationTokenCompare = new CancellationTokenSource();
        private static Scalar _lineColor = new Scalar(90, 89, 82);
        private static long _idleTime = 3600000;

        private readonly IStringLocalizer<Resources> _resources;
        private readonly ILogger<MainForm> _logger;

        public MainForm(
            InfoOverlay infoOverlay,
            CompareOverlay compareOverlay,
            IStringLocalizer<Resources> resources,
            ILogger<MainForm> logger
        )
        {
            var style = GetWindowLong(Handle, GWL_EXSTYLE);

            SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED);

            if (Environment.OSVersion.Version.Major >= 6)
            {
                _nFlags = 0x2;
            }

            InitializeComponent();
            SettingUI();
            SetHook();

            _overlayInfo = infoOverlay;
            _overlayInfo.Owner = this;
            _overlayInfo.Show();

            _overlayCompare = compareOverlay;
            _overlayCompare.Owner = this;
            _overlayCompare.Show();

            _resources = resources;
            _logger = logger;
        }

        private void SettingUI()
        {
            MinimizeBox = false;
            MaximizeBox = false;
            Version.Text = Program.Settings["Version"];
            MinimizetoTrayWhenStartup.Checked = Convert.ToBoolean(Program.Settings["MinimizetoTrayWhenStartup"]);
            CloseOverlayWhenMouseMoved.Checked = Convert.ToBoolean(Program.Settings["CloseOverlayWhenMouseMoved"]);
            RandomItem.Checked = Convert.ToBoolean(Program.Settings["RandomItem"]);
            last_price_box.Checked = Convert.ToBoolean(Program.Settings["Show_Last_Price"]);
            day_price_box.Checked = Convert.ToBoolean(Program.Settings["Show_Day_Price"]);
            week_price_box.Checked = Convert.ToBoolean(Program.Settings["Show_Week_Price"]);
            sell_to_trader_box.Checked = Convert.ToBoolean(Program.Settings["Sell_to_Trader"]);
            buy_from_trader_box.Checked = Convert.ToBoolean(Program.Settings["Buy_From_Trader"]);
            needs_box.Checked = Convert.ToBoolean(Program.Settings["Needs"]);
            barters_and_crafts_box.Checked = Convert.ToBoolean(Program.Settings["Barters_and_Crafts"]);
            ShowOverlay_Button.Text = ((Keys)int.Parse(Program.Settings["ShowOverlay_Key"])).ToString();
            HideOverlay_Button.Text = ((Keys)int.Parse(Program.Settings["HideOverlay_Key"])).ToString();
            CompareOverlay_Button.Text = ((Keys)int.Parse(Program.Settings["CompareOverlay_Key"])).ToString();
            TransParent_Bar.Value = int.Parse(Program.Settings["Overlay_Transparent"]);
            TransParent_Text.Text = Program.Settings["Overlay_Transparent"];

            TrayIcon.Visible = true;
            check_idle_time.Start();
        }

        private void SetHook()
        {
            SetHook(false);
        }

        private void SetHook(bool force)
        {
            try
            {
                if (force)
                {
                    _logger.LogDebug("Force Unhook");
                    UnHook();
                }
                if (_hhookKeyboard == IntPtr.Zero)
                {
                    _procKeyboard = HookKeyboardProc;
                    _hhookKeyboard = SetWindowsHookEx(WH_KEYBOARD_LL, _procKeyboard, _hInstance, 0);
                }
                if (Convert.ToBoolean(Program.Settings["CloseOverlayWhenMouseMoved"]))
                {
                    SetMouseHook();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on Unhook");
            }
        }

        private void SetMouseHook()
        {
            if (_hhookMouse == IntPtr.Zero)
            {
                _procMouse = HookMouseProc;
                _hhookMouse = SetWindowsHookEx(WH_MOUSE_LL, _procMouse, _hInstance, 0);
            }
        }

        private void UnsetMouseHook()
        {
            if (_hhookMouse != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hhookMouse);
                _hhookMouse = IntPtr.Zero;
                _procMouse = null;
            }
        }

        private void UnHook()
        {
            try
            {
                if (_hhookKeyboard != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(_hhookKeyboard);
                    _hhookKeyboard = IntPtr.Zero;
                    _procKeyboard = null;
                }
                UnsetMouseHook();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on Unhook");
            }
        }

        private IntPtr HookKeyboardProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (code >= 0 && wParam == (IntPtr)WM_KEYUP)
                {
                    if (_pressKeyControl == null)
                    {
                        if (Program.FinishLoadingBallistics)
                        {
                            var vkCode = Marshal.ReadInt32(lParam);
                            if (vkCode == int.Parse(Program.Settings["ShowOverlay_Key"]))
                            {
                                var CurrentTime = DateTime.Now.Ticks;
                                if (CurrentTime - _pressTime > 2000000)
                                {
                                    _point = MousePosition;
                                    LoadingItemInfo();
                                }
                                else
                                {
                                    Debug.WriteLine("key pressed in 0.2 seconds.");
                                }
                                _pressTime = CurrentTime;
                            }
                            else if (vkCode == int.Parse(Program.Settings["CompareOverlay_Key"]))
                            {
                                _point = MousePosition;
                                LoadingItemCompare();
                            }
                            else if (vkCode == int.Parse(Program.Settings["HideOverlay_Key"])
                                || vkCode == 9 //tab
                                || vkCode == 27 //esc
                                )
                            {
                                CloseItemInfo();
                                CloseItemCompare();
                            }
                        }
                        else
                        {
                            _point = MousePosition;
                            _overlayInfo?.ShowWaitBallistics(_point);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on hook keyboard");
            }

            return CallNextHookEx(_hhookKeyboard, code, (int)wParam, lParam);
        }

        private IntPtr HookMouseProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (!_isInfoClosed && code >= 0
                    && wParam == (IntPtr)WM_MOUSEMOVE
                    && (Math.Abs(MousePosition.X - _point.X) > 5 || Math.Abs(MousePosition.Y - _point.Y) > 5))
                {
                    CloseItemInfo();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on hook mouse");
            }

            return CallNextHookEx(_hhookMouse, code, (int)wParam, lParam);
        }

        private uint GetIdleTime()
        {
            var LastUserAction = new LASTINPUTINFO();
            LastUserAction.CbSize = (uint)Marshal.SizeOf(LastUserAction);

            GetLastInputInfo(ref LastUserAction);

            return (uint)Environment.TickCount - LastUserAction.DwTime;
        }

        public long GetTickCount()
        {
            return Environment.TickCount;
        }

        private void CloseApp()
        {
            UnHook();

            TrayIcon.Dispose();

            CloseItemInfo();
            CloseItemCompare();

            Program.SaveSettings();
            Application.Exit();
        }

        public void LoadingItemInfo()
        {
            _isInfoClosed = false;
            _cancellationTokenInfo.Cancel();
            _cancellationTokenInfo = new CancellationTokenSource();
            _overlayInfo?.ShowLoadingInfo(_point, _cancellationTokenInfo.Token);
            var task = Task.Factory.StartNew(() => FindItemTask(true, _cancellationTokenInfo.Token));
        }

        public void LoadingItemCompare()
        {
            if (_isCompareClosed)
            {
                _isCompareClosed = false;
                _cancellationTokenCompare.Cancel();
                _cancellationTokenCompare = new CancellationTokenSource();
            }
            _overlayCompare?.ShowLoadingCompare(_point, _cancellationTokenCompare.Token);

            var task = Task.Factory.StartNew(() => FindItemTask(false, _cancellationTokenCompare.Token));
        }

        public void CloseItemInfo()
        {
            _isInfoClosed = true;
            _cancellationTokenInfo.Cancel();
            _overlayInfo?.HideInfo();
        }

        public void CloseItemCompare()
        {
            _isCompareClosed = true;
            _cancellationTokenCompare.Cancel();
            _overlayCompare?.HideCompare();
        }

        private int FindItemTask(bool isiteminfo, CancellationToken cts_one)
        {
            if (Convert.ToBoolean(Program.Settings["RandomItem"]))
            {
                if (!cts_one.IsCancellationRequested)
                {
                    var item = Program.ItemList[new Random().Next(Program.ItemList.Count - 1)];

                    FindItemInfo(item, isiteminfo, cts_one);
                }
            }
            else
            {
                var fullimage = CaptureScreen(CheckIsTarkov());
                if (fullimage != null)
                {
                    if (!cts_one.IsCancellationRequested)
                    {
                        FindItem(fullimage, isiteminfo, cts_one);
                    }
                }
                else
                {
                    if (!cts_one.IsCancellationRequested)
                    {
                        if (isiteminfo)
                        {
                            _overlayInfo?.ShowInfo(null, cts_one);
                        }
                        else
                        {
                            _overlayCompare?.ShowCompare(null, cts_one);
                        }
                    }
                    Debug.WriteLine("image null");
                }
            }

            return 0;
        }

        private IntPtr CheckIsTarkov()
        {
            var hWnd = GetForegroundWindow();
            if (hWnd != IntPtr.Zero)
            {
                var sbWinText = new StringBuilder(260);

                GetWindowText(hWnd, sbWinText, 260);

                if (sbWinText.ToString().Equals(_resources["AppName"]))
                {
                    return hWnd;
                }
            }

            _logger.LogWarning("Tarkov Not Found");

            return IntPtr.Zero;
        }

        private Bitmap? CaptureScreen(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                using var Graphicsdata = Graphics.FromHwnd(hWnd);
                var rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);
                var bmp = new Bitmap(rect.Width, rect.Height);
                using (var g = Graphics.FromImage(bmp))
                {
                    var hdc = g.GetHdc();
                    PrintWindow(hWnd, hdc, _nFlags);
                    g.ReleaseHdc(hdc);
                }
                return bmp;
            }
            else
            {
                _logger.LogWarning("Tarkov Window Not Found");

                return null;
            }
        }

        private string GetTesseract(Mat textmat)
        {
            var text = "";
            try
            {
                using var ocr = new TesseractEngine(@"./Resources/tessdata", "eng", EngineMode.Default); //should use once
                using var temp = BitmapConverter.ToBitmap(textmat);
                using var texts = ocr.Process(Pix.LoadFromMemory(ImageToByte(temp)));
                text = texts.GetText().Replace("\n", " ").Split(Currencies.AllCurrencies)[0].Trim();

                _logger.LogDebug("GetTesseract Text: {TesseractText}", text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on GetTesseract");
            }

            return text;
        }

        private static byte[]? ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[]?)converter.ConvertTo(img, typeof(byte[]));
        }

        private void FindItem(Bitmap fullimage, bool isiteminfo, CancellationToken cts_one)
        {
            var item = new Item();
            using (var ScreenMat_original = BitmapConverter.ToMat(fullimage))
            using (var ScreenMat = ScreenMat_original.CvtColor(ColorConversionCodes.BGRA2BGR))
            using (var rac_img = ScreenMat.InRange(_lineColor, _lineColor))
            {
                rac_img.FindContours(out var contours, out var hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                foreach (var contour in contours)
                {
                    if (!cts_one.IsCancellationRequested)
                    {
                        var rect2 = Cv2.BoundingRect(contour);
                        if (rect2.Width > 5 && rect2.Height > 10)
                        {
                            ScreenMat.Rectangle(rect2, Scalar.Black, 2);
                            using var temp = ScreenMat.SubMat(rect2);
                            using var temp2 = temp.Threshold(0, 255, ThresholdTypes.BinaryInv);
                            var text = GetTesseract(temp2);
                            if (!text.Equals(""))
                            {
                                item = MatchItemName(text.ToLower().Trim().ToCharArray());
                                break;
                            }
                        }
                    }
                }
                if (!cts_one.IsCancellationRequested)
                {
                    FindItemInfo(item, isiteminfo, cts_one);
                }
            }
            fullimage.Dispose();
        }

        private Item MatchItemName(char[] itemName)
        {
            var result = new Item();
            var distance = 999;
            foreach (var item in Program.ItemList)
            {
                var levenshteinDistance = LevenshteinDistance(itemName, item.NameCompare);
                if (levenshteinDistance < distance)
                {
                    result = item;
                    distance = levenshteinDistance;

                    if (item.IsName2)
                    {
                        item.IsName2 = false;
                    }
                    if (distance == 0)
                    {
                        break;
                    }
                }

                levenshteinDistance = LevenshteinDistance(itemName, item.NameCompare2);
                if (levenshteinDistance < distance)
                {
                    result = item;
                    distance = levenshteinDistance;
                    item.IsName2 = true;
                    if (distance == 0)
                    {
                        break;
                    }
                }
            }

            _logger.LogDebug("Distance {Distance}, TextMatch: {NameDisplay} & {NameDisplay2}", distance, result.NameDisplay, result.NameDisplay2);

            return result;
        }

        private int GetMinimum(int val1, int val2, int val3)
        {
            var minNumber = val1;
            if (minNumber > val2)
            {
                minNumber = val2;
            }

            if (minNumber > val3)
            {
                minNumber = val3;
            }

            return minNumber;
        }

        private int LevenshteinDistance(char[] s, char[] t)
        {
            var m = s.Length;
            var n = t.Length;

            var d = new int[m + 1, n + 1];

            for (var i = 1; i < m; i++)
            {
                d[i, 0] = i;
            }

            for (var j = 1; j < n; j++)
            {
                d[0, j] = j;
            }

            for (var j = 1; j < n; j++)
            {
                for (var i = 1; i < m; i++)
                {
                    if (s[i] == t[j])
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        d[i, j] = GetMinimum(d[i - 1, j], d[i, j - 1], d[i - 1, j - 1]) + 1;
                    }
                }
            }
            return d[m - 1, n - 1];
        }

        private void FindItemInfo(Item item, bool isiteminfo, CancellationToken cts_one)
        {
            if (item.MarketAddress != null)
            {
                try
                {
                    if (item.LastFetch == null || (DateTime.Now - item.LastFetch).TotalHours >= 1)
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();

                        using var httpClient = new HttpClient();
                        using (var response = httpClient.GetAsync($"{Links.TARKOV_MARKET}{item.MarketAddress}").ConfigureAwait(false).GetAwaiter().GetResult())
                        {
                            using var content = response.Content;
                            var json = content.ReadAsStringAsync().Result;

                            doc.LoadHtml(json);
                        }

                        var node_tm = doc.DocumentNode.SelectSingleNode("//div[@class='market-data']");
                        HtmlAgilityPack.HtmlNode? sub_node_tm = null;
                        HtmlAgilityPack.HtmlNode? sub_node_tm2 = null;
                        HtmlAgilityPack.HtmlNodeCollection? nodes = null;
                        HtmlAgilityPack.HtmlNodeCollection? subnodes = null;
                        HtmlAgilityPack.HtmlNodeCollection? subnodes2 = null;
                        if (node_tm != null)
                        {
                            nodes = node_tm.SelectNodes(".//div[@class='w-25']");
                            sub_node_tm = node_tm.SelectSingleNode(".//div[@class='sub']");
                            if (sub_node_tm != null)
                            {
                                item.LastUpdate = sub_node_tm.InnerText.Trim();
                            }
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    sub_node_tm = node.FirstChild;
                                    if (sub_node_tm != null)
                                    {
                                        if (sub_node_tm.InnerText.Trim().Contains("Flea price"))
                                        {
                                            sub_node_tm = node.SelectSingleNode(".//div[@class='big bold alt']");
                                            if (sub_node_tm != null)
                                            {
                                                item.PriceLast = sub_node_tm.InnerText.Trim();
                                            }
                                        }
                                        else if (sub_node_tm.InnerText.Trim().Contains("Average price"))
                                        {
                                            subnodes = node.SelectNodes(".//span[@class='bold alt']");
                                            if (subnodes != null && subnodes.Count >= 2)
                                            {
                                                item.PriceDay = subnodes[0].InnerText.Trim();
                                                item.PriceWeek = subnodes[1].InnerText.Trim();
                                            }
                                        }
                                        else
                                        {
                                            sub_node_tm2 = sub_node_tm.SelectSingleNode(".//div[@class='bold']");
                                            if (sub_node_tm2 != null)
                                            {
                                                var temp = sub_node_tm.InnerText.Trim().Split(' ');
                                                if (temp.Length > 2)
                                                {
                                                    if (sub_node_tm.InnerText.Trim().Contains("LL"))
                                                    {
                                                        item.BuyFromTrader = temp[0].Trim() + " " + temp[1];
                                                        sub_node_tm = node.SelectSingleNode(".//div[@class='bold alt']");
                                                        if (sub_node_tm != null)
                                                        {
                                                            item.BuyFromTraderPrice = sub_node_tm.InnerText.Trim();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        item.SellToTrader = temp[0].Trim();
                                                        sub_node_tm = node.SelectSingleNode(".//div[@class='bold alt']");
                                                        if (sub_node_tm != null)
                                                        {
                                                            item.SellToTraderPrice = sub_node_tm.InnerText.Trim();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (isiteminfo)
                        {
                            _overlayInfo?.ShowInfo(item, cts_one);
                        }
                        else
                        {
                            _overlayCompare?.ShowCompare(item, cts_one);
                        }

                        var isdisconnected = false;
                        do
                        {
                            isdisconnected = false;
                            try
                            {
                                _logger.LogDebug("Search for item info on Wiki {WikiAddress}", $"{Links.WIKI}{item.WikiAddress}");
                                try
                                {
                                    using var response = httpClient.GetAsync($"{Links.WIKI}{item.WikiAddress}").ConfigureAwait(false).GetAwaiter().GetResult();
                                    using var content = response.Content;
                                    var json = content.ReadAsStringAsync().Result;

                                    doc.LoadHtml(json);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Not found, searching in new link {WikiAddressNameDisplay2}", $"{Links.WIKI}{item.NameDisplay2.Replace(" ", "_")}");

                                    Debug.WriteLine(ex.Message);

                                    using var response = httpClient.GetAsync($"{Links.WIKI}{ item.NameDisplay2.Replace(" ", "_")}").ConfigureAwait(false).GetAwaiter().GetResult();
                                    using var content = response.Content;
                                    var json = content.ReadAsStringAsync().Result;

                                    doc.LoadHtml(json);
                                }

                                node_tm = doc.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']");
                                if (node_tm != null)
                                {
                                    var sb = new StringBuilder();
                                    nodes = node_tm.SelectNodes(".//li");
                                    if (nodes != null)
                                    {
                                        foreach (var node in nodes)
                                        {
                                            if (node.InnerText.Contains(" to be found "))
                                            {
                                                sb.Append(node.InnerText).Append("\n");
                                            }
                                        }
                                    }
                                    sub_node_tm = node_tm.SelectSingleNode(".//td[@class='va-infobox-cont']");
                                    if (sub_node_tm != null)
                                    {
                                        nodes = sub_node_tm.SelectNodes(".//table[@class='va-infobox-group']");
                                        if (nodes != null)
                                        {
                                            foreach (var node in nodes)
                                            {
                                                var temp_node = node.SelectSingleNode(".//th[@class='va-infobox-header']");
                                                if (temp_node != null)
                                                {
                                                    if (temp_node.InnerText.Trim().Equals("General data"))
                                                    {
                                                        var temp_node_list = sub_node_tm.SelectNodes(".//td[@class='va-infobox-label']");
                                                        var temp_node_list2 = sub_node_tm.SelectNodes(".//td[@class='va-infobox-content']");
                                                        if (temp_node_list != null && temp_node_list2 != null && temp_node_list.Count == temp_node_list2.Count)
                                                        {
                                                            for (var n = 0; n < temp_node_list.Count; n++)
                                                            {
                                                                if (temp_node_list[n].InnerText.Trim().Contains("Type"))
                                                                {
                                                                    item.Type = temp_node_list2[n].InnerText.Trim();
                                                                    if (Program.BEType.Contains(item.Type))
                                                                    {
                                                                        if (!Program.DicBallistic.TryGetValue(item.NameDisplay, out item.Ballistic))
                                                                        {
                                                                            Program.DicBallistic.TryGetValue(item.NameDisplay2, out item.Ballistic);
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (temp_node.InnerText.Trim().Equals("Performance"))
                                                    {
                                                        var temp_node_list = sub_node_tm.SelectNodes(".//td[@class='va-infobox-label']");
                                                        var temp_node_list2 = sub_node_tm.SelectNodes(".//td[@class='va-infobox-content']");
                                                        if (temp_node_list != null && temp_node_list2 != null && temp_node_list.Count == temp_node_list2.Count)
                                                        {
                                                            for (var n = 0; n < temp_node_list.Count; n++)
                                                            {
                                                                if (temp_node_list[n].InnerText.Trim().Contains("Recoil"))
                                                                {
                                                                    item.Recoil = temp_node_list2[n].InnerText.Trim();
                                                                }
                                                                else if (temp_node_list[n].InnerText.Trim().Contains("Ergonomics"))
                                                                {
                                                                    item.Ergo = temp_node_list2[n].InnerText.Trim();
                                                                }
                                                                else if (temp_node_list[n].InnerText.Trim().Contains("Accuracy"))
                                                                {
                                                                    item.Accuracy = temp_node_list2[n].InnerText.Trim();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (temp_node.InnerText.Trim().Equals("Ammunition"))
                                                    {
                                                        var temp_node_list = sub_node_tm.SelectNodes(".//td[@class='va-infobox-label']");
                                                        var temp_node_list2 = sub_node_tm.SelectNodes(".//td[@class='va-infobox-content']");
                                                        if (temp_node_list != null && temp_node_list2 != null && temp_node_list.Count == temp_node_list2.Count)
                                                        {
                                                            for (var n = 0; n < temp_node_list.Count; n++)
                                                            {
                                                                if (temp_node_list[n].InnerText.Trim().Contains("Default ammo"))
                                                                {
                                                                    Program.DicBallistic.TryGetValue(temp_node_list2[n].InnerText.Trim(), out item.Ballistic);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    nodes = node_tm.SelectNodes(".//table[@class='wikitable']");
                                    if (nodes != null)
                                    {
                                        var craftsb = new StringBuilder();
                                        foreach (var node in nodes)
                                        {
                                            subnodes = node.SelectNodes(".//tr");
                                            if (subnodes != null)
                                            {
                                                foreach (var node2 in subnodes)
                                                {
                                                    subnodes2 = node2.SelectNodes(".//th");
                                                    if (subnodes2 != null && subnodes2.Count >= 5)
                                                    {
                                                        var craftlist = new List<string>();
                                                        foreach (var temp in subnodes2)
                                                        {
                                                            foreach (var temp2 in temp.ChildNodes)
                                                            {
                                                                if (!temp2.InnerText.Trim().Equals(""))
                                                                {
                                                                    craftlist.Add(temp2.InnerText.Trim());
                                                                }
                                                            }
                                                        }

                                                        var firstarrow = craftlist.IndexOf("→");
                                                        var secondarrow = craftlist.LastIndexOf("→");
                                                        var firstlist = craftlist.GetRange(0, firstarrow);
                                                        var secondlist = craftlist.GetRange(firstarrow + 1, secondarrow - firstarrow - 1);
                                                        var thirdlist = craftlist.GetRange(secondarrow + 1, craftlist.Count - secondarrow - 1);
                                                        firstlist.Reverse();
                                                        if (secondlist.Count <= 2)
                                                        {
                                                            secondlist.Reverse();
                                                        }
                                                        thirdlist.Reverse();
                                                        craftsb.Append(string.Format("{0} → {2} ({1})"
                                                            , string.Join(" ", firstlist), string.Join(secondlist.Count <= 2 ? " in " : " ", secondlist), string.Join(" ", thirdlist))).Append("\n");
                                                    }
                                                }
                                            }
                                        }
                                        if (!craftsb.ToString().Trim().Equals(""))
                                        {
                                            item.Bartersandcrafts = craftsb.ToString().Trim();
                                        }
                                    }
                                    if (!sb.ToString().Trim().Equals(""))
                                    {
                                        item.Needs = sb.ToString().Trim();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                isdisconnected = true;
                                _logger.LogError(ex, "Exception on searching, wiki reconnected...");
                            }
                        } while (!cts_one.IsCancellationRequested && isdisconnected);
                    }
                    item.LastFetch = DateTime.Now;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception on searching...");
                }
            }

            if (isiteminfo)
            {
                _overlayInfo?.ShowInfo(item, cts_one);
            }
            else
            {
                _overlayCompare?.ShowCompare(item, cts_one);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseApp();
        }

        private void TrayExit_Click(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void TrayShow_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing) //must be checked
            {
                TrayIcon.Visible = true;
                Hide();
                e.Cancel = true;
            }
        }

        private void MinimizetoTrayWhenStartupCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["MinimizetoTrayWhenStartup"] = (sender as CheckBox).Checked.ToString();
        }

        private void Tarkov_Official_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Links.OFFICIAL_SITE,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void TarkovWikiClick(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Links.WIKI,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void TarkovMarketClick(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Links.TARKOV_MARKET,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void CloseOverlayWhenMouseMovedCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["CloseOverlayWhenMouseMoved"] = (sender as CheckBox).Checked.ToString();
            if ((sender as CheckBox).Checked)
            {
                SetMouseHook();
            }
            else
            {
                UnsetMouseHook();
            }
        }

        public void ChangePressKeyData(Keys? keycode)
        {
            if (_pressKeyControl != null)
            {
                if (keycode != null)
                {
                    _pressKeyControl.Text = keycode.ToString();
                }
                _pressKeyControl = null;
            }
        }

        private void OverlayButtonClick(object sender, EventArgs e)
        {
            _pressKeyControl = (sender as Control);
            var selected = 0;
            if (_pressKeyControl == ShowOverlay_Button)
            {
                selected = 1;
            }
            else if (_pressKeyControl == HideOverlay_Button)
            {
                selected = 2;
            }
            else if (_pressKeyControl == CompareOverlay_Button)
            {
                selected = 3;
            }
            if (selected != 0)
            {
                var kpc = new KeyPressCheck(selected);
                kpc.ShowDialog(this);
            }
        }

        private void TransParentBarScroll(object sender, EventArgs e)
        {
            var tb = (sender as TrackBar);
            Program.Settings["Overlay_Transparent"] = tb.Value.ToString();
            TransParent_Text.Text = Program.Settings["Overlay_Transparent"] + "%";
            _overlayInfo?.ChangeTransparent(tb.Value);
        }

        private void GithubClick(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Links.GITHUB_REPO,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void CheckUpdate_Click(object sender, EventArgs e)
        {
            (sender as Control).Enabled = false;
            Task task = Task.Factory.StartNew(() => UpdateTask(sender as Control));
        }

        private int UpdateTask(Control control)
        {
            try
            {
                var check = string.Empty;
                using var httpClient = new HttpClient();
                using (var response = httpClient.GetAsync($"{Links.GITHUB_REPO}").ConfigureAwait(false).GetAwaiter().GetResult())
                {
                    using var content = response.Content;
                    check = content.ReadAsStringAsync().Result;
                }
                if (!check.Equals(""))
                {
                    var sp = check.Split('\n')[0];
                    if (sp.Contains("Tarkov Price Viewer"))
                    {
                        var sp2 = sp.Split(' ');
                        sp = sp2[sp2.Length - 1].Trim();
                        if (!Program.Settings["Version"].Equals(sp))
                        {
                            MessageBox.Show($"New version ({sp}) found. Please download new version. Current Version is {Program.Settings["Version"]}");
                            Process.Start(Links.GITHUB_REPO);
                        }
                        else
                        {
                            MessageBox.Show("Current version is newest.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on checking for updates...");
                Debug.WriteLine(ex.Message);
            }

            Action show = delegate ()
            {
                control.Enabled = true;
            };
            Invoke(show);

            return 0;
        }

        private void LastPriceBoxCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["Show_Last_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void DayPriceBoxCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["Show_Day_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void WeekPriceBoxCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["Show_Week_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void SellToTraderBoxCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["Sell_to_Trader"] = (sender as CheckBox).Checked.ToString();
        }

        private void BuyFromTraderBoxCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["Buy_From_Trader"] = (sender as CheckBox).Checked.ToString();
        }

        private void NeedsBoxCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["Needs"] = (sender as CheckBox).Checked.ToString();
        }

        private void BartersAndCraftsBoxCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["Barters_and_Crafts"] = (sender as CheckBox).Checked.ToString();
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void RandomItemCheckedChanged(object sender, EventArgs e)
        {
            Program.Settings["RandomItem"] = (sender as CheckBox).Checked.ToString();
        }

        private void CheckIdleTimeTick(object sender, EventArgs e)
        {
            if (GetIdleTime() >= _idleTime)
            {
                _idleTime += 3600000;
                SetHook(true);
            }
            else
            {
                if (_idleTime > 3600000)
                {
                    _idleTime = 3600000;
                }
                SetHook();
            }
        }

        private void RefreshBClick(object sender, EventArgs e)
        {
            SetHook(true);
        }
    }
}
