using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oybab.Res;
using System.Drawing;

namespace Oybab.ServicePC.Pattern
{
    /// <summary>
    /// 蓝色皮肤
    /// </summary>
    internal sealed class PaletteBlue : PaletteOffice2010Silver
    {
        private static PaletteBlue self;
        internal System.Drawing.Font currentResourceFont = new System.Drawing.Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
        private System.Drawing.Font currentLittleResourceFont = new System.Drawing.Font(Resources.GetRes().GetString("FontName2"), 9f);

        public static PaletteBlue GetSelf()
        {
            if (null == self)
                self = new PaletteBlue();
            return self;
        }



        private string fontName = Resources.GetRes().GetString("FontName2");
        private float fontSize = float.Parse(Resources.GetRes().GetString("FontSize"));
        public override string BaseFontName
        {
            get
            {
                return this.fontName;
            }
            set
            {
                base.BaseFontName = value;
            }
        }

        public override float BaseFontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                base.BaseFontSize = value;
            }
        }


        /// <summary>
        /// 解决在宋体下日历字体太小
        /// </summary>
        /// <param name="style"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public override System.Drawing.Font GetContentShortTextFont(PaletteContentStyle style, PaletteState state)
        {
            if (style == PaletteContentStyle.ButtonCalendarDay)
            {
                if (Resources.GetRes().GetString("LargeFont") != "0")
                    return new System.Drawing.Font("Arial", fontSize);
            }

            return base.GetContentShortTextFont(style, state);
        }


        /// <summary>
        /// 重新加载
        /// </summary>
        internal void Reload()
        {
            currentResourceFont = new System.Drawing.Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            currentLittleResourceFont = new System.Drawing.Font(Resources.GetRes().GetString("FontName2"), 9f);
            fontName = Resources.GetRes().GetString("FontName2");
            fontSize = float.Parse(Resources.GetRes().GetString("FontSize"));

            PaletteBlue.GetSelf().DefineFonts();
        }



        public override System.Windows.Forms.Padding GetContentPadding(PaletteContentStyle style, PaletteState state)
        {
            if (Resources.GetRes().MainLang.Culture.Name != "ug-CN" && Resources.GetRes().MainLang.Culture.Name != "zh-CN")
                return base.GetContentPadding(style, state);

            // 按钮不显示问题
            if ((int)style < 19)
                return new Padding(0, 0, 0, -4);
            // 标题位置显示
            else if ((int)style == 37)
            {
                if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
                    return new Padding(4, 0, 0, -6);
                else
                    return new Padding(4, 0, 0, -8);
            }
            else
                return base.GetContentPadding(style, state);
        }








        #region For DPI




        private SizeF _dpi;
        private SizeF _scaleFactor;

        public SizeF Dpi
        {
            get
            {
                if (_dpi.IsEmpty)
                {
                    using (var g = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        _dpi = new SizeF(g.DpiX, g.DpiY);
                    }
                }
                return _dpi;
            }
        }

        public SizeF ScaleFactor
        {
            get
            {
                if (_scaleFactor.IsEmpty)
                {
                    _scaleFactor = new SizeF(Dpi.Width / 96.0F, Dpi.Height / 96.0F);
                }
                return _scaleFactor;
            }
        }

        public override Image GetButtonSpecImage(PaletteButtonSpecStyle style, PaletteState state)
        {
            return GetScaledImage(base.GetButtonSpecImage(style, state));
        }

        public override Image GetCheckBoxImage(bool enabled, System.Windows.Forms.CheckState checkState, bool tracking, bool pressed)
        {
            return GetScaledImage(base.GetCheckBoxImage(enabled, checkState, tracking, pressed));
        }

        public override Image GetContentShortTextImage(PaletteContentStyle style, PaletteState state)
        {
            return GetScaledImage(base.GetContentShortTextImage(style, state));
        }

        public override Image GetContentLongTextImage(PaletteContentStyle style, PaletteState state)
        {
            return GetScaledImage(base.GetContentLongTextImage(style, state));
        }

        public override Image GetContextMenuCheckedImage()
        {
            return GetScaledImage(base.GetContextMenuCheckedImage());
        }

        public override Image GetContextMenuIndeterminateImage()
        {
            return GetScaledImage(base.GetContextMenuIndeterminateImage());
        }

        public override Image GetContextMenuSubMenuImage()
        {
            return GetScaledImage(base.GetContextMenuSubMenuImage());
        }

        public override Image GetDropDownButtonImage(PaletteState state)
        {
            return GetScaledImage(base.GetDropDownButtonImage(state));
        }

        public override Image GetGalleryButtonImage(PaletteRibbonGalleryButton button, PaletteState state)
        {
            return GetScaledImage(base.GetGalleryButtonImage(button, state));
        }

        public override Image GetRadioButtonImage(bool enabled, bool checkState, bool tracking, bool pressed)
        {
            return GetScaledImage(base.GetRadioButtonImage(enabled, checkState, tracking, pressed));
        }

        public override Image GetTreeViewImage(bool expanded)
        {
            return GetScaledImage(base.GetTreeViewImage(expanded));
        }

        private Image GetScaledImage(Image img)
        {
            if ((img == null) || (ScaleFactor.Width == 1 && ScaleFactor.Height == 1))
                return img;

            Bitmap bmp = new Bitmap((int)(img.Width * ScaleFactor.Width), (int)(img.Height * ScaleFactor.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            using (var tmpBmp = new Bitmap(img))
            {
                tmpBmp.MakeTransparent(Color.Magenta);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.DrawImage(tmpBmp, 0, 0, bmp.Width, bmp.Height);
                }
            }

            return bmp;
        }


        #endregion  For DPI

    }
}
