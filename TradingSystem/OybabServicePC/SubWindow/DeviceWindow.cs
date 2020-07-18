using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.ServicePC.Tools;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class DeviceWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<Device> resultList = null;

        public DeviceWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            Notification.Instance.NotificationDevice += (obj, value, args) => { this.BeginInvoke(new Action(() => { if (krpdgList.Enabled && resultList.Any(x => x.DeviceId == value.DeviceId)) krpdgList.Enabled = false; })); };
            this.ControlBox = false;
            new CustomTooltip(this.krpdgList);
            this.krpcmRoomId.SetParent(this.krpdgList);
            
            krptDeviceNo.Font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            this.Text = Resources.GetRes().GetString("DeviceManager");
            ResetPage();
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krpbBeginPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveFirst.png"));
            krpbPrewPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.previous.png"));
            krpbNextPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.next.png"));
            krpbEngPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveLast.png"));
            krpbClickToPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.select.png"));

            krpbBeginPage.StateCommon.Back.ImageStyle = krpbPrewPage.StateCommon.Back.ImageStyle = krpbNextPage.StateCommon.Back.ImageStyle = krpbEngPage.StateCommon.Back.ImageStyle = krpbClickToPage.StateCommon.Back.ImageStyle = PaletteImageStyle.CenterMiddle;

            krplPage.Text = Resources.GetRes().GetString("Page");

            krpbSearch.Text = Resources.GetRes().GetString("Search");
            krplDeviceNo.Text = Resources.GetRes().GetString("DeviceNo");


            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptDeviceNo.Location = new Point(krptDeviceNo.Location.X, krptDeviceNo.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());

            }
            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemSave, Resources.GetRes().GetString("Save"), Resources.GetRes().GetString("SaveDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Save.png")), (sender, e) => { Save(); });
            //删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Device.ico"));

            //初始化
            Init();


            //防止直接增加数据的时候插入本地队列失败
            resultList = new List<Device>();

        }

        /// <summary>
        /// 初始化右键
        /// </summary>
        /// <param name="index"></param>
        /// <param name="message"></param>
        /// <param name="image"></param>
        /// <param name="handler"></param>
        private void LoadContextMenu(KryptonContextMenuItem item, string message, string ExtraMessage, Image image, EventHandler handler)
        {
            item.Text = message;
            item.Image = image;
            item.ExtraText = ExtraMessage;
            item.Click += handler;
        }

        /// <summary>
        /// 重置分页
        /// </summary>
        private void ResetPage()
        {
            krplPageCount.Text = "1";
            krptCurrentPage.Text = "1";
            krptCurrentPage.Enabled = krpbBeginPage.Enabled = krpbPrewPage.Enabled = krpbNextPage.Enabled = krpbEngPage.Enabled = krpbClickToPage.Enabled = false; 
        }


        /// <summary>
        /// 设置列
        /// </summary>
        private void Init()
        {
            krpcmDeviceId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmDeviceNo.HeaderText = Resources.GetRes().GetString("DeviceNo");
            krpcmDeviceType.HeaderText = Resources.GetRes().GetString("DeviceType");
            krpcmIpAddress.HeaderText = Resources.GetRes().GetString("IpAddress");
            krpcmRoomId.HeaderText = Resources.GetRes().GetString("RoomNo");
            krpcmIsEnable.HeaderText = Resources.GetRes().GetString("IsEnable");
            

            ReloadDeviceTextbox();


            krpcmDeviceType.Items.AddRange(new string[] { Resources.GetRes().GetString("All"), Resources.GetRes().GetString("Computer"), Resources.GetRes().GetString("Tablet"), Resources.GetRes().GetString("Mobile") });


            RealoadRoomNo();


            // 点歌系统就显示包厢编号
            if (Resources.GetRes().IsRequired("Vod"))
                krpcmRoomId.Visible = true;


        }


        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSearch_Click(object sender, EventArgs e)
        {
            if (!krpdgList.Enabled)
            {
                krpdgList.Enabled = true;
                krpdgList.Rows.Clear();
            }

            //为未保存数据而忽略当前操作
            if (!IgnoreOperateForSave())
                return;

            //查找数据
            resultList = Resources.GetRes().Devices.OrderByDescending(x => x.Order).ThenBy(x => x.DeviceNo.Length).ThenBy(x => x.DeviceNo).ToList();
            if (krptDeviceNo.Text.Trim() != "")
                resultList = resultList.Where(x => x.DeviceNo.Contains(krptDeviceNo.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            

            //设定页面数据
            ResetPage();
            if (resultList.Count() > 0)
            {
                //resultList.Reverse();
                AllPage = (int)((resultList.Count() - 1 + ListCount) / ListCount);
                krplPageCount.Text = AllPage.ToString();

                CurrentPage = 1;
                krptCurrentPage.Text = CurrentPage.ToString();

                //打开第一页
                OpenPageTo(CurrentPage, false);
            }
            else
            {
                krpdgList.Rows.Clear();
            }

        }

        /// <summary>
        /// 打开某页
        /// </summary>
        /// <param name="pageNo"></param>
        private void OpenPageTo(int pageNo, bool Manual = true)
        {
            //先判断是否能去这个页
            if (pageNo < 1 || pageNo > AllPage)
            {
                return;
            }
            if (CurrentPage == pageNo && Manual)
                return;

            //为未保存数据而忽略当前操作
            if (Manual && !IgnoreOperateForSave())
                return;
                

            //设定按钮
            krptCurrentPage.Enabled = AllPage > 1;
            krpbBeginPage.Enabled = pageNo > 1;
            krpbEngPage.Enabled = pageNo < AllPage;
            krpbNextPage.Enabled = pageNo < AllPage;
            krpbPrewPage.Enabled = pageNo > 1;
            krpbClickToPage.Enabled = AllPage > 1;
            

            CurrentPage = pageNo;
            krptCurrentPage.Text = CurrentPage.ToString();

            //获取数据
            var currentResult = resultList.Skip((CurrentPage - 1) * ListCount).Take(ListCount);
            //添加到数据集中
            krpdgList.Rows.Clear();
            foreach (var item in currentResult)
            {
                AddToGrid("", item.DeviceId.ToString(), item.DeviceNo, GetTypeName(item.DeviceType), item.IpAddress, null == item.RoomId ? "" : Resources.GetRes().Rooms.Where(x=>x.RoomId == item.RoomId).FirstOrDefault().RoomNo, item.IsEnable);
            }
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="deviceNo"></param>
        /// <param name="type"></param>
        /// <param name="ip"></param>
        /// <param name="roomName"></param>
        /// <param name="isEnable"></param>
        private void AddToGrid(string editMark, string Id, string deviceNo, string type, string ip, string roomName, long isEnable)
        {
            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, deviceNo, type, ip, roomName, isEnable.ToString());
            else
                krpdgList.Rows.Add(editMark, Id, deviceNo, type, ip, roomName, isEnable.ToString());
        }

        /// <summary>
        /// 转到首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbBeginPage_Click(object sender, EventArgs e)
        {
            OpenPageTo(1);
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbPrewPage_Click(object sender, EventArgs e)
        {
            OpenPageTo(CurrentPage - 1);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbNextPage_Click(object sender, EventArgs e)
        {
            OpenPageTo(CurrentPage + 1);
        }

        /// <summary>
        /// 末页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbEngPage_Click(object sender, EventArgs e)
        {
            OpenPageTo(AllPage);
        }

        /// <summary>
        /// 转到指定页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbClickToPage_Click(object sender, EventArgs e)
        {
            int page = 0;
            int.TryParse(krptCurrentPage.Text, out page);
            OpenPageTo(page);
        }
        /// <summary>
        /// 同上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptCurrentPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                krpbClickToPage_Click(null, null);
        }

        private string temp = "";
       
        /// <summary>
        /// 刚开始编辑的时候存下值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (null != krpdgList.CurrentCell.Value)
                temp = krpdgList.CurrentCell.Value.ToString();
            else
                temp = "";
        }

        private DataGridViewCell _celWasEndEdit;

        /// <summary>
        /// 编辑完了以后,需要添加型号表示已修改.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _celWasEndEdit = krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //导致整行选中出现没改动也误以为改动情况
            if (null == krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
            {
                if (null == temp)
                    return;
                else if (temp == "")
                    krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                else
                {
                    krpdgList.Rows[e.RowIndex].Cells["krpcmEdit"].Value = "*";
                    krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                }
            }
            else if (!krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Equals(temp))
                krpdgList.Rows[e.RowIndex].Cells["krpcmEdit"].Value = "*";
        }



        private void krpdgList_SelectionChanged(object sender, EventArgs e)
        {
            {

                if (MouseButtons != 0)
                {
                    SetCurrentCell(krpdgList.CurrentCell.ColumnIndex - 1, krpdgList.CurrentCell.RowIndex);
                }

                else if (_celWasEndEdit != null && krpdgList.CurrentCell != null)
                {
                    // if we are currently in the next line of last edit cell

                    int iColumn = _celWasEndEdit.ColumnIndex;
                    int iRow = _celWasEndEdit.RowIndex;

                    SetCurrentCell(iColumn, iRow);
                }
                _celWasEndEdit = null;
            }
        }

        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                krpdgList.RowHeadersWidth,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1 + ((CurrentPage - 1) * ListCount)).ToString(),
                krpdgList.RowHeadersDefaultCellStyle.Font,
                rectangle,
                krpdgList.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        /// <summary>
        /// 下拉框立即显示(替换为EditingControlShowing)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1 && null != (krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex] as KryptonDataGridViewComboBoxCell ))
            {
                krpdgList.BeginEdit(true);
                KryptonDataGridViewComboBoxEditingControl control = krpdgList.EditingControl as KryptonDataGridViewComboBoxEditingControl;
                if (null != control)
                    control.DroppedDown = true;
            }
        }




        /// <summary>
        /// 退出前判断是否还有数据没保存
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //有尚未保存的数据
            if (!IgnoreOperateForSave())
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 是否为未保存数据而忽略当前的操作
        /// </summary>
        /// <returns></returns>
        private bool IgnoreOperateForSave()
        {
            bool notHandle = false;
            foreach (DataGridViewRow row in krpdgList.Rows)
            {
                if (row.Cells["krpcmEdit"].Value.Equals("*"))
                {
                    notHandle = true;
                    break;
                }
            }
            if (notHandle)
            {
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("DeviceManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        private void Add(){
            OpenPageTo(1);
            AddToGrid("*", "-1", "", GetTypeName(0) , "", "", 1);
            krpdgList.FirstDisplayedScrollingRowIndex = 0;
        }

        /// <summary>
        /// 保存新增或改动的数据
        /// </summary>
        private void Save()
        {
            if (null != krpdgList.SelectedRows[0])
            {
                //如果是插入
                if (krpdgList.SelectedRows[0].Cells["krpcmDeviceId"].Value.ToString().Equals("-1"))
                {



                    Device model = new Device();
                    try
                    {
                        model.DeviceId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmDeviceId"].Value.ToString());
                        model.DeviceNo = krpdgList.SelectedRows[0].Cells["krpcmDeviceNo"].Value.ToString().Trim();
                        model.DeviceType = GetTypeNo(krpdgList.SelectedRows[0].Cells["krpcmDeviceType"].Value.ToString());

                        model.IpAddress = krpdgList.SelectedRows[0].Cells["krpcmIpAddress"].Value.ToString().Trim();
                        Room room = Resources.GetRes().Rooms.Where(x => x.RoomNo == krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString()).FirstOrDefault();
                        model.IsEnable = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsEnable"].Value.ToString());





                        //判断空
                        if (string.IsNullOrWhiteSpace(model.DeviceNo) || string.IsNullOrWhiteSpace(model.IpAddress))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Devices.Where(x => (x.DeviceNo.Equals(model.DeviceNo, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("DeviceNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }



                        if (model.DeviceType == 0)
                        {
                            //判断是否已存在(*号IP)
                            if (Resources.GetRes().Devices.Where(x => x.DeviceType >= 0 && (x.IpAddress.Equals(model.IpAddress, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("IpAddress2")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        else
                        {
                            //判断是否已存在(*号IP)
                            if (Resources.GetRes().Devices.Where(x => (x.DeviceType == 0 || x.DeviceType == model.DeviceType) && (x.IpAddress.Equals(model.IpAddress, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("IpAddress2")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        //先判断下拉框
                        if (string.IsNullOrWhiteSpace(krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString()))
                        {
                            model.RoomId = null;
                        }
                        else if (null != room)
                        {
                            model.RoomId = room.RoomId;
                        }
                        else
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_NotFound"), Resources.GetRes().GetString("RoomNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // 如果输入了包厢号,但是别的已经用过了.则提示 
                        if (null != model.RoomId && Resources.GetRes().Devices.Where(x => x.RoomId == model.RoomId).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("RoomNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }




                        // 添加数量不能超过指定的限制
                        if (model.IsEnable == 1 && Resources.GetRes().Devices.Where(x => x.IsEnable == 1).Count() >= Resources.GetRes().DeviceCount)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_DeviceCountOutOfLimit"), Common.GetCommon().GetFormat()), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }



                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, Resources.GetRes().GetString("SaveFailt"));
                        return;
                    }

                    StartLoad(this, null);

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            bool result = OperatesService.GetOperates().ServiceAddDevice(model);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result)
                                {
                                    krpdgList.SelectedRows[0].Cells["krpcmDeviceId"].Value = model.DeviceId;
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    resultList.Insert(0, model);
                                    Resources.GetRes().Devices.Add(model);
                                    ReloadDeviceTextbox(true);
                                }
                                else
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveFailt"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                {
                                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }), false, Resources.GetRes().GetString("SaveFailt"));
                            }));
                        }
                        StopLoad(this, null);
                    });
                }
                //如果是编辑
                else
                {
                    Device model = new Device();
                    try
                    {

                        model.DeviceId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmDeviceId"].Value.ToString());

                        model = Resources.GetRes().Devices.Where(x => x.DeviceId == model.DeviceId).FirstOrDefault().FastCopy();

                        model.DeviceNo = krpdgList.SelectedRows[0].Cells["krpcmDeviceNo"].Value.ToString().Trim();
                        model.DeviceType = GetTypeNo(krpdgList.SelectedRows[0].Cells["krpcmDeviceType"].Value.ToString());

                        model.IpAddress = krpdgList.SelectedRows[0].Cells["krpcmIpAddress"].Value.ToString().Trim();
                        Room room = Resources.GetRes().Rooms.Where(x => x.RoomNo == krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString()).FirstOrDefault();
                        model.IsEnable = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsEnable"].Value.ToString());



                        //判断空
                        if (string.IsNullOrWhiteSpace(model.DeviceNo) || string.IsNullOrWhiteSpace(model.IpAddress))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Devices.Where(x => x.DeviceId != model.DeviceId && (x.DeviceNo.Equals(model.DeviceNo, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("DeviceNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }



                        if (model.DeviceType == 0)
                        {
                            //判断是否已存在(*号IP)
                            if (Resources.GetRes().Devices.Where(x => x.DeviceId != model.DeviceId && x.DeviceType >= 0 && (x.IpAddress.Equals(model.IpAddress, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("IpAddress2")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        else
                        {
                            //判断是否已存在(*号IP)
                            if (Resources.GetRes().Devices.Where(x => x.DeviceId != model.DeviceId && (x.DeviceType == 0 || x.DeviceType == model.DeviceType) && (x.IpAddress.Equals(model.IpAddress, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("IpAddress2")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }



                        //先判断下拉框
                        if (string.IsNullOrWhiteSpace(krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString()))
                        {
                            model.RoomId = null;
                        }
                        else if (null != room)
                        {
                            model.RoomId = room.RoomId;
                        }
                        else
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_NotFound"), Resources.GetRes().GetString("RoomNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // 如果输入了包厢号,但是别的已经用过了.则提示 
                        if (null != model.RoomId && Resources.GetRes().Devices.Where(x => x.DeviceId != model.DeviceId && x.RoomId == model.RoomId).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("RoomNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                        // 添加数量不能超过指定的限制
                        if (model.IsEnable == 1 && Resources.GetRes().Devices.Where(x => x.IsEnable == 1 && x.DeviceId != model.DeviceId).Count() >= Resources.GetRes().DeviceCount)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_DeviceCountOutOfLimit"), Common.GetCommon().GetFormat()), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, Resources.GetRes().GetString("SaveFailt"));
                        return;
                    }

                    StartLoad(this, null);

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {

                            ResultModel result = OperatesService.GetOperates().ServiceEditDevice(model);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    Device oldModel = resultList.Where(x => x.DeviceId == model.DeviceId).FirstOrDefault();

                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, model);

                                    no = Resources.GetRes().Devices.IndexOf(oldModel);
                                    Resources.GetRes().Devices.RemoveAt(no);
                                    Resources.GetRes().Devices.Insert(no, model);

                                    ReloadDeviceTextbox(true);
                                }
                                else
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveFailt"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }));

                        }
                        catch (Exception ex)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                {
                                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }), false, Resources.GetRes().GetString("SaveFailt"));
                            }));
                        }
                        StopLoad(this, null);
                    });
                }
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        private void Delete()
        {
            long Id = -1;
            try
            {
                //确认删除
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("SureDelete"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmDeviceId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                    return;
                }
                //否则先删除数据库

                //如果已经有使用,则提示并拒绝删除
                if (Resources.GetRes().RoomsModel.Where(x => null != x.PayOrder && x.PayOrder.DeviceId == Id).Count() > 0)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUsed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }), false, Resources.GetRes().GetString("DeleteFailt"));
                return;
            }

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {

                    ResultModel result = OperatesService.GetOperates().ServiceDelDevice(Resources.GetRes().Devices.Where(x => x.DeviceId == Id).FirstOrDefault());
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Device oldModel = Resources.GetRes().Devices.Where(x => x.DeviceId == Id).FirstOrDefault();
                            krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                            resultList.Remove(oldModel);
                            Resources.GetRes().Devices.Remove(oldModel);
                            ReloadDeviceTextbox(true);
                        }
                        else
                        {
                            if (result.IsDataHasRefrence)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUsed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else if (result.UpdateModel)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUnSame"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteFailt"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, Resources.GetRes().GetString("DeleteFailt"));
                    }));
                }
                StopLoad(this, null);
            });
        }

        /// <summary>
        /// 显示右键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //右键
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.RowIndex == -1 || null == krpdgList.Rows[e.RowIndex] || krpdgList.RowCount == 0)
                {
                    kryptonContextMenuItemSave.Enabled = false;
                    kryptonContextMenuItemDelete.Enabled = false;
                }
                else
                {
                    // 没行选中时判断一下, 是否单击了空的地方, 如果是则只显示新增之类的
                    if (krpdgList.Rows[e.RowIndex].Selected == false)
                    {
                        DataGridView.HitTestInfo hit = krpdgList.HitTest(e.X, e.Y);
                        if (hit.Type == DataGridViewHitTestType.None)
                        {
                            return;
                        }
                        else
                        {
                            ExitEditMode();

                            krpdgList.Rows[e.RowIndex].Selected = true;
                            if (krpdgList.SelectedRows.Count == 0)
                                return;
                        }
                    }

                    //如果有改动才可以保存
                    if (krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                        kryptonContextMenuItemSave.Enabled = true;
                    kryptonContextMenuItemDelete.Enabled = true;
                }
                kryptonContextMenuItemAdd.Enabled = true;
                //显示
                krpContextMenu.Show(krpdgList.RectangleToScreen(krpdgList.ClientRectangle),
                     KryptonContextMenuPositionH.Left, KryptonContextMenuPositionV.Top);
            }
        }


        /// <summary>
        /// 退出编辑模式
        /// </summary>
        private void ExitEditMode()
        {
            if (krpdgList.IsCurrentCellInEditMode)
            {
                krpdgList.EndEdit();
                krpdgList.ClearSelection();
            }
        }

        /// <summary>
        /// 单击空白处事件,只为显示增加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_MouseClick(object sender, MouseEventArgs e)
        {
            //右键
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCellMouseEventArgs args = new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, e);
                krpdgList_CellMouseClick(sender, args);
            }
        }


        private Keys lastKeyPressed = Keys.EraseEof;
        /// <summary>
        /// 设置快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Control)
                lastKeyPressed = Keys.EraseEof;
            
        }


        private void krpdgList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && lastKeyPressed != Keys.EraseEof)
            {
                //任何情况下都可以增加
                if (e.KeyCode == Keys.N)
                {
                    Add();
                }
                //当前选中的行保存
                else if (e.KeyCode == Keys.S)
                {
                    if (krpdgList.SelectedRows.Count > 0 && krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    {
                        ExitEditMode();
                        Save();
                    }
                        
                }
                //当前选中的可以删除
                else if (e.KeyCode == Keys.D)
                {
                    if (krpdgList.SelectedRows.Count > 0)
                        Delete();
                }
            }
            else if (e.Control)
                lastKeyPressed = e.KeyCode;
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (null != krpdgList.CurrentCell)
                {
                    int iColumn = krpdgList.CurrentCell.ColumnIndex;
                    int iRow = krpdgList.CurrentCell.RowIndex;

                    SetCurrentCell(iColumn, iRow);
                }

            }
        }

        /// <summary>
        /// 设置当前行
        /// </summary>
        /// <param name="iColumn"></param>
        /// <param name="iRow"></param>
        private void SetCurrentCell(int iColumn, int iRow)
        {
            if (iRow == -1) return;
            // 如果是最后一列
            if (iColumn == krpdgList.Columns.Count - 1)
            {
                // 如果不是最后一行则换行
                if (iRow != krpdgList.Rows.Count - 1)
                {
                    if (krpdgList[0, iRow + 1].Visible == true)
                        krpdgList.CurrentCell = krpdgList[0, iRow + 1];
                    else
                        SetCurrentCell(0, iRow + 1);
                };
            }
            else
            {
                // 继续换到下一列
                if (krpdgList[iColumn + 1, iRow].Visible == true)
                    krpdgList.CurrentCell = krpdgList[iColumn + 1, iRow];
                else
                    SetCurrentCell(iColumn + 1, iRow);
            }
        }

        /// <summary>
        /// 为了选中行的时候能用快捷键,并退出其他编辑模式,把位置定位到只读的单元
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                krpdgList.CurrentCell = e.Row.Cells[0];
            }
        }


        /// <summary>
        /// 类型被修改
        /// </summary>
        public event EventHandler ChangeDevice;
        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;


        /// <summary>
        /// 重新加载平板编号搜索框
        /// </summary>
        private void ReloadDeviceTextbox(bool TrigChangeEvent = false)
        {
            krptDeviceNo.SetValues(null, false);
            if (Resources.GetRes().Devices.Count > 0)
            {
                krptDeviceNo.SetValues(Resources.GetRes().Devices.OrderByDescending(x => x.Order).ThenBy(x => x.DeviceNo.Length).ThenBy(x => x.DeviceNo).Select(x => x.DeviceNo).ToArray(), false);
            }

            if (TrigChangeEvent)
            {
                if (null != ChangeDevice)
                    ChangeDevice(null, null);
            }
        }

        /// <summary>
        /// 重新加载房间编号
        /// </summary>
        internal void RealoadRoomNo()
        {
            krpcmRoomId.SetValues(Resources.GetRes().Rooms.Where(x => x.HideType == 0).OrderByDescending(x => x.Order).ThenBy(x => x.RoomNo.Length).ThenBy(x => x.RoomNo).Select(x => x.RoomNo).ToArray(), false);
        }


        /// <summary>
        /// 获取类型编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetTypeNo(string type)
        {
            if (type == Resources.GetRes().GetString("All"))
                return 0;
            else if (type == Resources.GetRes().GetString("Computer"))
                return 1;
            else if (type == Resources.GetRes().GetString("Tablet"))
                return 2;
            else if (type == Resources.GetRes().GetString("Mobile"))
                return 3;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取类型名称
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetTypeName(long typeNo)
        {
            if (typeNo == 0)
                return Resources.GetRes().GetString("All");
            else if (typeNo == 1)
                return Resources.GetRes().GetString("Computer");
            else if (typeNo == 2)
                return Resources.GetRes().GetString("Tablet");
            else if (typeNo == 3)
                return Resources.GetRes().GetString("Mobile");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }


        /// <summary>
        /// 回车确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbSearch_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

    }
}
