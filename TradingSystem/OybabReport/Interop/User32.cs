using System;
using System.Runtime.InteropServices;

namespace Oybab.Report.Interop
{
    public static class User32
    {
        //[DllImport("user32.dll")]
        //private static extern int DrawText(IntPtr hdc, string lpchText, int cchText, ref RECT lprc, DT dwDTFormat);
        [DllImport("user32.dll", EntryPoint = "DrawTextW")] //CharSet=CharSet.Unicod
        internal static extern int DrawText(IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string str, int len, ref RECT lprc, DT dwDTFormat);
        
        [Flags]
        public enum DT : uint
        {
            Top = 0x00000000,
            Left = 0x00000000,
            Center = 0x00000001,
            Right = 0x00000002,
            VCenter = 0x00000004,
            Bottom = 0x00000008,
            WordBreak = 0x00000010,
            SingleLine = 0x00000020,
            ExpandTabs = 0x00000040,
            TabStop = 0x00000080,
            NoClip = 0x00000100,
            ExternalLeading = 0x00000200,
            CalcRect = 0x00000400,
            NoPrefix = 0x00000800,
            Internal = 0x00001000,
            EditControl = 0x00002000,
            PathEllipsis = 0x00004000,
            EndEllipsis = 0x00008000,
            ModifyString = 0x00010000,
            RtlReading = 0x00020000,
            WordEllipsis = 0x00040000,
            NoFullWidthCharBreak = 0x00080000,
            Hideprefix = 0x00100000,
            PrefixOnly = 0x00200000
        }


        
        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, DeviceCaps nIndex);

        public enum DeviceCaps
        {
            DriverVersion   = 0,
            Technology      = 2,
            HorzSize        = 4,
            VertSize        = 6,
            HorzRes         = 8,
            VertRes         = 10,
            BitsPixel       = 12,
            Planes          = 14,
            NumBrushes      = 16,
            NumPens         = 18,
            NumMarkers      = 20,
            NumFonts        = 22,
            NumColors       = 24,
            PDeviceSize     = 26,
            CurveCaps       = 28,
            LineCaps        = 30,
            PolygonalCaps   = 32,
            TextCaps        = 34,
            ClipCaps        = 36,
            RasterCaps      = 38,
            AspextX         = 40,
            AspectY         = 42,
            AspectXY        = 44,
            LogPixelsX      = 88,
            LogPixelsY      = 90,
            NumReserved     = 106,
            ColorRes        = 108,
            PhysicalWidth   = 110,
            PhysicalHeight  = 111,
            PhysicalOffsetX = 112,
            PhysicalOffsetY = 113,
            ScalingFactorX  = 114,
            ScalingFactorY  = 115,
            VRefresh        = 116,
            DesktopVertRes  = 117,
            DesktopHorzRes  = 118,
            BltAlignment    = 119,
            ShadeBlendCaps  = 120,
            ColorMgmtCaps   = 121
        }
    }
}
