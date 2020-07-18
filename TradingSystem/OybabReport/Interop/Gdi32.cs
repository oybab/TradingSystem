using System;
using System.Runtime.InteropServices;

namespace Oybab.Report.Interop
{
    public static class Gdi32
    {
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);
        
        [DllImport("gdi32.dll")]
        internal static extern int SetBkMode(IntPtr hdc, BkMode iBkMode);
        
        public enum BkMode
        {
            Transparent = 1,
            Opaque = 2
        }
        
        [DllImport("gdi32.dll")]
        internal static extern uint SetTextColor(IntPtr hdc, COLORREF crColor);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement, int nOrientation, FontWeight fnWeight, uint fdwItalic, uint fdwUnderline, uint fdwStrikeOut, FontCharSet fdwCharSet, FontPrecision fdwOutputPrecision, FontClipPrecision fdwClipPrecision, FontQuality fdwQuality, FontPitchAndFamily fdwPitchAndFamily, string lpszFace);
        
        public enum FontWeight
        {
            DontCare = 0,
            Thin = 100,
            ExtraLight = 200,
            Light = 300,
            Normal = 400,
            Medium = 500,
            Semibold = 600,
            Bold = 700,
            ExtraBold = 800,
            Heavy = 900
        }

        public enum FontCharSet : byte
        {
            Ansi = 0,
            Default = 1,
            Symbol = 2,
            ShiftJIS = 128,
            Hangeul = 129,
            Hangul = 129,
            GB2312 = 134,
            ChineseBig5 = 136,
            OEM = 255,
            Johab = 130,
            Hebrew = 177,
            Arabic = 178,
            Greek = 161,
            Turkish = 162,
            Vietnamese = 163,
            Thai = 222,
            EastEurope = 238,
            Russian = 204,
            Max = 77,
            Baltic = 186
        }

        public enum FontPrecision : byte
        {
            Default = 0,
            String = 1,
            Character = 2,
            Stroke = 3,
            TT = 4,
            Device = 5,
            Raster = 6,
            TTOnly = 7,
            Outline = 8,
            ScreenOutline = 9,
            PSOnly = 10
        }

        public enum FontClipPrecision : byte
        {
            CLIP_DEFAULT_PRECIS = 0,
            CLIP_CHARACTER_PRECIS = 1,
            CLIP_STROKE_PRECIS = 2,
            CLIP_MASK = 0xf,
            CLIP_LH_ANGLES = (1 << 4),
            CLIP_TT_ALWAYS = (2 << 4),
            CLIP_DFA_DISABLE = (4 << 4),
            CLIP_EMBEDDED = (8 << 4)
        }
        public enum FontQuality : byte
        {
            Default = 0,
            Draft = 1,
            Proof = 2,
            NonAntialiased = 3,
            Antialiased = 4,
            ClearType = 5,
            ClearTypeNatural = 6
        }

        [Flags]
        public enum FontPitchAndFamily : byte
        {
            PitchDefault = 0,
            PitchFixed = 1,
            PitchVariable = 2,
            FamilyDontCare = 0 << 4,
            FamilyRoman = 1 << 4,
            FamilySwiss = 2 << 4,
            FamilyModern = (3 << 4),
            FamilyScript = (4 << 4),
            FamilyDecorative = (5 << 4)
        }
    }
}
