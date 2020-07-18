namespace Oybab.Reports.Common
{
    public sealed partial class ProductLabelMedium
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

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraPrinting.BarCode.EAN8Generator eaN8Generator1 = new DevExpress.XtraPrinting.BarCode.EAN8Generator();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrBarCode1 = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrcsFont8 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrcsFont9 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrcsFont10 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrcsFont7 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrcsFont6 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrcsFont5 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrcsFont4 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrcsLine = new DevExpress.XtraReports.UI.XRControlStyle();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrBarCode1,
            this.xrLabel2,
            this.xrLabel1});
            this.Detail.Dpi = 254F;
            this.Detail.HeightF = 300F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrBarCode1
            // 
            this.xrBarCode1.AutoModule = true;
            this.xrBarCode1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "BarcodeNo")});
            this.xrBarCode1.Dpi = 254F;
            this.xrBarCode1.LocationFloat = new DevExpress.Utils.PointFloat(3.670201E-05F, 153.8663F);
            this.xrBarCode1.Module = 7.9F;
            this.xrBarCode1.Name = "xrBarCode1";
            this.xrBarCode1.Padding = new DevExpress.XtraPrinting.PaddingInfo(24, 24, 0, 0, 254F);
            this.xrBarCode1.SizeF = new System.Drawing.SizeF(400F, 142F);
            this.xrBarCode1.StyleName = "xrcsFont8";
            this.xrBarCode1.StylePriority.UsePadding = false;
            this.xrBarCode1.StylePriority.UseTextAlignment = false;
            this.xrBarCode1.Symbology = eaN8Generator1;
            this.xrBarCode1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel2
            // 
            this.xrLabel2.CanGrow = false;
            this.xrLabel2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Price", "{0:￥0.00}")});
            this.xrLabel2.Dpi = 254F;
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(2.883729E-05F, 105.8784F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(400F, 46.70368F);
            this.xrLabel2.StyleName = "xrcsFont9";
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.CanGrow = false;
            this.xrLabel1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "ProductName")});
            this.xrLabel1.Dpi = 254F;
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(2.883729E-05F, 31F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 0, 0, 254F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(400F, 70.87836F);
            this.xrLabel1.StyleName = "xrcsFont8";
            this.xrLabel1.StylePriority.UsePadding = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "xrLabel1";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // TopMargin
            // 
            this.TopMargin.Dpi = 254F;
            this.TopMargin.HeightF = 0F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Dpi = 254F;
            this.BottomMargin.HeightF = 0F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 254F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrcsFont8
            // 
            this.xrcsFont8.Font = new System.Drawing.Font("微软雅黑", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrcsFont8.Name = "xrcsFont8";
            // 
            // xrcsFont9
            // 
            this.xrcsFont9.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrcsFont9.Name = "xrcsFont9";
            // 
            // xrcsFont10
            // 
            this.xrcsFont10.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrcsFont10.Name = "xrcsFont10";
            // 
            // xrcsFont7
            // 
            this.xrcsFont7.Font = new System.Drawing.Font("微软雅黑", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrcsFont7.Name = "xrcsFont7";
            // 
            // xrcsFont6
            // 
            this.xrcsFont6.Font = new System.Drawing.Font("微软雅黑", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrcsFont6.Name = "xrcsFont6";
            // 
            // xrcsFont5
            // 
            this.xrcsFont5.Font = new System.Drawing.Font("微软雅黑", 5.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrcsFont5.Name = "xrcsFont5";
            // 
            // xrcsFont4
            // 
            this.xrcsFont4.Font = new System.Drawing.Font("微软雅黑", 3.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrcsFont4.Name = "xrcsFont4";
            // 
            // xrcsLine
            // 
            this.xrcsLine.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Dash;
            this.xrcsLine.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrcsLine.BorderWidth = 0.1F;
            this.xrcsLine.Font = new System.Drawing.Font("微软雅黑", 1.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrcsLine.Name = "xrcsLine";
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(Oybab.Report.Model.ProductLabel);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // ProductLabelMedium
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.objectDataSource1});
            this.DataSource = this.objectDataSource1;
            this.Dpi = 254F;
            this.Font = new System.Drawing.Font("微软雅黑", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 300;
            this.PageWidth = 400;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.ReportUnit = DevExpress.XtraReports.UI.ReportUnit.TenthsOfAMillimeter;
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrcsFont8,
            this.xrcsFont9,
            this.xrcsFont10,
            this.xrcsFont7,
            this.xrcsFont6,
            this.xrcsFont5,
            this.xrcsFont4,
            this.xrcsLine});
            this.Version = "15.1";
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRControlStyle xrcsFont8;
        private DevExpress.XtraReports.UI.XRControlStyle xrcsFont9;
        private DevExpress.XtraReports.UI.XRControlStyle xrcsFont10;
        private DevExpress.XtraReports.UI.XRControlStyle xrcsFont7;
        private DevExpress.XtraReports.UI.XRControlStyle xrcsFont6;
        private DevExpress.XtraReports.UI.XRControlStyle xrcsFont5;
        private DevExpress.XtraReports.UI.XRControlStyle xrcsFont4;
        private DevExpress.XtraReports.UI.XRControlStyle xrcsLine;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRBarCode xrBarCode1;
    }
}
