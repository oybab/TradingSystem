using DevExpress.XtraPrinting.Native;
using Oybab.Report.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oybab.Report
{


    public static class FastDrawingExtensions
    {
        public static IntPtr ToHfontFast(this Font font, float dpi)
        {
            return Gdi32.CreateFont(-(int)Math.Floor(font.GetHeightFast(dpi) + 0.5f), 0, 0, 0,
                font.Bold ? Gdi32.FontWeight.Bold : Gdi32.FontWeight.Normal,
                font.Italic ? 1u : 0u,
                font.Underline ? 1u : 0u,
                font.Strikeout ? 1u : 0u,
                (Gdi32.FontCharSet)font.GdiCharSet,
                Gdi32.FontPrecision.Default,
                Gdi32.FontClipPrecision.CLIP_DEFAULT_PRECIS,
                Gdi32.FontQuality.Default,
                Gdi32.FontPitchAndFamily.FamilyDontCare,
                font.Name);
        }

        public static float GetHeightFast(this Font font, float dpi)
        {
            switch (font.Unit)
            {
                case GraphicsUnit.Display:
                case GraphicsUnit.Pixel:
                    return font.Size;
                case GraphicsUnit.Point:
                    return font.Size * dpi / 72f;
                case GraphicsUnit.Document:
                    return font.Size * dpi / 300f;
                case GraphicsUnit.Inch:
                    return font.Size * dpi;
                case GraphicsUnit.Millimeter:
                    return font.Size * dpi / 25.4f;
                default:
                    throw new InvalidOperationException(font.Unit + " is an invalid value for Font.Unit.");
            }
        }
    }
    public sealed class GdiPlusFixedTextRenderingGraphicsModifier : GdiPlusGraphicsModifier
    {
        private readonly GdiTextRenderer textRenderer = new GdiTextRenderer();

        public override void DrawString(Graphics gr, string s, Font font, Brush brush, RectangleF bounds, StringFormat format)
        {
            switch (format.Trimming)
            {
                case StringTrimming.Character:
                    format.Trimming = StringTrimming.None;
                    break;
                case StringTrimming.Word:
                    format.Trimming = StringTrimming.EllipsisWord;
                    break;
            }

            format.FormatFlags &= ~(StringFormatFlags.NoClip | StringFormatFlags.LineLimit);

            textRenderer.DrawStringFallback(gr, s, font, brush, bounds, format);
        }
    }

    internal sealed class GdiTextRenderer : IDisposable
    {
        /// <summary>
        /// Draws the string using GDI's DrawText for high fidelity ClearType if possible, otherwise falls back to Graphics.DrawString.
        /// </summary>
        public void DrawStringFallback(Graphics graphics, string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
        {
            var gdiFlags = TryParseGdiFlags(format);
            if (gdiFlags != null)
            {
                var solidBrush = brush as SolidBrush;
                if (solidBrush != null && solidBrush.Color.A == 255)
                {
                    var transform = GetAspectScaleAndTranslation(graphics);
                    if (transform != null)
                    {
                        var translatedBounds = transform.Value.Apply(layoutRectangle);
                        var nearestPixel = new Rectangle((int)Math.Floor(translatedBounds.X + .5f), (int)Math.Floor(translatedBounds.Y + .5f), (int)Math.Ceiling(translatedBounds.Width), (int)Math.Ceiling(translatedBounds.Height));
                        DrawText(graphics, s, font, transform.Value.FontScale, nearestPixel, solidBrush.Color, gdiFlags.Value);
                        return;
                    }
                }
            }

            graphics.DrawString(s, font, brush, layoutRectangle, format);
        }

        private static TextFormatFlags? TryParseGdiFlags(StringFormat stringFormat)
        {
            if (stringFormat == null) return TextFormatFlags.Default;

            const StringFormatFlags unsupportedFlags =
                StringFormatFlags.DirectionVertical |
                StringFormatFlags.DisplayFormatControl |
                StringFormatFlags.NoFontFallback;

            if ((stringFormat.FormatFlags & unsupportedFlags) != 0) return null;
            if ((stringFormat.FormatFlags & StringFormatFlags.LineLimit) != 0 && (stringFormat.FormatFlags & StringFormatFlags.NoClip) == 0) return null;
            
            var r = (TextFormatFlags)0;
            if ((stringFormat.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0) r |= TextFormatFlags.RightToLeft;
            if ((stringFormat.FormatFlags & StringFormatFlags.FitBlackBox) != 0) r |= TextFormatFlags.NoPadding;
            if ((stringFormat.FormatFlags & StringFormatFlags.NoClip) != 0) r |= TextFormatFlags.NoClipping;
            if ((stringFormat.FormatFlags & StringFormatFlags.NoWrap) == 0) r |= TextFormatFlags.WordBreak | TextFormatFlags.NoClipping | TextFormatFlags.TextBoxControl;

            switch (stringFormat.Alignment)
            {
                case StringAlignment.Center:
                    r |= TextFormatFlags.HorizontalCenter;
                    break;
                case StringAlignment.Far:
                    r |= TextFormatFlags.Right;
                    break;
            }

            switch (stringFormat.LineAlignment)
            {
                case StringAlignment.Center:
                    r |= TextFormatFlags.VerticalCenter;
                    break;
                case StringAlignment.Far:
                    r |= TextFormatFlags.Bottom;
                    break;
            }

            switch (stringFormat.HotkeyPrefix)
            {
                case HotkeyPrefix.None:
                    r |= TextFormatFlags.NoPrefix;
                    break;
                case HotkeyPrefix.Hide:
                    r |= TextFormatFlags.HidePrefix;
                    break;
            }

            switch (stringFormat.Trimming)
            {
                case StringTrimming.Character:
                case StringTrimming.Word:
                    return null;
                case StringTrimming.EllipsisCharacter:
                    r |= TextFormatFlags.EndEllipsis;
                    break;
                case StringTrimming.EllipsisPath:
                    r |= TextFormatFlags.PathEllipsis;
                    break;
                case StringTrimming.EllipsisWord:
                    r |= TextFormatFlags.WordEllipsis;
                    break;
            }

            return r;
        }


        public struct AspectScaleAndTranslation
        {
            public float Scale { get; private set; }
            public float FontScale { get; private set; }
            public float TranslateX { get; private set; }
            public float TranslateY { get; private set; }

            public AspectScaleAndTranslation(float scale, float fontScale, float translateX, float translateY)
                : this()
            {
                this.Scale = scale;
                this.TranslateX = translateX;
                this.TranslateY = translateY;
                this.FontScale = fontScale;
            }

            public RectangleF Apply(RectangleF rectangle)
            {
                return new RectangleF(rectangle.X * this.Scale + this.TranslateX, rectangle.Y * this.Scale + this.TranslateY, rectangle.Width * this.Scale, rectangle.Height * this.Scale);
            }

            public Font CreateScaledFont(Font font)
            {
                return new Font(font.FontFamily, font.SizeInPoints * this.FontScale, font.Style);
            }
        }

        public static AspectScaleAndTranslation? GetAspectScaleAndTranslation(Graphics graphics)
        {
            const int newX_xFactor = 0, newY_xFactor = 1, newX_yFactor = 2, newY_yFactor = 3, newX_const = 4, newY_const = 5;
            var matrix = graphics.Transform.Elements;
            if (matrix[newY_xFactor] != 0 || matrix[newX_yFactor] != 0) return null; // Rotated or sheared

            var fontScale = matrix[newX_xFactor];
            if (fontScale < 0) return null; // Scale is negative
            if (fontScale != matrix[newY_yFactor]) return null; // X and Y do not have the same scale

            var unitScale = graphics.PageScale;
            switch (graphics.PageUnit)
            {
                case GraphicsUnit.Document:
                    unitScale *= graphics.DpiY / 300f;
                    break;
                case GraphicsUnit.Inch:
                    unitScale *= graphics.DpiY;
                    break;
                case GraphicsUnit.Point:
                    unitScale *= graphics.DpiY / 72f;
                    break;
                case GraphicsUnit.Millimeter:
                    unitScale *= graphics.DpiY / 25.4f;
                    break;
            }

            return new AspectScaleAndTranslation(fontScale * unitScale, fontScale, matrix[newX_const] * unitScale, matrix[newY_const] * unitScale);
        }






        public void DrawText(IDeviceContext dc, string text, Font font, float fontScale, Rectangle bounds, Color foreColor, TextFormatFlags flags)
        {
            var hdc = dc.GetHdc();
            try
            {
                DrawText(hdc, text, font, fontScale, bounds, foreColor, flags);
            }
            finally
            {
                dc.ReleaseHdc();
            }
        }
        public void DrawText(IntPtr hdc, string text, Font font, float fontScale, Rectangle bounds, Color foreColor, TextFormatFlags flags)
        {
            if (string.IsNullOrEmpty(text) || foreColor.A == 0) return;

            var rect = (RECT)bounds;
            var hPrevFont = Gdi32.SelectObject(hdc, GetHFont(font, User32.GetDeviceCaps(hdc, User32.DeviceCaps.LogPixelsY) * fontScale));
            try
            {
                // Handle vertical alignment when SingleLine is not specified
                if ((flags & (TextFormatFlags.VerticalCenter | TextFormatFlags.Bottom)) != 0 && (flags & TextFormatFlags.SingleLine) == 0)
                {
                    if ((flags & TextFormatFlags.WordBreak) == 0 && text.IndexOfAny(new[] { '\r', '\n' }) == -1)
                    {
                        flags |= TextFormatFlags.SingleLine;
                    }
                    else
                    {
                        // Measure the height and change the bounds to vertically align the text
                        User32.DrawText(hdc, text, text.Length, ref rect, (User32.DT)flags | User32.DT.CalcRect);
                        if ((flags & TextFormatFlags.VerticalCenter) != 0)
                            rect.Top += (bounds.Bottom - rect.Bottom) / 2;
                        else
                            rect.Top += bounds.Bottom - rect.Bottom;
                        rect.Bottom = bounds.Bottom;
                        rect.Right = bounds.Right;
                    }
                }

                Gdi32.SetBkMode(hdc, Gdi32.BkMode.Transparent);
                Gdi32.SetTextColor(hdc, foreColor);
                User32.DrawText(hdc, text, text.Length, ref rect, (User32.DT)flags);
            }
            finally
            {
                Gdi32.SelectObject(hdc, hPrevFont);
            }
        }

        private Font lastFont;
        private float lastScale;
        private IntPtr lastHFont;
        private IntPtr GetHFont(Font font, float dpi)
        {
            // Cache the most recent font
            if (ReferenceEquals(lastFont, font) && lastScale == dpi) return lastHFont;
            lastFont = font;
            lastScale = dpi;
            Gdi32.DeleteObject(lastHFont);
            return lastHFont = font.ToHfontFast(dpi);
        }

        public void Dispose()
        {
            Gdi32.DeleteObject(lastHFont);
        }
    }
}
