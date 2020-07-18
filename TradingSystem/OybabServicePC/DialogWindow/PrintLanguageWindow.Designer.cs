namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PrintLanguageWindow
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
            this.krpbPrint = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpcLanguage = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krplLanguage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbPrint
            // 
            this.krpbPrint.Location = new System.Drawing.Point(112, 70);
            this.krpbPrint.Name = "krpbPrint";
            this.krpbPrint.Size = new System.Drawing.Size(90, 25);
            this.krpbPrint.TabIndex = 4;
            this.krpbPrint.Values.Text = "Print";
            this.krpbPrint.Click += new System.EventHandler(this.krpbChange_Click);
            // 
            // krpcLanguage
            // 
            this.krpcLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcLanguage.DropDownWidth = 121;
            this.krpcLanguage.Location = new System.Drawing.Point(176, 24);
            this.krpcLanguage.Name = "krpcLanguage";
            this.krpcLanguage.Size = new System.Drawing.Size(121, 21);
            this.krpcLanguage.TabIndex = 1;
            this.krpcLanguage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krpcLanguage_KeyDown);
            // 
            // krplLanguage
            // 
            this.krplLanguage.Location = new System.Drawing.Point(34, 24);
            this.krplLanguage.Name = "krplLanguage";
            this.krplLanguage.Size = new System.Drawing.Size(64, 20);
            this.krplLanguage.TabIndex = 5;
            this.krplLanguage.Values.Text = "Language";
            // 
            // PrintLanguageWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(340, 107);
            this.Controls.Add(this.krpcLanguage);
            this.Controls.Add(this.krplLanguage);
            this.Controls.Add(this.krpbPrint);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintLanguageWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PrintLanguageWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbPrint;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplLanguage;
    }
}