using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.InteropServices;
using TarkovPriceViewer;
using TarkovPriceViewer.Infrastructure.Constants;
using TarkovPriceViewer.Properties;

namespace TarkovPriceChecker
{
    public partial class CompareOverlay : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_LAYERED = 0x80000;

        private static int _compareSize = 0;
        private static bool _isMoving = false;
        private static int _x, _y;

        private readonly IStringLocalizer<Resources> _resources;
        private readonly ILogger<CompareOverlay> _logger;

        private readonly object _lock = new object();

        public CompareOverlay(
            IStringLocalizer<Resources> resources,
            ILogger<CompareOverlay> logger
        )
        {
            InitializeComponent();

            _resources = resources;
            _logger = logger;

            TopMost = true;

            var style = GetWindowLong(Handle, GWL_EXSTYLE);
            SetWindowLong(Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TOOLWINDOW);

            SettingFormPos();
            InitializeCompareData();
            InitializeBallistics();

            iteminfo_panel.Visible = false;
            itemcompare_panel.Visible = false;
        }

        public void SettingFormPos()
        {
            Location = new Point(0, 0);
            Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        public void InitializeCompareData()
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
            ItemCompareGrid.SortCompare += new DataGridViewSortCompareEventHandler(ItemCompareGridSortCompare);
            ResizeGrid(ItemCompareGrid);
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

        private void ItemCompareGridSortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index is 0 or 5 or 6)
            {
                return;
            }

            int.TryParse(string.Join("", e.CellValue1.ToString().Replace(",", "").Split(Currencies.AllCurrencies)), out var s1);
            int.TryParse(string.Join("", e.CellValue2.ToString().Replace(",", "").Split(Currencies.AllCurrencies)), out var s2);
            e.SortResult = s1.CompareTo(s2);
            e.Handled = true;
        }

        public void ResizeGrid(DataGridView view)
        {
            view.ClientSize = new Size(view.Columns.GetColumnsWidth(DataGridViewElementStates.None) + 10,
                view.Rows.GetRowsHeight(DataGridViewElementStates.None) + 20);
            view.Refresh();
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
                            if (--_compareSize > 0)
                            {
                                itemcompare_text.Text = string.Format("{0} Left : {1}", _resources["Loading"], _compareSize);
                            }
                            else
                            {
                                itemcompare_text.Text = string.Format("{0}", _resources["PressCompareKey"]);
                            }
                        }
                    }
                }
            };
            Invoke(show);
        }

        public DataGridViewRow CheckItemExist(Item item)
        {
            DataGridViewRow? value = null;
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
                            _compareSize = 0;
                            ItemCompareGrid.Rows.Clear();
                            ResizeGrid(ItemCompareGrid);
                            itemcompare_panel.Location = point;
                            itemcompare_panel.Visible = true;
                            itemcompare_text.Text = string.Format("{0}", _resources["PressCompareKey"]);
                        }

                        itemcompare_text.Text = string.Format("{0} Left : {1}", _resources["Loading"], ++_compareSize);
                    }
                }
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
