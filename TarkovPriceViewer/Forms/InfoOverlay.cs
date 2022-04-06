using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using TarkovPriceViewer.Infrastructure.Entities;
using TarkovPriceViewer.Infrastructure.Settings;
using TarkovPriceViewer.Resources;

namespace TarkovPriceViewer.Forms
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

        private readonly ILogger<InfoOverlay> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly object _lock = new object();

        public InfoOverlay(
            ILogger<InfoOverlay> logger,
            IServiceProvider serviceProvider
        )
        {
            InitializeComponent();

            _logger = logger;
            _serviceProvider = serviceProvider;

            TopMost = true;

            using var scope = _serviceProvider.CreateScope();
            var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SettingsOptions>>().Value;

            var style = GetWindowLong(Handle, GWL_EXSTYLE);
            Opacity = int.Parse(options.OverlayTransparency) * 0.01;
            SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT);

            SettingFormPos();
            InitializeBallistics();

            itemInfoPanel.Visible = false;
        }

        public void SettingFormPos()
        {
            Location = new Point(0, 0);
            Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        public void InitializeBallistics()
        {
            itemInfoBall.ColumnCount = 9;
            itemInfoBall.Columns[0].Name = "Type";
            itemInfoBall.Columns[1].Name = "Name";
            itemInfoBall.Columns[2].Name = "Damage";
            itemInfoBall.Columns[3].Name = "1";
            itemInfoBall.Columns[4].Name = "2";
            itemInfoBall.Columns[5].Name = "3";
            itemInfoBall.Columns[6].Name = "4";
            itemInfoBall.Columns[7].Name = "5";
            itemInfoBall.Columns[8].Name = "6";
            itemInfoBall.Visible = false;
            itemInfoBall.ClearSelection();
            ResizeGrid(itemInfoBall);
        }

        public void ResizeGrid(DataGridView view)
        {
            view.ClientSize = new Size(view.Columns.GetColumnsWidth(DataGridViewElementStates.None) + 10,
                view.Rows.GetRowsHeight(DataGridViewElementStates.None) + 20);
            view.Refresh();
        }

        public void SetBallisticsColor(Item item)
        {
            for (var b = 0; b < itemInfoBall.Rows.Count; b++)
            {
                for (var i = 0; i < itemInfoBall.Rows[b].Cells.Count; i++)
                {
                    if (i == 0)
                    {
                        if (itemInfoBall.Rows[b].Cells[i].Value.Equals(item.NameDisplay) || itemInfoBall.Rows[b].Cells[i].Value.Equals(item.NameDisplay2))
                        {
                            itemInfoBall.Rows[b].Cells[i].Style.ForeColor = Color.Gold;
                        }
                    }
                    else if (i >= 3)
                    {
                        try
                        {
                            int.TryParse((string)itemInfoBall.Rows[b].Cells[i].Value, out var level);
                            itemInfoBall.Rows[b].Cells[i].Style.BackColor = _beColor[level];
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception on SetBallisticsColor");
                        }
                    }
                }
            }
            itemInfoBall.Refresh();
        }

        public void ShowInfo(Item item, CancellationToken cancellationToken)
        {
            Action show = delegate ()
            {
                if (cancellationToken.IsCancellationRequested is false)
                {
                    lock (_lock)
                    {
                        if (item == null || item.MarketAddress == null)
                        {
                            itemInfoText.Text = Resource.NotFound;
                        }
                        else if (item.PriceLast == null)
                        {
                            itemInfoText.Text = Resource.NoFlea;
                        }
                        else
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SettingsOptions>>().Value;

                            var sb = new StringBuilder();
                            sb.Append(string.Format("Name : {0}\n\n", item.IsName2 ? item.NameDisplay2 : item.NameDisplay));
                            if (options.ShowLastPrice)
                            {
                                sb.Append(string.Format("Last Price : {0} ({1})\n", item.PriceLast, item.LastUpdate));
                            }
                            if (options.ShowDayPrice && item.PriceDay != null)
                            {
                                sb.Append(string.Format("Day Price : {0}\n", item.PriceDay));
                            }
                            if (options.ShowWeekPrice && item.PriceWeek != null)
                            {
                                sb.Append(string.Format("Week Price : {0}\n", item.PriceWeek));
                            }
                            if (options.SellToTrader && item.SellToTrader != null)
                            {
                                sb.Append(string.Format("\nSell to Trader : {0}", item.SellToTrader));
                                sb.Append(string.Format("\nSell to Trader Price : {0}\n", item.SellToTraderPrice));
                            }
                            if (options.BuyFromTrader && item.BuyFromTrader != null)
                            {
                                sb.Append(string.Format("\nBuy From Trader : {0}", item.BuyFromTrader));
                                sb.Append(string.Format("\nBuy From Trader Price : {0}\n", item.BuyFromTraderPrice.Replace(" ", "").Replace(@"~", @" ≈")));
                            }
                            if (options.Needs && item.Needs != null)
                            {
                                sb.Append(string.Format("\nNeeds :\n{0}\n", item.Needs));
                            }
                            if (options.BartersNCrafts && item.BartersNCrafts != null)
                            {
                                sb.Append(string.Format("\nBarters & Crafts :\n{0}\n", item.BartersNCrafts));
                            }
                            itemInfoText.Text = sb.ToString().Trim();

                            SetTextColors(item);

                            if (item.Ballistic != null)
                            {
                                foreach (var ballistic in item.Ballistic.BallisticList)
                                {
                                    itemInfoBall.Rows.Add(ballistic.Data());
                                }
                                itemInfoBall.Visible = true;
                                SetBallisticsColor(item);
                                ResizeGrid(itemInfoBall);
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
            var mc = _moneyRegex.Matches(itemInfoText.Text);
            foreach (Match m in mc)
            {
                itemInfoText.Select(m.Index, m.Length);
                itemInfoText.SelectionColor = Color.Gold;
            }
        }

        public void SetInraidColor()
        {
            var mc = _inRaidRegex.Matches(itemInfoText.Text);
            foreach (Match m in mc)
            {
                itemInfoText.Select(m.Index, m.Length);
                itemInfoText.SelectionColor = Color.Red;
            }
        }

        public void SetCraftColor(Item item)
        {
            var mc = new Regex(item.NameDisplay).Matches(itemInfoText.Text);
            foreach (Match m in mc)
            {
                itemInfoText.Select(m.Index, m.Length);
                itemInfoText.SelectionColor = Color.Green;
            }
        }

        public void ShowLoadingInfo(Point point, CancellationToken cancellationToken)
        {
            Action show = delegate ()
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    itemInfoBall.Rows.Clear();
                    itemInfoBall.Visible = false;
                    itemInfoText.Text = Resource.Loading;
                    itemInfoPanel.Location = point;
                    itemInfoPanel.Visible = true;
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
                    itemInfoBall.Rows.Clear();
                    itemInfoBall.Visible = false;
                    itemInfoText.Text = Resource.NotFinishLoading;
                    itemInfoPanel.Location = point;
                    itemInfoPanel.Visible = true;
                }
            };
            Invoke(show);
        }

        public void HideInfo()
        {
            Action show = delegate ()
            {
                itemInfoBall.Visible = false;
                itemInfoPanel.Visible = false;
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
            itemInfoBall.Location = new Point(itemInfoText.Location.X, itemInfoText.Location.Y + itemInfoText.Height + 15);
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
    }
}
