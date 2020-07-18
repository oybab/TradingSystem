using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oybab.ServicePC.Tools
{
    internal sealed class CustomFlowLayoutPanel : FlowLayoutPanel
    {

        public CustomFlowLayoutPanel()
            : base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();

            base.OnScroll(se);
        }


        protected override void OnResize(EventArgs eventargs)
        {
            this.Invalidate();

            base.OnResize(eventargs);
        }

       

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (Environment.OSVersion.Version.Major >= 6) //没试,减少压力 && Environment.OSVersion.Platform == PlatformID.Win32NT
                {
                    cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                }
                return cp;
            }
        }



    }
}
