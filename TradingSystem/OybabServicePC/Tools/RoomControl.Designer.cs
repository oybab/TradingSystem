namespace Oybab.ServicePC.Tools
{
    internal sealed partial class RoomControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.krplLabel = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.pbCall = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCall)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.krplLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 106);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(128, 22);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.RoomControl_DragDrop);
            this.tableLayoutPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.RoomControl_DragEnter);
            this.tableLayoutPanel1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseDoubleClick);
            this.tableLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseDown);
            this.tableLayoutPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseMove);
            this.tableLayoutPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseUp);
            // 
            // krplLabel
            // 
            this.krplLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.krplLabel.Location = new System.Drawing.Point(3, 3);
            this.krplLabel.Name = "krplLabel";
            this.krplLabel.Size = new System.Drawing.Size(122, 16);
            this.krplLabel.StateCommon.DrawFocus = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            this.krplLabel.StateCommon.Padding = new System.Windows.Forms.Padding(-2);
            this.krplLabel.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.krplLabel.TabIndex = 4;
            this.krplLabel.Values.Text = "0";
            this.krplLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.RoomControl_DragDrop);
            this.krplLabel.DragEnter += new System.Windows.Forms.DragEventHandler(this.RoomControl_DragEnter);
            this.krplLabel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RoomControl_KeyDown);
            this.krplLabel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RoomControl_KeyUp);
            this.krplLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseDoubleClick);
            this.krplLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseDown);
            this.krplLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseMove);
            this.krplLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseUp);
            // 
            // pbCall
            // 
            this.pbCall.BackColor = System.Drawing.Color.Transparent;
            this.pbCall.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbCall.Location = new System.Drawing.Point(3, 1);
            this.pbCall.Name = "pbCall";
            this.pbCall.Size = new System.Drawing.Size(24, 24);
            this.pbCall.TabIndex = 1;
            this.pbCall.TabStop = false;
            // 
            // RoomControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.pbCall);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(20);
            this.Name = "RoomControl";
            this.Size = new System.Drawing.Size(128, 128);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.RoomControl_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.RoomControl_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RoomControl_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RoomControl_KeyUp);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RoomControl_MouseUp);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbCall)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplLabel;
        private System.Windows.Forms.PictureBox pbCall;
    }
}
