namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class AddMemberWindow
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
            this.krpbAdd = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplMemberNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptMemberNo = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.SuspendLayout();
            // 
            // krpbAdd
            // 
            this.krpbAdd.Location = new System.Drawing.Point(138, 66);
            this.krpbAdd.Name = "krpbAdd";
            this.krpbAdd.Size = new System.Drawing.Size(90, 25);
            this.krpbAdd.TabIndex = 4;
            this.krpbAdd.Values.Text = "Add";
            this.krpbAdd.Visible = false;
            this.krpbAdd.Click += new System.EventHandler(this.krpbAdd_Click);
            // 
            // krplMemberNo
            // 
            this.krplMemberNo.Location = new System.Drawing.Point(21, 24);
            this.krplMemberNo.Name = "krplMemberNo";
            this.krplMemberNo.Size = new System.Drawing.Size(77, 20);
            this.krplMemberNo.TabIndex = 1;
            this.krplMemberNo.Values.Text = "Member No";
            // 
            // krptMemberNo
            // 
            this.krptMemberNo.Location = new System.Drawing.Point(138, 24);
            this.krptMemberNo.MaxLength = 12;
            this.krptMemberNo.Name = "krptMemberNo";
            this.krptMemberNo.Size = new System.Drawing.Size(203, 23);
            this.krptMemberNo.TabIndex = 2;
            this.krptMemberNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptMemberNo_KeyDown);
            // 
            // AddMemberWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(373, 107);
            this.Controls.Add(this.krptMemberNo);
            this.Controls.Add(this.krplMemberNo);
            this.Controls.Add(this.krpbAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddMemberWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AddMemberWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberNo;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptMemberNo;
    }
}