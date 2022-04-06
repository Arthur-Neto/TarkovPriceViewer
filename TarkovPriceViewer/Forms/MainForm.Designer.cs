namespace TarkovPriceViewer.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayshow = new System.Windows.Forms.ToolStripMenuItem();
            this.trayexit = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.WeekPriceChkBox = new System.Windows.Forms.CheckBox();
            this.DayPriceChkBox = new System.Windows.Forms.CheckBox();
            this.BuyFromTraderChkBox = new System.Windows.Forms.CheckBox();
            this.NeddsChkBox = new System.Windows.Forms.CheckBox();
            this.BartersNCraftsChkBox = new System.Windows.Forms.CheckBox();
            this.SellToTraderChkBox = new System.Windows.Forms.CheckBox();
            this.LastPriceChkBox = new System.Windows.Forms.CheckBox();
            this.ShowOverlayButton = new System.Windows.Forms.Button();
            this.ShowOverlay_Desc = new System.Windows.Forms.Label();
            this.TransparentText = new System.Windows.Forms.Label();
            this.CloseOverlayWhenMouseMoved = new System.Windows.Forms.CheckBox();
            this.TransparentBar = new System.Windows.Forms.TrackBar();
            this.HideOverlay_Desc2 = new System.Windows.Forms.Label();
            this.TransParent_Desc = new System.Windows.Forms.Label();
            this.HideOverlayButton = new System.Windows.Forms.Button();
            this.HideOverlay_Desc = new System.Windows.Forms.Label();
            this.CheckUpdate = new System.Windows.Forms.Button();
            this.Github = new System.Windows.Forms.Button();
            this.TarkovMarket = new System.Windows.Forms.Button();
            this.TarkovWiki = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.DataProvidedBy = new System.Windows.Forms.Label();
            this.Tarkov_Official = new System.Windows.Forms.Button();
            this.MinimizetoTrayWhenStartup = new System.Windows.Forms.CheckBox();
            this.Version = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.Exit_Button = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.CompareOverlay_Desc = new System.Windows.Forms.Label();
            this.CompareOverlayButton = new System.Windows.Forms.Button();
            this.CompareOverlay_Desc2 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.RandomItem = new System.Windows.Forms.CheckBox();
            this.ForFunRandom_Desc = new System.Windows.Forms.Label();
            this.CheckIdleTime = new System.Windows.Forms.Timer(this.components);
            this.refresh_b = new System.Windows.Forms.Button();
            this.TrayMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TransparentBar)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = this.TrayMenu;
            this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
            this.TrayIcon.Text = "TarkovPriceViewer";
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIconMouseDoubleClick);
            // 
            // TrayMenu
            // 
            this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayshow,
            this.trayexit});
            this.TrayMenu.Name = "TrayMenu";
            this.TrayMenu.Size = new System.Drawing.Size(104, 48);
            // 
            // trayshow
            // 
            this.trayshow.Name = "trayshow";
            this.trayshow.Size = new System.Drawing.Size(103, 22);
            this.trayshow.Text = "Show";
            this.trayshow.Click += new System.EventHandler(this.TrayShowClick);
            // 
            // trayexit
            // 
            this.trayexit.Name = "trayexit";
            this.trayexit.Size = new System.Drawing.Size(103, 22);
            this.trayexit.Text = "Exit";
            this.trayexit.Click += new System.EventHandler(this.TrayExitClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.WeekPriceChkBox);
            this.panel1.Controls.Add(this.DayPriceChkBox);
            this.panel1.Controls.Add(this.BuyFromTraderChkBox);
            this.panel1.Controls.Add(this.NeddsChkBox);
            this.panel1.Controls.Add(this.BartersNCraftsChkBox);
            this.panel1.Controls.Add(this.SellToTraderChkBox);
            this.panel1.Controls.Add(this.LastPriceChkBox);
            this.panel1.Controls.Add(this.ShowOverlayButton);
            this.panel1.Controls.Add(this.ShowOverlay_Desc);
            this.panel1.Location = new System.Drawing.Point(12, 131);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 131);
            this.panel1.TabIndex = 1;
            // 
            // WeekPriceChkBox
            // 
            this.WeekPriceChkBox.AutoSize = true;
            this.WeekPriceChkBox.ForeColor = System.Drawing.SystemColors.Control;
            this.WeekPriceChkBox.Location = new System.Drawing.Point(197, 46);
            this.WeekPriceChkBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.WeekPriceChkBox.Name = "WeekPriceChkBox";
            this.WeekPriceChkBox.Size = new System.Drawing.Size(82, 19);
            this.WeekPriceChkBox.TabIndex = 7;
            this.WeekPriceChkBox.TabStop = false;
            this.WeekPriceChkBox.Text = "week price";
            this.WeekPriceChkBox.UseVisualStyleBackColor = true;
            this.WeekPriceChkBox.CheckedChanged += new System.EventHandler(this.WeekPriceBoxCheckedChanged);
            // 
            // DayPriceChkBox
            // 
            this.DayPriceChkBox.AutoSize = true;
            this.DayPriceChkBox.ForeColor = System.Drawing.SystemColors.Control;
            this.DayPriceChkBox.Location = new System.Drawing.Point(114, 46);
            this.DayPriceChkBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DayPriceChkBox.Name = "DayPriceChkBox";
            this.DayPriceChkBox.Size = new System.Drawing.Size(74, 19);
            this.DayPriceChkBox.TabIndex = 7;
            this.DayPriceChkBox.TabStop = false;
            this.DayPriceChkBox.Text = "day price";
            this.DayPriceChkBox.UseVisualStyleBackColor = true;
            this.DayPriceChkBox.CheckedChanged += new System.EventHandler(this.DayPriceBoxCheckedChanged);
            // 
            // BuyFromTraderChkBox
            // 
            this.BuyFromTraderChkBox.AutoSize = true;
            this.BuyFromTraderChkBox.ForeColor = System.Drawing.SystemColors.Control;
            this.BuyFromTraderChkBox.Location = new System.Drawing.Point(132, 74);
            this.BuyFromTraderChkBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BuyFromTraderChkBox.Name = "BuyFromTraderChkBox";
            this.BuyFromTraderChkBox.Size = new System.Drawing.Size(109, 19);
            this.BuyFromTraderChkBox.TabIndex = 7;
            this.BuyFromTraderChkBox.TabStop = false;
            this.BuyFromTraderChkBox.Text = "buy from trader";
            this.BuyFromTraderChkBox.UseVisualStyleBackColor = true;
            this.BuyFromTraderChkBox.CheckedChanged += new System.EventHandler(this.BuyFromTraderBoxCheckedChanged);
            // 
            // NeddsChkBox
            // 
            this.NeddsChkBox.AutoSize = true;
            this.NeddsChkBox.ForeColor = System.Drawing.SystemColors.Control;
            this.NeddsChkBox.Location = new System.Drawing.Point(32, 100);
            this.NeddsChkBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.NeddsChkBox.Name = "NeddsChkBox";
            this.NeddsChkBox.Size = new System.Drawing.Size(57, 19);
            this.NeddsChkBox.TabIndex = 7;
            this.NeddsChkBox.TabStop = false;
            this.NeddsChkBox.Text = "needs";
            this.NeddsChkBox.UseVisualStyleBackColor = true;
            this.NeddsChkBox.CheckedChanged += new System.EventHandler(this.NeedsBoxCheckedChanged);
            // 
            // BartersNCraftsChkBox
            // 
            this.BartersNCraftsChkBox.AutoSize = true;
            this.BartersNCraftsChkBox.ForeColor = System.Drawing.SystemColors.Control;
            this.BartersNCraftsChkBox.Location = new System.Drawing.Point(97, 100);
            this.BartersNCraftsChkBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BartersNCraftsChkBox.Name = "BartersNCraftsChkBox";
            this.BartersNCraftsChkBox.Size = new System.Drawing.Size(117, 19);
            this.BartersNCraftsChkBox.TabIndex = 7;
            this.BartersNCraftsChkBox.TabStop = false;
            this.BartersNCraftsChkBox.Text = "barters and crafts";
            this.BartersNCraftsChkBox.UseVisualStyleBackColor = true;
            this.BartersNCraftsChkBox.CheckedChanged += new System.EventHandler(this.BartersAndCraftsBoxCheckedChanged);
            // 
            // SellToTraderChkBox
            // 
            this.SellToTraderChkBox.AutoSize = true;
            this.SellToTraderChkBox.ForeColor = System.Drawing.SystemColors.Control;
            this.SellToTraderChkBox.Location = new System.Drawing.Point(32, 72);
            this.SellToTraderChkBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.SellToTraderChkBox.Name = "SellToTraderChkBox";
            this.SellToTraderChkBox.Size = new System.Drawing.Size(91, 19);
            this.SellToTraderChkBox.TabIndex = 7;
            this.SellToTraderChkBox.TabStop = false;
            this.SellToTraderChkBox.Text = "sell to trader";
            this.SellToTraderChkBox.UseVisualStyleBackColor = true;
            this.SellToTraderChkBox.CheckedChanged += new System.EventHandler(this.SellToTraderBoxCheckedChanged);
            // 
            // LastPriceChkBox
            // 
            this.LastPriceChkBox.AutoSize = true;
            this.LastPriceChkBox.ForeColor = System.Drawing.SystemColors.Control;
            this.LastPriceChkBox.Location = new System.Drawing.Point(32, 45);
            this.LastPriceChkBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LastPriceChkBox.Name = "LastPriceChkBox";
            this.LastPriceChkBox.Size = new System.Drawing.Size(73, 19);
            this.LastPriceChkBox.TabIndex = 7;
            this.LastPriceChkBox.TabStop = false;
            this.LastPriceChkBox.Text = "last price";
            this.LastPriceChkBox.UseVisualStyleBackColor = true;
            this.LastPriceChkBox.CheckedChanged += new System.EventHandler(this.LastPriceBoxCheckedChanged);
            // 
            // ShowOverlayButton
            // 
            this.ShowOverlayButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ShowOverlayButton.Location = new System.Drawing.Point(188, 10);
            this.ShowOverlayButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ShowOverlayButton.Name = "ShowOverlayButton";
            this.ShowOverlayButton.Size = new System.Drawing.Size(59, 29);
            this.ShowOverlayButton.TabIndex = 1;
            this.ShowOverlayButton.TabStop = false;
            this.ShowOverlayButton.Text = "F9";
            this.ShowOverlayButton.UseVisualStyleBackColor = false;
            this.ShowOverlayButton.Click += new System.EventHandler(this.OverlayButtonClick);
            // 
            // ShowOverlay_Desc
            // 
            this.ShowOverlay_Desc.AutoSize = true;
            this.ShowOverlay_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ShowOverlay_Desc.ForeColor = System.Drawing.SystemColors.Control;
            this.ShowOverlay_Desc.Location = new System.Drawing.Point(21, 16);
            this.ShowOverlay_Desc.Name = "ShowOverlay_Desc";
            this.ShowOverlay_Desc.Size = new System.Drawing.Size(120, 15);
            this.ShowOverlay_Desc.TabIndex = 0;
            this.ShowOverlay_Desc.Text = "Show Overlay Key";
            // 
            // TransparentText
            // 
            this.TransparentText.AutoSize = true;
            this.TransparentText.ForeColor = System.Drawing.SystemColors.Control;
            this.TransparentText.Location = new System.Drawing.Point(250, 34);
            this.TransparentText.Name = "TransparentText";
            this.TransparentText.Size = new System.Drawing.Size(29, 15);
            this.TransparentText.TabIndex = 4;
            this.TransparentText.Text = "80%";
            // 
            // CloseOverlayWhenMouseMoved
            // 
            this.CloseOverlayWhenMouseMoved.AutoSize = true;
            this.CloseOverlayWhenMouseMoved.ForeColor = System.Drawing.SystemColors.Control;
            this.CloseOverlayWhenMouseMoved.Location = new System.Drawing.Point(23, 64);
            this.CloseOverlayWhenMouseMoved.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CloseOverlayWhenMouseMoved.Name = "CloseOverlayWhenMouseMoved";
            this.CloseOverlayWhenMouseMoved.Size = new System.Drawing.Size(211, 19);
            this.CloseOverlayWhenMouseMoved.TabIndex = 6;
            this.CloseOverlayWhenMouseMoved.TabStop = false;
            this.CloseOverlayWhenMouseMoved.Text = "Close Overlay When Mouse Moved";
            this.CloseOverlayWhenMouseMoved.UseVisualStyleBackColor = true;
            this.CloseOverlayWhenMouseMoved.CheckedChanged += new System.EventHandler(this.CloseOverlayWhenMouseMovedCheckedChanged);
            // 
            // TransparentBar
            // 
            this.TransparentBar.Location = new System.Drawing.Point(9, 34);
            this.TransparentBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TransparentBar.Maximum = 100;
            this.TransparentBar.Name = "TransparentBar";
            this.TransparentBar.Size = new System.Drawing.Size(235, 45);
            this.TransparentBar.TabIndex = 3;
            this.TransparentBar.TabStop = false;
            this.TransparentBar.Value = 80;
            this.TransparentBar.Scroll += new System.EventHandler(this.TransParentBarScroll);
            // 
            // HideOverlay_Desc2
            // 
            this.HideOverlay_Desc2.AutoSize = true;
            this.HideOverlay_Desc2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HideOverlay_Desc2.ForeColor = System.Drawing.Color.Red;
            this.HideOverlay_Desc2.Location = new System.Drawing.Point(34, 45);
            this.HideOverlay_Desc2.Name = "HideOverlay_Desc2";
            this.HideOverlay_Desc2.Size = new System.Drawing.Size(212, 15);
            this.HideOverlay_Desc2.TabIndex = 0;
            this.HideOverlay_Desc2.Text = "※ Tab, Esc Keys are fixed to use";
            // 
            // TransParent_Desc
            // 
            this.TransParent_Desc.AutoSize = true;
            this.TransParent_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.TransParent_Desc.ForeColor = System.Drawing.SystemColors.Control;
            this.TransParent_Desc.Location = new System.Drawing.Point(21, 15);
            this.TransParent_Desc.Name = "TransParent_Desc";
            this.TransParent_Desc.Size = new System.Drawing.Size(104, 15);
            this.TransParent_Desc.TabIndex = 0;
            this.TransParent_Desc.Text = "Control Opacity";
            // 
            // HideOverlayButton
            // 
            this.HideOverlayButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.HideOverlayButton.Location = new System.Drawing.Point(184, 10);
            this.HideOverlayButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.HideOverlayButton.Name = "HideOverlayButton";
            this.HideOverlayButton.Size = new System.Drawing.Size(59, 29);
            this.HideOverlayButton.TabIndex = 1;
            this.HideOverlayButton.TabStop = false;
            this.HideOverlayButton.Text = "F10";
            this.HideOverlayButton.UseVisualStyleBackColor = false;
            this.HideOverlayButton.Click += new System.EventHandler(this.OverlayButtonClick);
            // 
            // HideOverlay_Desc
            // 
            this.HideOverlay_Desc.AutoSize = true;
            this.HideOverlay_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.HideOverlay_Desc.ForeColor = System.Drawing.SystemColors.Control;
            this.HideOverlay_Desc.Location = new System.Drawing.Point(17, 16);
            this.HideOverlay_Desc.Name = "HideOverlay_Desc";
            this.HideOverlay_Desc.Size = new System.Drawing.Size(115, 15);
            this.HideOverlay_Desc.TabIndex = 0;
            this.HideOverlay_Desc.Text = "Hide Overlay Key";
            // 
            // CheckUpdate
            // 
            this.CheckUpdate.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CheckUpdate.Location = new System.Drawing.Point(3, 4);
            this.CheckUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CheckUpdate.Name = "CheckUpdate";
            this.CheckUpdate.Size = new System.Drawing.Size(115, 29);
            this.CheckUpdate.TabIndex = 5;
            this.CheckUpdate.TabStop = false;
            this.CheckUpdate.Text = "Check Update";
            this.CheckUpdate.UseVisualStyleBackColor = false;
            this.CheckUpdate.Click += new System.EventHandler(this.CheckUpdateClick);
            // 
            // Github
            // 
            this.Github.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Github.Location = new System.Drawing.Point(487, 5);
            this.Github.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Github.Name = "Github";
            this.Github.Size = new System.Drawing.Size(110, 29);
            this.Github.TabIndex = 5;
            this.Github.TabStop = false;
            this.Github.Text = "Github";
            this.Github.UseVisualStyleBackColor = false;
            this.Github.Click += new System.EventHandler(this.GithubClick);
            // 
            // TarkovMarket
            // 
            this.TarkovMarket.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TarkovMarket.Location = new System.Drawing.Point(255, 5);
            this.TarkovMarket.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TarkovMarket.Name = "TarkovMarket";
            this.TarkovMarket.Size = new System.Drawing.Size(110, 29);
            this.TarkovMarket.TabIndex = 5;
            this.TarkovMarket.TabStop = false;
            this.TarkovMarket.Text = "Tarkov Market";
            this.TarkovMarket.UseVisualStyleBackColor = false;
            this.TarkovMarket.Click += new System.EventHandler(this.TarkovMarketClick);
            // 
            // TarkovWiki
            // 
            this.TarkovWiki.BackColor = System.Drawing.Color.WhiteSmoke;
            this.TarkovWiki.Location = new System.Drawing.Point(371, 5);
            this.TarkovWiki.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TarkovWiki.Name = "TarkovWiki";
            this.TarkovWiki.Size = new System.Drawing.Size(110, 29);
            this.TarkovWiki.TabIndex = 5;
            this.TarkovWiki.TabStop = false;
            this.TarkovWiki.Text = "Tarkov Wiki";
            this.TarkovWiki.UseVisualStyleBackColor = false;
            this.TarkovWiki.Click += new System.EventHandler(this.TarkovWikiClick);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.Github);
            this.panel4.Controls.Add(this.DataProvidedBy);
            this.panel4.Controls.Add(this.TarkovWiki);
            this.panel4.Controls.Add(this.Tarkov_Official);
            this.panel4.Controls.Add(this.TarkovMarket);
            this.panel4.Location = new System.Drawing.Point(12, 85);
            this.panel4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(600, 39);
            this.panel4.TabIndex = 7;
            // 
            // DataProvidedBy
            // 
            this.DataProvidedBy.AutoSize = true;
            this.DataProvidedBy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DataProvidedBy.ForeColor = System.Drawing.SystemColors.Control;
            this.DataProvidedBy.Location = new System.Drawing.Point(11, 11);
            this.DataProvidedBy.Name = "DataProvidedBy";
            this.DataProvidedBy.Size = new System.Drawing.Size(116, 15);
            this.DataProvidedBy.TabIndex = 6;
            this.DataProvidedBy.Text = "Data Provided By";
            // 
            // Tarkov_Official
            // 
            this.Tarkov_Official.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Tarkov_Official.Location = new System.Drawing.Point(139, 5);
            this.Tarkov_Official.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Tarkov_Official.Name = "Tarkov_Official";
            this.Tarkov_Official.Size = new System.Drawing.Size(110, 29);
            this.Tarkov_Official.TabIndex = 5;
            this.Tarkov_Official.TabStop = false;
            this.Tarkov_Official.Text = "Tarkov Official";
            this.Tarkov_Official.UseVisualStyleBackColor = false;
            this.Tarkov_Official.Click += new System.EventHandler(this.TarkovOfficialClick);
            // 
            // MinimizetoTrayWhenStartup
            // 
            this.MinimizetoTrayWhenStartup.AutoSize = true;
            this.MinimizetoTrayWhenStartup.ForeColor = System.Drawing.SystemColors.Control;
            this.MinimizetoTrayWhenStartup.Location = new System.Drawing.Point(343, 432);
            this.MinimizetoTrayWhenStartup.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimizetoTrayWhenStartup.Name = "MinimizetoTrayWhenStartup";
            this.MinimizetoTrayWhenStartup.Size = new System.Drawing.Size(188, 19);
            this.MinimizetoTrayWhenStartup.TabIndex = 6;
            this.MinimizetoTrayWhenStartup.TabStop = false;
            this.MinimizetoTrayWhenStartup.Text = "Minimize to Tray When Startup";
            this.MinimizetoTrayWhenStartup.UseVisualStyleBackColor = true;
            this.MinimizetoTrayWhenStartup.CheckedChanged += new System.EventHandler(this.MinimizetoTrayWhenStartupCheckedChanged);
            // 
            // Version
            // 
            this.Version.AutoSize = true;
            this.Version.ForeColor = System.Drawing.SystemColors.Control;
            this.Version.Location = new System.Drawing.Point(123, 10);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(45, 15);
            this.Version.TabIndex = 9;
            this.Version.Text = "Version";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::TarkovPriceViewer.Resources.Resource.title;
            this.pictureBox1.Location = new System.Drawing.Point(140, 15);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(350, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.CloseOverlayWhenMouseMoved);
            this.panel2.Controls.Add(this.HideOverlay_Desc);
            this.panel2.Controls.Add(this.HideOverlayButton);
            this.panel2.Controls.Add(this.HideOverlay_Desc2);
            this.panel2.Location = new System.Drawing.Point(316, 131);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(296, 94);
            this.panel2.TabIndex = 11;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.TransparentText);
            this.panel3.Controls.Add(this.TransParent_Desc);
            this.panel3.Controls.Add(this.TransparentBar);
            this.panel3.Location = new System.Drawing.Point(12, 341);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(296, 64);
            this.panel3.TabIndex = 12;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.Version);
            this.panel5.Controls.Add(this.CheckUpdate);
            this.panel5.Location = new System.Drawing.Point(12, 420);
            this.panel5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(184, 35);
            this.panel5.TabIndex = 13;
            // 
            // Exit_Button
            // 
            this.Exit_Button.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Exit_Button.Location = new System.Drawing.Point(537, 426);
            this.Exit_Button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Exit_Button.Name = "Exit_Button";
            this.Exit_Button.Size = new System.Drawing.Size(74, 29);
            this.Exit_Button.TabIndex = 5;
            this.Exit_Button.TabStop = false;
            this.Exit_Button.Text = "Exit";
            this.Exit_Button.UseVisualStyleBackColor = false;
            this.Exit_Button.Click += new System.EventHandler(this.ExitButtonClick);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.CompareOverlay_Desc);
            this.panel6.Controls.Add(this.CompareOverlayButton);
            this.panel6.Controls.Add(this.CompareOverlay_Desc2);
            this.panel6.Location = new System.Drawing.Point(316, 232);
            this.panel6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(296, 91);
            this.panel6.TabIndex = 14;
            // 
            // CompareOverlay_Desc
            // 
            this.CompareOverlay_Desc.AutoSize = true;
            this.CompareOverlay_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CompareOverlay_Desc.ForeColor = System.Drawing.SystemColors.Control;
            this.CompareOverlay_Desc.Location = new System.Drawing.Point(17, 15);
            this.CompareOverlay_Desc.Name = "CompareOverlay_Desc";
            this.CompareOverlay_Desc.Size = new System.Drawing.Size(143, 15);
            this.CompareOverlay_Desc.TabIndex = 0;
            this.CompareOverlay_Desc.Text = "Compare Overlay Key";
            // 
            // CompareOverlayButton
            // 
            this.CompareOverlayButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CompareOverlayButton.Location = new System.Drawing.Point(184, 9);
            this.CompareOverlayButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CompareOverlayButton.Name = "CompareOverlayButton";
            this.CompareOverlayButton.Size = new System.Drawing.Size(59, 29);
            this.CompareOverlayButton.TabIndex = 1;
            this.CompareOverlayButton.TabStop = false;
            this.CompareOverlayButton.Text = "F8";
            this.CompareOverlayButton.UseVisualStyleBackColor = false;
            this.CompareOverlayButton.Click += new System.EventHandler(this.OverlayButtonClick);
            // 
            // CompareOverlay_Desc2
            // 
            this.CompareOverlay_Desc2.AutoSize = true;
            this.CompareOverlay_Desc2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.CompareOverlay_Desc2.ForeColor = System.Drawing.Color.Red;
            this.CompareOverlay_Desc2.Location = new System.Drawing.Point(34, 41);
            this.CompareOverlay_Desc2.Name = "CompareOverlay_Desc2";
            this.CompareOverlay_Desc2.Size = new System.Drawing.Size(97, 15);
            this.CompareOverlay_Desc2.TabIndex = 0;
            this.CompareOverlay_Desc2.Text = "※ Experiential";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.RandomItem);
            this.panel7.Controls.Add(this.ForFunRandom_Desc);
            this.panel7.Location = new System.Drawing.Point(12, 270);
            this.panel7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(296, 64);
            this.panel7.TabIndex = 15;
            // 
            // RandomItem
            // 
            this.RandomItem.AutoSize = true;
            this.RandomItem.ForeColor = System.Drawing.SystemColors.Control;
            this.RandomItem.Location = new System.Drawing.Point(32, 34);
            this.RandomItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.RandomItem.Name = "RandomItem";
            this.RandomItem.Size = new System.Drawing.Size(159, 19);
            this.RandomItem.TabIndex = 6;
            this.RandomItem.TabStop = false;
            this.RandomItem.Text = "Show Random Item Price";
            this.RandomItem.UseVisualStyleBackColor = true;
            this.RandomItem.CheckedChanged += new System.EventHandler(this.RandomItemCheckedChanged);
            // 
            // ForFunRandom_Desc
            // 
            this.ForFunRandom_Desc.AutoSize = true;
            this.ForFunRandom_Desc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ForFunRandom_Desc.ForeColor = System.Drawing.SystemColors.Control;
            this.ForFunRandom_Desc.Location = new System.Drawing.Point(21, 11);
            this.ForFunRandom_Desc.Name = "ForFunRandom_Desc";
            this.ForFunRandom_Desc.Size = new System.Drawing.Size(154, 15);
            this.ForFunRandom_Desc.TabIndex = 0;
            this.ForFunRandom_Desc.Text = "For Fun! Random Item!";
            // 
            // checkIdleTime
            // 
            this.CheckIdleTime.Interval = 60000;
            this.CheckIdleTime.Tick += new System.EventHandler(this.CheckIdleTimeTick);
            // 
            // refresh_b
            // 
            this.refresh_b.BackColor = System.Drawing.Color.WhiteSmoke;
            this.refresh_b.Location = new System.Drawing.Point(537, 391);
            this.refresh_b.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.refresh_b.Name = "refresh_b";
            this.refresh_b.Size = new System.Drawing.Size(75, 29);
            this.refresh_b.TabIndex = 16;
            this.refresh_b.Text = "Refresh";
            this.refresh_b.UseVisualStyleBackColor = false;
            this.refresh_b.Click += new System.EventHandler(this.RefreshBClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(624, 468);
            this.Controls.Add(this.refresh_b);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.Exit_Button);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.MinimizetoTrayWhenStartup);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TarkovPriceViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormFormClosed);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.TrayMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TransparentBar)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.ContextMenuStrip TrayMenu;
        private System.Windows.Forms.ToolStripMenuItem trayshow;
        private System.Windows.Forms.ToolStripMenuItem trayexit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label ShowOverlay_Desc;
        private System.Windows.Forms.Button CheckUpdate;
        private System.Windows.Forms.Button ShowOverlayButton;
        private System.Windows.Forms.Button HideOverlayButton;
        private System.Windows.Forms.Label HideOverlay_Desc;
        private System.Windows.Forms.Label TransParent_Desc;
        private System.Windows.Forms.Button Github;
        private System.Windows.Forms.Button TarkovMarket;
        private System.Windows.Forms.Button TarkovWiki;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label DataProvidedBy;
        private System.Windows.Forms.CheckBox MinimizetoTrayWhenStartup;
        private System.Windows.Forms.CheckBox CloseOverlayWhenMouseMoved;
        private System.Windows.Forms.Label HideOverlay_Desc2;
        private System.Windows.Forms.TrackBar TransparentBar;
        private System.Windows.Forms.Label TransparentText;
        private System.Windows.Forms.Label Version;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox LastPriceChkBox;
        private System.Windows.Forms.CheckBox WeekPriceChkBox;
        private System.Windows.Forms.CheckBox DayPriceChkBox;
        private System.Windows.Forms.CheckBox BuyFromTraderChkBox;
        private System.Windows.Forms.CheckBox SellToTraderChkBox;
        private System.Windows.Forms.Button Tarkov_Official;
        private System.Windows.Forms.CheckBox NeddsChkBox;
        private System.Windows.Forms.CheckBox BartersNCraftsChkBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button Exit_Button;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label CompareOverlay_Desc;
        private System.Windows.Forms.Button CompareOverlayButton;
        private System.Windows.Forms.Label CompareOverlay_Desc2;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.CheckBox RandomItem;
        private System.Windows.Forms.Label ForFunRandom_Desc;
        private System.Windows.Forms.Timer CheckIdleTime;
        private System.Windows.Forms.Button refresh_b;
    }
}

