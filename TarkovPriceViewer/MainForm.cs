using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using TarkovPriceViewer;
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

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        private enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        private static readonly int WH_KEYBOARD_LL = 13;
        private static readonly int WM_KEYUP = 0x101;
        private static readonly int WH_MOUSE_LL = 14;
        private static readonly int WM_MOUSEMOVE = 0x200;
        private static bool isinfoclosed = true;
        private static bool iscompareclosed = true;
        private static LowLevelProc _proc_keyboard = null;
        private static LowLevelProc _proc_mouse = null;
        private static IntPtr hhook_keyboard = IntPtr.Zero;
        private static IntPtr hhook_mouse = IntPtr.Zero;
        private static readonly IntPtr h_instance = LoadLibrary("User32");
        private static System.Drawing.Point point = new System.Drawing.Point(0, 0);
        private static int nFlags = 0x0;
        private static readonly Overlay overlay_info = new Overlay(true);
        private static readonly Overlay overlay_compare = new Overlay(false);
        private static long presstime = 0;
        private static CancellationTokenSource cts_info = new CancellationTokenSource();
        private static CancellationTokenSource cts_compare = new CancellationTokenSource();
        private static Control press_key_control = null;
        private static Scalar linecolor = new Scalar(90, 89, 82);
        private static long idle_time = 3600000;

        public MainForm()
        {
            var style = GetWindowLong(Handle, GWL_EXSTYLE);
            SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED);
            if (Environment.OSVersion.Version.Major >= 6)
            {
                nFlags = 0x2;
            }
            InitializeComponent();
            SettingUI();
            SetHook();
            overlay_info.Owner = this;
            overlay_info.Show();
            overlay_compare.Owner = this;
            overlay_compare.Show();
        }

        private void SettingUI()
        {
            MinimizeBox = false;
            MaximizeBox = false;
            Version.Text = Program.settings["Version"];
            MinimizetoTrayWhenStartup.Checked = Convert.ToBoolean(Program.settings["MinimizetoTrayWhenStartup"]);
            CloseOverlayWhenMouseMoved.Checked = Convert.ToBoolean(Program.settings["CloseOverlayWhenMouseMoved"]);
            RandomItem.Checked = Convert.ToBoolean(Program.settings["RandomItem"]);
            last_price_box.Checked = Convert.ToBoolean(Program.settings["Show_Last_Price"]);
            day_price_box.Checked = Convert.ToBoolean(Program.settings["Show_Day_Price"]);
            week_price_box.Checked = Convert.ToBoolean(Program.settings["Show_Week_Price"]);
            sell_to_trader_box.Checked = Convert.ToBoolean(Program.settings["Sell_to_Trader"]);
            buy_from_trader_box.Checked = Convert.ToBoolean(Program.settings["Buy_From_Trader"]);
            needs_box.Checked = Convert.ToBoolean(Program.settings["Needs"]);
            barters_and_crafts_box.Checked = Convert.ToBoolean(Program.settings["Barters_and_Crafts"]);
            ShowOverlay_Button.Text = ((Keys)int.Parse(Program.settings["ShowOverlay_Key"])).ToString();
            HideOverlay_Button.Text = ((Keys)int.Parse(Program.settings["HideOverlay_Key"])).ToString();
            CompareOverlay_Button.Text = ((Keys)int.Parse(Program.settings["CompareOverlay_Key"])).ToString();
            TransParent_Bar.Value = int.Parse(Program.settings["Overlay_Transparent"]);
            TransParent_Text.Text = Program.settings["Overlay_Transparent"];

            TrayIcon.Visible = true;
            check_idle_time.Start();
        }

        private void MainForm_load(object sender, EventArgs e)
        {
            //not use
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
                    Debug.WriteLine("force unhook.");
                    UnHook();
                }
                if (hhook_keyboard == IntPtr.Zero)
                {
                    _proc_keyboard = hookKeyboardProc;
                    hhook_keyboard = SetWindowsHookEx(WH_KEYBOARD_LL, _proc_keyboard, h_instance, 0);
                }
                if (Convert.ToBoolean(Program.settings["CloseOverlayWhenMouseMoved"]))
                {
                    setMouseHook();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void setMouseHook()
        {
            if (hhook_mouse == IntPtr.Zero)
            {
                _proc_mouse = hookMouseProc;
                hhook_mouse = SetWindowsHookEx(WH_MOUSE_LL, _proc_mouse, h_instance, 0);
            }
        }

        private void unsetMouseHook()
        {
            if (hhook_mouse != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hhook_mouse);
                hhook_mouse = IntPtr.Zero;
                _proc_mouse = null;
            }
        }

        private void UnHook()
        {
            try
            {
                if (hhook_keyboard != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(hhook_keyboard);
                    hhook_keyboard = IntPtr.Zero;
                    _proc_keyboard = null;
                }
                unsetMouseHook();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private IntPtr hookKeyboardProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (code >= 0 && wParam == (IntPtr)WM_KEYUP)
                {
                    if (press_key_control == null)
                    {
                        if (Program.finishloadingballistics)
                        {
                            var vkCode = Marshal.ReadInt32(lParam);
                            if (vkCode == int.Parse(Program.settings["ShowOverlay_Key"]))
                            {
                                var CurrentTime = DateTime.Now.Ticks;
                                if (CurrentTime - presstime > 2000000)
                                {
                                    point = Control.MousePosition;
                                    LoadingItemInfo();
                                }
                                else
                                {
                                    Debug.WriteLine("key pressed in 0.2 seconds.");
                                }
                                presstime = CurrentTime;
                            }
                            else if (vkCode == int.Parse(Program.settings["CompareOverlay_Key"]))
                            {
                                point = Control.MousePosition;
                                LoadingItemCompare();
                            }
                            else if (vkCode == int.Parse(Program.settings["HideOverlay_Key"])
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
                            point = Control.MousePosition;
                            overlay_info.ShowWaitBallistics(point);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return CallNextHookEx(hhook_keyboard, code, (int)wParam, lParam);
        }

        private IntPtr hookMouseProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (!isinfoclosed && code >= 0
                    && wParam == (IntPtr)WM_MOUSEMOVE
                    && (Math.Abs(Control.MousePosition.X - point.X) > 5 || Math.Abs(Control.MousePosition.Y - point.Y) > 5))
                {
                    CloseItemInfo();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return CallNextHookEx(hhook_mouse, code, (int)wParam, lParam);
        }

        private uint GetIdleTime()
        {
            var LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)Marshal.SizeOf(LastUserAction);
            GetLastInputInfo(ref LastUserAction);
            return ((uint)Environment.TickCount - LastUserAction.dwTime);
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
            isinfoclosed = false;
            cts_info.Cancel();
            cts_info = new CancellationTokenSource();
            overlay_info.ShowLoadingInfo(point, cts_info.Token);
            Task task = Task.Factory.StartNew(() => FindItemTask(true, cts_info.Token));
        }

        public void LoadingItemCompare()
        {
            if (iscompareclosed)
            {
                iscompareclosed = false;
                cts_compare.Cancel();
                cts_compare = new CancellationTokenSource();
            }
            overlay_compare.ShowLoadingCompare(point, cts_compare.Token);
            Task task = Task.Factory.StartNew(() => FindItemTask(false, cts_compare.Token));
        }

        public void CloseItemInfo()
        {
            isinfoclosed = true;
            cts_info.Cancel();
            overlay_info.HideInfo();
        }

        public void CloseItemCompare()
        {
            iscompareclosed = true;
            cts_compare.Cancel();
            overlay_compare.HideCompare();
        }

        private int FindItemTask(bool isiteminfo, CancellationToken cts_one)
        {
            if (Convert.ToBoolean(Program.settings["RandomItem"]))
            {
                if (!cts_one.IsCancellationRequested)
                {
                    var item = Program.itemlist[new Random().Next(Program.itemlist.Count - 1)];
                    //item = MatchItemName("7.62x54r_7n37".ToLower().Trim().ToCharArray());
                    FindItemInfo(item, isiteminfo, cts_one);
                }
            }
            else
            {
                var fullimage = CaptureScreen(CheckisTarkov());
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
                            overlay_info.ShowInfo(null, cts_one);
                        }
                        else
                        {
                            overlay_compare.ShowCompare(null, cts_one);
                        }
                    }
                    Debug.WriteLine("image null");
                }
            }
            return 0;
        }

        private IntPtr CheckisTarkov()
        {
            var hWnd = GetForegroundWindow();
            if (hWnd != IntPtr.Zero)
            {
                var sbWinText = new StringBuilder(260);
                GetWindowText(hWnd, sbWinText, 260);
                if (sbWinText.ToString() == Program.appname)
                {
                    return hWnd;
                }
            }
            Debug.WriteLine("error - no app");
            return IntPtr.Zero;
        }

        private Bitmap CaptureScreen(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                using (var Graphicsdata = Graphics.FromHwnd(hWnd))
                {
                    var rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);
                    var bmp = new Bitmap(rect.Width, rect.Height);
                    using (var g = Graphics.FromImage(bmp))
                    {
                        var hdc = g.GetHdc();
                        PrintWindow(hWnd, hdc, nFlags);
                        g.ReleaseHdc(hdc);
                    }
                    return bmp;
                }
            }
            else
            {
#if DEBUG
                try
                {
                    return new Bitmap(@"img\test.png");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("no test img" + e.Message);
                }
#endif
                Debug.WriteLine("error - no window");
                return null;
            }
        }

        private void ShowtestImage(Mat mat)
        {
            ShowtestImage("test", mat);
        }

        private void ShowtestImage(string name, Mat mat)
        {
            Action show = delegate ()
            {
                Cv2.ImShow(name, mat);
            };
            Invoke(show);
        }

        private string getTesseract(Mat textmat)
        {
            var text = "";
            try
            {
                using var ocr = new TesseractEngine(@"./Resources/tessdata", "eng", EngineMode.Default); //should use once
                using var temp = BitmapConverter.ToBitmap(textmat);
                using var texts = ocr.Process(Pix.LoadFromMemory(ImageToByte(temp)));
                text = texts.GetText().Replace("\n", " ").Split(Program.splitcur)[0].Trim();
                Debug.WriteLine("text : " + text);
            }
            catch (Exception e)
            {
                Debug.WriteLine("tesseract error " + e.Message);
            }

            return text;
        }

        public static byte[] ImageToByte(Image img)
        {
            var converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void FindItem(Bitmap fullimage, bool isiteminfo, CancellationToken cts_one)
        {
            var item = new Item();
            using (var ScreenMat_original = BitmapConverter.ToMat(fullimage))
            using (var ScreenMat = ScreenMat_original.CvtColor(ColorConversionCodes.BGRA2BGR))
            using (var rac_img = ScreenMat.InRange(linecolor, linecolor))
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
                            var text = getTesseract(temp2);
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

        private Item MatchItemName(char[] itemname)
        {
            var result = new Item();
            var d = 999;
            foreach (var item in Program.itemlist)
            {
                var d2 = levenshteinDistance(itemname, item.NameCompare);
                if (d2 < d)
                {
                    result = item;
                    d = d2;
                    if (item.IsName2)
                    {
                        item.IsName2 = false;
                    }
                    if (d == 0)
                    {
                        break;
                    }
                }
                d2 = levenshteinDistance(itemname, item.NameCompare2);
                if (d2 < d)
                {
                    result = item;
                    d = d2;
                    item.IsName2 = true;
                    if (d == 0)
                    {
                        break;
                    }
                }
            }
            Debug.WriteLine(d + " text match : " + result.NameDisplay + " & " + result.NameDisplay2);
            return result;
        }

        private int getMinimum(int val1, int val2, int val3)
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

        private int levenshteinDistance(char[] s, char[] t)
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
                        d[i, j] = getMinimum(d[i - 1, j], d[i, j - 1], d[i - 1, j - 1]) + 1;
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
                        using (var wc = new TPVWebClient())
                        {
                            var doc = new HtmlAgilityPack.HtmlDocument();
                            Debug.WriteLine(Program.tarkovmarket + item.MarketAddress);
                            doc.LoadHtml(wc.DownloadString(Program.tarkovmarket + item.MarketAddress));
                            var node_tm = doc.DocumentNode.SelectSingleNode("//div[@class='market-data']");
                            HtmlAgilityPack.HtmlNode sub_node_tm = null;
                            HtmlAgilityPack.HtmlNode sub_node_tm2 = null;
                            HtmlAgilityPack.HtmlNodeCollection nodes = null;
                            HtmlAgilityPack.HtmlNodeCollection subnodes = null;
                            HtmlAgilityPack.HtmlNodeCollection subnodes2 = null;
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
                                overlay_info.ShowInfo(item, cts_one);
                            }
                            else
                            {
                                overlay_compare.ShowCompare(item, cts_one);
                            }
                            var isdisconnected = false;
                            do
                            {
                                isdisconnected = false;
                                try
                                {
                                    Debug.WriteLine(Program.wiki + item.WikiAddress);
                                    try
                                    {
                                        doc.LoadHtml(wc.DownloadString(Program.wiki + item.WikiAddress));
                                    }
                                    catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                                    {
                                        Debug.WriteLine(ex.Message);
                                        Debug.WriteLine(Program.wiki + item.NameDisplay2.Replace(" ", "_"));
                                        doc.LoadHtml(wc.DownloadString(Program.wiki + item.NameDisplay2.Replace(" ", "_")));
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
                                                                            if (!Program.blist.TryGetValue(item.NameDisplay, out item.Ballistic))
                                                                            {
                                                                                Program.blist.TryGetValue(item.NameDisplay2, out item.Ballistic);
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
                                                                        Program.blist.TryGetValue(temp_node_list2[n].InnerText.Trim(), out item.Ballistic);
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
                                catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
                                {
                                    isdisconnected = true;
                                    Debug.WriteLine("wiki reconnected...");
                                }
                            } while (!cts_one.IsCancellationRequested && isdisconnected);
                        }
                    }
                    item.LastFetch = DateTime.Now;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            if (isiteminfo)
            {
                overlay_info.ShowInfo(item, cts_one);
            }
            else
            {
                overlay_compare.ShowCompare(item, cts_one);
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//must be checked
            {
                TrayIcon.Visible = true;
                Hide();
                e.Cancel = true;
            }
        }

        private void MinimizetoTrayWhenStartup_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["MinimizetoTrayWhenStartup"] = (sender as CheckBox).Checked.ToString();
        }

        private void Tarkov_Official_Click(object sender, EventArgs e)
        {
            Process.Start(Program.official);
        }

        private void TarkovWiki_Click(object sender, EventArgs e)
        {
            Process.Start(Program.wiki);
        }

        private void TarkovMarket_Click(object sender, EventArgs e)
        {
            Process.Start(Program.tarkovmarket);
        }

        private void CloseOverlayWhenMouseMoved_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["CloseOverlayWhenMouseMoved"] = (sender as CheckBox).Checked.ToString();
            if ((sender as CheckBox).Checked)
            {
                setMouseHook();
            }
            else
            {
                unsetMouseHook();
            }
        }

        public void ChangePressKeyData(Keys? keycode)
        {
            if (press_key_control != null)
            {
                if (keycode != null)
                {
                    press_key_control.Text = keycode.ToString();
                }
                press_key_control = null;
            }
        }

        private void Overlay_Button_Click(object sender, EventArgs e)
        {
            press_key_control = (sender as Control);
            var selected = 0;
            if (press_key_control == ShowOverlay_Button)
            {
                selected = 1;
            }
            else if (press_key_control == HideOverlay_Button)
            {
                selected = 2;
            }
            else if (press_key_control == CompareOverlay_Button)
            {
                selected = 3;
            }
            if (selected != 0)
            {
                var kpc = new KeyPressCheck(selected);
                kpc.ShowDialog(this);
            }
        }

        private void TransParent_Bar_Scroll(object sender, EventArgs e)
        {
            var tb = (sender as TrackBar);
            Program.settings["Overlay_Transparent"] = tb.Value.ToString();
            TransParent_Text.Text = Program.settings["Overlay_Transparent"] + "%";
            overlay_info.ChangeTransparent(tb.Value);
        }

        private void Github_Click(object sender, EventArgs e)
        {
            Process.Start(Program.github);
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
                using (var wc = new TPVWebClient())
                {
                    var check = wc.DownloadString(Program.checkupdate);
                    if (!check.Equals(""))
                    {
                        var sp = check.Split('\n')[0];
                        if (sp.Contains("Tarkov Price Viewer"))
                        {
                            var sp2 = sp.Split(' ');
                            sp = sp2[sp2.Length - 1].Trim();
                            if (!Program.settings["Version"].Equals(sp))
                            {
                                MessageBox.Show("New version (" + sp + ") found. Please download new version. Current Version is " + Program.settings["Version"]);
                                Process.Start(Program.github);
                            }
                            else
                            {
                                MessageBox.Show("Current version is newest.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Can not check update. Please check your network.");
            }
            Action show = delegate ()
            {
                control.Enabled = true;
            };
            Invoke(show);
            return 0;
        }

        private void last_price_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Show_Last_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void day_price_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Show_Day_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void week_price_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Show_Week_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void sell_to_trader_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Sell_to_Trader"] = (sender as CheckBox).Checked.ToString();
        }

        private void buy_from_trader_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Buy_From_Trader"] = (sender as CheckBox).Checked.ToString();
        }

        private void needs_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Needs"] = (sender as CheckBox).Checked.ToString();
        }

        private void barters_and_crafts_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Barters_and_Crafts"] = (sender as CheckBox).Checked.ToString();
        }

        private void Exit_Button_Click(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void RandomItem_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["RandomItem"] = (sender as CheckBox).Checked.ToString();
        }

        private void check_idle_time_Tick(object sender, EventArgs e)
        {
            if (GetIdleTime() >= idle_time)
            {
                idle_time += 3600000;
                SetHook(true);
            }
            else
            {
                if (idle_time > 3600000)
                {
                    idle_time = 3600000;
                }
                SetHook();
            }
        }

        private void refresh_b_Click(object sender, EventArgs e)
        {
            SetHook(true);
        }
    }
}
