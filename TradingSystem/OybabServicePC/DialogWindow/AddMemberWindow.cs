using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using System.IO;
using Oybab.Res.Tools;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class AddMemberWindow : KryptonForm
    {
        public object ReturnValue { get; private set; } //返回值
        private bool IsMember = false;
        private List<long> Ids = new List<long>();
        private bool IsScan = false;

        public AddMemberWindow(bool IsMember = true, List<long> Ids = null, bool IsScan = false)
        {
            InitializeComponent();
            this.IsMember = IsMember;
            this.Ids = Ids;
            if (null == Ids)
                Ids = new List<long>();
            this.IsScan = IsScan;
            if (!IsScan)
                krpbAdd.Visible = true;


            krpbAdd.Text = Resources.GetRes().GetString("Add");
            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptMemberNo.Location = new Point(krptMemberNo.Location.X, krptMemberNo.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            if (IsMember)
            {
                this.Text = Resources.GetRes().GetString("AddMember");
                krplMemberNo.Text = Resources.GetRes().GetString("MemberNo");
                this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.AddMember.ico"));
            }
            else
            {
                this.Text = Resources.GetRes().GetString("AddSupplier");
                krplMemberNo.Text = Resources.GetRes().GetString("SupplierNo");
                this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.AddSupplier.ico"));
            }



            this.Load += AddMemberWindow_Load;

        }

        private void AddMemberWindow_Load(object sender, EventArgs e)
        {
            if (IsScan && null != cardNo)
            {
                krpbAdd_Click(null, null);
            }
        }

        private string cardNo = null;

        internal void SearchByScanner(string cardNo)
        {
            if (null != cardNo)
            {
                this.cardNo = cardNo;
            }
        }



        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAdd_Click(object sender, EventArgs e)
        {
            //先不让用户单击按钮
            krpbAdd.Enabled = false;

            //判断是否空
            if (null == cardNo && krptMemberNo.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplMemberNo.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string memberNo = null;
                if (!string.IsNullOrWhiteSpace(krptMemberNo.Text.Trim()))
                    memberNo = krptMemberNo.Text.Trim();

                StartLoad(this, null);
                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        if (IsMember)
                        {
                            List<Member> Members;
                            bool result = OperatesService.GetOperates().ServiceGetMembers(0, memberNo, cardNo, null, null, true, out Members);

                            //如果验证成功
                            //修改成功
                            this.BeginInvoke(new Action(() =>
                            {
                                if (result && Members.Count > 0)
                                {
                                    // 检查下会员是否到期先
                                    if (Members.FirstOrDefault().ExpiredTime != 0 && DateTime.ParseExact(Members.FirstOrDefault().ExpiredTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture) < DateTime.Now)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("MemberExpired"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else if (Members.FirstOrDefault().IsEnable == 0)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("Exception_MemberDisabled"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else if (Ids.Contains(Members.FirstOrDefault().MemberId))
                                    {
                                        KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("Member")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        ReturnValue = Members.FirstOrDefault();
                                        DialogResult = System.Windows.Forms.DialogResult.OK;
                                        this.Close();
                                    }

                                }
                                else
                                {
                                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("MemberNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }));
                        }
                        else
                        {
                            List<Supplier> Suppliers;
                            bool result = OperatesService.GetOperates().ServiceGetSupplier(0, memberNo, cardNo, null, null, true, out Suppliers);

                            //如果验证成功
                            //修改成功
                            this.BeginInvoke(new Action(() =>
                            {
                                if (result && Suppliers.Count > 0)
                                {
                                    // 检查下会员是否到期先
                                    if (Suppliers.FirstOrDefault().ExpiredTime != 0 && DateTime.ParseExact(Suppliers.FirstOrDefault().ExpiredTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture) < DateTime.Now)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("SupplierExpired"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else if (Suppliers.FirstOrDefault().IsEnable == 0)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("Exception_SupplierDisabled"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else if (Ids.Contains(Suppliers.FirstOrDefault().SupplierId))
                                    {
                                        KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("Supplier")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        ReturnValue = Suppliers.FirstOrDefault();
                                        DialogResult = System.Windows.Forms.DialogResult.OK;
                                        this.Close();
                                    }

                                }
                                else
                                {
                                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("SupplierNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }));
                        }

                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                            {
                                KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }));
                    }

                    StopLoad(this, null);

                    if (!IsScan && DialogResult != DialogResult.OK)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            krpbAdd.Enabled = true;
                        }));
                    }

                    if (this.IsScan && DialogResult != DialogResult.OK)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            this.Close();
                        }));
                    }

                   
                });
            }
            

        }


        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

        private void krptMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !IsScan)
            {
                krpbAdd_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }


        }

        
    }
}
