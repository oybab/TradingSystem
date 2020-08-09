using System;
using System.Collections.Generic;
using System.Text;

namespace Oybab.TradingSystemX.VM.ModelsForViews
{
    public sealed class Font
    {
        public string FontFamily { get; set; }
        public int FontSize { get; set; }

        public Font(string FontFamily, int FontSize)
        {
            this.FontFamily = FontFamily;
            this.FontSize = FontSize;
        }
    }
}
