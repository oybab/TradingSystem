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
    internal sealed partial class BalanceTransferWindow : KryptonForm
    {
        public bool ReturnValue { get; private set; } //返回值
        private Balance OldBalance;
        private Balance NewBalance;



        public BalanceTransferWindow(long oldBalanceId)
        {
            OldBalance = Resources.GetRes().Balances.Where(x => x.BalanceId == oldBalanceId).FirstOrDefault();
            

            
            InitializeComponent();





            this.Text = Resources.GetRes().GetString("BalanceTransfer");

            krplNewBalanceName.Text = Resources.GetRes().GetString("NewBalance");
            krplOldBalanceName.Text = Resources.GetRes().GetString("OldBalance");
            krplChangePrice.Text = Resources.GetRes().GetString("ChangePrice");

            krpbSave.Text = Resources.GetRes().GetString("Save");

           
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ReplaceRoom.ico"));



            if (Resources.GetRes().MainLangIndex == 0)
                krplOldBalanceNameValue.Text = OldBalance.BalanceName0;
            else if (Resources.GetRes().MainLangIndex == 1)
                krplOldBalanceNameValue.Text = OldBalance.BalanceName1;
            else if (Resources.GetRes().MainLangIndex == 2)
                krplOldBalanceNameValue.Text = OldBalance.BalanceName2;

            ReloadBalances();


            krptChangePrice.Text = "0";

        }


        /// <summary>
        /// 加载余额列表
        /// </summary>
        private void ReloadBalances()
        {
            if (Resources.GetRes().Balances.Count() > 1)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krpcbNewBalanceName.Items.AddRange(Resources.GetRes().Balances.Where(x => x.BalanceId != OldBalance.BalanceId).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).Select(x => x.BalanceName0).ToArray());
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcbNewBalanceName.Items.AddRange(Resources.GetRes().Balances.Where(x=>x.BalanceId != OldBalance.BalanceId).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).Select(x => x.BalanceName1).ToArray());
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcbNewBalanceName.Items.AddRange(Resources.GetRes().Balances.Where(x => x.BalanceId != OldBalance.BalanceId).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).Select(x => x.BalanceName2).ToArray());
            }

            if (krpcbNewBalanceName.Items.Count > 0)
                krpcbNewBalanceName.SelectedIndex = 0;
            else
                krpbSave.Enabled = false;

        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSave_Click(object sender, EventArgs e)
        {
            double price = double.Parse(krptChangePrice.Text);

            if (price <= 0)
                return;

            string newBalanceName = krpcbNewBalanceName.SelectedItem.ToString();

            if (Resources.GetRes().MainLangIndex == 0)
                NewBalance = Resources.GetRes().Balances.Where(x => x.BalanceName0 == newBalanceName).FirstOrDefault();
            else if (Resources.GetRes().MainLangIndex == 1)
                NewBalance = Resources.GetRes().Balances.Where(x => x.BalanceName1 == newBalanceName).FirstOrDefault();
            else if (Resources.GetRes().MainLangIndex == 2)
                NewBalance = Resources.GetRes().Balances.Where(x => x.BalanceName2 == newBalanceName).FirstOrDefault();

            

            BalancePay balancePay1 = new BalancePay();
            balancePay1.BalanceId = NewBalance.BalanceId;
            balancePay1.Price = price;

            BalancePay balancePay2 = new BalancePay();
            balancePay2.BalanceId = OldBalance.BalanceId;
            balancePay2.Price = -price;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                ResultModel result = new ResultModel();
                try
                {
                    BalancePay newBalancePay1;
                    BalancePay newBalancePay2;
                    Balance newBalance1;
                    Balance newBalance2;
                    result = OperatesService.GetOperates().ServiceTransferBalancePay(balancePay1, balancePay2, out newBalancePay1, out newBalance1, out newBalancePay2, out newBalance2);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ReturnValue = true;
                            this.Close();
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
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }), false, Resources.GetRes().GetString("SaveFailt"));
                    }));
                }
                StopLoad(this, null);
            });
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
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

       
        /// <summary>
        /// 火车保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptChangePrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && krpbSave.Enabled)
            {
                krpbSave_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,2})?$");

        private string ChangePrice = "0";
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


        }
    }
}
