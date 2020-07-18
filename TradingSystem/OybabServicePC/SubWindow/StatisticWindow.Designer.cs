namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class StatisticWindow
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
            this.components = new System.ComponentModel.Container();
            this.krpcmAddTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpContextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems3 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItemAdd = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemShow = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems5 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.krptbEndTime = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.krplEndTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptbStartTime = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.krplAddTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.krpbSummary = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpcbOrderType = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krplOrderType = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbSpendPaysStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbBalancePaysSpendStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbSpendProductsStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbSalePaysStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbSpendTypeStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbSpendStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbBalancePaysIncomeStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbAdminSaleStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbProductProfitStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbSellProductsStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbIncomeTypeStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbIncomeStatistic = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbOrderType)).BeginInit();
            this.SuspendLayout();
            // 
            // krpcmAddTime
            // 
            this.krpcmAddTime.HeaderText = "AddTime";
            this.krpcmAddTime.Name = "krpcmAddTime";
            this.krpcmAddTime.ReadOnly = true;
            this.krpcmAddTime.Width = 150;
            // 
            // krpContextMenu
            // 
            this.krpContextMenu.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItems3});
            // 
            // kryptonContextMenuItems3
            // 
            this.kryptonContextMenuItems3.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItemAdd,
            this.kryptonContextMenuItemShow});
            this.kryptonContextMenuItems3.StandardStyle = false;
            // 
            // kryptonContextMenuItemAdd
            // 
            this.kryptonContextMenuItemAdd.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.kryptonContextMenuItemAdd.Text = "Menu Item";
            // 
            // kryptonContextMenuItemShow
            // 
            this.kryptonContextMenuItemShow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.kryptonContextMenuItemShow.Text = "Menu Item";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.krptbEndTime);
            this.flowLayoutPanel1.Controls.Add(this.krplEndTime);
            this.flowLayoutPanel1.Controls.Add(this.krptbStartTime);
            this.flowLayoutPanel1.Controls.Add(this.krplAddTime);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(275, 9);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(650, 28);
            this.flowLayoutPanel1.TabIndex = 62;
            // 
            // krptbEndTime
            // 
            this.krptbEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krptbEndTime.CalendarShowToday = false;
            this.krptbEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.krptbEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.krptbEndTime.Location = new System.Drawing.Point(477, 3);
            this.krptbEndTime.Name = "krptbEndTime";
            this.krptbEndTime.Size = new System.Drawing.Size(170, 21);
            this.krptbEndTime.TabIndex = 7;
            this.krptbEndTime.ValueNullable = new System.DateTime(((long)(0)));
            // 
            // krplEndTime
            // 
            this.krplEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplEndTime.Location = new System.Drawing.Point(452, 3);
            this.krplEndTime.Name = "krplEndTime";
            this.krplEndTime.Size = new System.Drawing.Size(19, 20);
            this.krplEndTime.TabIndex = 6;
            this.krplEndTime.Values.Text = " - ";
            // 
            // krptbStartTime
            // 
            this.krptbStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krptbStartTime.CalendarShowToday = false;
            this.krptbStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.krptbStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.krptbStartTime.Location = new System.Drawing.Point(276, 3);
            this.krptbStartTime.Name = "krptbStartTime";
            this.krptbStartTime.Size = new System.Drawing.Size(170, 21);
            this.krptbStartTime.TabIndex = 5;
            this.krptbStartTime.ValueNullable = new System.DateTime(((long)(0)));
            // 
            // krplAddTime
            // 
            this.krplAddTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplAddTime.Location = new System.Drawing.Point(210, 3);
            this.krplAddTime.Name = "krplAddTime";
            this.krplAddTime.Size = new System.Drawing.Size(60, 20);
            this.krplAddTime.TabIndex = 4;
            this.krplAddTime.Values.Text = "AddTime";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.krpcbOrderType);
            this.panel2.Controls.Add(this.krplOrderType);
            this.panel2.Controls.Add(this.krpbSpendPaysStatistic);
            this.panel2.Controls.Add(this.krpbBalancePaysSpendStatistic);
            this.panel2.Controls.Add(this.krpbSpendProductsStatistic);
            this.panel2.Controls.Add(this.krpbSalePaysStatistic);
            this.panel2.Controls.Add(this.krpbSpendTypeStatistic);
            this.panel2.Controls.Add(this.krpbSpendStatistic);
            this.panel2.Controls.Add(this.krpbBalancePaysIncomeStatistic);
            this.panel2.Controls.Add(this.krpbAdminSaleStatistic);
            this.panel2.Controls.Add(this.krpbProductProfitStatistic);
            this.panel2.Controls.Add(this.krpbSellProductsStatistic);
            this.panel2.Controls.Add(this.krpbIncomeTypeStatistic);
            this.panel2.Controls.Add(this.krpbIncomeStatistic);
            this.panel2.Location = new System.Drawing.Point(8, 47);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(914, 525);
            this.panel2.TabIndex = 142;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.krpbSummary);
            this.panel1.Location = new System.Drawing.Point(352, 470);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(217, 47);
            this.panel1.TabIndex = 149;
            // 
            // krpbSummary
            // 
            this.krpbSummary.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.krpbSummary.Location = new System.Drawing.Point(35, 8);
            this.krpbSummary.Name = "krpbSummary";
            this.krpbSummary.Size = new System.Drawing.Size(147, 30);
            this.krpbSummary.TabIndex = 182;
            this.krpbSummary.Values.Text = "Summary";
            this.krpbSummary.Click += new System.EventHandler(this.krpbSummary_Click);
            // 
            // krpcbOrderType
            // 
            this.krpcbOrderType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpcbOrderType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.krpcbOrderType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcbOrderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbOrderType.DropDownWidth = 149;
            this.krpcbOrderType.Location = new System.Drawing.Point(681, 20);
            this.krpcbOrderType.Name = "krpcbOrderType";
            this.krpcbOrderType.Size = new System.Drawing.Size(102, 21);
            this.krpcbOrderType.TabIndex = 148;
            // 
            // krplOrderType
            // 
            this.krplOrderType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplOrderType.Location = new System.Drawing.Point(600, 20);
            this.krplOrderType.Name = "krplOrderType";
            this.krplOrderType.Size = new System.Drawing.Size(56, 20);
            this.krplOrderType.TabIndex = 147;
            this.krplOrderType.Values.Text = "Bill Type";
            // 
            // krpbSpendPaysStatistic
            // 
            this.krpbSpendPaysStatistic.Location = new System.Drawing.Point(152, 182);
            this.krpbSpendPaysStatistic.Name = "krpbSpendPaysStatistic";
            this.krpbSpendPaysStatistic.Size = new System.Drawing.Size(218, 30);
            this.krpbSpendPaysStatistic.TabIndex = 145;
            this.krpbSpendPaysStatistic.Values.Text = "Spend Pays Statistic";
            this.krpbSpendPaysStatistic.Click += new System.EventHandler(this.krpbSpendPaysStatistic_Click);
            // 
            // krpbBalancePaysSpendStatistic
            // 
            this.krpbBalancePaysSpendStatistic.Location = new System.Drawing.Point(152, 418);
            this.krpbBalancePaysSpendStatistic.Name = "krpbBalancePaysSpendStatistic";
            this.krpbBalancePaysSpendStatistic.Size = new System.Drawing.Size(218, 30);
            this.krpbBalancePaysSpendStatistic.TabIndex = 155;
            this.krpbBalancePaysSpendStatistic.Values.Text = "Balance Pays Spend Statistic";
            this.krpbBalancePaysSpendStatistic.Click += new System.EventHandler(this.krpbBalancePaysSpendStatistic_Click);
            // 
            // krpbSpendProductsStatistic
            // 
            this.krpbSpendProductsStatistic.Location = new System.Drawing.Point(152, 218);
            this.krpbSpendProductsStatistic.Name = "krpbSpendProductsStatistic";
            this.krpbSpendProductsStatistic.Size = new System.Drawing.Size(218, 30);
            this.krpbSpendProductsStatistic.TabIndex = 150;
            this.krpbSpendProductsStatistic.Values.Text = "Spend Products Statistic";
            this.krpbSpendProductsStatistic.Click += new System.EventHandler(this.krpbSpendProductsStatistic_Click);
            // 
            // krpbSalePaysStatistic
            // 
            this.krpbSalePaysStatistic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbSalePaysStatistic.Location = new System.Drawing.Point(554, 182);
            this.krpbSalePaysStatistic.Name = "krpbSalePaysStatistic";
            this.krpbSalePaysStatistic.Size = new System.Drawing.Size(215, 30);
            this.krpbSalePaysStatistic.TabIndex = 146;
            this.krpbSalePaysStatistic.Values.Text = "Sale Pays Statistic";
            this.krpbSalePaysStatistic.Click += new System.EventHandler(this.krpbSalePaysStatistic_Click);
            // 
            // krpbSpendTypeStatistic
            // 
            this.krpbSpendTypeStatistic.Location = new System.Drawing.Point(152, 111);
            this.krpbSpendTypeStatistic.Name = "krpbSpendTypeStatistic";
            this.krpbSpendTypeStatistic.Size = new System.Drawing.Size(218, 30);
            this.krpbSpendTypeStatistic.TabIndex = 142;
            this.krpbSpendTypeStatistic.Values.Text = "Spend Type Statistic";
            this.krpbSpendTypeStatistic.Click += new System.EventHandler(this.krpbSpendTypeStatistic_Click);
            // 
            // krpbSpendStatistic
            // 
            this.krpbSpendStatistic.Location = new System.Drawing.Point(152, 75);
            this.krpbSpendStatistic.Name = "krpbSpendStatistic";
            this.krpbSpendStatistic.Size = new System.Drawing.Size(218, 30);
            this.krpbSpendStatistic.TabIndex = 142;
            this.krpbSpendStatistic.Values.Text = "Spend Statistic";
            this.krpbSpendStatistic.Click += new System.EventHandler(this.krpbSpendStatistic_Click);
            // 
            // krpbBalancePaysIncomeStatistic
            // 
            this.krpbBalancePaysIncomeStatistic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbBalancePaysIncomeStatistic.Location = new System.Drawing.Point(554, 418);
            this.krpbBalancePaysIncomeStatistic.Name = "krpbBalancePaysIncomeStatistic";
            this.krpbBalancePaysIncomeStatistic.Size = new System.Drawing.Size(215, 30);
            this.krpbBalancePaysIncomeStatistic.TabIndex = 156;
            this.krpbBalancePaysIncomeStatistic.Values.Text = "Balance Pays Income Statistic";
            this.krpbBalancePaysIncomeStatistic.Click += new System.EventHandler(this.krpbBalancePaysIncomeStatistic_Click);
            // 
            // krpbAdminSaleStatistic
            // 
            this.krpbAdminSaleStatistic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbAdminSaleStatistic.Location = new System.Drawing.Point(554, 347);
            this.krpbAdminSaleStatistic.Name = "krpbAdminSaleStatistic";
            this.krpbAdminSaleStatistic.Size = new System.Drawing.Size(215, 30);
            this.krpbAdminSaleStatistic.TabIndex = 154;
            this.krpbAdminSaleStatistic.Values.Text = "Admin Sale Statistic";
            this.krpbAdminSaleStatistic.Click += new System.EventHandler(this.krpbAdminSaleStatistic_Click);
            // 
            // krpbProductProfitStatistic
            // 
            this.krpbProductProfitStatistic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbProductProfitStatistic.Location = new System.Drawing.Point(554, 311);
            this.krpbProductProfitStatistic.Name = "krpbProductProfitStatistic";
            this.krpbProductProfitStatistic.Size = new System.Drawing.Size(215, 30);
            this.krpbProductProfitStatistic.TabIndex = 153;
            this.krpbProductProfitStatistic.Values.Text = "Product Profit Statistic";
            this.krpbProductProfitStatistic.Click += new System.EventHandler(this.krpbProductProfitStatistic_Click);
            // 
            // krpbSellProductsStatistic
            // 
            this.krpbSellProductsStatistic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbSellProductsStatistic.Location = new System.Drawing.Point(554, 218);
            this.krpbSellProductsStatistic.Name = "krpbSellProductsStatistic";
            this.krpbSellProductsStatistic.Size = new System.Drawing.Size(215, 30);
            this.krpbSellProductsStatistic.TabIndex = 151;
            this.krpbSellProductsStatistic.Values.Text = "Sell Products Statistic";
            this.krpbSellProductsStatistic.Click += new System.EventHandler(this.krpbSellProductsStatistic_Click);
            // 
            // krpbIncomeTypeStatistic
            // 
            this.krpbIncomeTypeStatistic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbIncomeTypeStatistic.Location = new System.Drawing.Point(554, 111);
            this.krpbIncomeTypeStatistic.Name = "krpbIncomeTypeStatistic";
            this.krpbIncomeTypeStatistic.Size = new System.Drawing.Size(215, 30);
            this.krpbIncomeTypeStatistic.TabIndex = 144;
            this.krpbIncomeTypeStatistic.Values.Text = "Income Type Statistic";
            this.krpbIncomeTypeStatistic.Click += new System.EventHandler(this.krpbIncomeTypeStatistic_Click);
            // 
            // krpbIncomeStatistic
            // 
            this.krpbIncomeStatistic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbIncomeStatistic.Location = new System.Drawing.Point(554, 75);
            this.krpbIncomeStatistic.Name = "krpbIncomeStatistic";
            this.krpbIncomeStatistic.Size = new System.Drawing.Size(215, 30);
            this.krpbIncomeStatistic.TabIndex = 144;
            this.krpbIncomeStatistic.Values.Text = "Income Statistic";
            this.krpbIncomeStatistic.Click += new System.EventHandler(this.krpbImcomeStatistic_Click);
            // 
            // StatisticWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(935, 580);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(648, 294);
            this.Name = "StatisticWindow";
            this.Text = "StatisticWindow";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpcbOrderType)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu krpContextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems3;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemShow;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems5;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAddTime;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker krptbEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker krptbStartTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplAddTime;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSummary;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbOrderType;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplOrderType;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSpendPaysStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbBalancePaysSpendStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSpendProductsStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSalePaysStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSpendStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbBalancePaysIncomeStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbAdminSaleStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbProductProfitStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSellProductsStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbIncomeStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette kryptonPalette1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSpendTypeStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbIncomeTypeStatistic;
    }
}