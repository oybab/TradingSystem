using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oybab.Res.Server.Model;
using Oybab.DAL;

namespace Oybab.ServicePC.Tools
{
    internal sealed partial class RoomControl : UserControl
    {
        private Form form;
        private FlowLayoutPanel parentPanel;
        private RoomModel model;

        private Image EmptyImage;
        private Image OccupiedImage;
        private Image OccupiedImage_Timeup;
        private Image NewOrderAlertImage;
        private Image CustomerCallImage;

        private bool IsCall = false;
        private bool IsNewOrder = false;
        private string State;


        /// <summary>
        /// 右键单击
        /// </summary>
        private Action<RoomModel, RoomControl> MouseRightClick;

        /// <summary>
        /// 左键双击
        /// </summary>
        private Action<RoomModel, RoomControl> MouseDoubleClicks;

        /// <summary>
        /// 选中
        /// </summary>
        private Action<RoomModel, RoomControl> SelectedChange;

        /// <summary>
        /// 选中
        /// </summary>
        private Action<RoomModel, RoomControl, KeyEventArgs> KeyDowns;



        public RoomControl(Form form, FlowLayoutPanel parentPanel, RoomModel model, string RoomNo, bool IsCall, bool IsNewOrder, string State, Action<RoomModel, RoomControl> MouseRightClick, Action<RoomModel, RoomControl> MouseDoubleClicks, Action<RoomModel, RoomControl> Selected, Action<RoomModel, RoomControl, KeyEventArgs> KeyDowns, Image EmptyImage, Image OccupiedImage, Image OccupiedImage_Timeup, Image NewOrderAlertImage, Image CustomerCallImage)
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);


            this.form = form;
            this.parentPanel = parentPanel;
            this.model = model;
            this.RoomNo = RoomNo;
            this.EmptyImage = EmptyImage;
            this.OccupiedImage = OccupiedImage;
            this.OccupiedImage_Timeup = OccupiedImage_Timeup;
            this.NewOrderAlertImage = NewOrderAlertImage;
            this.CustomerCallImage = CustomerCallImage;
            this.IsCall = IsCall;
            this.IsNewOrder = IsNewOrder;
            this.MouseRightClick = MouseRightClick;
            this.MouseDoubleClicks = MouseDoubleClicks;
            this.SelectedChange = Selected;
            this.KeyDowns = KeyDowns;
            this.State = State;


            SetTextLayout();

            RefreshState();
            RefreshImageState();
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




