
namespace TarkovPriceViewer.Forms
{
    partial class CompareOverlay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ItemComparePanel = new System.Windows.Forms.Panel();
            this.ItemCompareTxt = new System.Windows.Forms.RichTextBox();
            this.ItemCompareGrid = new System.Windows.Forms.DataGridView();
            this.ItemComparePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCompareGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // itemcompare_panel
            // 
            this.ItemComparePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemComparePanel.AutoSize = true;
            this.ItemComparePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ItemComparePanel.BackColor = System.Drawing.Color.Black;
            this.ItemComparePanel.Controls.Add(this.ItemCompareTxt);
            this.ItemComparePanel.Controls.Add(this.ItemCompareGrid);
            this.ItemComparePanel.ForeColor = System.Drawing.Color.Black;
            this.ItemComparePanel.Location = new System.Drawing.Point(12, 119);
            this.ItemComparePanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ItemComparePanel.Name = "itemcompare_panel";
            this.ItemComparePanel.Padding = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.ItemComparePanel.Size = new System.Drawing.Size(123, 103);
            this.ItemComparePanel.TabIndex = 7;
            this.ItemComparePanel.LocationChanged += new System.EventHandler(this.ItemWindowPanelLocationChanged);
            this.ItemComparePanel.SizeChanged += new System.EventHandler(this.ItemWindowPanelSizeChanged);
            this.ItemComparePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ItemWindowPanelPaint);
            // 
            // itemcompare_text
            // 
            this.ItemCompareTxt.BackColor = System.Drawing.Color.Black;
            this.ItemCompareTxt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ItemCompareTxt.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.ItemCompareTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ItemCompareTxt.ForeColor = System.Drawing.Color.White;
            this.ItemCompareTxt.Location = new System.Drawing.Point(10, 16);
            this.ItemCompareTxt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ItemCompareTxt.Name = "itemcompare_text";
            this.ItemCompareTxt.ReadOnly = true;
            this.ItemCompareTxt.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.ItemCompareTxt.Size = new System.Drawing.Size(100, 25);
            this.ItemCompareTxt.TabIndex = 0;
            this.ItemCompareTxt.TabStop = false;
            this.ItemCompareTxt.Text = "text";
            this.ItemCompareTxt.WordWrap = false;
            this.ItemCompareTxt.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.ItemWindowTextContentsResized);
            this.ItemCompareTxt.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ItemCompareTextMouseDown);
            this.ItemCompareTxt.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ItemCompareTextMouseMove);
            this.ItemCompareTxt.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ItemCompareTextMouseUp);
            // 
            // ItemCompareGrid
            // 
            this.ItemCompareGrid.AllowUserToAddRows = false;
            this.ItemCompareGrid.AllowUserToDeleteRows = false;
            this.ItemCompareGrid.AllowUserToResizeColumns = false;
            this.ItemCompareGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.ItemCompareGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ItemCompareGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ItemCompareGrid.BackgroundColor = System.Drawing.Color.Black;
            this.ItemCompareGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.ItemCompareGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.ItemCompareGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.DefaultCellStyle = dataGridViewCellStyle3;
            this.ItemCompareGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ItemCompareGrid.EnableHeadersVisualStyles = false;
            this.ItemCompareGrid.GridColor = System.Drawing.Color.White;
            this.ItemCompareGrid.Location = new System.Drawing.Point(10, 49);
            this.ItemCompareGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ItemCompareGrid.MultiSelect = false;
            this.ItemCompareGrid.Name = "ItemCompareGrid";
            this.ItemCompareGrid.ReadOnly = true;
            this.ItemCompareGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.ItemCompareGrid.RowHeadersVisible = false;
            this.ItemCompareGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Black;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.ItemCompareGrid.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ItemCompareGrid.RowTemplate.Height = 23;
            this.ItemCompareGrid.RowTemplate.ReadOnly = true;
            this.ItemCompareGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.ItemCompareGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.ItemCompareGrid.ShowCellErrors = false;
            this.ItemCompareGrid.ShowCellToolTips = false;
            this.ItemCompareGrid.ShowEditingIcon = false;
            this.ItemCompareGrid.ShowRowErrors = false;
            this.ItemCompareGrid.Size = new System.Drawing.Size(100, 38);
            this.ItemCompareGrid.TabIndex = 0;
            this.ItemCompareGrid.TabStop = false;
            // 
            // CompareOverlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(800, 562);
            this.ControlBox = false;
            this.Controls.Add(this.ItemComparePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompareOverlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Overlay";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.ItemComparePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ItemCompareGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel ItemComparePanel;
        private System.Windows.Forms.DataGridView ItemCompareGrid;
        private System.Windows.Forms.RichTextBox ItemCompareTxt;
    }
}