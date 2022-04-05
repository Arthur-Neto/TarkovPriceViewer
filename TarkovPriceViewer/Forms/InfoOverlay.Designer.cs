
namespace TarkovPriceViewer.Forms
{
    partial class InfoOverlay
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
            this.itemInfoPanel = new System.Windows.Forms.Panel();
            this.itemInfoBall = new System.Windows.Forms.DataGridView();
            this.itemInfoText = new System.Windows.Forms.RichTextBox();
            this.itemInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemInfoBall)).BeginInit();
            this.SuspendLayout();
            // 
            // itemInfoPanel
            // 
            this.itemInfoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemInfoPanel.AutoSize = true;
            this.itemInfoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.itemInfoPanel.BackColor = System.Drawing.Color.Black;
            this.itemInfoPanel.Controls.Add(this.itemInfoBall);
            this.itemInfoPanel.Controls.Add(this.itemInfoText);
            this.itemInfoPanel.ForeColor = System.Drawing.Color.Black;
            this.itemInfoPanel.Location = new System.Drawing.Point(12, 15);
            this.itemInfoPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.itemInfoPanel.Name = "itemInfoPanel";
            this.itemInfoPanel.Padding = new System.Windows.Forms.Padding(10, 12, 10, 12);
            this.itemInfoPanel.Size = new System.Drawing.Size(223, 99);
            this.itemInfoPanel.TabIndex = 8;
            this.itemInfoPanel.LocationChanged += new System.EventHandler(this.ItemWindowPanelLocationChanged);
            this.itemInfoPanel.SizeChanged += new System.EventHandler(this.ItemWindowPanelSizeChanged);
            this.itemInfoPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ItemWindowPanelPaint);
            // 
            // itemInfoBall
            // 
            this.itemInfoBall.AllowUserToAddRows = false;
            this.itemInfoBall.AllowUserToDeleteRows = false;
            this.itemInfoBall.AllowUserToResizeColumns = false;
            this.itemInfoBall.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.itemInfoBall.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.itemInfoBall.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.itemInfoBall.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.itemInfoBall.BackgroundColor = System.Drawing.Color.Black;
            this.itemInfoBall.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.itemInfoBall.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.itemInfoBall.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.itemInfoBall.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.itemInfoBall.DefaultCellStyle = dataGridViewCellStyle3;
            this.itemInfoBall.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.itemInfoBall.EnableHeadersVisualStyles = false;
            this.itemInfoBall.GridColor = System.Drawing.Color.White;
            this.itemInfoBall.Location = new System.Drawing.Point(10, 45);
            this.itemInfoBall.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.itemInfoBall.MultiSelect = false;
            this.itemInfoBall.Name = "itemInfoBall";
            this.itemInfoBall.ReadOnly = true;
            this.itemInfoBall.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.itemInfoBall.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.itemInfoBall.RowHeadersVisible = false;
            this.itemInfoBall.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.itemInfoBall.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.itemInfoBall.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.itemInfoBall.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.Black;
            this.itemInfoBall.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.itemInfoBall.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.itemInfoBall.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.Black;
            this.itemInfoBall.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.itemInfoBall.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.itemInfoBall.RowTemplate.Height = 23;
            this.itemInfoBall.RowTemplate.ReadOnly = true;
            this.itemInfoBall.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.itemInfoBall.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.itemInfoBall.ShowCellErrors = false;
            this.itemInfoBall.ShowCellToolTips = false;
            this.itemInfoBall.ShowEditingIcon = false;
            this.itemInfoBall.ShowRowErrors = false;
            this.itemInfoBall.Size = new System.Drawing.Size(200, 38);
            this.itemInfoBall.TabIndex = 1;
            this.itemInfoBall.TabStop = false;
            // 
            // iteminfo_text
            // 
            this.itemInfoText.BackColor = System.Drawing.Color.Black;
            this.itemInfoText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.itemInfoText.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.itemInfoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.itemInfoText.ForeColor = System.Drawing.Color.White;
            this.itemInfoText.Location = new System.Drawing.Point(10, 12);
            this.itemInfoText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.itemInfoText.Name = "iteminfo_text";
            this.itemInfoText.ReadOnly = true;
            this.itemInfoText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.itemInfoText.Size = new System.Drawing.Size(100, 25);
            this.itemInfoText.TabIndex = 0;
            this.itemInfoText.TabStop = false;
            this.itemInfoText.Text = "text";
            this.itemInfoText.WordWrap = false;
            this.itemInfoText.ContentsResized += new System.Windows.Forms.ContentsResizedEventHandler(this.ItemWindowTextContentsResized);
            // 
            // InfoOverlay
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
            this.Controls.Add(this.itemInfoPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InfoOverlay";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Overlay";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.itemInfoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.itemInfoBall)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel itemInfoPanel;
        private System.Windows.Forms.RichTextBox itemInfoText;
        private System.Windows.Forms.DataGridView itemInfoBall;
    }
}