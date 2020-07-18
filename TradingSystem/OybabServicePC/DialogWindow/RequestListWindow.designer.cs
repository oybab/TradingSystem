namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class RequestListWindow
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
            this.krptBtnSave = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krptlbSinger1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptlbSinger2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptlbSinger3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptlbSinger4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptlbSinger5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptcbList5 = new Oybab.ServicePC.Tools.ComTextBox();
            this.krptcbList4 = new Oybab.ServicePC.Tools.ComTextBox();
            this.krptcbList3 = new Oybab.ServicePC.Tools.ComTextBox();
            this.krptcbList2 = new Oybab.ServicePC.Tools.ComTextBox();
            this.krptcbList1 = new Oybab.ServicePC.Tools.ComTextBox();
            this.SuspendLayout();
            // 
            // krptBtnSave
            // 
            this.krptBtnSave.Location = new System.Drawing.Point(96, 158);
            this.krptBtnSave.Name = "krptBtnSave";
            this.krptBtnSave.Size = new System.Drawing.Size(90, 25);
            this.krptBtnSave.TabIndex = 0;
            this.krptBtnSave.Values.Text = "Save";
            this.krptBtnSave.Click += new System.EventHandler(this.krptBtnSave_Click);
            // 
            // krptlbSinger1
            // 
            this.krptlbSinger1.Location = new System.Drawing.Point(42, 24);
            this.krptlbSinger1.Name = "krptlbSinger1";
            this.krptlbSinger1.Size = new System.Drawing.Size(17, 20);
            this.krptlbSinger1.TabIndex = 2;
            this.krptlbSinger1.Values.Text = "1";
            // 
            // krptlbSinger2
            // 
            this.krptlbSinger2.Location = new System.Drawing.Point(42, 63);
            this.krptlbSinger2.Name = "krptlbSinger2";
            this.krptlbSinger2.Size = new System.Drawing.Size(17, 20);
            this.krptlbSinger2.TabIndex = 2;
            this.krptlbSinger2.Values.Text = "2";
            // 
            // krptlbSinger3
            // 
            this.krptlbSinger3.Location = new System.Drawing.Point(42, 102);
            this.krptlbSinger3.Name = "krptlbSinger3";
            this.krptlbSinger3.Size = new System.Drawing.Size(17, 20);
            this.krptlbSinger3.TabIndex = 2;
            this.krptlbSinger3.Values.Text = "3";
            // 
            // krptlbSinger4
            // 
            this.krptlbSinger4.Location = new System.Drawing.Point(42, 141);
            this.krptlbSinger4.Name = "krptlbSinger4";
            this.krptlbSinger4.Size = new System.Drawing.Size(17, 20);
            this.krptlbSinger4.TabIndex = 2;
            this.krptlbSinger4.Values.Text = "4";
            this.krptlbSinger4.Visible = false;
            // 
            // krptlbSinger5
            // 
            this.krptlbSinger5.Location = new System.Drawing.Point(42, 180);
            this.krptlbSinger5.Name = "krptlbSinger5";
            this.krptlbSinger5.Size = new System.Drawing.Size(17, 20);
            this.krptlbSinger5.TabIndex = 2;
            this.krptlbSinger5.Values.Text = "5";
            this.krptlbSinger5.Visible = false;
            // 
            // krptcbList5
            // 
            this.krptcbList5.Location = new System.Drawing.Point(85, 180);
            this.krptcbList5.Name = "krptcbList5";
            this.krptcbList5.Size = new System.Drawing.Size(158, 20);
            this.krptcbList5.TabIndex = 5;
            this.krptcbList5.Visible = false;
            // 
            // krptcbList4
            // 
            this.krptcbList4.Location = new System.Drawing.Point(85, 141);
            this.krptcbList4.Name = "krptcbList4";
            this.krptcbList4.Size = new System.Drawing.Size(158, 20);
            this.krptcbList4.TabIndex = 4;
            this.krptcbList4.Visible = false;
            // 
            // krptcbList3
            // 
            this.krptcbList3.Location = new System.Drawing.Point(85, 102);
            this.krptcbList3.Name = "krptcbList3";
            this.krptcbList3.Size = new System.Drawing.Size(158, 20);
            this.krptcbList3.TabIndex = 3;
            // 
            // krptcbList2
            // 
            this.krptcbList2.Location = new System.Drawing.Point(85, 63);
            this.krptcbList2.Name = "krptcbList2";
            this.krptcbList2.Size = new System.Drawing.Size(158, 20);
            this.krptcbList2.TabIndex = 2;
            // 
            // krptcbList1
            // 
            this.krptcbList1.Location = new System.Drawing.Point(85, 24);
            this.krptcbList1.Name = "krptcbList1";
            this.krptcbList1.Size = new System.Drawing.Size(158, 20);
            this.krptcbList1.TabIndex = 1;
            // 
            // RequestListWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(284, 195);
            this.Controls.Add(this.krptlbSinger5);
            this.Controls.Add(this.krptlbSinger4);
            this.Controls.Add(this.krptlbSinger3);
            this.Controls.Add(this.krptlbSinger2);
            this.Controls.Add(this.krptlbSinger1);
            this.Controls.Add(this.krptBtnSave);
            this.Controls.Add(this.krptcbList5);
            this.Controls.Add(this.krptcbList4);
            this.Controls.Add(this.krptcbList3);
            this.Controls.Add(this.krptcbList2);
            this.Controls.Add(this.krptcbList1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RequestListWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RequestListWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Oybab.ServicePC.Tools.ComTextBox krptcbList1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krptBtnSave;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krptlbSinger1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krptlbSinger2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krptlbSinger3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krptlbSinger4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krptlbSinger5;
        private Oybab.ServicePC.Tools.ComTextBox krptcbList2;
        private Oybab.ServicePC.Tools.ComTextBox krptcbList3;
        private Oybab.ServicePC.Tools.ComTextBox krptcbList4;
        private Oybab.ServicePC.Tools.ComTextBox krptcbList5;
    }
}