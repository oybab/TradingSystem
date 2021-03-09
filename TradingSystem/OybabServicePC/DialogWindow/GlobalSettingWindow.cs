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
    internal sealed partial class GlobalSettingWindow : KryptonForm
    {
        public PrintInfo ReturnValue { get; private set; } //返回值
        private PrintInfo printInfo;
        

        public GlobalSettingWindow(PrintInfo printInfo)
        {
            InitializeComponent();

            this.printInfo = printInfo;

            this.Text = Resources.GetRes().GetString("GlobalSetting");
            krplPriceSymbol.Text = Resources.GetRes().GetString("PriceSymbol");
            krpbChange.Text = Resources.GetRes().GetString("Change");
            krplMain.Text = Resources.GetRes().GetString("Main");
            krpSystemLanguage.Text = Resources.GetRes().GetString("SystemLanguage");
            krpCompanyName.Text = Resources.GetRes().GetString("CompanyName");


            string[] Names = Resources.GetRes().AllLangList.OrderBy(x => x.Value.LangOrder).Select(x => x.Value.LangName).ToArray();


            krpcLanguage0.Items.AddRange(Names);
            krpcLanguage1.Items.AddRange(Names);
            krpcLanguage2.Items.AddRange(Names);


            
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.PrintMessage.ico"));


            Init();


        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {

            krpt0.Text = Resources.GetRes().KEY_NAME_0;
            krpt1.Text = Resources.GetRes().KEY_NAME_1;
            krpt2.Text = Resources.GetRes().KEY_NAME_2;

            krpcLanguage0.SelectedItem = Resources.GetRes().GetMainLangByMainLangIndex(0).LangName;
            krpcLanguage1.SelectedItem = Resources.GetRes().GetMainLangByMainLangIndex(1).LangName;
            krpcLanguage2.SelectedItem = Resources.GetRes().GetMainLangByMainLangIndex(2).LangName;

            krptPriceSymbol.Text = Resources.GetRes().PrintInfo.PriceSymbol;

        }


        private void ReloadLanguageName()
        {
            if (null != krpcLanguage0.SelectedItem)
                krpl0.Text = krpcLanguage0.SelectedItem.ToString();
            if (null != krpcLanguage1.SelectedItem)
                krpl1.Text = krpcLanguage1.SelectedItem.ToString();
            if (null != krpcLanguage2.SelectedItem)
                krpl2.Text = krpcLanguage2.SelectedItem.ToString();
        }

        /// <summary>
        /// 修改过期时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChange_Click(object sender, EventArgs e)
        {
            //判断是否空
            if (krptPriceSymbol.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("PriceSymbol")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if ((krpcLanguage0.SelectedItem == krpcLanguage1.SelectedItem) || (krpcLanguage1.SelectedItem == krpcLanguage2.SelectedItem) || (krpcLanguage0.SelectedItem == krpcLanguage2.SelectedItem))
            {
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("SystemLanguageCantSame"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (krpt0.Text.Trim().Equals("") || krpt1.Text.Trim().Equals("") || krpt2.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("BillTitle")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; ;
            }

            else
            {
                string lang = "";
                lang = Resources.GetRes().GetLangByLangName(krpcLanguage0.SelectedItem.ToString()).LangIndex.ToString();
                lang = lang + "," + Resources.GetRes().GetLangByLangName(krpcLanguage1.SelectedItem.ToString()).LangIndex;
                lang = lang + "," + Resources.GetRes().GetLangByLangName(krpcLanguage2.SelectedItem.ToString()).LangIndex;

                
                if (lang != printInfo.MainLangList)
                {
                    var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("SystemLanguageChangeWarn"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.No)
                    {
                        return;
                    }
                }

                printInfo.MainLangList = lang;
                printInfo.PriceSymbol = krptPriceSymbol.Text.Trim();

                if (string.IsNullOrWhiteSpace(krpt0.Text))
                    printInfo.Name0 = null;
                else
                    printInfo.Name0 = krpt0.Text;

                if (string.IsNullOrWhiteSpace(krpt1.Text))
                    printInfo.Name1 = null;
                else
                    printInfo.Name1 = krpt1.Text;

                if (string.IsNullOrWhiteSpace(krpt2.Text))
                    printInfo.Name2 = null;
                else
                    printInfo.Name2 = krpt2.Text;



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





        

        private void krpcLanguage0_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadLanguageName();
        }
    }
}
