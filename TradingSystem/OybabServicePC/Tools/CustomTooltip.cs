using ComponentFactory.Krypton.Toolkit;
using Oybab.Res;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Oybab.ServicePC.Tools
{
    internal sealed class CustomTooltip
    {
        private KryptonDataGridView krpdgList;
        private Form form;
        public CustomTooltip(KryptonDataGridView krpdgList)
        {
            this.krpdgList = krpdgList;
            this.form = krpdgList.FindForm();


                this.krpdgList.ShowCellToolTips = false;
                this.krpdgList.CellMouseEnter += this.krpdgList_CellMouseEnter;

                
                this.krpdgList.CellBeginEdit += this.krpdgList_CellBeginEdit;



                this.krpdgList.MouseMove += krpdgList_MouseMove;



                this._showTimer = new Timer()
                {
                    Interval = 1000
                };


                this._showTimer.Tick += new EventHandler(this.OnTimerTick);

                this._customTooltip.UseAnimation = false;
                this._customTooltip.UseFading = false;
                this._customTooltip.OwnerDraw = true;
                this._customTooltip.Popup += (sender, popupEventArgs) =>
                {

                    // on popip set the size of tool tip
                    Size size = TextRenderer.MeasureText(this._toolTipText.ToString(), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().currentResourceFont, new Size(this.form.Width, 0), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
                    size.Height = size.Height + 2;
                    size.Width = size.Width + 2;

                    popupEventArgs.ToolTipSize = size;

                };
                this._customTooltip.Draw += (sender, drawToolTipEventArgs) =>
                {



                    Rectangle rect = new Rectangle(drawToolTipEventArgs.Bounds.X, drawToolTipEventArgs.Bounds.Y, drawToolTipEventArgs.Bounds.Width, drawToolTipEventArgs.Bounds.Height);


                    LinearGradientBrush itemTracking = new LinearGradientBrush(rect, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColor1(PaletteBackStyle.ControlToolTip, PaletteState.Normal), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColor2(PaletteBackStyle.ControlToolTip, PaletteState.Normal), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBackColorAngle(PaletteBackStyle.ControlToolTip, PaletteState.Normal));
                    LinearGradientBrush itemBorderSelected = new LinearGradientBrush(rect, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColor1(PaletteBorderStyle.ControlToolTip, PaletteState.Normal), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColor2(PaletteBorderStyle.ControlToolTip, PaletteState.Normal), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().GetBorderColorAngle(PaletteBorderStyle.ControlToolTip, PaletteState.Normal));
                    drawToolTipEventArgs.Graphics.FillRectangle(itemTracking, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                    drawToolTipEventArgs.Graphics.DrawRectangle(new Pen(itemBorderSelected), rect.X, rect.Y, rect.Width - 1, rect.Height - 1);


                    TextRenderer.DrawText(drawToolTipEventArgs.Graphics, drawToolTipEventArgs.ToolTipText, Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().currentResourceFont, rect, Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
                };

        }

        

       

   



        


        private ToolTip _customTooltip = new ToolTip();
        private Timer _showTimer = null;
        private object _toolTipText = null;
        private int TooltipCellRowIndex = -1;
        private int TooltipCellColumnIndex = -1;
        private int CurrentCellRowIndex = -1;
        private int CurrentCellColumnIndex = -1;
        private bool IsTooltipDisplayed = false;
        private DateTime ShowTime = DateTime.MinValue;
        private bool IsRun = false;




        private void OnTimerTick(object sender, EventArgs e)
        {
            if (this._showTimer != null)
            {
                if (!IsRun)
                    return;

                this._showTimer.Stop();
                if (null != _toolTipText && !string.IsNullOrWhiteSpace(_toolTipText.ToString()))
                {
                    //if (TooltipCellRowIndex ==  CurrentCellRowIndex && TooltipCellColumnIndex ==  CurrentCellColumnIndex)
                    //{

                    SizeF sizeF = TextRenderer.MeasureText(this._toolTipText.ToString(), Oybab.ServicePC.Pattern.PaletteBlue.GetSelf().currentResourceFont);
                    float textLength = sizeF.Width;

                    if (krpdgList.RowCount > TooltipCellRowIndex && krpdgList.ColumnCount > TooltipCellColumnIndex &&  textLength > this.krpdgList[TooltipCellColumnIndex, TooltipCellRowIndex].Size.Width)
                    {

                        IsTooltipDisplayed = true;
                        ShowTime = DateTime.Now;

                        Rectangle cellRect = krpdgList.GetCellDisplayRectangle(TooltipCellColumnIndex, TooltipCellRowIndex, false);
                        Point locationOnForm = krpdgList.FindForm().PointToClient(krpdgList.Parent.PointToScreen(krpdgList.Location));

                        string text = this._toolTipText.ToString();

                        if (text.Length > 1000)
                            this._toolTipText = text = text.Substring(0, 1000) +".........";


                        _customTooltip.Show(text,
                             form,
                             cellRect.X + locationOnForm.X + 10,
                             cellRect.Y + locationOnForm.Y + 25 - 10); // Duration: 5 seconds.
                        // }
                    }
                }

            }
        }

        private void krpdgList_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            CurrentCellRowIndex = e.RowIndex;
            CurrentCellColumnIndex = e.ColumnIndex;

            if (e.ColumnIndex <= -1 || e.RowIndex <= -1)
            {
                _showTimer.Stop();
                return;
            }

            if (TooltipCellRowIndex != CurrentCellRowIndex || TooltipCellColumnIndex != CurrentCellColumnIndex)
            {
                TooltipCellRowIndex = e.RowIndex;
                TooltipCellColumnIndex = e.ColumnIndex;

                this._toolTipText = krpdgList.Rows[TooltipCellRowIndex].Cells[TooltipCellColumnIndex].Value;
                this._showTimer.Stop();
                this._showTimer.Start();
                IsRun = true;
            }




        }





        ///// <summary>
        ///// 不能窗口隐藏时还是显示着 没用
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void form_VisibleChanged(object sender, EventArgs e)
        //{
        //    if (IsRun)
        //    {
        //        new Action(() =>
        //        {
        //            System.Threading.Thread.Sleep(1100);

        //            if (IsTooltipDisplayed)
        //            {
        //                _customTooltip.Hide(form);
        //                this._showTimer.Stop();

        //                IsTooltipDisplayed = false;
        //                IsRun = false;
        //            }
        //        }).BeginInvoke(null, null);
        //    }
        //}


        /// <summary>
        /// 这个来代替底部的事件,因为如果tooltip太长, 显示在鼠标底部时会触发底部的事件.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsRun && IsTooltipDisplayed)
            {
                if ((DateTime.Now - ShowTime).TotalSeconds > 1)
                {
                    _customTooltip.Hide(form);
                    this._showTimer.Stop();

                    IsTooltipDisplayed = false;
                    IsRun = false;
                }
            }
        }



        //private void krpdgList_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (IsTooltipDisplayed)
        //    {
        //        _customTooltip.Hide(form);
        //        this._showTimer.Stop();
        //        IsTooltipDisplayed = false;
        //    }

        //}

        

        private void krpdgList_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (IsRun)
            {
                _customTooltip.Hide(form);
                this._showTimer.Stop();
                IsTooltipDisplayed = false;
                IsRun = false;
            }
        }

    }




    //internal sealed class MyToolTip : ToolTip
    //{

    //    public MyToolTip()
    //    {
    //        IsHitTestVisible = true;
    //    }
    //    public bool IsHitTestVisible { get; set; }
    //    public bool ReadOnly { get; set; }
    //    protected override void WndProc(ref Message m)
    //    {
    //        if (!IsHitTestVisible)
    //        {
    //            if (m.Msg == 0x21)//WM_MOUSEACTIVATE = 0x21
    //            {
    //                m.Result = (IntPtr)4;//no activation and discard mouse message
    //                return;
    //            }
    //            //WM_MOUSEMOVE = 0x200, WM_LBUTTONUP = 0x202
    //            if (m.Msg == 0x200 || m.Msg == 0x202) return;
    //        }
    //        //WM_SETFOCUS = 0x7
    //        if (ReadOnly && m.Msg == 0x7) return;
    //        base.WndProc(ref m);
    //    }
    //    //Discard key messages
    //    public override bool PreProcessMessage(ref Message msg)
    //    {
    //        if (ReadOnly) return true;
    //        return base.PreProcessMessage(ref msg);
    //    }
    //}

    
}
