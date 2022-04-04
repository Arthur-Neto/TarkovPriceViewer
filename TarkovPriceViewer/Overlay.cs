using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using TarkovPriceViewer;

namespace TarkovPriceChecker
{
    public partial class Overlay : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private static readonly int GWL_EXSTYLE = -20;
        private static readonly int WS_EX_TOOLWINDOW = 0x00000080;
        private static readonly int WS_EX_LAYERED = 0x80000;
        private static readonly int WS_EX_TRANSPARENT = 0x20;
        private static int compare_size = 0;
        private static bool isinfoform = true;
        private static bool ismoving = false;
        private static int x, y;

        private readonly object _lock = new object();

        public Overlay(bool _isinfoform)
        {
            InitializeComponent();
            isinfoform = _isinfoform;
            TopMost = true;
            var style = GetWindowLong(Handle, GWL_EXSTYLE);
            if (isinfoform)
            {
                Opacity = int.Parse(Program.settings["Overlay_Transparent"]) * 0.01;
                SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT);
            }
            else
            {
                SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TOOLWINDOW);
            }
            settingFormPos();
            initializeCompareData();
            initializeBallistics();
            iteminfo_panel.Visible = false;
            itemcompare_panel.Visible = false;
        }

        public void settingFormPos()
        {
            Location = new Point(0, 0);
            Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        public void initializeCompareData()
        {
            ItemCompareGrid.ColumnCount = 7;
            ItemCompareGrid.Columns[0].Name = "Name";
            ItemCompareGrid.Columns[1].Name = "Recoil";
            ItemCompareGrid.Columns[2].Name = "Accuracy";
            ItemCompareGrid.Columns[3].Name = "Ergo";
            ItemCompareGrid.Columns[4].Name = "Flea";
            ItemCompareGrid.Columns[5].Name = "NPC";
            ItemCompareGrid.Columns[6].Name = "LL";
            ItemCompareGrid.Visible = false;
            ItemCompareGrid.ClearSelection();
            ItemCompareGrid.SortCompare += new DataGridViewSortCompareEventHandler(ItemCompareGrid_SortCompare);
            ResizeGrid(ItemCompareGrid);
        }

        public void initializeBallistics()
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

        private void ItemCompareGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0 || e.Column.Index == 5 || e.Column.Index == 6)
            {
                return;
            }

            int.TryParse(string.Join("", e.CellValue1.ToString().Replace(",", "").Split(Program.splitcur)), out var s1);
            int.TryParse(string.Join("", e.CellValue2.ToString().Replace(",", "").Split(Program.splitcur)), out var s2);
            e.SortResult = s1.CompareTo(s2);
            e.Handled = true;
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
                            iteminfo_ball.Rows[b].Cells[i].Style.BackColor = Program.BEColor[level];
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
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
                            iteminfo_text.Text = Program.notfound;
                        }
                        else if (item.PriceLast == null)
                        {
                            iteminfo_text.Text = Program.noflea;
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            sb.Append(string.Format("Name : {0}\n\n", item.IsName2 ? item.NameDisplay2 : item.NameDisplay));
                            if (Convert.ToBoolean(Program.settings["Show_Last_Price"]))
                            {
                                sb.Append(string.Format("Last Price : {0} ({1})\n", item.PriceLast, item.LastUpdate));
                            }
                            if (Convert.ToBoolean(Program.settings["Show_Day_Price"]) && item.PriceDay != null)
                            {
                                sb.Append(string.Format("Day Price : {0}\n", item.PriceDay));
                            }
                            if (Convert.ToBoolean(Program.settings["Show_Week_Price"]) && item.PriceWeek != null)
                            {
                                sb.Append(string.Format("Week Price : {0}\n", item.PriceWeek));
                            }
                            if (Convert.ToBoolean(Program.settings["Sell_to_Trader"]) && item.SellToTrader != null)
                            {
                                sb.Append(string.Format("\nSell to Trader : {0}", item.SellToTrader));
                                sb.Append(string.Format("\nSell to Trader Price : {0}\n", item.SellToTraderPrice));
                            }
                            if (Convert.ToBoolean(Program.settings["Buy_From_Trader"]) && item.BuyFromTrader != null)
                            {
                                sb.Append(string.Format("\nBuy From Trader : {0}", item.BuyFromTrader));
                                sb.Append(string.Format("\nBuy From Trader Price : {0}\n", item.BuyFromTraderPrice.Replace(" ", "").Replace(@"~", @" ≈")));
                            }
                            if (Convert.ToBoolean(Program.settings["Needs"]) && item.Needs != null)
                            {
                                sb.Append(string.Format("\nNeeds :\n{0}\n", item.Needs));
                            }
                            if (Convert.ToBoolean(Program.settings["Barters_and_Crafts"]) && item.Bartersandcrafts != null)
                            {
                                sb.Append(string.Format("\nBarters & Crafts :\n{0}\n", item.Bartersandcrafts));
                            }
                            iteminfo_text.Text = sb.ToString().Trim();
                            setTextColors(item);
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

        public void ShowCompare(Item item, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    lock (_lock)
                    {
                        var temp = CheckItemExist(item);
                        if (item != null && item.MarketAddress != null)
                        {
                            if (temp != null)
                            {
                                ItemCompareGrid.Rows.Remove(temp);
                            }
                            ItemCompareGrid.Rows.Add(item.Data());
                            if (ItemCompareGrid.SortedColumn != null)
                            {
                                ItemCompareGrid.Sort(ItemCompareGrid.SortedColumn,
                                    ItemCompareGrid.SortOrder.Equals(SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);
                            }
                            ItemCompareGrid.Visible = true;
                            ResizeGrid(ItemCompareGrid);
                        }
                        if (temp == null)
                        {
                            if (--compare_size > 0)
                            {
                                itemcompare_text.Text = string.Format("{0} Left : {1}", Program.loading, compare_size);
                            }
                            else
                            {
                                itemcompare_text.Text = string.Format("{0}", Program.presscomparekey);
                            }
                        }
                    }
                }
            };
            Invoke(show);
        }

        public DataGridViewRow CheckItemExist(Item item)
        {
            DataGridViewRow value = null;
            foreach (DataGridViewRow r in ItemCompareGrid.Rows)
            {
                if ((item.IsName2 ? item.NameDisplay2 : item.NameDisplay).Equals(r.Cells[0].Value))
                {
                    value = r;
                    break;
                }
            }
            return value;
        }

        public void setTextColors(Item item)
        {
            setPriceColor();
            setInraidColor();
            setCraftColor(item);
        }

        public void setPriceColor()
        {
            var mc = Program.money_filter.Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Gold;
            }
        }

        public void setInraidColor()
        {
            var mc = Program.inraid_filter.Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Red;
            }
        }

        public void setCraftColor(Item item)
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
                    iteminfo_text.Text = Program.loading;
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
                    iteminfo_text.Text = Program.notfinishloading;
                    iteminfo_panel.Location = point;
                    iteminfo_panel.Visible = true;
                }
            };
            Invoke(show);
        }

        public void ShowLoadingCompare(Point point, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    lock (_lock)
                    {
                        if (!itemcompare_panel.Visible)
                        {
                            compare_size = 0;
                            ItemCompareGrid.Rows.Clear();
                            ResizeGrid(ItemCompareGrid);
                            itemcompare_panel.Location = point;
                            itemcompare_panel.Visible = true;
                            itemcompare_text.Text = string.Format("{0}", Program.presscomparekey);
                        }
                        itemcompare_text.Text = string.Format("{0} Left : {1}", Program.loading, ++compare_size);
                    }
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

        public void HideCompare()
        {
            Action show = delegate ()
            {
                ItemCompareGrid.Visible = false;
                itemcompare_panel.Visible = false;
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

        private void itemwindow_panel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void itemwindow_panel_SizeChanged(object sender, EventArgs e)
        {
            iteminfo_ball.Location = new Point(iteminfo_text.Location.X, iteminfo_text.Location.Y + iteminfo_text.Height + 15);
            FixLocation(sender as Control);
        }

        private void itemwindow_panel_LocationChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Control);
        }

        private void Overlay_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void itemwindow_text_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            (sender as Control).ClientSize = new Size(e.NewRectangle.Width + 1, e.NewRectangle.Height + 1);
        }

        private void itemcompare_text_MouseMove(object sender, MouseEventArgs e)
        {
            if (ismoving)
            {
                itemcompare_panel.Location = new Point(Cursor.Position.X - x, Cursor.Position.Y - y);
            }
        }

        private void itemcompare_text_MouseDown(object sender, MouseEventArgs e)
        {
            x = Cursor.Position.X - itemcompare_panel.Location.X;
            y = Cursor.Position.Y - itemcompare_panel.Location.Y;
            ismoving = true;
        }

        private void itemcompare_text_MouseUp(object sender, MouseEventArgs e)
        {
            ismoving = false;
        }
    }
}
