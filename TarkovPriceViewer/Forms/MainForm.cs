using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using TarkovPriceViewer.Infrastructure.Constants;
using TarkovPriceViewer.Infrastructure.Entities;
using TarkovPriceViewer.Infrastructure.JsonWriter;
using TarkovPriceViewer.Infrastructure.Services;
using TarkovPriceViewer.Infrastructure.Settings;
using TarkovPriceViewer.Properties;
using Tesseract;

namespace TarkovPriceViewer.Forms
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

        private static LowLevelProc? _procKeyboard = null;
        private static LowLevelProc? _procMouse = null;
        private static Control? _pressKeyControl = null;

        private static IntPtr _hhookKeyboard = IntPtr.Zero;
        private static IntPtr _hhookMouse = IntPtr.Zero;

        private static readonly HashSet<string> _beType = new HashSet<string> { "Round", "Slug", "Buckshot", "Grenade launcher cartridge" };

        private static System.Drawing.Point _point = new System.Drawing.Point(0, 0);
        private static CancellationTokenSource _cancellationTokenInfo = new CancellationTokenSource();
        private static CancellationTokenSource _cancellationTokenCompare = new CancellationTokenSource();
        private static Scalar _lineColor = new Scalar(90, 89, 82);
        private static long _idleTime = 3600000;
        private static string _transparency = "80";

        private List<Item>? _itemList;
        private Dictionary<string, Ballistic>? _ballisticDic;

        private readonly ILogger<MainForm> _logger;
        private readonly InfoOverlay _overlayInfo;
        private readonly CompareOverlay _overlayCompare;
        private readonly KeyPressCheck _keyPressCheck;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBallisticService _ballisticService;
        private readonly IItemService _itemService;
        private readonly IStringLocalizer<Resources> _resources;
        private readonly IWritableOptions<SettingsOptions> _writableOptions;

        public MainForm(
            ILogger<MainForm> logger,
            InfoOverlay infoOverlay,
            CompareOverlay compareOverlay,
            KeyPressCheck keyPressCheck,
            IServiceProvider serviceProvider,
            IBallisticService ballisticService,
            IItemService itemService,
            IStringLocalizer<Resources> resources,
            IWritableOptions<SettingsOptions> writableOptions
        )
        {
            var style = GetWindowLong(Handle, GWL_EXSTYLE);

            SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED);

            if (Environment.OSVersion.Version.Major >= 6)
            {
                _nFlags = 0x2;
            }

            InitializeComponent();

            _ballisticService = ballisticService;
            _itemService = itemService;

            _keyPressCheck = keyPressCheck;
            _serviceProvider = serviceProvider;

            _overlayInfo = infoOverlay;
            _overlayInfo.Owner = this;
            _overlayInfo.Show();

            _overlayCompare = compareOverlay;
            _overlayCompare.Owner = this;
            _overlayCompare.Show();

            _resources = resources;
            _writableOptions = writableOptions;
            _logger = logger;

            SettingUI();
            SetHook();
        }

        private void SettingUI()
        {
            using var scope = _serviceProvider.CreateScope();
            var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SettingsOptions>>().Value;

            _transparency = options.OverlayTransparency;

            MinimizeBox = false;
            MaximizeBox = false;
            Version.Text = Program.VERSION;
            MinimizetoTrayWhenStartup.Checked = options.MinimizetoTrayWhenStartup;
            CloseOverlayWhenMouseMoved.Checked = options.CloseOverlayWhenMouseMoved;
            RandomItem.Checked = options.RandomItem;
            LastPriceChkBox.Checked = options.ShowLastPrice;
            DayPriceChkBox.Checked = options.ShowDayPrice;
            WeekPriceChkBox.Checked = options.ShowWeekPrice;
            SellToTraderChkBox.Checked = options.SellToTrader;
            BuyFromTraderChkBox.Checked = options.BuyFromTrader;
            NeddsChkBox.Checked = options.Needs;
            BartersNCraftsChkBox.Checked = options.BartersNCrafts;
            ShowOverlayButton.Text = ((Keys)int.Parse(options.ShowOverlayKey)).ToString();
            HideOverlayButton.Text = ((Keys)int.Parse(options.HideOverlayKey)).ToString();
            CompareOverlayButton.Text = ((Keys)int.Parse(options.CompareOverlayKey)).ToString();
            TransparentBar.Value = int.Parse(_transparency);
            TransparentText.Text = _transparency;

            TrayIcon.Visible = true;
            checkIdleTime.Start();
        }

        private void SetHook()
        {
            SetHook(false);
        }

        private void SetHook(bool force)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SettingsOptions>>().Value;

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
                if (options.CloseOverlayWhenMouseMoved)
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
                using var scope = _serviceProvider.CreateScope();
                var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SettingsOptions>>().Value;

                if (code >= 0 && wParam == (IntPtr)WM_KEYUP)
                {
                    if (_pressKeyControl == null)
                    {
                        var vkCode = Marshal.ReadInt32(lParam);
                        if (vkCode == int.Parse(options.ShowOverlayKey))
                        {
                            var CurrentTime = DateTime.Now.Ticks;
                            if (CurrentTime - _pressTime > 2000000)
                            {
                                _point = MousePosition;
                                LoadingItemInfoAsync();
                            }
                            else
                            {
                                _logger.LogDebug("key pressed in 0.2 seconds");
                            }
                            _pressTime = CurrentTime;
                        }
                        else if (vkCode == int.Parse(options.CompareOverlayKey))
                        {
                            _point = MousePosition;
                            LoadingItemCompareAsync();
                        }
                        else if (vkCode == int.Parse(options.HideOverlayKey)
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

        private void CloseApp()
        {
            UnHook();

            TrayIcon.Dispose();

            CloseItemInfo();
            CloseItemCompare();

            _writableOptions.Update(opt =>
            {
                opt.OverlayTransparency = TransparentBar.Value.ToString();
            });

            Application.Exit();
        }

        public async void LoadingItemInfoAsync()
        {
            _isInfoClosed = false;
            _cancellationTokenInfo.Cancel();
            _cancellationTokenInfo = new CancellationTokenSource();
            _overlayInfo?.ShowLoadingInfo(_point, _cancellationTokenInfo.Token);

            await FindItemTaskAsync(true, _cancellationTokenInfo.Token);
        }

        public async void LoadingItemCompareAsync()
        {
            if (_isCompareClosed)
            {
                _isCompareClosed = false;
                _cancellationTokenCompare.Cancel();
                _cancellationTokenCompare = new CancellationTokenSource();
            }
            _overlayCompare?.ShowLoadingCompare(_point, _cancellationTokenCompare.Token);

            await FindItemTaskAsync(false, _cancellationTokenCompare.Token);
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

        private Task<int> FindItemTaskAsync(bool isItemInfo, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SettingsOptions>>().Value;

            if (options.RandomItem)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    var item = _itemList[new Random().Next(_itemList.Count - 1)];

                    FindItemInfoAsync(item, isItemInfo, cancellationToken);
                }
            }
            else
            {
                var fullimage = CaptureScreen(CheckIsTarkov());
                if (fullimage != null)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        FindItem(fullimage, isItemInfo, cancellationToken);
                    }
                }
                else
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        if (isItemInfo)
                        {
                            _overlayInfo?.ShowInfo(null, cancellationToken);
                        }
                        else
                        {
                            _overlayCompare?.ShowCompare(null, cancellationToken);
                        }
                    }

                    _logger.LogDebug("Finding item, image null");
                }
            }

            return Task.FromResult(0);
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

        private string GetTesseract(Mat textMat)
        {
            var text = string.Empty;
            try
            {
                using var ocr = new TesseractEngine(@"./Resources/tessdata", "eng", EngineMode.Default); //should use once
                using var textMatToBitmap = BitmapConverter.ToBitmap(textMat);
                using var texts = ocr.Process(Pix.LoadFromMemory(ImageToByte(textMatToBitmap)));
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

        private void FindItem(Bitmap fullImage, bool isItemInfo, CancellationToken cancellationToken)
        {
            var item = new Item();
            using (var screenMatOriginal = BitmapConverter.ToMat(fullImage))
            using (var screenMat = screenMatOriginal.CvtColor(ColorConversionCodes.BGRA2BGR))
            using (var racImg = screenMat.InRange(_lineColor, _lineColor))
            {
                racImg.FindContours(out var contours, out var hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                foreach (var contour in contours)
                {
                    if (cancellationToken.IsCancellationRequested is false)
                    {
                        var rect2 = Cv2.BoundingRect(contour);
                        if (rect2.Width > 5 && rect2.Height > 10)
                        {
                            screenMat.Rectangle(rect2, Scalar.Black, 2);
                            using var temp = screenMat.SubMat(rect2);
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
                if (cancellationToken.IsCancellationRequested is false)
                {
                    FindItemInfoAsync(item, isItemInfo, cancellationToken);
                }
            }
            fullImage.Dispose();
        }

        private Item MatchItemName(char[] itemName)
        {
            var result = new Item();
            var distance = 999;
            foreach (var item in _itemList)
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

        private async void FindItemInfoAsync(Item item, bool isItemInfo, CancellationToken cancellationToken)
        {
            if (item.MarketAddress != null)
            {
                try
                {
                    if (item.LastFetch == null || (DateTime.Now - item.LastFetch).TotalHours >= 1)
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();

                        using var httpClient = new HttpClient();
                        using (var response = await httpClient.GetAsync($"{Links.TARKOV_MARKET}{item.MarketAddress}"))
                        {
                            using var content = response.Content;
                            var json = await content.ReadAsStringAsync(cancellationToken);

                            doc.LoadHtml(json);
                        }

                        var nodeTm = doc.DocumentNode.SelectSingleNode("//div[@class='market-data']");
                        HtmlAgilityPack.HtmlNode? subNodeTm = null;
                        HtmlAgilityPack.HtmlNode? subNodeTm2 = null;
                        HtmlAgilityPack.HtmlNodeCollection? nodes = null;
                        HtmlAgilityPack.HtmlNodeCollection? subNodes = null;
                        HtmlAgilityPack.HtmlNodeCollection? subNodes2 = null;
                        if (nodeTm != null)
                        {
                            nodes = nodeTm.SelectNodes(".//div[@class='w-25']");
                            subNodeTm = nodeTm.SelectSingleNode(".//div[@class='sub']");
                            if (subNodeTm != null)
                            {
                                item.LastUpdate = subNodeTm.InnerText.Trim();
                            }
                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    subNodeTm = node.FirstChild;
                                    if (subNodeTm != null)
                                    {
                                        if (subNodeTm.InnerText.Trim().Contains("Flea price"))
                                        {
                                            subNodeTm = node.SelectSingleNode(".//div[@class='big bold alt']");
                                            if (subNodeTm != null)
                                            {
                                                item.PriceLast = subNodeTm.InnerText.Trim();
                                            }
                                        }
                                        else if (subNodeTm.InnerText.Trim().Contains("Average price"))
                                        {
                                            subNodes = node.SelectNodes(".//span[@class='bold alt']");
                                            if (subNodes != null && subNodes.Count >= 2)
                                            {
                                                item.PriceDay = subNodes[0].InnerText.Trim();
                                                item.PriceWeek = subNodes[1].InnerText.Trim();
                                            }
                                        }
                                        else
                                        {
                                            subNodeTm2 = subNodeTm.SelectSingleNode(".//div[@class='bold']");
                                            if (subNodeTm2 != null)
                                            {
                                                var temp = subNodeTm.InnerText.Trim().Split(' ');
                                                if (temp.Length > 2)
                                                {
                                                    if (subNodeTm.InnerText.Trim().Contains("LL"))
                                                    {
                                                        item.BuyFromTrader = temp[0].Trim() + " " + temp[1];
                                                        subNodeTm = node.SelectSingleNode(".//div[@class='bold alt']");
                                                        if (subNodeTm != null)
                                                        {
                                                            item.BuyFromTraderPrice = subNodeTm.InnerText.Trim();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        item.SellToTrader = temp[0].Trim();
                                                        subNodeTm = node.SelectSingleNode(".//div[@class='bold alt']");
                                                        if (subNodeTm != null)
                                                        {
                                                            item.SellToTraderPrice = subNodeTm.InnerText.Trim();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (isItemInfo)
                        {
                            _overlayInfo?.ShowInfo(item, cancellationToken);
                        }
                        else
                        {
                            _overlayCompare?.ShowCompare(item, cancellationToken);
                        }

                        var isDisconnected = false;
                        do
                        {
                            isDisconnected = false;
                            try
                            {
                                _logger.LogDebug("Search for item info on Wiki {WikiAddress}", $"{Links.WIKI}{item.WikiAddress}");
                                try
                                {
                                    using var response = await httpClient.GetAsync($"{Links.WIKI}{item.WikiAddress}");
                                    using var content = response.Content;
                                    var json = await content.ReadAsStringAsync();

                                    doc.LoadHtml(json);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Not found, searching in new link {WikiAddressNameDisplay2}", $"{Links.WIKI}{item.NameDisplay2.Replace(" ", "_")}");

                                    using var response = await httpClient.GetAsync($"{Links.WIKI}{ item.NameDisplay2.Replace(" ", "_")}");
                                    using var content = response.Content;
                                    var json = await content.ReadAsStringAsync(cancellationToken);

                                    doc.LoadHtml(json);
                                }

                                nodeTm = doc.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']");
                                if (nodeTm != null)
                                {
                                    var sb = new StringBuilder();
                                    nodes = nodeTm.SelectNodes(".//li");
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
                                    subNodeTm = nodeTm.SelectSingleNode(".//td[@class='va-infobox-cont']");
                                    if (subNodeTm != null)
                                    {
                                        nodes = subNodeTm.SelectNodes(".//table[@class='va-infobox-group']");
                                        if (nodes != null)
                                        {
                                            foreach (var node in nodes)
                                            {
                                                var tempNode = node.SelectSingleNode(".//th[@class='va-infobox-header']");
                                                if (tempNode != null)
                                                {
                                                    if (tempNode.InnerText.Trim().Equals("General data"))
                                                    {
                                                        var tempNodeList = subNodeTm.SelectNodes(".//td[@class='va-infobox-label']");
                                                        var tempNodeList2 = subNodeTm.SelectNodes(".//td[@class='va-infobox-content']");
                                                        if (tempNodeList != null && tempNodeList2 != null && tempNodeList.Count == tempNodeList2.Count)
                                                        {
                                                            for (var n = 0; n < tempNodeList.Count; n++)
                                                            {
                                                                if (tempNodeList[n].InnerText.Trim().Contains("Type"))
                                                                {
                                                                    item.Type = tempNodeList2[n].InnerText.Trim();
                                                                    if (_beType.Contains(item.Type))
                                                                    {
                                                                        if (!_ballisticDic.TryGetValue(item.NameDisplay, out item.Ballistic))
                                                                        {
                                                                            _ballisticDic.TryGetValue(item.NameDisplay2, out item.Ballistic);
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (tempNode.InnerText.Trim().Equals("Performance"))
                                                    {
                                                        var tempNodeList = subNodeTm.SelectNodes(".//td[@class='va-infobox-label']");
                                                        var tempNodeList2 = subNodeTm.SelectNodes(".//td[@class='va-infobox-content']");
                                                        if (tempNodeList != null && tempNodeList2 != null && tempNodeList.Count == tempNodeList2.Count)
                                                        {
                                                            for (var n = 0; n < tempNodeList.Count; n++)
                                                            {
                                                                if (tempNodeList[n].InnerText.Trim().Contains("Recoil"))
                                                                {
                                                                    item.Recoil = tempNodeList2[n].InnerText.Trim();
                                                                }
                                                                else if (tempNodeList[n].InnerText.Trim().Contains("Ergonomics"))
                                                                {
                                                                    item.Ergo = tempNodeList2[n].InnerText.Trim();
                                                                }
                                                                else if (tempNodeList[n].InnerText.Trim().Contains("Accuracy"))
                                                                {
                                                                    item.Accuracy = tempNodeList2[n].InnerText.Trim();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (tempNode.InnerText.Trim().Equals("Ammunition"))
                                                    {
                                                        var tempNodeList = subNodeTm.SelectNodes(".//td[@class='va-infobox-label']");
                                                        var tempNodeList2 = subNodeTm.SelectNodes(".//td[@class='va-infobox-content']");
                                                        if (tempNodeList != null && tempNodeList2 != null && tempNodeList.Count == tempNodeList2.Count)
                                                        {
                                                            for (var n = 0; n < tempNodeList.Count; n++)
                                                            {
                                                                if (tempNodeList[n].InnerText.Trim().Contains("Default ammo"))
                                                                {
                                                                    _ballisticDic.TryGetValue(tempNodeList2[n].InnerText.Trim(), out item.Ballistic);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    nodes = nodeTm.SelectNodes(".//table[@class='wikitable']");
                                    if (nodes != null)
                                    {
                                        var craftsb = new StringBuilder();
                                        foreach (var node in nodes)
                                        {
                                            subNodes = node.SelectNodes(".//tr");
                                            if (subNodes != null)
                                            {
                                                foreach (var node2 in subNodes)
                                                {
                                                    subNodes2 = node2.SelectNodes(".//th");
                                                    if (subNodes2 != null && subNodes2.Count >= 5)
                                                    {
                                                        var craftList = new List<string>();
                                                        foreach (var temp in subNodes2)
                                                        {
                                                            foreach (var temp2 in temp.ChildNodes)
                                                            {
                                                                if (!temp2.InnerText.Trim().Equals(""))
                                                                {
                                                                    craftList.Add(temp2.InnerText.Trim());
                                                                }
                                                            }
                                                        }

                                                        var firstArrow = craftList.IndexOf("→");
                                                        var secondArrow = craftList.LastIndexOf("→");
                                                        var firstList = craftList.GetRange(0, firstArrow);
                                                        var secondList = craftList.GetRange(firstArrow + 1, secondArrow - firstArrow - 1);
                                                        var thirdList = craftList.GetRange(secondArrow + 1, craftList.Count - secondArrow - 1);

                                                        firstList.Reverse();
                                                        if (secondList.Count <= 2)
                                                        {
                                                            secondList.Reverse();
                                                        }
                                                        thirdList.Reverse();
                                                        craftsb.Append(
                                                                    string.Format("{0} → {2} ({1})",
                                                                    string.Join(" ", firstList),
                                                                    string.Join(secondList.Count <= 2 ? " in " : " ", secondList),
                                                                    string.Join(" ", thirdList)))
                                                               .Append("\n");
                                                    }
                                                }
                                            }
                                        }
                                        if (craftsb.ToString().Trim().Equals(string.Empty) is false)
                                        {
                                            item.BartersNCrafts = craftsb.ToString().Trim();
                                        }
                                    }
                                    if (sb.ToString().Trim().Equals(string.Empty) is false)
                                    {
                                        item.Needs = sb.ToString().Trim();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                isDisconnected = true;
                                _logger.LogError(ex, "Exception on searching, wiki reconnected...");
                            }
                        } while (cancellationToken.IsCancellationRequested is false && isDisconnected);
                    }
                    item.LastFetch = DateTime.Now;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception on searching...");
                }
            }

            if (isItemInfo)
            {
                _overlayInfo?.ShowInfo(item, cancellationToken);
            }
            else
            {
                _overlayCompare?.ShowCompare(item, cancellationToken);
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
            _writableOptions.Update(opt =>
            {
                opt.MinimizetoTrayWhenStartup = (sender as CheckBox).Checked;
            });
        }

        private void TarkovOfficialClick(object sender, EventArgs e)
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
            _writableOptions.Update(opt =>
            {
                opt.CloseOverlayWhenMouseMoved = (sender as CheckBox).Checked;
            });
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
            _keyPressCheck.Button = 0;
            if (_pressKeyControl == ShowOverlayButton)
            {
                _keyPressCheck.Button = 1;
            }
            else if (_pressKeyControl == HideOverlayButton)
            {
                _keyPressCheck.Button = 2;
            }
            else if (_pressKeyControl == CompareOverlayButton)
            {
                _keyPressCheck.Button = 3;
            }
            if (_keyPressCheck.Button != 0)
            {
                _keyPressCheck.ShowDialog(this);
            }
        }

        private void TransParentBarScroll(object sender, EventArgs e)
        {
            var tb = (sender as TrackBar);
            _transparency = tb.Value.ToString();
            TransparentText.Text = $"{_transparency}%";
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

        private async void CheckUpdateClick(object sender, EventArgs e)
        {
            (sender as Control).Enabled = false;
            await UpdateTaskAsync(sender as Control);
        }

        private async Task<int> UpdateTaskAsync(Control control)
        {
            try
            {
                var check = string.Empty;
                using var httpClient = new HttpClient();
                using (var response = await httpClient.GetAsync($"{Links.GITHUB_REPO}"))
                {
                    using var content = response.Content;
                    check = await content.ReadAsStringAsync();
                }
                if (check.Equals(string.Empty) is false)
                {
                    var sp = check.Split('\n')[0];
                    if (sp.Contains("Tarkov Price Viewer"))
                    {
                        var sp2 = sp.Split(' ');
                        sp = sp2[sp2.Length - 1].Trim();
                        if (Program.VERSION.Equals(sp) is false)
                        {
                            MessageBox.Show($"New version ({sp}) found. Please download new version. Current Version is {Program.VERSION}");
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
            _writableOptions.Update(opt =>
            {
                opt.ShowLastPrice = (sender as CheckBox).Checked;
            });
        }

        private void DayPriceBoxCheckedChanged(object sender, EventArgs e)
        {
            _writableOptions.Update(opt =>
            {
                opt.ShowDayPrice = (sender as CheckBox).Checked;
            });
        }

        private void WeekPriceBoxCheckedChanged(object sender, EventArgs e)
        {
            _writableOptions.Update(opt =>
            {
                opt.ShowWeekPrice = (sender as CheckBox).Checked;
            });
        }

        private void SellToTraderBoxCheckedChanged(object sender, EventArgs e)
        {
            _writableOptions.Update(opt =>
            {
                opt.SellToTrader = (sender as CheckBox).Checked;
            });
        }

        private void BuyFromTraderBoxCheckedChanged(object sender, EventArgs e)
        {
            _writableOptions.Update(opt =>
            {
                opt.BuyFromTrader = (sender as CheckBox).Checked;
            });
        }

        private void NeedsBoxCheckedChanged(object sender, EventArgs e)
        {
            _writableOptions.Update(opt =>
            {
                opt.Needs = (sender as CheckBox).Checked;
            });
        }

        private void BartersAndCraftsBoxCheckedChanged(object sender, EventArgs e)
        {
            _writableOptions.Update(opt =>
            {
                opt.BartersNCrafts = (sender as CheckBox).Checked;
            });
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void RandomItemCheckedChanged(object sender, EventArgs e)
        {
            _writableOptions.Update(opt =>
            {
                opt.RandomItem = (sender as CheckBox).Checked;
            });
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

        private async void MainFormLoad(object sender, EventArgs e)
        {
            _itemList = await _itemService.GetItemListAsync();
            _ballisticDic = await _ballisticService.GetBallisticsAsync();
        }
    }
}
