using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using TarkovPriceViewer;
using TarkovPriceViewer.Infrastructure.Settings;
using TarkovPriceViewer.Properties;

namespace TarkovPriceChecker
{
    public partial class InfoOverlay : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;

        private static bool _isMoving = false;
        private static int _x, _y;

        private static readonly Regex _inRaidRegex = new Regex(@"in raid");
        private static readonly Regex _moneyRegex = new Regex(@"([\d,]+[₽\$€]|[₽\$€][\d,]+)");

        private static readonly Color[] _beColor = new Color[]
        {
            ColorTranslator.FromHtml("#B32425"),
            ColorTranslator.FromHtml("#DD3333"),
            ColorTranslator.FromHtml("#EB6C0D"),
            ColorTranslator.FromHtml("#AC6600"),
            ColorTranslator.FromHtml("#FB9C0E"),
            ColorTranslator.FromHtml("#006400"),
            ColorTranslator.FromHtml("#009900")
        };

        private readonly IStringLocalizer<Resources> _resources;
        private readonly ILogger<InfoOverlay> _logger;
        private readonly AppSettings _appSettings;

        private readonly object _lock = new object();

        public InfoOverlay(
            IStringLocalizer<Resources> resources,
            ILogger<InfoOverlay> logger,
            IOptions<AppSettings> options
        )
        {
            InitializeComponent();

            _resources = resources;
            _logger = logger;
            _appSettings = options.Value;
            TopMost = true;

            var style = GetWindowLong(Handle, GWL_EXSTYLE);
            Opacity = int.Parse(_appSettings.OverlayTransparency) * 0.01;
            SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT);

            SettingFormPos();
            InitializeBallistics();

