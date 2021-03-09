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

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PrintLanguageWindow : KryptonForm
    {
        public long ReturnValue { get; private set; } //返回值
        public PrintLanguageWindow()
        {
            InitializeComponent();
            this.Text = Resources.GetRes().GetString("Print");
            
            krpbPrint.Text = Resources.GetRes().GetString("Print");
            krplLanguage.Text = Resources.GetRes().GetString("Language");
            krpcLanguage.Items.AddRange(Resources.GetRes().AllLangList.OrderBy(x => x.Value.LangOrder).Select(x => x.Value.LangName).ToArray());

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Print.ico"));


            if (Resources.GetRes().DefaultPrintLang == -1)
            {
                krpcLanguage.SelectedItem = Resources.GetRes().MainLang.LangName;
            }
            else
            {
                krpcLanguage.SelectedIndex = Resources.GetRes().DefaultPrintLang;
            }
            



        }

        /// <summary>
        /// 选择打印语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChange_Click(object sender, EventArgs e)
        {
            ReturnValue = Resources.GetRes().GetMainLangByLangName(krpcLanguage.SelectedItem.ToString()).LangIndex;
            Resources.GetRes().DefaultPrintLang = krpcLanguage.SelectedIndex;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 回车修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcLanguage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbChange_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

    }
}
