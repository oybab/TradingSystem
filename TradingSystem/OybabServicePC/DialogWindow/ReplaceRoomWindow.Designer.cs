namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class ReplaceRoomWindow
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
            this.krplRoomNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomNoValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplNewRoom = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbNewRoom = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbNewRoom)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbSave
            // 
            this.krpbSave.Location = new System.Drawing.Point(87, 145);
            this.krpbSave.Name = "krpbSave";
            this.krpbSave.Size = new System.Drawing.Size(90, 25);
            this.krpbSave.TabIndex = 4;
            this.krpbSave.Values.Text = "Save";
            this.krpbSave.Click += new System.EventHandler(this.krpbSave_Click);
            // 
            // krplRoomNo
            // 
            this.krplRoomNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRoomNo.Location = new System.Drawing.Point(43, 32);
            this.krplRoomNo.Name = "krplRoomNo";
            this.krplRoomNo.Size = new System.Drawing.Size(60, 20);
            this.krplRoomNo.TabIndex = 5;
            this.krplRoomNo.Values.Text = "RoomNo";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(113, 32);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel3.TabIndex = 6;
            this.kryptonLabel3.Values.Text = ":";
            // 
            // krplRoomNoValue
            // 
            this.krplRoomNoValue.Location = new System.Drawing.Point(149, 32);
            this.krplRoomNoValue.Name = "krplRoomNoValue";
            this.krplRoomNoValue.Size = new System.Drawing.Size(37, 20);
            this.krplRoomNoValue.TabIndex = 7;
            this.krplRoomNoValue.Values.Text = "0000";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(113, 96);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel2.TabIndex = 6;
            this.kryptonLabel2.Values.Text = ":";
            // 
            // krplNewRoom
            // 
            this.krplNewRoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplNewRoom.Location = new System.Drawing.Point(35, 96);
            this.krplNewRoom.Name = "krplNewRoom";
            this.krplNewRoom.Size = new System.Drawing.Size(68, 20);
            this.krplNewRoom.TabIndex = 5;
            this.krplNewRoom.Values.Text = "NewRoom";
            // 
            // krpcbNewRoom
            // 
            this.krpcbNewRoom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbNewRoom.DropDownWidth = 149;
            this.krpcbNewRoom.Location = new System.Drawing.Point(149, 96);
            this.krpcbNewRoom.Name = "krpcbNewRoom";
            this.krpcbNewRoom.Size = new System.Drawing.Size(114, 21);
            this.krpcbNewRoom.TabIndex = 12;
            this.krpcbNewRoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krpcbNewRoom_KeyDown);
            // 
            // ReplaceRoomWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(290, 182);
            this.Controls.Add(this.krpcbNewRoom);
            this.Controls.Add(this.krplNewRoom);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.krplRoomNo);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.krplRoomNoValue);
            this.Controls.Add(this.krpbSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReplaceRoomWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ReplaceRoomWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpcbNewRoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSave;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomNoValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplNewRoom;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbNewRoom;
    }
}