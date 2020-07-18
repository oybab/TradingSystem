using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Oybab.Report.Interop
{
    [StructLayout(LayoutKind.Explicit)]
    public struct COLORREF : IEquatable<COLORREF>
    {
        [FieldOffset(0)] public readonly uint Value;
        [FieldOffset(0)] public readonly byte R;
        [FieldOffset(1)] public readonly byte G;
        [FieldOffset(2)] public readonly byte B;

        private COLORREF(uint value)
        {
            R = default(byte);
            G = default(byte);
            B = default(byte);
            Value = value;
        }

        public COLORREF(byte r, byte g, byte b)
        {
            Value = default(uint);
            R = r;
            G = g;
            B = b;
        }

        public override int GetHashCode()
        {
            return unchecked((int)Value);
        }

        public override bool Equals(object obj)
        {
            return obj is COLORREF && Equals((COLORREF)obj);
        }

        public bool Equals(COLORREF other)
        {
            return other.Value == Value;
        }

        public override string ToString()
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", R, G, B);
        }

        public static implicit operator Color(COLORREF colorRef)
        {
            return Color.FromArgb(colorRef.R, colorRef.G, colorRef.B);
        }

        public static implicit operator COLORREF(Color color)
        {
            return new COLORREF(color.R, color.G, color.B);
        }

        public static implicit operator uint(COLORREF colorRef)
        {
            return colorRef.Value;
        }

        public static implicit operator COLORREF(uint value)
        {
            return new COLORREF(value);
        }
    }
}