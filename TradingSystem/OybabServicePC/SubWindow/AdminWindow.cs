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
using Oybab.ServicePC.DialogWindow;
using System.Diagnostics;
using Oybab.ServicePC.Tools;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class AdminWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<Admin> resultList = null;

        public AdminWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            Notification.Instance.NotificationAdmin += (obj, value, args) => { this.BeginInvoke(new Action(() => { if (krpdgList.Enabled && resultList.Any(x => x.AdminId == value.AdminId)) krpdgList.Enabled = false; })); };
            this.krpcmMenu.SetParent(this.krpdgList, new Func<string, string>((x) =>
            {
                return ChangeMenu(x);
            }));

            new CustomTooltip(this.krpdgList);
            this.ControlBox = false;
            
            krptAdminName.Font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            this.Text = Resources.GetRes().GetString("AdminManager");
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
            krplAdminName.Text = Resources.GetRes().GetString("AdminName");


            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptAdminName.Location = new Point(krptAdminName.Location.X, krptAdminName.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());

            }

            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemSave, Resources.GetRes().GetString("Save"), Resources.GetRes().GetString("SaveDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Save.png")), (sender, e) => { Save(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemReset, Resources.GetRes().GetString("Reset"), Resources.GetRes().GetString("ResetDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Reset.png")), (sender, e) => { Reset(); });
            //删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });
            // 显示管理员支付
            LoadContextMenu(kryptonContextMenuItemAdminPay, Resources.GetRes().GetString("AdminPay"), Resources.GetRes().GetString("AdminPayDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.AdminPay.png")), (sender, e) => { ShowAdminPay(); });
            // 显示权限
            LoadContextMenu(kryptonContextMenuItemMenuList, Resources.GetRes().GetString("PermissionsMenu"), Resources.GetRes().GetString("PermissionsMenuDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.MenuList.png")), (sender, e) => { ShowPermissionsMenu(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Admin.ico"));

            //初始化
            Init();


            //防止直接增加数据的时候插入本地队列失败
            resultList = new List<Admin>();

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
            krpcmAdminId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmAdminNo.HeaderText = Resources.GetRes().GetString("AdminNo");
            krpcmAdminName0.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("AdminName"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krpcmAdminName1.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("AdminName"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krpcmAdminName2.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("AdminName"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);
            krpcmSex.HeaderText = Resources.GetRes().GetString("Sex");
            krpcmMobile.HeaderText = Resources.GetRes().GetString("Mobile");
            krpcmIDNumber.HeaderText = Resources.GetRes().GetString("IDNumber");
            krpcmMode.HeaderText = Resources.GetRes().GetString("Mode");
            krpcmMenu.HeaderText = Resources.GetRes().GetString("PermissionsMenu");
            krpcmIsOnlyOwn.HeaderText = Resources.GetRes().GetString("IsOnlyOwn");
            krpcmIsEnable.HeaderText = Resources.GetRes().GetString("IsEnable");

            ReloadAdminTextbox();

            List<string> modeList = new List<string>() { Resources.GetRes().GetString("Guest"), Resources.GetRes().GetString("Employee") };
            if (Resources.GetRes().AdminModel.AdminNo == "1000")
                modeList.Add(Resources.GetRes().GetString("Admin"));

            krpcmMode.Items.AddRange(modeList.ToArray());
            krpcmSex.Items.AddRange(new string[] { Resources.GetRes().GetString("Male"), Resources.GetRes().GetString("Female")});

            // 设置菜单权限
            List<string> menus = new List<string>();
            menus.AddRange(new List<string> { Resources.GetRes().GetString("Computer"), Resources.GetRes().GetString("Tablet"), Resources.GetRes().GetString("Phone"), Resources.GetRes().GetString("ChangeLanguage"), Resources.GetRes().GetString("InnerBill"), Resources.GetRes().GetString("IncomeManager"), Resources.GetRes().GetString("ExpenditureManager"), Resources.GetRes().GetString("MemberManager") });

            // 如果KEY中桌子数为0, 则不显示桌子
            if (Resources.GetRes().RoomCount > 0)
            {
                menus.Add(Resources.GetRes().GetString("RoomManager"));
            }

            menus.AddRange(new List<string> { Resources.GetRes().GetString("ProductTypeManager"), Resources.GetRes().GetString("ProductManager"), Resources.GetRes().GetString("AdminManager"), Resources.GetRes().GetString("DeviceManager"), Resources.GetRes().GetString("PrinterManager"), Resources.GetRes().GetString("SupplierManager"), Resources.GetRes().GetString("RequestManager"), Resources.GetRes().GetString("AdminLog"), Resources.GetRes().GetString("BalanceManager"), Resources.GetRes().GetString("FinanceLog"), Resources.GetRes().GetString("Statistic"), Resources.GetRes().GetString("ChangeSet")});
            menus.Add(Resources.GetRes().GetString("OuterBill"));


            menus.AddRange(new List<string> { Resources.GetRes().GetString("IncomeTradingManage"), Resources.GetRes().GetString("OpenCashDrawer"), Resources.GetRes().GetString("TemporaryChangePrice"), Resources.GetRes().GetString("ReplaceRoom"), Resources.GetRes().GetString("CancelOrder"), Resources.GetRes().GetString("DeleteProduct"), Resources.GetRes().GetString("DecreaseProductCount"), Resources.GetRes().GetString("ReturnMoney"), Resources.GetRes().GetString("BindMemberByNo"), Resources.GetRes().GetString("ChangeCostPrice"), Resources.GetRes().GetString("ChangeUnitPrice"), Resources.GetRes().GetString("BindSupplierByNo") });

            krpcmMenu.SetValues(menus.ToArray(), true);


            krpcbIsDisplayAll.Text = Resources.GetRes().GetString("IsDisplayAll");
            krpcbMultipleLanguage.Text = Resources.GetRes().GetString("MultiLanguage");
            krpcbIsDisplayAll_CheckedChanged(null, null);
            krpcbMultipleLanguage_CheckedChanged(null, null);
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
            resultList = Resources.GetRes().Admins.OrderByDescending(x => x.Order).ThenBy(x => x.AdminNo.Length).ThenBy(x => x.AdminNo).ToList();
            if (krptAdminName.Text.Trim() != "")
                resultList = resultList.Where(x => x.AdminName0.Contains(krptAdminName.Text.Trim(), StringComparison.OrdinalIgnoreCase) || x.AdminName1.Contains(krptAdminName.Text.Trim(), StringComparison.OrdinalIgnoreCase) || x.AdminName2.Contains(krptAdminName.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            

            //设定页面数据
            ResetPage();
            if (resultList.Count() > 0)
            {
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
                AddToGrid("", item.AdminId.ToString(), item.AdminNo, item.AdminName0, item.AdminName1, item.AdminName2, GetSexName(item.Sex), item.Mobile ?? "", item.IDNumber ?? "", GetModeName(item.Mode), GetMenuNames(item.Menu), item.IsOnlyOwn, item.IsEnable);
            }


            SetFreeze();
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="adminNo"></param>
        /// <param name="adminName0"></param>
        /// <param name="adminName1"></param>
        /// <param name="adminName2"></param>
        /// <param name="sex"></param>
        /// <param name="mobile"></param>
        /// <param name="idNumber"></param>
        /// <param name="mode"></param>
        /// <param name="menu"></param>
        /// <param name="isOnlyOwn"></param>
        /// <param name="isEnable"></param>
        private void AddToGrid(string editMark, string Id, string adminNo, string adminName0, string adminName1, string adminName2, string sex, string mobile, string idNumber, string mode, string menu, long isOnlyOwn, long isEnable)
        {
            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, adminNo, adminName0, adminName1, adminName2, sex, mobile, idNumber, mode, menu, isOnlyOwn.ToString(), isEnable.ToString());
            else
                krpdgList.Rows.Add(editMark, Id, adminNo, adminName0, adminName1, adminName2, sex, mobile, idNumber, mode, menu, isOnlyOwn.ToString(), isEnable.ToString());
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
        /// 由于多选输入框按Enter时会跳到下一个CELL, 所以这里判断一下. 除非输入框的下拉框没显示, 不然不能跳(选择权限菜单)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && this.krpdgList.IsCurrentCellInEditMode && this.krpdgList.CurrentCell.ColumnIndex == 10 && this.krpdgList.CurrentCell is Oybab.ServicePC.Tools.ComTextColumn.ComCell)    //监听回车事件 
            {
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("AdminManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            AddToGrid("*", "-1", "", "", "", "", GetSexName(1), "",  "", GetModeName(1), "", 0, 1);
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
                if (krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString().Equals("-1"))
                {
                    Admin model = new Admin();
                    try
                    {

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmAdminName0"], krpdgList.SelectedRows[0].Cells["krpcmAdminName1"], krpdgList.SelectedRows[0].Cells["krpcmAdminName2"], true, false, krpcbMultipleLanguage.Checked);


                        model.AdminId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString());
                        model.AdminNo = krpdgList.SelectedRows[0].Cells["krpcmAdminNo"].Value.ToString().Trim();
                        model.AdminName0 = krpdgList.SelectedRows[0].Cells["krpcmAdminName0"].Value.ToString().Trim();
                        model.AdminName1 = krpdgList.SelectedRows[0].Cells["krpcmAdminName1"].Value.ToString().Trim();
                        model.AdminName2 = krpdgList.SelectedRows[0].Cells["krpcmAdminName2"].Value.ToString().Trim();
                        model.Sex = GetSexNo(krpdgList.SelectedRows[0].Cells["krpcmSex"].Value.ToString());
                        model.Mobile = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmMobile"].Value.ToString());
                        model.Mode = GetModeNo(krpdgList.SelectedRows[0].Cells["krpcmMode"].Value.ToString());
                        model.Menu = GetValueOrNull(GetMenuNos(krpdgList.SelectedRows[0].Cells["krpcmMenu"].Value.ToString()));
                        model.IDNumber = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmIDNumber"].Value.ToString());
                        model.IsOnlyOwn = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsOnlyOwn"].Value.ToString());
                        model.IsEnable = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsEnable"].Value.ToString());
                        model.Password = "123456";


                        //判断空
                        if (string.IsNullOrWhiteSpace(model.AdminNo) || string.IsNullOrWhiteSpace(model.AdminName0) || string.IsNullOrWhiteSpace(model.AdminName1) || string.IsNullOrWhiteSpace(model.AdminName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Admins.Where(x => (x.AdminNo.Equals(model.AdminNo, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("AdminNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Admins.Where(x => (x.AdminName0.Equals(model.AdminName0, StringComparison.OrdinalIgnoreCase) || x.AdminName1.Equals(model.AdminName1, StringComparison.OrdinalIgnoreCase) || x.AdminName2.Equals(model.AdminName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("AdminName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            bool result = OperatesService.GetOperates().ServiceAddAdmin(model);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result)
                                {
                                    krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value = model.AdminId;
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    resultList.Insert(0, model);
                                    Resources.GetRes().Admins.Add(model);
                                    ReloadAdminTextbox(true);
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
                    Admin model = new Admin();
                    try
                    {

                        model.AdminId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString());

                        model = Resources.GetRes().Admins.Where(x => x.AdminId == model.AdminId).FirstOrDefault().FastCopy();

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmAdminName0"], krpdgList.SelectedRows[0].Cells["krpcmAdminName1"], krpdgList.SelectedRows[0].Cells["krpcmAdminName2"], false, Ext.AllSame(model.AdminName0, model.AdminName1, model.AdminName2), krpcbMultipleLanguage.Checked);



                        model.AdminNo = krpdgList.SelectedRows[0].Cells["krpcmAdminNo"].Value.ToString().Trim();
                        model.AdminName0 = krpdgList.SelectedRows[0].Cells["krpcmAdminName0"].Value.ToString().Trim();
                        model.AdminName1 = krpdgList.SelectedRows[0].Cells["krpcmAdminName1"].Value.ToString().Trim();
                        model.AdminName2 = krpdgList.SelectedRows[0].Cells["krpcmAdminName2"].Value.ToString().Trim();
                        model.Sex = GetSexNo(krpdgList.SelectedRows[0].Cells["krpcmSex"].Value.ToString());
                        model.Mobile = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmMobile"].Value.ToString());
                        model.Mode = GetModeNo(krpdgList.SelectedRows[0].Cells["krpcmMode"].Value.ToString());
                        model.Menu = GetValueOrNull(GetMenuNos(krpdgList.SelectedRows[0].Cells["krpcmMenu"].Value.ToString()));
                        model.IDNumber = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmIDNumber"].Value.ToString());
                        model.IsOnlyOwn = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsOnlyOwn"].Value.ToString());
                        model.IsEnable = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsEnable"].Value.ToString());

                        

                        //判断空
                        if (string.IsNullOrWhiteSpace(model.AdminNo) || string.IsNullOrWhiteSpace(model.AdminName0) || string.IsNullOrWhiteSpace(model.AdminName1) || string.IsNullOrWhiteSpace(model.AdminName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Admins.Where(x => x.AdminId != model.AdminId && (x.AdminNo.Equals(model.AdminNo, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("AdminNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Admins.Where(x => x.AdminId != model.AdminId && (x.AdminName0.Equals(model.AdminName0, StringComparison.OrdinalIgnoreCase) || x.AdminName1.Equals(model.AdminName1, StringComparison.OrdinalIgnoreCase) || x.AdminName2.Equals(model.AdminName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {

                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("AdminName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            
                            ResultModel result = OperatesService.GetOperates().ServiceEditAdmin(model);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    Admin oldModel = resultList.Where(x => x.AdminId == model.AdminId).FirstOrDefault();

                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, model);

                                    no = Resources.GetRes().Admins.IndexOf(oldModel);
                                    Resources.GetRes().Admins.RemoveAt(no);
                                    Resources.GetRes().Admins.Insert(no, model);

                                    if (Resources.GetRes().AdminModel.AdminId == model.AdminId)
                                        Resources.GetRes().AdminModel = model;

                                    ReloadAdminTextbox(true);
                                }
                                else
                                {
                                    if (result.UpdateModel)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUnSame"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveFailt"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                    return;
                }
                //否则先删除数据库

                //如果已经有使用,则提示并拒绝删除
                if (Resources.GetRes().RoomsModel.Where(x => null != x.PayOrder && x.PayOrder.AdminId == Id).Count() > 0)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUsed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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
                return;
            }

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    
                    ResultModel result = OperatesService.GetOperates().ServiceDelAdmin(Resources.GetRes().Admins.Where(x => x.AdminId == Id).FirstOrDefault());
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Admin oldModel = Resources.GetRes().Admins.Where(x => x.AdminId == Id).FirstOrDefault();
                            krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                            resultList.Remove(oldModel);
                            Resources.GetRes().Admins.Remove(oldModel);
                            ReloadAdminTextbox(true);
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
        /// 重置
        /// </summary>
        private void Reset()
        {
            long Id = -1;
            try
            {
                //确认重置
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("SureReset"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString());

            }
            catch (Exception ex)
            {
                this.BeginInvoke(new Action(() =>
                {
                    ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                    {
                        KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Reset")));
                }));
                return;
            }

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    
                    Admin model = Resources.GetRes().Admins.Where(x => x.AdminId == Id).FirstOrDefault();
                    ResultModel result = OperatesService.GetOperates().ServiceResetAdmin(model, "123456");

                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), Resources.GetRes().GetString("Reset")), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Admin oldModel = resultList.Where(x => x.AdminId == model.AdminId).FirstOrDefault();

                            int no = resultList.IndexOf(oldModel);
                            resultList.RemoveAt(no);
                            resultList.Insert(no, model);

                            no = Resources.GetRes().Admins.IndexOf(oldModel);
                            Resources.GetRes().Admins.RemoveAt(no);
                            Resources.GetRes().Admins.Insert(no, model);

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
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Reset")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Reset")));
                    }));
                }
                StopLoad(this, null);
            });
        }



        /// <summary>
        /// 显示管理员支付详情
        /// </summary>
        private void ShowAdminPay()
        {
            long Id = -1;
            Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString());
            AdminPayWindow pay = new AdminPayWindow(resultList.Where(x => x.AdminId == Id).FirstOrDefault());
            pay.StartLoad += (x, y) =>
            {
                this.StartLoad(x, null);
            };
            pay.StopLoad += (x, y) =>
            {
                this.StopLoad(x, null);
            };
            pay.ShowDialog(this);
            
        }




        /// <summary>
        /// 打开管理员权限
        /// </summary>
        private void ShowPermissionsMenu()
        {
            long Id = -1;
            Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString());
            string menu = krpdgList.SelectedRows[0].Cells["krpcmMenu"].Value.ToString();

            string menuNew = ChangeMenu(menu, resultList.Where(x => x.AdminId == Id).FirstOrDefault());


            if (menu != menuNew)
            {
                krpdgList.SelectedRows[0].Cells["krpcmMenu"].Value = menuNew;
                krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "*";
            }
        }





        /// <summary>
        /// 打开管理员权限
        /// </summary>
        private string ChangeMenu(string menu, Admin admin = null)
        {
            // 先判断一下是不是管理员
            if (Resources.GetRes().AdminModel.Mode != 2)
                return menu;

            MenuListWindow list = new MenuListWindow(menu, admin);
            list.ShowDialog(this);
            if (list.DialogResult == System.Windows.Forms.DialogResult.OK)
                return list.ReturnValue;
            else
                return menu;

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
                    kryptonContextMenuItemAdd.Enabled = false;
                    kryptonContextMenuItemSave.Enabled = false;
                    kryptonContextMenuItemDelete.Enabled = false;
                    kryptonContextMenuItemReset.Enabled = false;
                    kryptonContextMenuItemAdminPay.Enabled = false;
                    kryptonContextMenuItemMenuList.Enabled = false;
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

                    string AdminNo = krpdgList.SelectedRows[0].Cells["krpcmAdminNo"].Value.ToString();

                    //如果有改动才可以保存
                    if (krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    {
                        if (Resources.GetRes().AdminModel.Mode == 2)
                        {
                            kryptonContextMenuItemSave.Enabled = true;
                        }

                        // 只有还没保存的才能删除
                        if (krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString() == "-1")
                            kryptonContextMenuItemDelete.Enabled = true;

                    }
                    else
                    {

                        if (Resources.GetRes().AdminModel.Mode == 2 && AdminNo != "1000" && AdminNo != Resources.GetRes().AdminModel.AdminNo) //if (Resources.GetRes().AdminModel.AdminNo != AdminNo && AdminNo != "1000") 
                            // 如果当前不是1000管理员, 并且对方也是管理员, 那么就能修改


                            if (AdminNo == "" || (Resources.GetRes().AdminModel.AdminNo != "1000" && Resources.GetRes().Admins.Where(x => x.AdminNo == AdminNo).FirstOrDefault().Mode == 2))
                            {

                            }
                            else
                            {
                                kryptonContextMenuItemReset.Enabled = true;
                            }

                    }



                    // 支付记录显示
                    if (!krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.Equals("-1") && !krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && Resources.GetRes().AdminModel.Mode == 2)
                    {
                        // 如果当前不是1000管理员, 并且对方也是管理员, 那么就能修改
                        if (AdminNo == "" || (Resources.GetRes().AdminModel.AdminNo != "1000" && Resources.GetRes().Admins.Where(x => x.AdminNo == AdminNo).FirstOrDefault().Mode == 2))
                        {

                        }
                        else
                        {
                            kryptonContextMenuItemAdminPay.Enabled = true;
                        }

                    }


                    if (Resources.GetRes().AdminModel.Mode == 2)
                    {
                        if (krpdgList.SelectedRows.Count > 0 && !krpdgList.SelectedRows[0].ReadOnly)
                            kryptonContextMenuItemMenuList.Enabled = true;
                        else
                            kryptonContextMenuItemMenuList.Enabled = false;
                    }

                }

                if (Resources.GetRes().AdminModel.Mode == 2)
                {
                    kryptonContextMenuItemAdd.Enabled = true;
                }

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
            //base.OnKeyDown(e);
            if (e.Control && lastKeyPressed != Keys.EraseEof)
            {
                //任何情况下都可以增加
                if (e.KeyCode == Keys.N && Resources.GetRes().AdminModel.Mode == 2)
                {
                    Add();
                }
                //当前选中的行保存
                else if (e.KeyCode == Keys.S && Resources.GetRes().AdminModel.Mode == 2)
                {
                    if (krpdgList.SelectedRows.Count > 0 && krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    {
                        ExitEditMode();
                        Save();
                    }
                }
                //当前选中的行重置
                else if (e.KeyCode == Keys.R)
                {
                    string AdminNo = krpdgList.SelectedRows[0].Cells["krpcmAdminNo"].Value.ToString();
                    if (krpdgList.SelectedRows.Count > 0 && !krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString() == "-1" && Resources.GetRes().AdminModel.Mode == 2 )
                    {
                        
                        if (AdminNo != "1000" && AdminNo != Resources.GetRes().AdminModel.AdminNo) //if (Resources.GetRes().AdminModel.AdminNo != AdminNo && AdminNo != "1000") 
                        {
                            // 如果当前不是1000管理员, 并且对方也是管理员, 那么就能修改
                            if (AdminNo == "" || ( Resources.GetRes().AdminModel.AdminNo != "1000" && Resources.GetRes().Admins.Where(x => x.AdminNo == AdminNo).FirstOrDefault().Mode == 2))
                                return;
                            Reset();
                        }
                    }
                }
                //当前选中的可以删除
                else if (e.KeyCode == Keys.D)
                {

                    // 只有还没保存的才能删除
                    if (krpdgList.SelectedRows.Count > 0 && krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.ToString() == "-1")
                        Delete();
                }
                // 显示管理员支付记录
                else if (e.KeyCode == Keys.E && krpdgList.SelectedRows.Count > 0 && !krpdgList.SelectedRows[0].Cells["krpcmAdminId"].Value.Equals("-1") && !krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && Resources.GetRes().AdminModel.Mode == 2)
                {
                    string AdminNo = krpdgList.SelectedRows[0].Cells["krpcmAdminNo"].Value.ToString();
                    // 如果当前不是1000管理员, 并且对方也是管理员, 那么就能修改
                    if (AdminNo == "" || ( Resources.GetRes().AdminModel.AdminNo != "1000" && Resources.GetRes().Admins.Where(x => x.AdminNo == AdminNo).FirstOrDefault().Mode == 2))
                        return;
                    ShowAdminPay();
                }
                // 显示余额支付记录
                else if (e.KeyCode == Keys.M && Resources.GetRes().AdminModel.Mode == 2 && krpdgList.SelectedRows.Count > 0 && !krpdgList.SelectedRows[0].ReadOnly)
                {
                    ShowPermissionsMenu();
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
            else if (e.KeyCode == Keys.F1 && krpdgList.Rows.Count > 0 && null != krpdgList.CurrentCell && krpdgList.CurrentCell.ColumnIndex == 10 && !krpdgList.CurrentCell.IsInEditMode)
            {
                // 先判断一下是不是管理员
                if (Resources.GetRes().AdminModel.Mode != 2)
                    return;

                // 如果当前不是1000管理员, 并且对方也是管理员, 那么就能修改
                if (Resources.GetRes().AdminModel.AdminNo != "1000")
                {
                    string selectedAdmin = krpdgList.Rows[krpdgList.CurrentCell.RowIndex].Cells["krpcmAdminNo"].Value.ToString();
                    if (selectedAdmin != "" && !krpdgList.Rows[krpdgList.CurrentCell.RowIndex].Cells["krpcmAdminId"].Value.Equals("-1") && Resources.GetRes().Admins.Where(x => x.AdminNo == selectedAdmin).FirstOrDefault().Mode == 2)
                        return;
                }

                krpdgList.BeginEdit(true);

                SendKeys.SendWait("{F1}");
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
        public event EventHandler ChangeAdmin;
        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;



        /// <summary>
        /// 重新加载管理员搜索框
        /// </summary>
        private void ReloadAdminTextbox(bool TrigChangeEvent = false)
        {
            krptAdminName.SetValues(null, false);
            if (Resources.GetRes().Admins.Count > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krptAdminName.SetValues(Resources.GetRes().Admins.OrderByDescending(x => x.Order).ThenBy(x => x.AdminNo.Length).ThenBy(x => x.AdminNo).Select(x => x.AdminName0).ToArray(), false);
                else if (Resources.GetRes().MainLangIndex == 1)
                    krptAdminName.SetValues(Resources.GetRes().Admins.OrderByDescending(x => x.Order).ThenBy(x => x.AdminNo.Length).ThenBy(x => x.AdminNo).Select(x => x.AdminName1).ToArray(), false); //new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "A3", "4B", "C5", "D6", "E7", "F8", "G9", "0H", "Irt", "J3", "K3", "3L", "3M", "3N", "O3", "P5", "Q6", "6R", "S6", "T6", "6U", "6V", "6W", "X6", "Y6", "6Z", "06", "16", "26", "36", "46", "65", "66", "76", "As", "dB", "dC", "Dd", "dE", "dF", "dG", "Hd", "dI", "Jd", "Kd", "dL", "Md", "Nd", "Od", "Pd", "dQ", "Rd", "Sd", "Td", "Ud", "dV", "Wd", "dX", "Yd", "Zd", "d0", "1d", "d2", "3d", "d4", "5d", "6d", "7d", "cA", "cB", "Cc", "cD", "Ec", "Fc", "cG", "Hc", "cI", "gJ", "gK", "gL", "gM", "gN", "gO", "gP", "gQ", "gR", "Sg", "gT", "gU", "gV", "gW", "gX", "Yg", "Zg", "g0", "g1", "g2", "g3", "g4", "g5", "g6", "g7", };
                else if (Resources.GetRes().MainLangIndex == 2)
                    krptAdminName.SetValues(Resources.GetRes().Admins.OrderByDescending(x => x.Order).ThenBy(x => x.AdminNo.Length).ThenBy(x => x.AdminNo).Select(x => x.AdminName2).ToArray(), false);
            }

            if (TrigChangeEvent)
            {
                if (null != ChangeAdmin)
                    ChangeAdmin(null, null);
            }
        }

        /// <summary>
        /// 获取模式编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetModeNo(string type)
        {
            if (type == Resources.GetRes().GetString("Guest"))
                return 0;
            else if (type == Resources.GetRes().GetString("Employee"))
                return 1;
            else if (type == Resources.GetRes().GetString("Admin"))
                return 2;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取模式名称
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetModeName(long typeNo)
        {
            if (typeNo == 0)
                return Resources.GetRes().GetString("Guest");
            else if (typeNo == 1)
                return Resources.GetRes().GetString("Employee");
            else if (typeNo == 2)
                return Resources.GetRes().GetString("Admin");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }




        /// <summary>
        /// 获取性别编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetSexNo(string type)
        {
            if (type == Resources.GetRes().GetString("Female"))
                return 0;
            else if (type == Resources.GetRes().GetString("Male"))
                return 1;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取心别名称
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetSexName(long typeNo)
        {
            if (typeNo == 0)
                return Resources.GetRes().GetString("Female");
            else if (typeNo == 1)
                return Resources.GetRes().GetString("Male");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }


        /// <summary>
        /// 获取菜单编号集合
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private string GetMenuNos(string types)
        {
            if (string.IsNullOrWhiteSpace(types))
                return "";

            string[] typeList = types.Split('&');

            return string.Join("&", typeList.Select(x => GetMenuNo(x)));
        }

        /// <summary>
        /// 获取菜单名称集合
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetMenuNames(string typeNo)
        {
            if (string.IsNullOrWhiteSpace(typeNo))
                return "";

            string[] typeList = typeNo.Split('&');

            return string.Join("&", typeList.Select(x => GetMenuName(x)));
        }


        /// <summary>
        /// 获取菜单编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private string GetMenuNo(string type)
        {
            if (type == Resources.GetRes().GetString("Computer"))
                return "m100";
            else if (type == Resources.GetRes().GetString("Tablet"))
                return "m200";
            else if (type == Resources.GetRes().GetString("Phone"))
                return "m300";
            else if (type == Resources.GetRes().GetString("ChangeLanguage"))
                return "o100";
            else if (type == Resources.GetRes().GetString("InnerBill"))
                return "1000";
            else if (type == Resources.GetRes().GetString("IncomeManager"))
                return "1100";
            else if (type == Resources.GetRes().GetString("ExpenditureManager"))
                return "1200";
            else if (type == Resources.GetRes().GetString("MemberManager"))
                return "1300";
            else if (type == Resources.GetRes().GetString("RoomManager"))
                return "1400";
            else if (type == Resources.GetRes().GetString("ProductTypeManager"))
                return "1500";
            else if (type == Resources.GetRes().GetString("ProductManager"))
                return "1600";
            else if (type == Resources.GetRes().GetString("AdminManager"))
                return "3000";
            else if (type == Resources.GetRes().GetString("DeviceManager"))
                return "3100";
            else if (type == Resources.GetRes().GetString("PrinterManager"))
                return "3200";
            else if (type == Resources.GetRes().GetString("SupplierManager"))
                return "3300";
            else if (type == Resources.GetRes().GetString("RequestManager"))
                return "3900";
            else if (type == Resources.GetRes().GetString("AdminLog"))
                return "3400";
            else if (type == Resources.GetRes().GetString("BalanceManager"))
                return "3800";
            else if (type == Resources.GetRes().GetString("FinanceLog"))
                return "3500";
            else if (type == Resources.GetRes().GetString("Statistic"))
                return "3600";
            else if (type == Resources.GetRes().GetString("ChangeSet"))
                return "5000";
            else if (type == Resources.GetRes().GetString("IncomeTradingManage"))
                return "p100";
            else if (type == Resources.GetRes().GetString("TemporaryChangePrice"))
                return "p200";
            else if (type == Resources.GetRes().GetString("ReplaceRoom"))
                return "p350";
            else if (type == Resources.GetRes().GetString("CancelOrder"))
                return "p300";
            else if (type == Resources.GetRes().GetString("ChangeCostPrice"))
                return "p400";
            else if (type == Resources.GetRes().GetString("ChangeUnitPrice"))
                return "p500";
            else if (type == Resources.GetRes().GetString("BindMemberByNo"))
                return "p600";
            else if (type == Resources.GetRes().GetString("BindSupplierByNo"))
                return "p700";
            else if (type == Resources.GetRes().GetString("DeleteProduct"))
                return "p800";
            else if (type == Resources.GetRes().GetString("DecreaseProductCount"))
                return "p850";
            else if (type == Resources.GetRes().GetString("ReturnMoney"))
                return "p880";
            else if (type == Resources.GetRes().GetString("OuterBill"))
                return "p900";
            else if (type == Resources.GetRes().GetString("OpenCashDrawer"))
                return "t000";
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取菜单名称
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetMenuName(string typeNo)
        {
            if (typeNo == "m100")
                return Resources.GetRes().GetString("Computer");
            else if (typeNo == "m200")
                return Resources.GetRes().GetString("Tablet");
            else if (typeNo == "m300")
                return Resources.GetRes().GetString("Phone");
            else if (typeNo == "o100")
                return Resources.GetRes().GetString("ChangeLanguage");
            else if (typeNo == "1000")
                return Resources.GetRes().GetString("InnerBill");
            else if (typeNo == "1100")
                return Resources.GetRes().GetString("IncomeManager");
            else if (typeNo == "1200")
                return Resources.GetRes().GetString("ExpenditureManager");
            else if (typeNo == "1300")
                return Resources.GetRes().GetString("MemberManager");
            else if (typeNo == "1400")
                return Resources.GetRes().GetString("RoomManager");
            else if (typeNo == "1500")
                return Resources.GetRes().GetString("ProductTypeManager");
            else if (typeNo == "1600")
                return Resources.GetRes().GetString("ProductManager");
            else if (typeNo == "3000")
                return Resources.GetRes().GetString("AdminManager");
            else if (typeNo == "3100")
                return Resources.GetRes().GetString("DeviceManager");
            else if (typeNo == "3200")
                return Resources.GetRes().GetString("PrinterManager");
            else if (typeNo == "3300")
                return Resources.GetRes().GetString("SupplierManager");
            else if (typeNo == "3900")
                return Resources.GetRes().GetString("RequestManager");
            else if (typeNo == "3400")
                return Resources.GetRes().GetString("AdminLog");
            else if (typeNo == "3800")
                return Resources.GetRes().GetString("BalanceManager");
            else if (typeNo == "3500")
                return Resources.GetRes().GetString("FinanceLog");
            else if (typeNo == "3600")
                return Resources.GetRes().GetString("Statistic");
            else if (typeNo == "5000")
                return Resources.GetRes().GetString("ChangeSet");
            else if (typeNo == "p100")
                return Resources.GetRes().GetString("IncomeTradingManage");
            else if (typeNo == "p200")
                return Resources.GetRes().GetString("TemporaryChangePrice");
            else if (typeNo == "p350")
                return Resources.GetRes().GetString("ReplaceRoom");
            else if (typeNo == "p300")
                return Resources.GetRes().GetString("CancelOrder");
            else if (typeNo == "p400")
                return Resources.GetRes().GetString("ChangeCostPrice");
            else if (typeNo == "p500")
                return Resources.GetRes().GetString("ChangeUnitPrice");
            else if (typeNo == "p600")
                return Resources.GetRes().GetString("BindMemberByNo");
            else if (typeNo == "p700")
                return Resources.GetRes().GetString("BindSupplierByNo");
            else if (typeNo == "p800")
                return Resources.GetRes().GetString("DeleteProduct");
            else if (typeNo == "p850")
                return Resources.GetRes().GetString("DecreaseProductCount");
            else if (typeNo == "p880")
                return Resources.GetRes().GetString("ReturnMoney");

            else if (typeNo == "p900")
                return Resources.GetRes().GetString("OuterBill"); 
            else if (typeNo == "t000")
                return Resources.GetRes().GetString("OpenCashDrawer");
            else
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
        }



        /// <summary>
        /// 设置冻结
        /// </summary>
        private void SetFreeze()
        {

            // 如果是超级管理员或者当前用户自己, 则冻结
            foreach (DataGridViewRow row in krpdgList.Rows)
            {
                if (Resources.GetRes().AdminModel.Mode != 2 || row.Cells["krpcmAdminNo"].Value.Equals("1000") || row.Cells["krpcmAdminNo"].Value.Equals(Resources.GetRes().AdminModel.AdminNo))
                    row.ReadOnly = true;
                else
                {
                    // 如果当前不是1000管理员, 并且对方也是管理员, 那么就能修改
                    if (Resources.GetRes().AdminModel.AdminNo != "1000")
                    {
                        if (Resources.GetRes().AdminModel.Mode != 2)
                            row.ReadOnly = true;
                        else if (row.Cells["krpcmAdminNo"].Value.ToString() != "" && Resources.GetRes().Admins.Where(x => x.AdminNo == row.Cells["krpcmAdminNo"].Value.ToString()).FirstOrDefault().Mode == 2)
                            row.ReadOnly = true;
                    }
                }
            }

        }



        /// <summary>
        /// 返回值或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValueOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            else
                return value.Trim();
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

        /// <summary>
        /// 切换显示所有功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbIsDisplayAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbIsDisplayAll.Checked)
            {
                krpcmIDNumber.Visible = false;
            }
            else
            {
                krpcmIDNumber.Visible = true;
  
            }
        }

        

        /// <summary>
        /// 切换显示所有语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbMultipleLanguage_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbMultipleLanguage.Checked)
            {
                krpcmAdminName0.Visible = false;
                krpcmAdminName1.Visible = false;
                krpcmAdminName2.Visible = false;

                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmAdminName0.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmAdminName1.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmAdminName2.Visible = true;
            }
            else
            {
                krpcmAdminName0.Visible = true;
                krpcmAdminName1.Visible = true;
                krpcmAdminName2.Visible = true;

            }
        }
    }
}
