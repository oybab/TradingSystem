namespace Oybab.ServicePC.DialogWindow
{
    partial class VodScrollWindow
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtZh = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.txtUg = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.txtEn = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.txtId = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.label2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.label3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.label4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnOutput = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnImport = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.txtInterval = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnUpdate = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.cbIsRestart = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.cbRemove = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.btnDownload = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // txtZh
            // 
            this.txtZh.Location = new System.Drawing.Point(307, 80);
            this.txtZh.Multiline = true;
            this.txtZh.Name = "txtZh";
            this.txtZh.Size = new System.Drawing.Size(542, 101);
            this.txtZh.TabIndex = 3;
            // 
            // txtUg
            // 
            this.txtUg.Font = new System.Drawing.Font("Oybab Tuz", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUg.Location = new System.Drawing.Point(307, 210);
            this.txtUg.Multiline = true;
            this.txtUg.Name = "txtUg";
            this.txtUg.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtUg.Size = new System.Drawing.Size(542, 101);
            this.txtUg.TabIndex = 4;
            // 
            // txtEn
            // 
            this.txtEn.Location = new System.Drawing.Point(307, 341);
            this.txtEn.Multiline = true;
            this.txtEn.Name = "txtEn";
            this.txtEn.Size = new System.Drawing.Size(542, 101);
            this.txtEn.TabIndex = 5;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(307, 26);
            this.txtId.MaxLength = 16;
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(246, 23);
            this.txtId.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(114, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 20);
            this.label1.TabIndex = 1;
            this.label1.Values.Text = "ID：";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(114, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 20);
            this.label2.TabIndex = 1;
            this.label2.Values.Text = "汉语：";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(114, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 20);
            this.label3.TabIndex = 1;
            this.label3.Values.Text = "维语：";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(114, 344);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 20);
            this.label4.TabIndex = 1;
            this.label4.Values.Text = "英文：";
            // 
            // btnOutput
            // 
            this.btnOutput.Location = new System.Drawing.Point(352, 477);
            this.btnOutput.Name = "btnOutput";
            this.btnOutput.Size = new System.Drawing.Size(75, 25);
            this.btnOutput.TabIndex = 11;
            this.btnOutput.Values.Text = "导出";
            this.btnOutput.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(157, 477);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 25);
            this.btnImport.TabIndex = 10;
            this.btnImport.Values.Text = "导入";
            this.btnImport.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(779, 29);
            this.txtInterval.MaxLength = 3;
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(70, 23);
            this.txtInterval.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(650, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 20);
            this.label5.TabIndex = 1;
            this.label5.Values.Text = "间隔（分钟）：";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(693, 477);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 25);
            this.btnUpdate.TabIndex = 15;
            this.btnUpdate.Values.Text = "上传";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // cbIsRestart
            // 
            this.cbIsRestart.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.cbIsRestart.Location = new System.Drawing.Point(621, 482);
            this.cbIsRestart.Name = "cbIsRestart";
            this.cbIsRestart.Size = new System.Drawing.Size(49, 20);
            this.cbIsRestart.TabIndex = 13;
            this.cbIsRestart.Text = "重启";
            this.cbIsRestart.Values.Text = "重启";
            // 
            // cbRemove
            // 
            this.cbRemove.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.cbRemove.Location = new System.Drawing.Point(562, 482);
            this.cbRemove.Name = "cbRemove";
            this.cbRemove.Size = new System.Drawing.Size(49, 20);
            this.cbRemove.TabIndex = 12;
            this.cbRemove.Text = "清除";
            this.cbRemove.Values.Text = "清除";
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(774, 477);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 25);
            this.btnDownload.TabIndex = 15;
            this.btnDownload.Values.Text = "下载";
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // VodScrollWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(913, 525);
            this.Controls.Add(this.cbRemove);
            this.Controls.Add(this.cbIsRestart);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnOutput);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEn);
            this.Controls.Add(this.txtUg);
            this.Controls.Add(this.txtInterval);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.txtZh);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VodScrollWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vod Scroll";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtZh;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtUg;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtEn;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtId;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label4;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOutput;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnImport;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtInterval;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label5;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnUpdate;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox cbIsRestart;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox cbRemove;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnDownload;
    }
}

