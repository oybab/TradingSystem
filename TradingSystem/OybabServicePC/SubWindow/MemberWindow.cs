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
using Oybab.ServicePC.Tools;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class MemberWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<Member> resultList = null;

        public MemberWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            Notification.Instance.NotificationMember += (obj, value, args) => { this.BeginInvoke(new Action(() => { if (krpdgList.Enabled && resultList.Any(x => x.MemberId == value.MemberId)) krpdgList.Enabled = false; })); };

            new CustomTooltip(this.krpdgList);
            this.ControlBox = false;


            krptMemberNo.Font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            this.Text = Resources.GetRes().GetString("MemberManager");
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
            krplMemberNo.Text = Resources.GetRes().GetString("MemberNo");



            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptMemberNo.Location = new Point(krptMemberNo.Location.X, krptMemberNo.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());

            }
            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemSave, Resources.GetRes().GetString("Save"), Resources.GetRes().GetString("SaveDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Save.png")), (sender, e) => { Save(); });
            //删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });
            // 显示会员支付
            LoadContextMenu(kryptonContextMenuItemPay, Resources.GetRes().GetString("MemberPay"), Resources.GetRes().GetString("MemberPayDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.MemberPay.png")), (sender, e) => { ShowMemberPay(); });
            // 显示会员到期时间
            LoadContextMenu(kryptonContextMenuItemChangeTime, Resources.GetRes().GetString("ChangeTime"), Resources.GetRes().GetString("MemberExpiredDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangeTime.png")), (sender, e) => { ChangeTime(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Members.ico"));

            //初始化
            Init();

            // 实例化一下, 免得新增时出现问题
            resultList = new List<Member>();

            // 刷卡
            Notification.Instance.NotificationCardReader += Instance_NotificationCardReader;

        }

        /// <summary>
        /// 刷卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        private void Instance_NotificationCardReader(object sender, string value, object args)
        {
            ScanBarcode(value);
        }



        /// <summary>
        /// 扫条形码
        /// </summary>
        private void ScanBarcode(string code)
        {

            if (null != newMember && newMember.Visible)
            {
                newMember.ScanCardNo(code);
            }
            else
            {

                if (!this.ContainsFocus)
                    return;


                //为未保存数据而忽略当前操作
                if (krptMemberNo.Focused && !IgnoreOperateForSave())
                    return;


                if (krptMemberNo.Focused)
                {
                    krptMemberNo.Text = "";
                    StartLoad(this, null);

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            List<Member> members;

                            bool result = OperatesService.GetOperates().ServiceGetMembers(0, null, code, null, null, true, out members);
                            this.BeginInvoke(new Action(() =>
                            {
                                if (result)
                                {
                                    this.resultList = members.OrderByDescending(x => x.MemberId).ToList();
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
                            }));
                        }
                        catch (Exception ex)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                {
                                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                            }));
                        }
                        StopLoad(this, null);

                        // 防止滚动条比例没准确显示导致不显示底部的数据
                        this.BeginInvoke(new Action(() =>
                        {
                            krpdgList.PerformLayout();
                        }));

                    });
                }
                else
                {
                    if (null != krpdgList.CurrentCell && krpdgList.CurrentCell.ColumnIndex == 3 && krpdgList.CurrentCell.Selected)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            if (krpdgList.CurrentCell.Value.ToString() != code && krpdgList.CurrentCell.Value.ToString() != "")
                            {
                                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("CardNoExists"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (confirm == DialogResult.Yes)
                                {
                                    krpdgList.CurrentCell.Value = code;
                                    krpdgList.Rows[krpdgList.CurrentCell.RowIndex].Cells["krpcmEdit"].Value = "*";
                                }
                            }
                            else
                            {
                                krpdgList.CurrentCell.Value = code;
                                krpdgList.Rows[krpdgList.CurrentCell.RowIndex].Cells["krpcmEdit"].Value = "*";
                            }
                        }));
                    }
                }
            }



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
            krpcmMemberId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmMemberNo.HeaderText = Resources.GetRes().GetString("MemberNo");
            krpcmCardNo.HeaderText = Resources.GetRes().GetString("CardNo");
            krpcmMemberName0.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("MemberName"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krpcmMemberName1.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("MemberName"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krpcmMemberName2.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("MemberName"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);
            krpcmSex.HeaderText = Resources.GetRes().GetString("Sex");
            krpcmOccupation.HeaderText = Resources.GetRes().GetString("Occupation");
            krpcmMobile.HeaderText = Resources.GetRes().GetString("Mobile");
            krpcmPhone.HeaderText = Resources.GetRes().GetString("Phone");
            krpcmIDNumber.HeaderText = Resources.GetRes().GetString("IDNumber");
            krpcmAddress0.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("Address"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krpcmAddress1.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("Address"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krpcmAddress2.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("Address"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);
            krpcmOfferRate.HeaderText = Resources.GetRes().GetString("OfferRate");
            krpcmBalancePrice.HeaderText = Resources.GetRes().GetString("BalancePrice");
            krpcmSpendPrice.HeaderText = Resources.GetRes().GetString("SpendPrice");
            krpcmFavorablePrice.HeaderText = Resources.GetRes().GetString("FavorablePrice");
            krpcmIsAllowBorrow.HeaderText = Resources.GetRes().GetString("Borrow");
            krpcmExpiredTime.HeaderText = Resources.GetRes().GetString("ExpiredTime");
            krpcmIsEnable.HeaderText = Resources.GetRes().GetString("IsEnable");
            krpcmRemark.HeaderText = Resources.GetRes().GetString("Remark");


            krpcmSex.Items.AddRange(new string[] { Resources.GetRes().GetString("Unknown"), Resources.GetRes().GetString("Male"), Resources.GetRes().GetString("Female") });
            krpcmOccupation.Items.AddRange(new string[] { Resources.GetRes().GetString("Unknown") });

            ReloadMemberTextbox();


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


            if (krptMemberNo.Text.Trim() == "")
                return;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<Member> members;
                    
                    bool result = OperatesService.GetOperates().ServiceGetMembers(-1, krptMemberNo.Text.Trim(), null, null, null, false, out members);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {
                            this.resultList = members.OrderByDescending(x => x.MemberId).ToList();
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
                    }));
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);

                // 防止滚动条比例没准确显示导致不显示底部的数据
                this.BeginInvoke(new Action(() =>
                {
                    krpdgList.PerformLayout();
                }));

            });

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
                AddToGrid("", item.MemberId.ToString(), item.MemberNo, item.CardNo ?? "", item.MemberName0, item.MemberName1, item.MemberName2, GetSexName(item.Sex), GetOccupationName(item.Occupation), item.Mobile ?? "", item.Phone ?? "", item.IDNumber ?? "", item.Address0 ?? "", item.Address1 ?? "", item.Address2 ?? "", item.OfferRate, item.BalancePrice, item.SpendPrice, item.FavorablePrice, item.IsAllowBorrow, item.ExpiredTime, item.IsEnable, item.Remark ?? "", 0);
            }

            SetColor();
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="memberNo"></param>
        /// <param name="cardNo"></param>
        /// <param name="memberNameZH"></param>
        /// <param name="memberNameUG"></param>
        /// <param name="memberNameEn"></param>
        /// <param name="sex"></param>
        /// <param name="Occupation"></param>
        /// <param name="mobile"></param>
        /// <param name="phone"></param>
        /// <param name="idNumber"></param>
        /// <param name="addressZH"></param>
        /// <param name="addressUG"></param>
        /// <param name="addressEn"></param>
        /// <param name="OfferRate"></param>
        /// <param name="BalancePrice"></param>
        /// <param name="SpendPrice"></param>
        /// <param name="FavorablePrice"></param>
        /// <param name="IsAllowBorrow"></param>
        /// <param name="ExpiredTime"></param>
        /// <param name="isEnable"></param>
        /// <param name="Remark"></param>
        /// <param name="mode"></param>
        private void AddToGrid(string editMark, string Id, string memberNo, string cardNo, string memberName0, string memberName1, string memberName2, string sex, string Occupation, string mobile, string phone, string idNumber, string address0, string address1, string address2, double OfferRate, double BalancePrice, double SpendPrice, double FavorablePrice, long IsAllowBorrow, long ExpiredTime, long isEnable, string Remark, int mode)
        {
            string ExpiredTimeStr = "";
            try
            {
                if (ExpiredTime != 0)
                    ExpiredTimeStr = DateTime.ParseExact(ExpiredTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
            if (mode == 0)
            {
                if (editMark == "*")
                    krpdgList.Rows.Insert(0, editMark, Id, memberNo, cardNo, memberName0, memberName1, memberName2, sex, Occupation, mobile, phone, idNumber, address0, address1, address2, OfferRate.ToString(), BalancePrice.ToString(), SpendPrice.ToString(), FavorablePrice.ToString(), IsAllowBorrow.ToString(), ExpiredTimeStr, isEnable.ToString(), Remark);
                else
                    krpdgList.Rows.Add(editMark, Id, memberNo, cardNo, memberName0, memberName1, memberName2, sex, Occupation, mobile, phone, idNumber, address0, address1, address2, OfferRate.ToString(), BalancePrice.ToString(), SpendPrice.ToString(), FavorablePrice.ToString(), IsAllowBorrow.ToString(), ExpiredTimeStr, isEnable.ToString(), Remark);
            }
            else if (mode == 2)
            {
                // 找到并替换
                int rowIndex = -1;
                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {

                    //只有有改动才可以继续
                    if (krpdgList.Rows[i].Cells["krpcmMemberId"].Value.ToString() == Id)
                    {
                        rowIndex = i;
                        break;
                    }
                }

                if (rowIndex != -1)
                {
                    this.krpdgList.Invoke(new Action(() =>
                    {
                        krpdgList.Rows.RemoveAt(rowIndex);
                        krpdgList.Rows.Insert(rowIndex, editMark, Id, memberNo, cardNo, memberName0, memberName1, memberName2, sex, Occupation, mobile, phone, idNumber, address0, address1, address2, OfferRate.ToString(), BalancePrice.ToString(), SpendPrice.ToString(), FavorablePrice.ToString(), IsAllowBorrow.ToString(), ExpiredTimeStr, isEnable.ToString(), Remark);
                        krpdgList.Rows[rowIndex].Selected = true;
                    }));
                }

            }
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
            else
            {
                Notification.Instance.NotificationCardReader -= Instance_NotificationCardReader;
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("MemberManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            AddToGrid("*", "-1", "", "", "", "", "", GetSexName(1), GetOccupationName(0), "", "", "", "", "", "", 100, 0, 0, 0, 0, 0, 1, "", 0);// long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))
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
                if (krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.ToString().Equals("-1"))
                {
                    Member model = new Member();
                    try
                    {

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmMemberName0"], krpdgList.SelectedRows[0].Cells["krpcmMemberName1"], krpdgList.SelectedRows[0].Cells["krpcmMemberName2"], true, false, krpcbMultipleLanguage.Checked);


                        model.MemberId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.ToString());
                        model.MemberNo = krpdgList.SelectedRows[0].Cells["krpcmMemberNo"].Value.ToString().Trim().Trim();
                        model.CardNo = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmCardNo"].Value.ToString().Trim());
                        model.MemberName0 = krpdgList.SelectedRows[0].Cells["krpcmMemberName0"].Value.ToString().Trim();
                        model.MemberName1 = krpdgList.SelectedRows[0].Cells["krpcmMemberName1"].Value.ToString().Trim();
                        model.MemberName2 = krpdgList.SelectedRows[0].Cells["krpcmMemberName2"].Value.ToString().Trim();
                        model.Sex = GetSexNo(krpdgList.SelectedRows[0].Cells["krpcmSex"].Value.ToString());
                        model.Occupation = GetOccupationNo(krpdgList.SelectedRows[0].Cells["krpcmOccupation"].Value.ToString());
                        model.Mobile = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmMobile"].Value.ToString());
                        model.Phone = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmPhone"].Value.ToString());
                        model.IDNumber = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmIDNumber"].Value.ToString());
                        model.Address0 = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAddress0"].Value.ToString());
                        model.Address1 = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAddress1"].Value.ToString());
                        model.Address2 = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAddress2"].Value.ToString());
                        model.OfferRate = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmOfferRate"].Value.ToString()), 2);
                        model.IsAllowBorrow = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsAllowBorrow"].Value.ToString());

                        string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                            model.ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));
                        else
                            model.ExpiredTime = 0;
                        model.IsEnable = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsEnable"].Value.ToString());
                        model.Remark = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmRemark"].Value.ToString());


                        //判断空
                        if (string.IsNullOrWhiteSpace(model.MemberNo) || string.IsNullOrWhiteSpace(model.MemberName0) || string.IsNullOrWhiteSpace(model.MemberName1) || string.IsNullOrWhiteSpace(model.MemberName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        long num;
                        if (!long.TryParse(model.MemberNo, out num))
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("MemberNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            bool IsMemberExists = false;
                            bool IsCardExists = false;
                            bool result = OperatesService.GetOperates().ServiceAddMember(model, out IsMemberExists, out IsCardExists);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result)
                                {
                                    krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value = model.MemberId;
                                    krpdgList.SelectedRows[0].Cells["krpcmMemberNo"].Value = model.MemberNo;
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    resultList.Insert(0, model);

                                    SetColor(true);
                                    ReloadMemberTextbox(true);
                                }
                                else
                                {
                                    if (IsMemberExists)
                                        KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("MemberNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    else if (IsCardExists)
                                        KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("CardNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    else
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
                    Member model = new Member();
                    try
                    {

                        model.MemberId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.ToString());

                        model = resultList.Where(x => x.MemberId == model.MemberId).FirstOrDefault().FastCopy();

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmMemberName0"], krpdgList.SelectedRows[0].Cells["krpcmMemberName1"], krpdgList.SelectedRows[0].Cells["krpcmMemberName2"], false, Ext.AllSame(model.MemberName0, model.MemberName1, model.MemberName2), krpcbMultipleLanguage.Checked);

                        model.MemberNo = krpdgList.SelectedRows[0].Cells["krpcmMemberNo"].Value.ToString().Trim();
                        model.CardNo = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmCardNo"].Value.ToString().Trim());
                        model.MemberName0 = krpdgList.SelectedRows[0].Cells["krpcmMemberName0"].Value.ToString().Trim();
                        model.MemberName1 = krpdgList.SelectedRows[0].Cells["krpcmMemberName1"].Value.ToString().Trim();
                        model.MemberName2 = krpdgList.SelectedRows[0].Cells["krpcmMemberName2"].Value.ToString().Trim();
                        model.Sex = GetSexNo(krpdgList.SelectedRows[0].Cells["krpcmSex"].Value.ToString());
                        model.Occupation = GetOccupationNo(krpdgList.SelectedRows[0].Cells["krpcmOccupation"].Value.ToString());
                        model.Mobile = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmMobile"].Value.ToString());
                        model.Phone = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmPhone"].Value.ToString());
                        model.IDNumber = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmIDNumber"].Value.ToString());
                        model.Address0 = krpdgList.SelectedRows[0].Cells["krpcmAddress0"].Value.ToString();
                        model.Address1 = krpdgList.SelectedRows[0].Cells["krpcmAddress1"].Value.ToString();
                        model.Address2 = krpdgList.SelectedRows[0].Cells["krpcmAddress2"].Value.ToString();
                        model.OfferRate = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmOfferRate"].Value.ToString()), 2);
                        model.IsAllowBorrow = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsAllowBorrow"].Value.ToString());

                        string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                            model.ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));
                        else
                            model.ExpiredTime = 0;
                        model.IsEnable = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsEnable"].Value.ToString());
                        model.Remark = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmRemark"].Value.ToString());


                        //判断空
                        if (string.IsNullOrWhiteSpace(model.MemberNo) || string.IsNullOrWhiteSpace(model.MemberName0) || string.IsNullOrWhiteSpace(model.MemberName1) || string.IsNullOrWhiteSpace(model.MemberName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        long num;
                        if (!long.TryParse(model.MemberNo, out num))
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("MemberNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            bool IsMemberExists = false;
                            bool IsCardExists = false;

                            ResultModel result = OperatesService.GetOperates().ServiceEditMember(model, out IsMemberExists, out IsCardExists);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    Member oldModel = resultList.Where(x => x.MemberId == model.MemberId).FirstOrDefault();

                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, model);

                                    SetColor(true);
                                    ReloadMemberTextbox(true);
                                }
                                else
                                {
                                    if (IsMemberExists)
                                        KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("MemberNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    else if (IsCardExists)
                                        KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("CardNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    else
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
        /// 显示会员支付详情
        /// </summary>
        private void ShowMemberPay()
        {
            long Id = -1;
            Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.ToString());
            MemberPayWindow pay = new MemberPayWindow(resultList.Where(x => x.MemberId == Id).FirstOrDefault());
            pay.StartLoad += (x, y) =>
            {
                this.StartLoad(x, null);
            };
            pay.StopLoad += (x, y) =>
            {
                this.StopLoad(x, null);
            };
            pay.ShowDialog(this);
            Member item = pay.ReturnValue;
            if (null != item)
            {
                int no = resultList.FindIndex(x => x.MemberId == Id);
                resultList.RemoveAt(no);
                resultList.Insert(no, item);

                AddToGrid("", item.MemberId.ToString(), item.MemberNo, item.CardNo ?? "", item.MemberName0, item.MemberName1, item.MemberName2, GetSexName(item.Sex), GetOccupationName(item.Occupation), item.Mobile ?? "", item.Phone ?? "", item.IDNumber ?? "", item.Address0 ?? "", item.Address1 ?? "", item.Address2 ?? "", item.OfferRate, item.BalancePrice, item.SpendPrice, item.FavorablePrice, item.IsAllowBorrow, item.ExpiredTime, item.IsEnable, item.Remark ?? "", 2);
                SetColor(true);
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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                    return;
                }
                //否则先删除数据库

                //如果已经有使用,则提示并拒绝删除
                if (Resources.GetRes().RoomsModel.Where(x => null != x.PayOrder && x.PayOrder.MemberId == Id).Count() > 0)
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
                    
                    ResultModel result = OperatesService.GetOperates().ServiceDelMember(resultList.Where(x => x.MemberId == Id).FirstOrDefault());
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Member oldModel = resultList.Where(x => x.MemberId == Id).FirstOrDefault();
                            krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                            resultList.Remove(oldModel);

                            ReloadMemberTextbox(true);
                        }
                        else
                        {
                            if (result.IsDataHasRefrence)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUsed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        /// 更改时间
        /// </summary>
        private void ChangeTime()
        {
            bool NoLimit = false;
            DateTime ExpiredTime = DateTime.Now;
            string krpdgListSelectedRowsCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();

            if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsCellsValueToString))
                ExpiredTime = DateTime.ParseExact(krpdgListSelectedRowsCellsValueToString, "yyyy-MM-dd HH:mm", null);
            else
                NoLimit = true;


            ExpiredTimeWindow expired = new ExpiredTimeWindow(ExpiredTime, NoLimit);
            expired.ShowDialog(this);

            if (expired.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString() != expired.ReturnValue)
                {
                    krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value = expired.ReturnValue;
                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "*";

                    SetColor(true);
                }
                
            }
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
                    kryptonContextMenuItemPay.Enabled = false;
                    kryptonContextMenuItemChangeTime.Enabled = false;
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
                    {
                        kryptonContextMenuItemSave.Enabled = true;

                        // 只有还没保存的才能删除
                        if (krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.ToString() == "-1")
                            kryptonContextMenuItemDelete.Enabled = true;
                    }

                    
                    kryptonContextMenuItemChangeTime.Enabled = true;

                    // 支付记录显示
                    if (krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.Equals("-1") || krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                        kryptonContextMenuItemPay.Enabled = false;
                    else
                        kryptonContextMenuItemPay.Enabled = true;
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
            //base.OnKeyDown(e);
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

                    // 只有还没保存的才能删除
                    if (krpdgList.SelectedRows.Count > 0 && krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.ToString() == "-1")
                        Delete();
                }
                // 选中时才能更改时间
                else if (e.KeyCode == Keys.T)
                {
                    if (krpdgList.SelectedRows.Count > 0)
                        ChangeTime();
                }
                // 显示支付记录
                else if (e.KeyCode == Keys.E && krpdgList.SelectedRows.Count > 0 && !krpdgList.SelectedRows[0].Cells["krpcmMemberId"].Value.Equals("-1") && !krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    ShowMemberPay();
                    
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
            else if (e.KeyCode == Keys.F12 && e.Alt)
            {
                newMember = new NewMemberWindow();
               
                newMember.AddMember += (x, y) =>
                {
                    var item = x as Tuple<string, string>;
                    AddToGrid("*", "-1", item.Item1, item.Item2, item.Item1, item.Item1, item.Item1, GetSexName(-1), GetOccupationName(0), "", "", "", "", "", "", 100, 0, 0, 0, 0, 0, 1, "", 0);// long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))
                    krpdgList.FirstDisplayedScrollingRowIndex = 0;
                };
                newMember.ShowDialog(this);
            }
        }
        NewMemberWindow newMember = null;

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
        public event EventHandler ChangeType;
        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;


        /// <summary>
        /// 重新加载产品类型搜索框
        /// </summary>
        private void ReloadMemberTextbox(bool TrigChangeEvent = false)
        {
           

            if (TrigChangeEvent)
            {
                if (null != ChangeType)
                    ChangeType(null, null);
            }
        }


        /// <summary>
        /// 获取隐藏类型编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetHideTypeNo(string hideType)
        {
            if (hideType == Resources.GetRes().GetString("Display"))
                return 0;
            else if (hideType == Resources.GetRes().GetString("Hide"))
                return 1;
            else if (hideType == Resources.GetRes().GetString("Backstage"))
                return 2;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取隐藏类型
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetHideType(long hideTypeNo)
        {
            if (hideTypeNo == 0)
                return Resources.GetRes().GetString("Display");
            else if (hideTypeNo == 1)
                return Resources.GetRes().GetString("Hide");
            else if (hideTypeNo == 2)
                return Resources.GetRes().GetString("Backstage");
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
            if (type == Resources.GetRes().GetString("Unknown"))
                return -1;
            else if (type == Resources.GetRes().GetString("Female"))
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
            if (typeNo == -1)
                return Resources.GetRes().GetString("Unknown");
            else if (typeNo == 0)
                return Resources.GetRes().GetString("Female");
            else if (typeNo == 1)
                return Resources.GetRes().GetString("Male");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        
             /// <summary>
        /// 获取职业编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetOccupationNo(string type)
        {
            return 99;
        }

        /// <summary>
        /// 获取职业名称
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetOccupationName(long typeNo)
        {
            return Resources.GetRes().GetString("Unknown");
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
        /// 设置醒目颜色
        /// </summary>
        private void SetColor(bool IsOnlySelected = false)
        {
            // 当前时间
            long now = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            if (IsOnlySelected)
            {
                try
                {
                    // 余额 小于0
                    double BalancePrice = double.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Value.ToString());
                    if (BalancePrice < 0)
                        krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Red;
                    else
                        krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Empty;
                   
                    // Warn Time (小于15天的显示警告)
                    long ExpiredTime = 0;
                    string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();
                    if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                        ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));


                    if (ExpiredTime != 0 && ExpiredTime < now)
                        krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Style.SelectionForeColor = Color.Red;
                    else
                        krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Style.SelectionForeColor = Color.Empty;

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }
            }
            else
            {
                //设置所有颜色
                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {
                    try
                    {
                        // 余额 小于0
                        double BalancePrice = double.Parse(krpdgList.Rows[i].Cells["krpcmBalancePrice"].Value.ToString());
                        if (BalancePrice < 0)
                            krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Red;
                        else
                            krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Empty;


                        // Warn Time (小于15天的显示警告)
                        long ExpiredTime = 0;
                        string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.Rows[i].Cells["krpcmExpiredTime"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                            ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));


                        if (ExpiredTime != 0 && ExpiredTime < now)
                            krpdgList.Rows[i].Cells["krpcmExpiredTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmExpiredTime"].Style.SelectionForeColor = Color.Red;
                        else
                            krpdgList.Rows[i].Cells["krpcmExpiredTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmExpiredTime"].Style.SelectionForeColor = Color.Empty;

                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }
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

                krpcmSex.Visible = false;
                krpcmPhone.Visible = false;
                krpcmIDNumber.Visible = false;
                
               
                krpcmSpendPrice.Visible = false;
                krpcmFavorablePrice.Visible = false;

                krpcmExpiredTime.Visible = false;
                krpcmRemark.Visible = false;

            }
            else
            {
                krpcmSex.Visible = true;
                krpcmPhone.Visible = true;
                krpcmIDNumber.Visible = true;
               
                
                krpcmSpendPrice.Visible = true;
                krpcmFavorablePrice.Visible = true;

                krpcmExpiredTime.Visible = true;
                krpcmRemark.Visible = true;
            }

            // 因为地址那里也得刷新
            krpcbMultipleLanguage_CheckedChanged(null, null);
        }

      
        /// <summary>
        /// 切换多语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbMultipleLanguage_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbMultipleLanguage.Checked)
            {
                krpcmMemberName0.Visible = false;
                krpcmMemberName1.Visible = false;
                krpcmMemberName2.Visible = false;

                krpcmAddress0.Visible = false;
                krpcmAddress1.Visible = false;
                krpcmAddress2.Visible = false;


                if (Resources.GetRes().MainLangIndex == 0)
                {
                    krpcmMemberName0.Visible = true;

                    if (krpcbIsDisplayAll.Checked)
                        krpcmAddress0.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    krpcmMemberName1.Visible = true;

                    if (krpcbIsDisplayAll.Checked)
                        krpcmAddress1.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    krpcmMemberName2.Visible = true;

                    if (krpcbIsDisplayAll.Checked)
                        krpcmAddress2.Visible = true;
                }
            }
            else
            {
                krpcmMemberName0.Visible = true;
                krpcmMemberName1.Visible = true;
                krpcmMemberName2.Visible = true;

                if (krpcbIsDisplayAll.Checked)
                {
                    krpcmAddress0.Visible = true;
                    krpcmAddress1.Visible = true;
                    krpcmAddress2.Visible = true;
                }



            }
        }
        
    }
}
