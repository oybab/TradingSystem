using System.Diagnostics;
using System.Drawing;

namespace Oybab.Report.Interop
{
    [DebuggerDisplay("{Left}, {Top} - {Right}, {Bottom}")]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static implicit operator Rectangle(RECT rectangle)
        {
            return Rectangle.FromLTRB(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }

        public static implicit operator RECT(Rectangle rectangle)
        {
            return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }
    }
}