            iteminfo_panel.Visible = false;
            itemcompare_panel.Visible = false;
        }

        public void SettingFormPos()
        {
            Location = new Point(0, 0);
            Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        public void InitializeBallistics()
        {
            iteminfo_ball.ColumnCount = 9;
            iteminfo_ball.Columns[0].Name = "Type";
            iteminfo_ball.Columns[1].Name = "Name";
            iteminfo_ball.Columns[2].Name = "Damage";
            iteminfo_ball.Columns[3].Name = "1";
            iteminfo_ball.Columns[4].Name = "2";
            iteminfo_ball.Columns[5].Name = "3";
            iteminfo_ball.Columns[6].Name = "4";
            iteminfo_ball.Columns[7].Name = "5";
            iteminfo_ball.Columns[8].Name = "6";
            iteminfo_ball.Visible = false;
            iteminfo_ball.ClearSelection();
            ResizeGrid(iteminfo_ball);
        }

        public void ResizeGrid(DataGridView view)
        {
            view.ClientSize = new Size(view.Columns.GetColumnsWidth(DataGridViewElementStates.None) + 10,
                view.Rows.GetRowsHeight(DataGridViewElementStates.None) + 20);
            view.Refresh();
        }

        public void SetBallisticsColor(Item item)
        {
            for (var b = 0; b < iteminfo_ball.Rows.Count; b++)
            {
                for (var i = 0; i < iteminfo_ball.Rows[b].Cells.Count; i++)
                {
                    if (i == 0)
                    {
                        if (iteminfo_ball.Rows[b].Cells[i].Value.Equals(item.NameDisplay) || iteminfo_ball.Rows[b].Cells[i].Value.Equals(item.NameDisplay2))
                        {
                            iteminfo_ball.Rows[b].Cells[i].Style.ForeColor = Color.Gold;
                        }
                    }
                    else if (i >= 3)
                    {
                        try
                        {
                            int.TryParse((string)iteminfo_ball.Rows[b].Cells[i].Value, out var level);
                            iteminfo_ball.Rows[b].Cells[i].Style.BackColor = _beColor[level];
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception on SetBallisticsColor");
                        }
                    }
                }
            }
            iteminfo_ball.Refresh();
        }

        public void ShowInfo(Item item, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    lock (_lock)
                    {
                        if (item == null || item.MarketAddress == null)
                        {
                            iteminfo_text.Text = _resources["NotFound"];
                        }
                        else if (item.PriceLast == null)
                        {
                            iteminfo_text.Text = _resources["NoFlea"];
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            sb.Append(string.Format("Name : {0}\n\n", item.IsName2 ? item.NameDisplay2 : item.NameDisplay));
                            if (_appSettings.ShowLastPrice)
                            {
                                sb.Append(string.Format("Last Price : {0} ({1})\n", item.PriceLast, item.LastUpdate));
                            }
                            if (_appSettings.ShowDayPrice && item.PriceDay != null)
                            {
                                sb.Append(string.Format("Day Price : {0}\n", item.PriceDay));
                            }
                            if (_appSettings.ShowWeekPrice && item.PriceWeek != null)
                            {
                                sb.Append(string.Format("Week Price : {0}\n", item.PriceWeek));
                            }
                            if (_appSettings.SellToTrader && item.SellToTrader != null)
                            {
                                sb.Append(string.Format("\nSell to Trader : {0}", item.SellToTrader));
                                sb.Append(string.Format("\nSell to Trader Price : {0}\n", item.SellToTraderPrice));
                            }
                            if (_appSettings.BuyFromTrader && item.BuyFromTrader != null)
                            {
                                sb.Append(string.Format("\nBuy From Trader : {0}", item.BuyFromTrader));
                                sb.Append(string.Format("\nBuy From Trader Price : {0}\n", item.BuyFromTraderPrice.Replace(" ", "").Replace(@"~", @" ≈")));
                            }
                            if (_appSettings.Needs && item.Needs != null)
                            {
                                sb.Append(string.Format("\nNeeds :\n{0}\n", item.Needs));
                            }
                            if (_appSettings.BartersNCrafts && item.Bartersandcrafts != null)
                            {
                                sb.Append(string.Format("\nBarters & Crafts :\n{0}\n", item.Bartersandcrafts));
                            }
                            iteminfo_text.Text = sb.ToString().Trim();
                            SetTextColors(item);
                            if (item.Ballistic != null)
                            {
                                foreach (var b in item.Ballistic.Calibarlist)
                                {
                                    iteminfo_ball.Rows.Add(b.Data());
                                }
                                iteminfo_ball.Visible = true;
                                SetBallisticsColor(item);
                                ResizeGrid(iteminfo_ball);
                            }
                        }
                    }
                }
            };
            Invoke(show);
        }

        public void SetTextColors(Item item)
        {
            SetPriceColor();
            SetInraidColor();
            SetCraftColor(item);
        }

        public void SetPriceColor()
        {
            var mc = _moneyRegex.Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Gold;
            }
        }

        public void SetInraidColor()
        {
            var mc = _inRaidRegex.Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Red;
            }
        }

        public void SetCraftColor(Item item)
        {
            var mc = new Regex(item.NameDisplay).Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Green;
            }
        }

        public void ShowLoadingInfo(Point point, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    iteminfo_ball.Rows.Clear();
                    iteminfo_ball.Visible = false;
                    iteminfo_text.Text = _resources["Loading"];
                    iteminfo_panel.Location = point;
                    iteminfo_panel.Visible = true;
                }
            };
            Invoke(show);
        }

        public void ShowWaitBallistics(Point point)
        {
            Action show = delegate ()
            {
                lock (_lock)
                {
                    iteminfo_ball.Rows.Clear();
                    iteminfo_ball.Visible = false;
                    iteminfo_text.Text = _resources["NotFinishLoading"];
                    iteminfo_panel.Location = point;
                    iteminfo_panel.Visible = true;
                }
            };
            Invoke(show);
        }

        public void HideInfo()
        {
            Action show = delegate ()
            {
                iteminfo_ball.Visible = false;
                iteminfo_panel.Visible = false;
            };
            Invoke(show);
        }

        public void ChangeTransparent(int value)
        {
            Action show = delegate ()
            {
                Opacity = value * 0.01;
            };
            Invoke(show);
        }

        private void FixLocation(Control p)
        {
            var totalwidth = p.Location.X + p.Width;
            var totalheight = p.Location.Y + p.Height;
            var x = p.Location.X;
            var y = p.Location.Y;
            if (totalwidth > Width)
            {
                x -= totalwidth - Width;
            }
            if (totalheight > Height)
            {
                y -= totalheight - Height;
            }
            if (x != p.Location.X || y != p.Location.Y)
            {
                p.Location = new Point(x, y);
            }
            p.Refresh();
        }

        private void ItemWindowPanelPaint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void ItemWindowPanelSizeChanged(object sender, EventArgs e)
        {
            iteminfo_ball.Location = new Point(iteminfo_text.Location.X, iteminfo_text.Location.Y + iteminfo_text.Height + 15);
            FixLocation(sender as Control);
        }

        private void ItemWindowPanelLocationChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Control);
        }

        private void ItemWindowTextContentsResized(object sender, ContentsResizedEventArgs e)
        {
            (sender as Control).ClientSize = new Size(e.NewRectangle.Width + 1, e.NewRectangle.Height + 1);
        }

        private void ItemCompareTextMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMoving)
            {
                itemcompare_panel.Location = new Point(Cursor.Position.X - _x, Cursor.Position.Y - _y);
            }
        }

        private void ItemCompareTextMouseDown(object sender, MouseEventArgs e)
        {
            _x = Cursor.Position.X - itemcompare_panel.Location.X;
            _y = Cursor.Position.Y - itemcompare_panel.Location.Y;
            _isMoving = true;
        }

        private void ItemCompareTextMouseUp(object sender, MouseEventArgs e)
        {
            _isMoving = false;
        }
    }
}
