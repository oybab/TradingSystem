using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;


/// <summary>
/// 原工具来自: https://www.codeproject.com/Articles/10688/BevelLine-Control-with-Designer-Selection-Rules
/// </summary>
namespace Oybab.ServicePC.Tools
{
	[Designer(typeof(Oybab.ServicePC.Tools.Design.BevelLineDesigner))]
    internal sealed class BevelLine : System.Windows.Forms.Control
	{
		#region Events
		public event EventHandler OrientationChanged;
		#endregion

		#region Private Properties
		private int bevelLineWidth;
		private Color topLineColor;
		private Color bottomLineColor;
		private bool blend;
		private int angle;
		private Orientation orientation;
		#endregion

		#region Constructors
        public BevelLine()
		{
			this.SetStyle(ControlStyles.UserPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.AllPaintingInWmPaint, true);

			bevelLineWidth = 1;
			topLineColor = SystemColors.ControlDark;
			bottomLineColor = SystemColors.ControlLightLight;
			orientation = Orientation.Horizontal;
			blend = false;
			angle = 90;
		}

		#endregion

		#region Public Properties
		[Description("The width of each line."), DefaultValue(1)]
		public int BevelLineWidth
		{
			get
			{
				return bevelLineWidth;
			}
			set
			{
				bevelLineWidth = value;
				OnResize(null);
			}
		}

		[Description(""), DefaultValue(typeof(Color), "ControlDark")]
		public Color TopLineColor
		{
			get
			{
				return topLineColor;
			}
			set
			{
				topLineColor = value;
				this.Invalidate();
			}
		}
		[Description(""), DefaultValue(typeof(Color), "ControlLightLight")]
		public Color BottomLineColor
		{
			get
			{
				return bottomLineColor;
			}
			set
			{
				bottomLineColor = value;
				this.Invalidate();
			}
		}

		[Description(""), DefaultValue(Orientation.Horizontal)]
		public System.Windows.Forms.Orientation Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				orientation = value;
				if (orientation == Orientation.Horizontal)
				{
					this.Width = this.Height;
					Angle = 90;
				}
				else
				{
					this.Height = this.Width;
					Angle = 0;
				}

				OnResize(null);

				if (OrientationChanged != null)
				{
					OrientationChanged(this, new EventArgs());
				}
			}
		}

		[Description("If true then the two colors will be blended together."), DefaultValue(false)]
		public bool Blend
		{
			get
			{
				return blend;
			}
			set
			{
				blend = value;
				this.Invalidate();
			}
		}

		[Description(""), Browsable(false)]
		public int Angle
		{
			get
			{
				return angle;
			}
			set
			{
				angle = value;
				this.Invalidate();

			}
		}

		#endregion

		#region Overriden Functions / Events
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			SolidBrush topBrush = new SolidBrush(topLineColor);
			SolidBrush bottomBrush = new SolidBrush(bottomLineColor);

			Rectangle blendRect;
			Rectangle topRect;
			Rectangle bottomRect;

			if (orientation == Orientation.Horizontal)
			{
				if (blend)
				{
					blendRect = new Rectangle(0, 0, this.Width, this.Height);
					g.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(blendRect, topLineColor, bottomLineColor, angle, false), blendRect);
				}
				else
				{
					topRect = new Rectangle(0, 0, this.Width, bevelLineWidth);
					bottomRect = new Rectangle(0, bevelLineWidth, this.Width, bevelLineWidth * 2);
					g.FillRectangle(topBrush, topRect);
					g.FillRectangle(bottomBrush, bottomRect);
				}
			}
			else
			{
				if (blend)
				{
					blendRect = new Rectangle(0, 0, this.Width, this.Height);
					g.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(blendRect, topLineColor, bottomLineColor, angle, false), blendRect);
				}
				else
				{
					topRect = new Rectangle(0, 0, bevelLineWidth, this.Height);
					bottomRect = new Rectangle(bevelLineWidth, 0, bevelLineWidth * 2, this.Height);
					g.FillRectangle(topBrush, topRect);
					g.FillRectangle(bottomBrush, bottomRect);
				}
			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			//base.OnPaintBackground (pevent);
		}
		protected override void OnResize(EventArgs e)
		{
			if (orientation == Orientation.Horizontal)
			{
				this.Height = bevelLineWidth * 2;
			}
			else
			{
				this.Width = bevelLineWidth * 2;
			}
			this.Invalidate();
		}

		#endregion

		#region Hidden Properties / Events
		[Browsable(false)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		[Browsable(false)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}
		[Browsable(false)]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}

		[Browsable(false)]
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		[Browsable(false)]
		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
			}
		}

		#endregion

	}
}
