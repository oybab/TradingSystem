namespace Oybab.ServicePC.DialogWindow
{
    partial class TimeChangeWindow
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
            this.krpbSave = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplStartTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplEndTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplChangeTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplStartTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplEndTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplChangeMark = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRemainingTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRemainingTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.krplHour = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptHour = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplMinute = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptMinute = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krpbRemoveTime = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpcbTimeUnlimited = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // krpbSave
            // 
            this.krpbSave.Location = new System.Drawing.Point(168, 222);
            this.krpbSave.Name = "krpbSave";
            this.krpbSave.Size = new System.Drawing.Size(90, 25);
            this.krpbSave.TabIndex = 20;
            this.krpbSave.Values.Text = "Save";
            this.krpbSave.Click += new System.EventHandler(this.krpbSave_Click);
            // 
            // krplStartTime
            // 
            this.krplStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplStartTime.Location = new System.Drawing.Point(75, 27);
            this.krplStartTime.Name = "krplStartTime";
            this.krplStartTime.Size = new System.Drawing.Size(63, 20);
            this.krplStartTime.TabIndex = 1;
            this.krplStartTime.Values.Text = "StartTime";
            // 
            // krplEndTime
            // 
            this.krplEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplEndTime.Location = new System.Drawing.Point(60, 65);
            this.krplEndTime.Name = "krplEndTime";
            this.krplEndTime.Size = new System.Drawing.Size(78, 20);
            this.krplEndTime.TabIndex = 1;
            this.krplEndTime.Values.Text = "krplEndTime";
            // 
            // krplChangeTime
            // 
            this.krplChangeTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplChangeTime.Location = new System.Drawing.Point(59, 102);
            this.krplChangeTime.Name = "krplChangeTime";
            this.krplChangeTime.Size = new System.Drawing.Size(79, 20);
            this.krplChangeTime.TabIndex = 1;
            this.krplChangeTime.Values.Text = "ChangeTime";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(177, 27);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = ":";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(177, 65);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = ":";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(177, 102);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel3.TabIndex = 1;
            this.kryptonLabel3.Values.Text = ":";
            // 
            // krplStartTimeValue
            // 
            this.krplStartTimeValue.Location = new System.Drawing.Point(223, 27);
            this.krplStartTimeValue.Name = "krplStartTimeValue";
            this.krplStartTimeValue.Size = new System.Drawing.Size(93, 20);
            this.krplStartTimeValue.TabIndex = 1;
            this.krplStartTimeValue.Values.Text = "????-??-?? ??:??";
            // 
            // krplEndTimeValue
            // 
            this.krplEndTimeValue.Location = new System.Drawing.Point(223, 65);
            this.krplEndTimeValue.Name = "krplEndTimeValue";
            this.krplEndTimeValue.Size = new System.Drawing.Size(93, 20);
            this.krplEndTimeValue.TabIndex = 1;
            this.krplEndTimeValue.Values.Text = "????-??-?? ??:??";
            // 
            // krplTotalTime
            // 
            this.krplTotalTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalTime.Location = new System.Drawing.Point(74, 146);
            this.krplTotalTime.Name = "krplTotalTime";
            this.krplTotalTime.Size = new System.Drawing.Size(64, 20);
            this.krplTotalTime.TabIndex = 1;
            this.krplTotalTime.Values.Text = "TotalTime";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(177, 146);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel4.TabIndex = 1;
            this.kryptonLabel4.Values.Text = ":";
            // 
            // krplTotalTimeValue
            // 
            this.krplTotalTimeValue.Location = new System.Drawing.Point(223, 146);
            this.krplTotalTimeValue.Name = "krplTotalTimeValue";
            this.krplTotalTimeValue.Size = new System.Drawing.Size(35, 20);
            this.krplTotalTimeValue.TabIndex = 1;
            this.krplTotalTimeValue.Values.Text = "??:??";
            // 
            // krplChangeMark
            // 
            this.krplChangeMark.Location = new System.Drawing.Point(196, 102);
            this.krplChangeMark.Name = "krplChangeMark";
            this.krplChangeMark.Size = new System.Drawing.Size(19, 20);
            this.krplChangeMark.TabIndex = 1;
            this.krplChangeMark.Values.Text = "+";
            // 
            // krplRemainingTimeValue
            // 
            this.krplRemainingTimeValue.Location = new System.Drawing.Point(223, 182);
            this.krplRemainingTimeValue.Name = "krplRemainingTimeValue";
            this.krplRemainingTimeValue.Size = new System.Drawing.Size(35, 20);
            this.krplRemainingTimeValue.TabIndex = 1;
            this.krplRemainingTimeValue.Values.Text = "??:??";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(177, 182);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel6.TabIndex = 1;
            this.kryptonLabel6.Values.Text = ":";
            // 
            // krplRemainingTime
            // 
            this.krplRemainingTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRemainingTime.Location = new System.Drawing.Point(43, 182);
            this.krplRemainingTime.Name = "krplRemainingTime";
            this.krplRemainingTime.Size = new System.Drawing.Size(95, 20);
            this.krplRemainingTime.TabIndex = 1;
            this.krplRemainingTime.Values.Text = "RemainingTime";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.krplHour);
            this.flowLayoutPanel1.Controls.Add(this.krptHour);
            this.flowLayoutPanel1.Controls.Add(this.krplMinute);
            this.flowLayoutPanel1.Controls.Add(this.krptMinute);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(223, 99);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(227, 31);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // krplHour
            // 
            this.krplHour.Location = new System.Drawing.Point(3, 3);
            this.krplHour.Name = "krplHour";
            this.krplHour.Size = new System.Drawing.Size(38, 20);
            this.krplHour.TabIndex = 6;
            this.krplHour.Values.Text = "Hour";
            // 
            // krptHour
            // 
            this.krptHour.Location = new System.Drawing.Point(47, 3);
            this.krptHour.MaxLength = 2;
            this.krptHour.Name = "krptHour";
            this.krptHour.Size = new System.Drawing.Size(31, 23);
            this.krptHour.TabIndex = 9;
            this.krptHour.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            this.krptHour.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptHour_KeyUp);
            // 
            // krplMinute
            // 
            this.krplMinute.Location = new System.Drawing.Point(84, 3);
            this.krplMinute.Name = "krplMinute";
            this.krplMinute.Size = new System.Drawing.Size(49, 20);
            this.krplMinute.TabIndex = 5;
            this.krplMinute.Values.Text = "Minute";
            // 
            // krptMinute
            // 
            this.krptMinute.Location = new System.Drawing.Point(139, 3);
            this.krptMinute.MaxLength = 2;
            this.krptMinute.Name = "krptMinute";
            this.krptMinute.Size = new System.Drawing.Size(31, 23);
            this.krptMinute.TabIndex = 10;
            this.krptMinute.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            this.krptMinute.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptMinute_KeyUp);
            // 
            // krpbRemoveTime
            // 
            this.krpbRemoveTime.Location = new System.Drawing.Point(397, 181);
            this.krpbRemoveTime.Name = "krpbRemoveTime";
            this.krpbRemoveTime.Size = new System.Drawing.Size(21, 25);
            this.krpbRemoveTime.TabIndex = 20;
            this.krpbRemoveTime.Values.Text = "-";
            this.krpbRemoveTime.Visible = false;
            this.krpbRemoveTime.Click += new System.EventHandler(this.krpbRemoveTime_Click);
            // 
            // krpcbTimeUnlimited
            // 
            this.krpcbTimeUnlimited.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbTimeUnlimited.Location = new System.Drawing.Point(43, 227);
            this.krpcbTimeUnlimited.Name = "krpcbTimeUnlimited";
            this.krpcbTimeUnlimited.Size = new System.Drawing.Size(104, 20);
            this.krpcbTimeUnlimited.TabIndex = 21;
            this.krpcbTimeUnlimited.Text = "Unlimited time";
            this.krpcbTimeUnlimited.Values.Text = "Unlimited time";
            this.krpcbTimeUnlimited.Visible = false;
            // 
            // TimeChangeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(430, 259);
            this.Controls.Add(this.krpcbTimeUnlimited);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.krplRemainingTime);
            this.Controls.Add(this.krplTotalTime);
            this.Controls.Add(this.krplChangeTime);
            this.Controls.Add(this.kryptonLabel6);
            this.Controls.Add(this.krplEndTime);
            this.Controls.Add(this.kryptonLabel4);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.krplRemainingTimeValue);
            this.Controls.Add(this.krplTotalTimeValue);
            this.Controls.Add(this.krplChangeMark);
            this.Controls.Add(this.krplEndTimeValue);
            this.Controls.Add(this.krplStartTimeValue);
            this.Controls.Add(this.krplStartTime);
            this.Controls.Add(this.krpbRemoveTime);
            this.Controls.Add(this.krpbSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TimeChangeWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TimeChangeWindow";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

      


        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSave;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplStartTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplChangeTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplStartTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplChangeMark;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRemainingTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRemainingTime;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplHour;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptHour;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMinute;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptMinute;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbRemoveTime;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbTimeUnlimited;
    }
}