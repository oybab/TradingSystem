namespace Oybab.Report.StatisticsForm
{
    partial class StatisticSummaryReportWindow
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
            this.documentViewer1 = new DevExpress.XtraPrinting.Preview.DocumentViewer();
            this.krpbPrint = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // documentViewer1
            // 
            this.documentViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.documentViewer1.IsMetric = true;
            this.documentViewer1.Location = new System.Drawing.Point(0, 0);
            this.documentViewer1.Name = "documentViewer1";
            this.documentViewer1.Size = new System.Drawing.Size(886, 563);
            this.documentViewer1.TabIndex = 0;
            // 
            // krpbPrint
            // 
            this.krpbPrint.Location = new System.Drawing.Point(1, 1);
            this.krpbPrint.Name = "krpbPrint";
            this.krpbPrint.Size = new System.Drawing.Size(31, 25);
            this.krpbPrint.TabIndex = 1;
            this.krpbPrint.Values.Text = "P";
            // 
            // StatisticSummaryReportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(886, 563);
            this.Controls.Add(this.krpbPrint);
            this.Controls.Add(this.documentViewer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "StatisticSummaryReportWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StatisticReportWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraPrinting.Preview.DocumentViewer documentViewer1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbPrint;
    }
}