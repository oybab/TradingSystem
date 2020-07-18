namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class SettingsWindow
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
            this.krpbChangeLocal = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpSystem = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krpbOpenPriceMonitor = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbOpenCashDrawer = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpcbCardReader = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpcbBarcodeReader = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpcbPriceMonitor = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpcbCashDrawer = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpcLanguage = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpcIsLocalPrint = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krptServerIP = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplIsLocalPrint = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplCardReader = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBarcodeReader = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPriceMonitor = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplCashDrawer = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplLanguage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.KrplServerIpAddress = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpServer = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krplPrintInfoAlert = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplGlobalSettingAlert = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptGlobalSetting = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbChangePrintInfo = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplGlobalSetting = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.KrplPrintInfo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbChangeServer = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.krpSystem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpSystem.Panel)).BeginInit();
            this.krpSystem.Panel.SuspendLayout();
            this.krpSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbCardReader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbBarcodeReader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbPriceMonitor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbCashDrawer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpServer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpServer.Panel)).BeginInit();
            this.krpServer.Panel.SuspendLayout();
            this.krpServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // krpbChangeLocal
            // 
            this.krpbChangeLocal.Location = new System.Drawing.Point(131, 333);
            this.krpbChangeLocal.Name = "krpbChangeLocal";
            this.krpbChangeLocal.Size = new System.Drawing.Size(90, 25);
            this.krpbChangeLocal.TabIndex = 0;
            this.krpbChangeLocal.Values.Text = "Change";
            this.krpbChangeLocal.Click += new System.EventHandler(this.krpbChangeLocal_Click);
            // 
            // krpSystem
            // 
            this.krpSystem.Location = new System.Drawing.Point(18, 12);
            this.krpSystem.Name = "krpSystem";
            // 
            // krpSystem.Panel
            // 
            this.krpSystem.Panel.Controls.Add(this.krpbOpenPriceMonitor);
            this.krpSystem.Panel.Controls.Add(this.krpbOpenCashDrawer);
            this.krpSystem.Panel.Controls.Add(this.krpcbCardReader);
            this.krpSystem.Panel.Controls.Add(this.krpcbBarcodeReader);
            this.krpSystem.Panel.Controls.Add(this.krpcbPriceMonitor);
            this.krpSystem.Panel.Controls.Add(this.krpcbCashDrawer);
            this.krpSystem.Panel.Controls.Add(this.krpcLanguage);
            this.krpSystem.Panel.Controls.Add(this.krpcIsLocalPrint);
            this.krpSystem.Panel.Controls.Add(this.krptServerIP);
            this.krpSystem.Panel.Controls.Add(this.krplIsLocalPrint);
            this.krpSystem.Panel.Controls.Add(this.krplCardReader);
            this.krpSystem.Panel.Controls.Add(this.krplBarcodeReader);
            this.krpSystem.Panel.Controls.Add(this.krplPriceMonitor);
            this.krpSystem.Panel.Controls.Add(this.krplCashDrawer);
            this.krpSystem.Panel.Controls.Add(this.krplLanguage);
            this.krpSystem.Panel.Controls.Add(this.KrplServerIpAddress);
            this.krpSystem.Size = new System.Drawing.Size(329, 303);
            this.krpSystem.TabIndex = 5;
            this.krpSystem.Text = "System";
            this.krpSystem.Values.Heading = "System";
            // 
            // krpbOpenPriceMonitor
            // 
            this.krpbOpenPriceMonitor.Enabled = false;
            this.krpbOpenPriceMonitor.Location = new System.Drawing.Point(233, 128);
            this.krpbOpenPriceMonitor.Name = "krpbOpenPriceMonitor";
            this.krpbOpenPriceMonitor.Size = new System.Drawing.Size(59, 25);
            this.krpbOpenPriceMonitor.TabIndex = 10;
            this.krpbOpenPriceMonitor.Values.Text = "Test";
            this.krpbOpenPriceMonitor.Click += new System.EventHandler(this.krpbOpenPriceMonitor_Click);
            // 
            // krpbOpenCashDrawer
            // 
            this.krpbOpenCashDrawer.Enabled = false;
            this.krpbOpenCashDrawer.Location = new System.Drawing.Point(233, 91);
            this.krpbOpenCashDrawer.Name = "krpbOpenCashDrawer";
            this.krpbOpenCashDrawer.Size = new System.Drawing.Size(59, 25);
            this.krpbOpenCashDrawer.TabIndex = 6;
            this.krpbOpenCashDrawer.Values.Text = "Open";
            this.krpbOpenCashDrawer.Click += new System.EventHandler(this.krpbOpenCashDrawer_Click);
            // 
            // krpcbCardReader
            // 
            this.krpcbCardReader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbCardReader.DropDownWidth = 121;
            this.krpcbCardReader.Location = new System.Drawing.Point(159, 206);
            this.krpcbCardReader.Name = "krpcbCardReader";
            this.krpcbCardReader.Size = new System.Drawing.Size(92, 21);
            this.krpcbCardReader.TabIndex = 14;
            this.krpcbCardReader.SelectedIndexChanged += new System.EventHandler(this.krpcbCardReader_SelectedIndexChanged);
            // 
            // krpcbBarcodeReader
            // 
            this.krpcbBarcodeReader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbBarcodeReader.DropDownWidth = 121;
            this.krpcbBarcodeReader.Location = new System.Drawing.Point(159, 169);
            this.krpcbBarcodeReader.Name = "krpcbBarcodeReader";
            this.krpcbBarcodeReader.Size = new System.Drawing.Size(92, 21);
            this.krpcbBarcodeReader.TabIndex = 12;
            this.krpcbBarcodeReader.SelectedIndexChanged += new System.EventHandler(this.krpcbBarcodeReader_SelectedIndexChanged);
            // 
            // krpcbPriceMonitor
            // 
            this.krpcbPriceMonitor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbPriceMonitor.DropDownWidth = 121;
            this.krpcbPriceMonitor.Location = new System.Drawing.Point(159, 129);
            this.krpcbPriceMonitor.Name = "krpcbPriceMonitor";
            this.krpcbPriceMonitor.Size = new System.Drawing.Size(65, 21);
            this.krpcbPriceMonitor.TabIndex = 8;
            this.krpcbPriceMonitor.SelectedIndexChanged += new System.EventHandler(this.krpcbPriceMonitor_SelectedIndexChanged);
            // 
            // krpcbCashDrawer
            // 
            this.krpcbCashDrawer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbCashDrawer.DropDownWidth = 121;
            this.krpcbCashDrawer.Location = new System.Drawing.Point(159, 92);
            this.krpcbCashDrawer.Name = "krpcbCashDrawer";
            this.krpcbCashDrawer.Size = new System.Drawing.Size(65, 21);
            this.krpcbCashDrawer.TabIndex = 4;
            this.krpcbCashDrawer.SelectedIndexChanged += new System.EventHandler(this.krpcbCashDrawer_SelectedIndexChanged);
            // 
            // krpcLanguage
            // 
            this.krpcLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcLanguage.DropDownWidth = 121;
            this.krpcLanguage.Location = new System.Drawing.Point(159, 241);
            this.krpcLanguage.Name = "krpcLanguage";
            this.krpcLanguage.Size = new System.Drawing.Size(121, 21);
            this.krpcLanguage.TabIndex = 16;
            // 
            // krpcIsLocalPrint
            // 
            this.krpcIsLocalPrint.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcIsLocalPrint.Location = new System.Drawing.Point(159, 59);
            this.krpcIsLocalPrint.Name = "krpcIsLocalPrint";
            this.krpcIsLocalPrint.Size = new System.Drawing.Size(42, 20);
            this.krpcIsLocalPrint.TabIndex = 3;
            this.krpcIsLocalPrint.Text = "Yes";
            this.krpcIsLocalPrint.Values.Text = "Yes";
            this.krpcIsLocalPrint.CheckedChanged += new System.EventHandler(this.krpcIsLocalPrint_CheckedChanged);
            // 
            // krptServerIP
            // 
            this.krptServerIP.Location = new System.Drawing.Point(159, 20);
            this.krptServerIP.MaxLength = 100;
            this.krptServerIP.Name = "krptServerIP";
            this.krptServerIP.Size = new System.Drawing.Size(118, 23);
            this.krptServerIP.TabIndex = 2;
            this.krptServerIP.Text = "192.168.1.200";
            // 
            // krplIsLocalPrint
            // 
            this.krplIsLocalPrint.Location = new System.Drawing.Point(16, 56);
            this.krplIsLocalPrint.Name = "krplIsLocalPrint";
            this.krplIsLocalPrint.Size = new System.Drawing.Size(79, 20);
            this.krplIsLocalPrint.TabIndex = 0;
            this.krplIsLocalPrint.Values.Text = "Is Local Print";
            // 
            // krplCardReader
            // 
            this.krplCardReader.Location = new System.Drawing.Point(16, 206);
            this.krplCardReader.Name = "krplCardReader";
            this.krplCardReader.Size = new System.Drawing.Size(74, 20);
            this.krplCardReader.TabIndex = 0;
            this.krplCardReader.Values.Text = "CardReader";
            // 
            // krplBarcodeReader
            // 
            this.krplBarcodeReader.Location = new System.Drawing.Point(16, 169);
            this.krplBarcodeReader.Name = "krplBarcodeReader";
            this.krplBarcodeReader.Size = new System.Drawing.Size(93, 20);
            this.krplBarcodeReader.TabIndex = 0;
            this.krplBarcodeReader.Values.Text = "BarcodeReader";
            // 
            // krplPriceMonitor
            // 
            this.krplPriceMonitor.Location = new System.Drawing.Point(16, 130);
            this.krplPriceMonitor.Name = "krplPriceMonitor";
            this.krplPriceMonitor.Size = new System.Drawing.Size(84, 20);
            this.krplPriceMonitor.TabIndex = 0;
            this.krplPriceMonitor.Values.Text = "Price Monitor";
            // 
            // krplCashDrawer
            // 
            this.krplCashDrawer.Location = new System.Drawing.Point(16, 93);
            this.krplCashDrawer.Name = "krplCashDrawer";
            this.krplCashDrawer.Size = new System.Drawing.Size(76, 20);
            this.krplCashDrawer.TabIndex = 0;
            this.krplCashDrawer.Values.Text = "CashDrawer";
            // 
            // krplLanguage
            // 
            this.krplLanguage.Location = new System.Drawing.Point(16, 241);
            this.krplLanguage.Name = "krplLanguage";
            this.krplLanguage.Size = new System.Drawing.Size(64, 20);
            this.krplLanguage.TabIndex = 0;
            this.krplLanguage.Values.Text = "Language";
            // 
            // KrplServerIpAddress
            // 
            this.KrplServerIpAddress.Location = new System.Drawing.Point(16, 20);
            this.KrplServerIpAddress.Name = "KrplServerIpAddress";
            this.KrplServerIpAddress.Size = new System.Drawing.Size(105, 20);
            this.KrplServerIpAddress.TabIndex = 0;
            this.KrplServerIpAddress.Values.Text = "Server IP Address";
            // 
            // krpServer
            // 
            this.krpServer.Location = new System.Drawing.Point(378, 12);
            this.krpServer.Name = "krpServer";
            // 
            // krpServer.Panel
            // 
            this.krpServer.Panel.Controls.Add(this.krplPrintInfoAlert);
            this.krpServer.Panel.Controls.Add(this.krplGlobalSettingAlert);
            this.krpServer.Panel.Controls.Add(this.krptGlobalSetting);
            this.krpServer.Panel.Controls.Add(this.krpbChangePrintInfo);
            this.krpServer.Panel.Controls.Add(this.krplGlobalSetting);
            this.krpServer.Panel.Controls.Add(this.KrplPrintInfo);
            this.krpServer.Size = new System.Drawing.Size(335, 303);
            this.krpServer.TabIndex = 6;
            this.krpServer.Text = "Server";
            this.krpServer.Values.Heading = "Server";
            // 
            // krplPrintInfoAlert
            // 
            this.krplPrintInfoAlert.Location = new System.Drawing.Point(273, 77);
            this.krplPrintInfoAlert.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplPrintInfoAlert.Name = "krplPrintInfoAlert";
            this.krplPrintInfoAlert.Size = new System.Drawing.Size(16, 20);
            this.krplPrintInfoAlert.StateCommon.ShortText.Color1 = System.Drawing.Color.Red;
            this.krplPrintInfoAlert.TabIndex = 11;
            this.krplPrintInfoAlert.Values.Text = "*";
            this.krplPrintInfoAlert.Visible = false;
            // 
            // krplGlobalSettingAlert
            // 
            this.krplGlobalSettingAlert.Location = new System.Drawing.Point(273, 20);
            this.krplGlobalSettingAlert.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplGlobalSettingAlert.Name = "krplGlobalSettingAlert";
            this.krplGlobalSettingAlert.Size = new System.Drawing.Size(16, 20);
            this.krplGlobalSettingAlert.StateCommon.ShortText.Color1 = System.Drawing.Color.Red;
            this.krplGlobalSettingAlert.TabIndex = 11;
            this.krplGlobalSettingAlert.Values.Text = "*";
            this.krplGlobalSettingAlert.Visible = false;
            // 
            // krptGlobalSetting
            // 
            this.krptGlobalSetting.Location = new System.Drawing.Point(199, 18);
            this.krptGlobalSetting.Name = "krptGlobalSetting";
            this.krptGlobalSetting.Size = new System.Drawing.Size(68, 25);
            this.krptGlobalSetting.TabIndex = 20;
            this.krptGlobalSetting.Values.Text = "Change";
            this.krptGlobalSetting.Click += new System.EventHandler(this.krptGlobalSetting_Click);
            // 
            // krpbChangePrintInfo
            // 
            this.krpbChangePrintInfo.Location = new System.Drawing.Point(199, 75);
            this.krpbChangePrintInfo.Name = "krpbChangePrintInfo";
            this.krpbChangePrintInfo.Size = new System.Drawing.Size(68, 25);
            this.krpbChangePrintInfo.TabIndex = 25;
            this.krpbChangePrintInfo.Values.Text = "Change";
            this.krpbChangePrintInfo.Click += new System.EventHandler(this.krpbChangePrintInfo_Click);
            // 
            // krplGlobalSetting
            // 
            this.krplGlobalSetting.Location = new System.Drawing.Point(25, 20);
            this.krplGlobalSetting.Name = "krplGlobalSetting";
            this.krplGlobalSetting.Size = new System.Drawing.Size(84, 20);
            this.krplGlobalSetting.TabIndex = 0;
            this.krplGlobalSetting.Values.Text = "GlobalSetting";
            // 
            // KrplPrintInfo
            // 
            this.KrplPrintInfo.Location = new System.Drawing.Point(25, 77);
            this.KrplPrintInfo.Name = "KrplPrintInfo";
            this.KrplPrintInfo.Size = new System.Drawing.Size(61, 20);
            this.KrplPrintInfo.TabIndex = 0;
            this.KrplPrintInfo.Values.Text = "Print Info";
            // 
            // krpbChangeServer
            // 
            this.krpbChangeServer.Location = new System.Drawing.Point(501, 333);
            this.krpbChangeServer.Name = "krpbChangeServer";
            this.krpbChangeServer.Size = new System.Drawing.Size(90, 25);
            this.krpbChangeServer.TabIndex = 1;
            this.krpbChangeServer.Values.Text = "Change";
            this.krpbChangeServer.Click += new System.EventHandler(this.krpbChangeServer_Click);
            // 
            // kryptonPalette1
            // 
            this.kryptonPalette1.BasePaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Global;
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(737, 374);
            this.Controls.Add(this.krpServer);
            this.Controls.Add(this.krpSystem);
            this.Controls.Add(this.krpbChangeServer);
            this.Controls.Add(this.krpbChangeLocal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SettingsWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpSystem.Panel)).EndInit();
            this.krpSystem.Panel.ResumeLayout(false);
            this.krpSystem.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpSystem)).EndInit();
            this.krpSystem.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpcbCardReader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbBarcodeReader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbPriceMonitor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbCashDrawer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpServer.Panel)).EndInit();
            this.krpServer.Panel.ResumeLayout(false);
            this.krpServer.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpServer)).EndInit();
            this.krpServer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbChangeLocal;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpSystem;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpServer;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptServerIP;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel KrplServerIpAddress;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbChangeServer;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcIsLocalPrint;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplIsLocalPrint;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbOpenCashDrawer;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbCashDrawer;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplCashDrawer;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel KrplPrintInfo;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbBarcodeReader;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBarcodeReader;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbChangePrintInfo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPrintInfoAlert;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbOpenPriceMonitor;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbPriceMonitor;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPriceMonitor;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbCardReader;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplCardReader;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette kryptonPalette1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplGlobalSettingAlert;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krptGlobalSetting;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplGlobalSetting;
    }
}