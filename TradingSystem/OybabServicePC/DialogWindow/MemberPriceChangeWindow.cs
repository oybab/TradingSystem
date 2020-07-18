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

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class MemberPriceChangeWindow : KryptonForm
    {
        private string Mark = "+";
        private double TotalPrice = 0;
        private double PaidPrice = 0;
        private double MemberBalancePrice = 0;
        private string ChangePrice = "0";
        private bool IsMember = false;
        private long IsAllowBorrow = 0;

        public double ReturnValue { get; private set; } //返回值

        public MemberPriceChangeWindow(string mark, double TotalPrice, double paidPrice, double MemberBalancePrice, long IsAllowBorrow, bool IsMember = true, bool IsReturn = false)
        {
            InitializeComponent();
            this.krptChangePrice.LostFocus += krptChangePrice_LostFocus;


            if (IsReturn)
                MemberBalancePrice = MemberBalancePrice + paidPrice;
            else
                MemberBalancePrice = MemberBalancePrice - paidPrice;

            this.Mark = mark;
            this.TotalPrice = TotalPrice;
            this.PaidPrice = paidPrice;
            this.MemberBalancePrice = MemberBalancePrice;
            this.IsAllowBorrow = IsAllowBorrow;
            this.IsMember = IsMember;
            if (Mark == "+")
                krplChangeMark.Text = "+";
            else if (Mark == "-")
                krplChangeMark.Text = "-";


            this.krplTotalPriceValue.Text = this.TotalPrice.ToString();
            this.krplPaidPriceValue.Text = this.PaidPrice.ToString();
            this.krptChangePrice.Text = "0";
            this.krplMemberBalancePriceValue.Text = this.MemberBalancePrice.ToString();


            this.Text = Resources.GetRes().GetString("ChangePrice");

            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");
            
            if (IsMember)
                krplMemberBalancePrice.Text = Resources.GetRes().GetString("MemberBalancePrice");
            else
                krplMemberBalancePrice.Text = Resources.GetRes().GetString("SupplierBalancePrice");

            krplPaidPrice.Text = Resources.GetRes().GetString("PaidPrice");
            krplChangePrice.Text = Resources.GetRes().GetString("ChangePrice");
            krplBalancePrice.Text = Resources.GetRes().GetString("BalancePrice");
            krpbSave.Text = Resources.GetRes().GetString("Save");

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptChangePrice.Location = new Point(krptChangePrice.Location.X, krptChangePrice.Location.Y - int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangePrice.ico"));


            Calc();


        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSave_Click(object sender, EventArgs e)
        {
            if (Mark == "+" && IsAllowBorrow == 0 && MemberBalancePrice < Math.Round((PaidPrice + Math.Round(double.Parse(ChangePrice), 2)), 2))
            {
                if (IsMember)
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("MemberBalanceNotEnough"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                else
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SupplierBalanceNotEnough"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Mark == "+")
                ReturnValue = Math.Round((PaidPrice + Math.Round(double.Parse(ChangePrice), 2)), 2);
            else if (Mark == "-")
                ReturnValue = Math.Round((PaidPrice - Math.Round(double.Parse(ChangePrice), 2)), 2);

            // 去掉的钱不能超出总支付金额
            if (Mark == "-")
            {
                double changePriceDouble = double.Parse(ChangePrice);
                if (changePriceDouble > PaidPrice)
                {
                    ChangePrice = PaidPrice.ToString();
                    ChangePrice = krptChangePrice.Text = PaidPrice.ToString();
                    krptChangePrice.SelectAll();

                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("CanNotExceed"), PaidPrice), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
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

        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbSave_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
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
            double balancePrice = 0;
            if (Mark == "+")
                krplBalancePriceValue.Text = (balancePrice = Math.Round((PaidPrice + Math.Round(double.Parse(ChangePrice), 2)) - TotalPrice, 2)).ToString();
            else if (Mark == "-")
            {

                krplBalancePriceValue.Text = (balancePrice = Math.Round((PaidPrice - Math.Round(double.Parse(ChangePrice), 2)) - TotalPrice, 2)).ToString();
            }


            if (balancePrice > 0)
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Blue;
            else if (balancePrice < 0)
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Red;
            else
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Empty;
        }

    }
}
