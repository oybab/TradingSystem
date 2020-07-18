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
using Oybab.ServerManager.Model.Models;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PrintInfoWindow : KryptonForm
    {
        public PrintInfo ReturnValue { get; private set; } //返回值
        private PrintInfo printInfo;

        public PrintInfoWindow(PrintInfo printInfo)
        {
            InitializeComponent();

            this.printInfo = printInfo;

            this.Text = krplPrintMessage.Text =  Resources.GetRes().GetString("PrintInfo");
            KrplOrderPhoneNo.Text = Resources.GetRes().GetString("OrderPhoneNo");
            krpbChange.Text = Resources.GetRes().GetString("Change");
            krpcbMultipleLanguage.Text = Resources.GetRes().GetString("MultiLanguage");

            krplIsPrintAfterBuy.Text = Resources.GetRes().GetString("PrintAfterBuy");
            krplIsPrintAfterCheckout.Text = Resources.GetRes().GetString("PrintAfterCheckout");
            krpcIsPrintAfterBuy.Text = krpcIsPrintAfterCheckout.Text = Resources.GetRes().GetString("Yes");

            List<string> Names = new List<string>();
            Names.AddRange(Resources.GetRes().MainLangList.Select(x => x.Value.LangName).ToArray());

            
            krpl0.Text = Names[0];
            krpl1.Text = Names[1];
            krpl2.Text = Names[2];


            krpOther.Text = Resources.GetRes().GetString("Other");
            krplPageHeight.Text = string.Format("{0} {1}", Resources.GetRes().GetString("BillWide"), Resources.GetRes().GetString("PageHeight"));


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.PrintMessage.ico"));


            Init();

            krpcbMultipleLanguage_CheckedChanged(null, null);
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {

            krpcIsPrintAfterBuy.Checked = this.printInfo.IsPrintBillAfterBuy;
            krpcIsPrintAfterCheckout.Checked = this.printInfo.IsPrintBillAfterCheckout;

            if (!string.IsNullOrWhiteSpace(printInfo.Phone))
            {
                krptPhoneNo.Text = printInfo.Phone;

                krpt1.Text = printInfo.Msg1;
                krpt2.Text = printInfo.Msg2;
                krpt0.Text = printInfo.Msg0;

            }else
            {
                krptPhoneNo.Text = "";



                krpt1.Text = "";
                krpt2.Text = "";
                krpt0.Text = "";
            }


            if (printInfo.PageHeight == 1400)
                krpcPageHeight.SelectedIndex = 1;
            else if (printInfo.PageHeight == 1300)
                krpcPageHeight.SelectedIndex = 0;
            else
                krpcPageHeight.SelectedIndex = 1;

        }

        /// <summary>
        /// 修改过期时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChange_Click(object sender, EventArgs e)
        {
            //判断是否空
            if (krptPhoneNo.Text.Trim().Equals("") && (!string.IsNullOrWhiteSpace(krpt2.Text) || !string.IsNullOrWhiteSpace(krpt0.Text) || !string.IsNullOrWhiteSpace(krpt1.Text)))
            {
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("InputPhoneNoFirst"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {

                // 隐藏功能时先把必要的复制掉(比如语言)
                CopyForHide(krpt0, krpt1, krpt2, false, Ext.AllSame(printInfo.Msg0, printInfo.Msg1, printInfo.Msg2));


                printInfo.IsPrintBillAfterBuy = krpcIsPrintAfterBuy.Checked;
                printInfo.IsPrintBillAfterCheckout = krpcIsPrintAfterCheckout.Checked;


                printInfo.Phone = GetValueOrNull(krptPhoneNo.Text);

                printInfo.Msg2 = GetValueOrNull(krpt2.Text);
                printInfo.Msg1 = GetValueOrNull(krpt1.Text);
                printInfo.Msg0 = GetValueOrNull(krpt0.Text);

                if (krpcPageHeight.SelectedIndex == 1)
                    printInfo.PageHeight = 1400;
                else if (krpcPageHeight.SelectedIndex == 0)
                    printInfo.PageHeight = 1300;

                this.ReturnValue = printInfo;

                this.DialogResult = DialogResult.OK;

                this.Close();
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
        /// 隐藏功能时复制名称
        /// </summary>
        private void CopyForHide(KryptonTextBox Cell0, KryptonTextBox Cell1, KryptonTextBox Cell2, bool IsNew, bool IsSameBefore)
        {
            if (!krpcbMultipleLanguage.Checked)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                {
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell1.Text.ToString())) || (!IsNew && IsSameBefore))
                        Cell1.Text = Cell0.Text;
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell2.Text.ToString())) || (!IsNew && IsSameBefore))
                        Cell2.Text = Cell0.Text;
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell0.Text.ToString())) || (!IsNew && IsSameBefore))
                        Cell0.Text = Cell1.Text;
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell2.Text.ToString())) || (!IsNew && IsSameBefore))
                        Cell2.Text = Cell1.Text;
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell0.Text.ToString())) || (!IsNew && IsSameBefore))
                        Cell0.Text = Cell2.Text;
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell1.Text.ToString())) || (!IsNew && IsSameBefore))
                        Cell1.Text = Cell2.Text;
                }
            }
        }



        /// <summary>
        /// 显示多语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbMultipleLanguage_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbMultipleLanguage.Checked)
            {
                fl2.Visible = false;
                fl0.Visible = false;
                fl1.Visible = false;

            
                if (Resources.GetRes().MainLangIndex == 0)
                {
                    fl0.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    fl1.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    fl2.Visible = true;
                }

                flOther.Size = new System.Drawing.Size(this.flOther.Size.Width, this.flOther.Size.Height - 160);
                krpOther.Size = new System.Drawing.Size(this.krpOther.Size.Width, this.krpOther.Size.Height - 160);
                this.Size = new System.Drawing.Size(this.Width, this.Height - 160);
            }
            else
            {
                fl2.Visible = true;
                fl0.Visible = true;
                fl1.Visible = true;


                flOther.Size = new System.Drawing.Size(this.flOther.Size.Width, this.flOther.Size.Height + 160);
                krpOther.Size = new System.Drawing.Size(this.krpOther.Size.Width, this.krpOther.Size.Height + 160);
                this.Size = new System.Drawing.Size(this.Width, this.Height + 160);
            }
        }
    }
}
