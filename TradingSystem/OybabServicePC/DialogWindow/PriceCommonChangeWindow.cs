using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Res.View.Models;
using Oybab.ServicePC.SubWindow;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PriceCommonChangeWindow : KryptonForm
    {
        private string Mark = "+";
        private double TotalPrice = 0;
        private string ChangePrice = "0";
        private bool IsMember = true;
        private bool IsCheckout = false;
        private Action Recalc = null;

        internal List<CommonPayModel> PayModel { get; private set; } //返回值


        public PriceCommonChangeWindow(string mark, double TotalPrice, List<CommonPayModel> PayModel, bool IsMember = true, bool IsCheckout = false, Action Recalc = null)
        {
            InitializeComponent();
            this.krptChangePrice.LostFocus += krptChangePrice_LostFocus;

            this.Mark = mark;
            this.TotalPrice = TotalPrice;
            this.PayModel = PayModel;
            this.IsMember = IsMember;
            this.IsCheckout = IsCheckout;
            this.Recalc = Recalc;



            this.krplTotalPriceValue.Text = this.TotalPrice.ToString();
            
            this.krptChangePrice.Text = "0";


            this.Text = Resources.GetRes().GetString("ChangePrice");

            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");


            if (IsCheckout)
            {
                krpbAdd.Visible = false;
                this.ControlBox = false;
            }



            krplPaidPrice.Text = Resources.GetRes().GetString("PaidPrice");
            krplChangePrice.Text =  Resources.GetRes().GetString("ChangePrice");
            krplBalancePrice.Text = Resources.GetRes().GetString("BalancePrice");
            krplBalancePay.Text = Resources.GetRes().GetString("BalancePay"); 
            krpbSave.Text = Resources.GetRes().GetString("Save");
            krpbAdd.Text = Resources.GetRes().GetString("Add");





            if (IsMember)
                krplMemberName.Text = Resources.GetRes().GetString("MemberName");
            else
                krplMemberName.Text = Resources.GetRes().GetString("SupplierName");

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptChangePrice.Location = new Point(krptChangePrice.Location.X, krptChangePrice.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangePrice.ico"));

            ReloadBalanceList();
            Calc();


            if (Mark == "+")
                krprbAdd.Checked = true;
            else if (Mark == "-")
                krprbSub.Checked = true;

        }

     
        internal void FocusControl()
        {
            this.krptChangePrice.Focus();
        }


        /// <summary>
        /// 刷新支付列表
        /// </summary>
        private void ReloadBalanceList()
        {
            int SelectedIndex = krpcbBalancePay.SelectedIndex;

            krpcbBalancePay.Items.Clear();
            if (Resources.GetRes().Balances.Count > 0)
            {
                long IncomeOrExpenditure = 0;
                if (IsMember)
                    IncomeOrExpenditure = 2;
                else
                    IncomeOrExpenditure = 3;

                List<Balance> balance = Resources.GetRes().Balances.Where(x => x.HideType == 0 || x.HideType == IncomeOrExpenditure).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).ToList();

               

                foreach (var item in balance)
                {

                    string BalanceName = "";


                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + PayModel.Where(x => x.BalanceId == item.BalanceId).Sum(x => x.OriginalPrice) + "";


                    BalanceName += "  ";
                    if (Resources.GetRes().MainLangIndex == 0)
                        BalanceName += item.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        BalanceName += item.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        BalanceName += item.BalanceName2;

                    krpcbBalancePay.Items.Add(new BalanceItemModel() { Text = BalanceName, Balance = item, IsBalance = true, IsChange = true });

                   
                }

                List<CommonPayModel> alreadyAddedModel = new List<CommonPayModel>();

                foreach (var item in PayModel)
                {
                    if (!alreadyAddedModel.Any(x=> x.BalanceId == item.BalanceId && x.MemberId == item.MemberId && x.SupplierId == x.SupplierId && x.ParentId == item.ParentId && x.IsChange == item.IsChange))
                    {
                        AddMemberOrSupplier(item);
                        alreadyAddedModel.Add(item);
                    }
                    
                }

                    krpcbBalancePay.SelectedIndex = SelectedIndex;
                if (krpcbBalancePay.SelectedIndex == -1)
                    krpcbBalancePay.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSave_Click(object sender, EventArgs e)
        {

            FindItemAndAddPrice(() =>
            {
                this.BeginInvoke(new Action(() =>
                {
                    if (!IsCheckout)
                    {
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        Recalc();
                    }
                }));
            });


            
        }


        private bool FindItemAndAddPrice(Action _action = null)
        {
            double changePriceDouble = double.Parse(ChangePrice);
            // 去掉的钱不能超出总支付金额
            if (Mark == "-")
            {

                if (!Common.GetCommon().IsReturnMoney() && changePriceDouble > Math.Round(PayModel.Sum(x => x.OriginalPrice), 2))
                {
                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("CanNotExceed"), Math.Round(PayModel.Sum(x => x.OriginalPrice), 2)), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            if (0 != changePriceDouble)
            {
                if (krpcbBalancePay.SelectedIndex == -1)
                {
                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Save")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                BalanceItemModel item = krpcbBalancePay.SelectedItem as BalanceItemModel;
                if (!item.IsChange)
                {
                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Save")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!item.IsBalance)
                {
                    
                    if (null != item.Member)
                    {
                        double originalPrice = PayModel.Where(x => x.IsChange && x.MemberId == item.Member.MemberId).Sum(x => x.OriginalPrice);
                        // 输入的钱不能多于余额
                        if (Mark == "+")
                        {
                            if (Math.Round(originalPrice / 100.0 * item.Member.OfferRate, 2) + Math.Round(changePriceDouble / 100.0 * item.Member.OfferRate, 2) > item.Member.BalancePrice)
                            {
                                if (item.Member.IsAllowBorrow == 1 || Resources.GetRes().AdminModel.Mode == 2)
                                {
                                    if (KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("ConfirmItemBalanceNotEnough"), Resources.GetRes().GetString("Member")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                                        return false;
                                }
                                else
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("MemberBalanceNotEnough"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                        }

                        originalPrice = PayModel.Where(x => x.MemberId == item.Member.MemberId).Sum(x => x.OriginalPrice);
                        // 去掉的钱不能多于该会员为这个订单总共消耗的金额
                        if (!Common.GetCommon().IsReturnMoney() && Mark == "-" && originalPrice + (-changePriceDouble) < 0)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("MemberBalanceNotEnough"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }

                    }
                    else if(null != item.Supplier)
                    {
                        double originalPrice = PayModel.Where(x => x.IsChange && x.SupplierId == item.Supplier.SupplierId).Sum(x => x.OriginalPrice);
                        // 输入的钱不能多于余额
                        if (Mark == "+")
                        {
                            if (Math.Round(originalPrice / 100.0 * item.Supplier.OfferRate, 2) + Math.Round(changePriceDouble / 100.0 * item.Supplier.OfferRate, 2) > item.Supplier.BalancePrice)
                            {
                                if (item.Supplier.IsAllowBorrow == 1 || Resources.GetRes().AdminModel.Mode == 2)
                                {
                                    if (KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("ConfirmItemBalanceNotEnough"), Resources.GetRes().GetString("Supplier")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                                        return false;
                                }
                                else
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SupplierBalanceNotEnough"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return false;
                                }
                            }
                        }

                        originalPrice = PayModel.Where(x => x.SupplierId == item.Supplier.SupplierId).Sum(x => x.OriginalPrice);
                        // 去掉的钱不能多于该会员为这个订单总共消耗的金额
                        if (!Common.GetCommon().IsReturnMoney() && Mark == "-" && originalPrice + (-changePriceDouble) < 0)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("SupplierBalanceNotEnough"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    } 
                }

                if (Mark == "-")
                    changePriceDouble = -changePriceDouble;


                FindItemAndAddToPayModel(item, changePriceDouble);

                ChangePrice = "0";
                this.krptChangePrice.Text = "0";

                

                ReloadBalanceList();

                Calc();


                krptChangePrice.Focus();
                krptChangePrice.SelectionStart = krptChangePrice.TextLength;
                krptChangePrice.SelectAll();

               
            }

            if (null != _action)
                _action();

            return true;
        }


        private void AddMemberOrSupplier(CommonPayModel item)
        {
            string BalanceName = "";
            if (IsMember && null != item.MemberId)
            {

                if (null != item.Member && item.IsChange)
                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + (item.Member.BalancePrice) + "";


           
                BalanceName +=  "(" + Resources.GetRes().PrintInfo.PriceSymbol + PayModel.Where(x => x.MemberId == item.MemberId && x.ParentId == item.ParentId).Sum(x => x.OriginalPrice) + ")";

                BalanceName += "  " + Resources.GetRes().GetString("Member");

                krpcbBalancePay.Items.Add(new BalanceItemModel() { Text = BalanceName, Member = item.Member, IsBalance = false, IsChange = item.IsChange });
            }
            else if (!IsMember && null != item.SupplierId)
            {
                
                if (null != item.Supplier && item.IsChange)
                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + (item.Supplier.BalancePrice) + "";

                BalanceName += "(" + Resources.GetRes().PrintInfo.PriceSymbol + PayModel.Where(x => x.SupplierId == item.SupplierId && x.ParentId == item.ParentId).Sum(x => x.OriginalPrice) + ")";

                BalanceName += "  " + Resources.GetRes().GetString("Member");

                krpcbBalancePay.Items.Add(new BalanceItemModel() { Text = BalanceName, Supplier = item.Supplier, IsBalance = false, IsChange = item.IsChange });
            }
        }

        private void FindItemAndAddToPayModel(BalanceItemModel item, double changePriceDouble)
        {
            CommonPayModel model = new CommonPayModel();

            if (item.IsBalance)
            {
                model.BalanceId = item.Balance.BalanceId;
                model.OriginalPrice = changePriceDouble;
                model.Rate = item.Balance.RemoveRate;


                if (model.Rate != 0)
                {
                    model.Price = Math.Round(model.OriginalPrice * model.Rate, 2);
                    model.RemovePrice = model.OriginalPrice - model.Price;
                }
                else
                {
                    model.Price = model.OriginalPrice;
                }


            }
            else if (!item.IsBalance)
            {
                if (this.IsMember)
                {
                    model.MemberId = item.Member.MemberId;
                    model.Rate = item.Member.OfferRate;
                    model.Member = item.Member;
                }
                else
                {
                    model.SupplierId = item.Supplier.SupplierId;
                    model.Rate = item.Supplier.OfferRate;
                    model.Supplier = item.Supplier;
                }

                model.IsChange = item.IsChange;
                model.OriginalPrice = changePriceDouble;

                if (model.Rate != 0)
                {
                    model.Price = Math.Round(model.OriginalPrice / 100.0 * model.Rate, 2);
                    model.RemovePrice = model.OriginalPrice - model.Price;
                }
                else
                {
                    model.Price = model.OriginalPrice;
                }




            }

            PayModel.Add(model);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


      




        /// <summary>
        /// 预设
        /// </summary>
        /// <param name="price"></param>
        internal void SetChangePrice(double price, string mark)
        {
            if (mark == "+")
                krprbAdd.Checked = true;
            else if (mark == "-")
                krprbSub.Checked = true;
            this.Mark = mark;

            ChangePrice = price.ToString();
            krptChangePrice.Text = ChangePrice;
            Calc();
        }



        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,2})?$");

        /// <summary>
        /// 价格改动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptChangePrice_KeyUp(object sender, KeyEventArgs e)
        {
            if (krptChangePrice.Text == "")
            {
                krptChangePrice.Text = "0";
                krptChangePrice.SelectAll();
            }
            if (!match.IsMatch(krptChangePrice.Text))
            {
                krptChangePrice.Text = ChangePrice;
                krptChangePrice.SelectionStart = krptChangePrice.TextLength;
                return;
            }
            ChangePrice = krptChangePrice.Text;


            Calc();

        }



        private void krptChangePrice_LostFocus(object sender, System.EventArgs e)
        {
            if (krptChangePrice.Text != ChangePrice)
            {
                krptChangePrice.Text = ChangePrice;
                krptChangePrice.SelectionStart = krptChangePrice.TextLength;
            }
        }


        


        /// <summary>
        /// 计算
        /// </summary>
        private void Calc()
        {
            this.krplPaidPriceValue.Text = Math.Round(PayModel.Sum(x => x.OriginalPrice), 2).ToString();

            double balancePrice = 0;
            if (Mark == "+")
                krplBalancePriceValue.Text = (balancePrice = Math.Round(Math.Round(PayModel.Sum(x => x.OriginalPrice), 2) + Math.Round(double.Parse(ChangePrice), 2)  - TotalPrice, 2)).ToString();
            else if (Mark == "-")
                krplBalancePriceValue.Text = (balancePrice = Math.Round(Math.Round(PayModel.Sum(x => x.OriginalPrice), 2) - Math.Round(double.Parse(ChangePrice), 2) - TotalPrice, 2)).ToString();


            if (balancePrice > 0)
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Blue;
            else if (balancePrice < 0)
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Red;
            else
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Empty;
        }


        

        private void krpbAdd_Click(object sender, EventArgs e)
        {
            FindItemAndAddPrice();
        }



        private bool IsAddedMemberOrSupplier = false;
        private BalanceItemModel CurrentBalance = null;
        /// <summary>
        /// 会员变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbBalancePay_SelectedIndexChanged(object sender, EventArgs e)
        {
            krpbSave.Enabled = true;
            krpbAdd.Enabled = true;

            BalanceItemModel item = krpcbBalancePay.SelectedItem as BalanceItemModel;
            CurrentBalance = item;
            if (!item.IsBalance)
            {
                flpMember.Visible = true;
                if (item.IsChange)
                {
                    btnMemberAdd.Visible = false;
                    btnMemberRemove.Visible = true;
                    krplMemberNameValue.Visible = true;
                    if (IsMember)
                    {
                        if (Resources.GetRes().MainLangIndex == 0)
                            krplMemberNameValue.Text = item.Member.MemberName0;
                        else if (Resources.GetRes().MainLangIndex == 1)
                            krplMemberNameValue.Text = item.Member.MemberName1;
                        else if (Resources.GetRes().MainLangIndex == 2)
                            krplMemberNameValue.Text = item.Member.MemberName2;
                    }
                    else
                    {
                        if (Resources.GetRes().MainLangIndex == 0)
                            krplMemberNameValue.Text = item.Supplier.SupplierName0;
                        else if (Resources.GetRes().MainLangIndex == 1)
                            krplMemberNameValue.Text = item.Supplier.SupplierName1;
                        else if (Resources.GetRes().MainLangIndex == 2)
                            krplMemberNameValue.Text = item.Supplier.SupplierName2;
                    }
                }
                else
                {
                    krpbSave.Enabled = false;
                    krpbAdd.Enabled = false;
                    flpMember.Visible = false;
                }
            }
            else
            {
                if (IsAddedMemberOrSupplier)
                {
                    flpMember.Visible = false;
                }
                else
                {
                    flpMember.Visible = true;
                    btnMemberAdd.Visible = true;
                    btnMemberRemove.Visible = false;
                    krplMemberNameValue.Visible = false;
                }
            }

            if (IsMember && !Common.GetCommon().IsBindMemberByNo())
            {
                flpMember.Visible = false;
            }
            else if (!IsMember && !Common.GetCommon().IsBindSupplierByNo())
            {
                flpMember.Visible = false;
            }
        }


        private AddMemberWindow memberWindow = null;
        private void btnMemberAdd_Click(object sender, EventArgs e)
        {
            OpenAddMember();
        }


        private void OpenAddMember(bool IsScan = false, string CodeNo = null)
        {
            List<long> Ids = new List<long>();

            if (IsMember)
                Ids = PayModel.Where(x => null != x.MemberId && x.AddTime == 0).Select(x => x.MemberId.Value).ToList();
            else
                Ids = PayModel.Where(x => null != x.SupplierId && x.AddTime == 0).Select(x => x.SupplierId.Value).ToList();

            AddMemberWindow window = new AddMemberWindow(IsMember, Ids, IsScan);
            memberWindow = window;
            window.StartLoad += (x, y) =>
            {
                this.StartLoad(x, y);
            };
            window.StopLoad += (x, y) =>
            {
                this.StopLoad(x, y);
            };

            if (IsScan)
                memberWindow.SearchByScanner(CodeNo);

            if (window.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Member member = window.ReturnValue as Member;
                Supplier supplier = window.ReturnValue as Supplier;

                AddMember(member, supplier);
            }
            memberWindow = null;
        }



        /// <summary>
        /// 添加会员
        /// </summary>
        /// <param name="member"></param>
        private void AddMember(Member member, Supplier supplier)
        {


            CommonPayModel model = new CommonPayModel();

            if (null != member)
            {
                model.MemberId = member.MemberId;
                model.Member = member;
                model.IsChange = true;
            }

            if (null != supplier)
            {
                model.SupplierId = supplier.SupplierId;
                model.Supplier = supplier;
                model.IsChange = true;
            }


            AddMemberOrSupplier(model);

            krpcbBalancePay.SelectedIndex = krpcbBalancePay.Items.Count - 1;


            IsAddedMemberOrSupplier = true;



            double balancePrice = double.Parse(krplBalancePriceValue.Text);

            // 如果会员的钱够, 则放进需要放的钱中.
            if (Mark == "+" && balancePrice < 0 && (null != member && member.BalancePrice >= Math.Abs(balancePrice)) || (null != supplier && supplier.BalancePrice >= Math.Abs(balancePrice)))
            {


                ChangePrice = Math.Abs(balancePrice).ToString();
                this.krptChangePrice.Text = ChangePrice;

                Calc();


                krptChangePrice.Focus();
                krptChangePrice.SelectionStart = krptChangePrice.TextLength;
                krptChangePrice.SelectAll();
            }
        }

        /// <summary>
        /// 去掉会员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMemberRemove_Click(object sender, EventArgs e)
        {
            BalanceItemModel model = krpcbBalancePay.SelectedItem as BalanceItemModel;

            long id = 0;

            if (null != model.Member)
                id = model.Member.MemberId;
            else if (null != model.Supplier)
                id = model.Supplier.SupplierId;


            List<CommonPayModel> removeModel = new List<CommonPayModel>();
            foreach (var item in PayModel.Where(x => x.IsChange))
            {
                if (IsMember && item.MemberId == id)
                    removeModel.Add(item);
                else if (!IsMember && item.SupplierId == id)
                    removeModel.Add(item);
            }


            foreach (var item in removeModel)
            {
                this.PayModel.Remove(item);
            }

            krpcbBalancePay.Items.Remove(model);

            IsAddedMemberOrSupplier = false;
            ReloadBalanceList();
            Calc();

            if (IsCheckout)
                Recalc();
        }

        /// <summary>
        /// 根据扫码绑定会员
        /// </summary>
        /// <param name="code"></param>
        internal void OpenMemberByScanner(string code)
        {
            if (null == memberWindow && !this.IsAddedMemberOrSupplier && null != CurrentBalance && CurrentBalance.IsBalance)
            {
                this.BeginInvoke(new Action(() =>
                {
                    OpenAddMember(true, code);
                }));
                
            }

        }






        /// <summary>
        /// 加号直接打开支付里
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            //// 会员操作
            //if ((keyData == (Keys.F10 | Keys.Control)))
            //{
            //    krplMemberName_MouseClick(null, null);
            //}
            //else 
            if (keyData == Keys.F10)
            {
                if (!this.IsAddedMemberOrSupplier && null != CurrentBalance && CurrentBalance.IsBalance && Common.GetCommon().IsBindMemberByNo())
                {
                    btnMemberAdd_Click(null, null);
                }
                else if (this.IsAddedMemberOrSupplier && null != CurrentBalance && !CurrentBalance.IsBalance && CurrentBalance.IsChange)
                {
                    btnMemberRemove_Click(null, null);
                }
            }
            else if (Form.ModifierKeys == Keys.None && keyData == Keys.Enter && krptChangePrice.Text.Trim() != "0")
            {
                krpbSave_Click(null, null);
                return true;
            }
            else if (Form.ModifierKeys == Keys.Control && (krptChangePrice.Focused || krpcbBalancePay.Focused))
            {
                if (krpcbBalancePay.Items.Count > 1)
                {
                    if (krpcbBalancePay.SelectedIndex < krpcbBalancePay.Items.Count - 1)
                        krpcbBalancePay.SelectedIndex = krpcbBalancePay.SelectedIndex + 1;
                    else
                        krpcbBalancePay.SelectedIndex = 0;
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }



        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

        private void krprbAdd_Click(object sender, EventArgs e)
        {
            if (krprbAdd.Checked)
                Mark = "+";
            else if (krprbSub.Checked)
                Mark = "-";

            Calc();
        }

        private void krplMemberName_MouseClick(object sender, MouseEventArgs e)
        {
            if (krpcbBalancePay.Items.Count > 0 && null != CurrentBalance && CurrentBalance.IsBalance && null != CurrentBalance.Balance)
            {
                BalanceItemModel model = krpcbBalancePay.Items[krpcbBalancePay.Items.Count - 1] as BalanceItemModel;


                double changePrice = double.Parse(this.krptChangePrice.Text);

                if (changePrice != 0 && null != model && null != model.Member && model.IsChange)
                {
                    string memberName = "";
                    string balanceName = "";


                    if (Resources.GetRes().MainLangIndex == 0)
                        memberName = model.Member.MemberName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        memberName = model.Member.MemberName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        memberName = model.Member.MemberName2;


                    if (Resources.GetRes().MainLangIndex == 0)
                        balanceName = CurrentBalance.Balance.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        balanceName = CurrentBalance.Balance.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        balanceName = CurrentBalance.Balance.BalanceName2;




                    var confirm = KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("ConfirmAddPay"), Resources.GetRes().PrintInfo.PriceSymbol, ChangePrice, balanceName, memberName), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes)
                        return;


                    MemberPay memberpay = new MemberPay();


                    memberpay.Price = changePrice;
                    memberpay.BalanceId = CurrentBalance.Balance.BalanceId;
                    memberpay.MemberId = model.Member.MemberId;


                    // 开始支付
                    StartLoad(this, null);

                    Task.Factory.StartNew(() =>
                    {
                        ResultModel result = new ResultModel();
                        double originalBalancePrice = model.Member.BalancePrice;
                        try
                        {
                            // 更新会员信息
                            model.Member.BalancePrice = model.Member.BalancePrice + memberpay.Price;

                            Member newMember;
                            MemberPay newMemberPay;
                            result = OperatesService.GetOperates().ServiceAddMemberPay(model.Member, memberpay, out newMember, out newMemberPay);
                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);



                                    // 去掉当前最后一个会员并重新添加
                                    btnMemberRemove_Click(null, null);
                                    AddMember(newMember, null);
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
                                    // 失败了就复原会员信息
                                    if (result.Result)
                                        model.Member.BalancePrice = originalBalancePrice;

                                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                }), false, Resources.GetRes().GetString("SaveFailt"));
                            }));
                        }
                        StopLoad(this, null);

                    });
                }
                else if (changePrice != 0 && null != model && null != model.Supplier && model.IsChange)
                {
                    string supplierName = "";
                    string balanceName = "";


                    if (Resources.GetRes().MainLangIndex == 0)
                        supplierName = model.Supplier.SupplierName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        supplierName = model.Supplier.SupplierName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        supplierName = model.Supplier.SupplierName2;


                    if (Resources.GetRes().MainLangIndex == 0)
                        balanceName = CurrentBalance.Balance.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        balanceName = CurrentBalance.Balance.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        balanceName = CurrentBalance.Balance.BalanceName2;




                    var confirm = KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("ConfirmAddPay"), Resources.GetRes().PrintInfo.PriceSymbol, ChangePrice, balanceName, supplierName), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm != DialogResult.Yes)
                        return;


                    SupplierPay supplierpay = new SupplierPay();


                    supplierpay.Price = changePrice;
                    supplierpay.BalanceId = CurrentBalance.Balance.BalanceId;
                    supplierpay.SupplierId = model.Supplier.SupplierId;


                    // 开始支付
                    StartLoad(this, null);

                    Task.Factory.StartNew(() =>
                    {
                        ResultModel result = new ResultModel();
                        double originalBalancePrice = model.Supplier.BalancePrice;
                        try
                        {
                            // 更新供应者信息
                            model.Supplier.BalancePrice = model.Supplier.BalancePrice + supplierpay.Price;

                            Supplier newSupplier;
                            SupplierPay newSupplierPay;
                            result = OperatesService.GetOperates().ServiceAddSupplierPay(model.Supplier, supplierpay, out newSupplier, out newSupplierPay);
                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);



                                    // 去掉当前最后一个供应者并重新添加
                                    btnMemberRemove_Click(null, null);
                                    AddMember(null, newSupplier);
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
                                    // 失败了就复原供应者信息
                                    if (result.Result)
                                        model.Supplier.BalancePrice = originalBalancePrice;

                                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                }), false, Resources.GetRes().GetString("SaveFailt"));
                            }));
                        }
                        StopLoad(this, null);

                    });
                }
            }
        }

        private void krplChangePrice_MouseClick(object sender, MouseEventArgs e)
        {
            double lessPrice = Math.Round(TotalPrice - Math.Round(PayModel.Sum(x => x.OriginalPrice), 2), 2);

            if (lessPrice != 0)
            {
                if (lessPrice > 0)
                {
                    SetChangePrice(lessPrice, "+");
                }
                else if (lessPrice < 0)
                {
                    SetChangePrice(Math.Abs(lessPrice), "-");
                }

            }
        }
    }
}
