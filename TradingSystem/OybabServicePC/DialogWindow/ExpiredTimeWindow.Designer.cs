namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class ExpiredTimeWindow
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
            this.krpbChange = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplExpiredTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptbExpired = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.krplNoLimit = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbIsNoLimit = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.SuspendLayout();
            // 
            // krpbChange
            // 
            this.krpbChange.Location = new System.Drawing.Point(107, 103);
            this.krpbChange.Name = "krpbChange";
            this.krpbChange.Size = new System.Drawing.Size(90, 25);
            this.krpbChange.TabIndex = 4;
            this.krpbChange.Values.Text = "Change";
            this.krpbChange.Click += new System.EventHandler(this.krpbChange_Click);
            // 
            // krplExpiredTime
            // 
            this.krplExpiredTime.Location = new System.Drawing.Point(21, 59);
            this.krplExpiredTime.Name = "krplExpiredTime";
            this.krplExpiredTime.Size = new System.Drawing.Size(81, 20);
            this.krplExpiredTime.TabIndex = 1;
            this.krplExpiredTime.Values.Text = "Expired Time";
            // 
            // krptbExpired
            // 
            this.krptbExpired.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krptbExpired.CalendarShowToday = false;
            this.krptbExpired.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.krptbExpired.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.krptbExpired.Location = new System.Drawing.Point(146, 58);
            this.krptbExpired.Name = "krptbExpired";
            this.krptbExpired.Size = new System.Drawing.Size(170, 21);
            this.krptbExpired.TabIndex = 6;
            this.krptbExpired.ValueNullable = new System.DateTime(((long)(0)));
            // 
            // krplNoLimit
            // 
            this.krplNoLimit.Location = new System.Drawing.Point(21, 19);
            this.krplNoLimit.Name = "krplNoLimit";
            this.krplNoLimit.Size = new System.Drawing.Size(57, 20);
            this.krplNoLimit.TabIndex = 1;
            this.krplNoLimit.Values.Text = "No Limit";
            // 
            // krpcbIsNoLimit
            // 
            this.krpcbIsNoLimit.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbIsNoLimit.Location = new System.Drawing.Point(146, 19);
            this.krpcbIsNoLimit.Name = "krpcbIsNoLimit";
            this.krpcbIsNoLimit.Size = new System.Drawing.Size(42, 20);
            this.krpcbIsNoLimit.TabIndex = 7;
            this.krpcbIsNoLimit.Text = "Yes";
            this.krpcbIsNoLimit.Values.Text = "Yes";
            this.krpcbIsNoLimit.CheckedChanged += new System.EventHandler(this.krpcbIsNoLimit_CheckedChanged);
            // 
            // ExpiredTimeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(340, 140);
            this.Controls.Add(this.krpcbIsNoLimit);
            this.Controls.Add(this.krptbExpired);
            this.Controls.Add(this.krplNoLimit);
            this.Controls.Add(this.krplExpiredTime);
            this.Controls.Add(this.krpbChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExpiredTimeWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ExpiredTimeWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbChange;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplExpiredTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker krptbExpired;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplNoLimit;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbIsNoLimit;
    }
}