        /// <summary>
        /// 防止Label慢慢刷新
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            // base.OnEnabledChanged(e);
        }


        private void SetTextLayout(){

            this.krplLabel.Text = model.RoomNo;
           
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private void RefreshState()
        {
            this.krplLabel.Text = model.RoomNo;
            if (null == model.PayOrder)
            {
                this.BackgroundImage = EmptyImage;
                lastImage = EmptyImage;
            }
            else
            {
                this.BackgroundImage = OccupiedImage;
                lastImage = OccupiedImage;
            }
        }

        /// <summary>
        /// 获取房间编号
        /// </summary>
        public long RoomId
        {
            get { return this.model.RoomId; }
        }

        /// <summary>
        /// 获取是否呼叫
        /// </summary>
        public bool Calld
        {
            set { this.IsCall = value; }
            get { return this.IsCall; }
        }


        /// <summary>
        /// 获取是否新订单
        /// </summary>
        public bool NewOrderd
        {
            set { this.NewOrderd = value; }
            get { return this.IsNewOrder; }
        }

        /// <summary>
        /// 在线状态
        /// </summary>
        public string OnlineState
        {
            get { return this.State; }
        }



        /// <summary>
        /// 在线状态
        /// </summary>
        public string RoomNo
        {
            set
            {
                this.model.RoomNo = value;
                SetTextLayout();
            }
            get { return this.model.RoomNo; }
        }


        private bool _selected;
        public bool Selected
        {
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    if (_selected && (this.BorderStyle != System.Windows.Forms.BorderStyle.FixedSingle || this.BorderStyle != System.Windows.Forms.BorderStyle.Fixed3D))
                        this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                    else if (!_selected && this.BorderStyle != System.Windows.Forms.BorderStyle.None)
                        this.BorderStyle = System.Windows.Forms.BorderStyle.None;
                }
            }
            get
            {
                return _selected;
            }
        }




        private Image lastImage = null;

        /// <summary>
        /// 刷新颜色
        /// </summary>
        public void RefreshImageState()
        {
            Image image = null;
            if (null == model.PayOrder)
            {
                image = EmptyImage;
            }
            else
            {

                image = OccupiedImage;
                if (null != this.model.PayOrder && null != this.model.PayOrder.EndTime)
                {

                    Room room = Oybab.Res.Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();
                    bool isTimeLeft = false;


                    if ((room.IsPayByTime == 1 || room.IsPayByTime == 2) && DateTime.ParseExact(this.model.PayOrder.EndTime.Value.ToString(), "yyyyMMddHHmmss", null) <= DateTime.Now)
                    {
                        isTimeLeft = true;
                    }
                    if (isTimeLeft)
                    {
                        image = OccupiedImage_Timeup;
                    }
                }
            }
            


            if (image != lastImage)
            {
                lastImage = image;
                this.BackgroundImage = image;
            }

            if (IsCall && null == this.pbCall.Image)
            {
                this.pbCall.Image = CustomerCallImage;
            }
            else if (!IsCall && null != this.pbCall.Image)
            {
                this.pbCall.Image = null;
            }
            
        }





        


        /// <summary>
        /// 单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomControl_MouseDown(object sender, MouseEventArgs e)
        {

            if (!Selected)
            {
                Selected = true;
                if (null != SelectedChange)
                    SelectedChange(model, this);
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (null != MouseRightClick)
                    MouseRightClick(model, this);
            }

            if (e.Clicks != 1)
            {
                mbDoDrag = false;
            }
            else
            {
                mbDoDrag = true;
            }
        }

        /// <summary>
        /// 双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RoomControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!Selected)
            {
                Selected = true;
            }

            if (null != SelectedChange)
                SelectedChange(model, this);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (null != MouseDoubleClicks)
                    MouseDoubleClicks(model, this);
            }
        }

        private bool mbDoDrag;



        private void RoomControl_MouseMove(object sender, MouseEventArgs e)
        {

            // Begins Drag operation only if DoDrag variable is set
            if (e.Button == MouseButtons.Left & mbDoDrag)
            {
                this.DoDragDrop(this, DragDropEffects.Move);
                // Disables DoDragDrop method until next drag operation
                mbDoDrag = false;
            }

        }

        private void RoomControl_DragDrop(object sender, DragEventArgs e)
        {
            RoomControl target = sender as RoomControl;
            if (target != null)
            {
                int targetIndex = FindRoomControlIndex(target);
                if (targetIndex != -1)
                {
                    string roomControlFormat = typeof(RoomControl).FullName;
                    if (e.Data.GetDataPresent(roomControlFormat))
                    {
                        RoomControl source = e.Data.GetData(roomControlFormat) as RoomControl;

                        if (null != source)
                        {
                            if (target != source)
                            {
                                int sourceIndex = this.FindRoomControlIndex(source);

                                if (sourceIndex != -1)
                                    this.parentPanel.Controls.SetChildIndex(source, targetIndex);

                                source.BorderStyle = BorderStyle.FixedSingle;

                            }
                            else
                            {
                                source.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                            }
                        }
                    }
                }

            }
        }

        private void RoomControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }



        private int FindRoomControlIndex(RoomControl control)
        {
            for (int i = 0; i < this.parentPanel.Controls.Count; i++)
            {
                RoomControl target = this.parentPanel.Controls[i] as RoomControl;

                if (control == target)
                    return i;
            }
            return -1;
        }

        private void RoomControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && lastKeyPressed != Keys.EraseEof)
            {
                if (e.KeyCode == Keys.N || e.KeyCode == Keys.L || e.KeyCode == Keys.O || e.KeyCode == Keys.C || e.KeyCode == Keys.R || e.KeyCode == Keys.T || e.KeyCode == Keys.U)
                {
                    if (this.Selected && null != KeyDowns)
                        KeyDowns(model, this, e);
                }
            }
            else if (e.Control)
                lastKeyPressed = e.KeyCode;
        }

        private Keys lastKeyPressed = Keys.EraseEof;
        private void RoomControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Control)
                lastKeyPressed = Keys.EraseEof;
        }

        private void RoomControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.BorderStyle != System.Windows.Forms.BorderStyle.FixedSingle)
                this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }
    }




    public static class ControlExtension
    {
        // TKey is control to drag, TValue is a flag used while dragging
        private static Dictionary<Control, bool> draggables =
                   new Dictionary<Control, bool>();
        private static System.Drawing.Size mouseOffset;

        /// <summary>
        /// Enabling/disabling dragging for control
        /// </summary>
        public static void Draggable(this Control control, bool Enable)
        {
            if (Enable)
            {
                // enable drag feature
                if (draggables.ContainsKey(control))
                {   // return if control is already draggable
                    return;
                }
                // 'false' - initial state is 'not dragging'
                draggables.Add(control, false);

                // assign required event handlersnnn
                control.MouseDown += new MouseEventHandler(control_MouseDown);
                control.MouseUp += new MouseEventHandler(control_MouseUp);
                control.MouseMove += new MouseEventHandler(control_MouseMove);
            }
            else
            {
                // disable drag feature
                if (!draggables.ContainsKey(control))
                {  // return if control is not draggable
                    return;
                }
                // remove event handlers
                control.MouseDown -= control_MouseDown;
                control.MouseUp -= control_MouseUp;
                control.MouseMove -= control_MouseMove;
                draggables.Remove(control);
            }
        }
        static void control_MouseDown(object sender, MouseEventArgs e)
        {
            mouseOffset = new System.Drawing.Size(e.Location);
            // turning on dragging
            draggables[(Control)sender] = true;

            initialControlLocation = ((Control)sender).Location;
        }
        static void control_MouseUp(object sender, MouseEventArgs e)
        {
            // turning off dragging
            draggables[(Control)sender] = false;
        }
        static void control_MouseMove(object sender, MouseEventArgs e)
        {
            // only if dragging is turned on
            if (draggables[(Control)sender] == true)
            {
                // calculations of control's new position
                System.Drawing.Point newLocationOffset = e.Location - mouseOffset;
                ((Control)sender).Left += newLocationOffset.X;
                ((Control)sender).Top += newLocationOffset.Y;
            }
        }



        private static Point initialControlLocation;
        public static bool IsDragging(this Control control)
        {
            return draggables.ContainsKey(control) && initialControlLocation != control.Location;
        }

    }









    //internal sealed class CustomLabelControl : ComponentFactory.Krypton.Toolkit.KryptonLabel
    //{
    //    protected override void OnEnabledChanged(EventArgs e)
    //    {
    //        //base.OnEnabledChanged(e);
    //    }
    //}
}

