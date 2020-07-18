using ComponentFactory.Krypton.Toolkit;
using Oybab.DAL;
using Oybab.Res;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class RequestListWindow : KryptonForm
    {
        public string ReturnValue { get; private set; } //返回值

        public RequestListWindow(string current)
        {
            InitializeComponent();
            Assembly asm = Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Request.ico"));

            this.Text = Resources.GetRes().GetString("RequestName");
            this.krptBtnSave.Text = Resources.GetRes().GetString("Save");

            string[] lists = null;


            if (Resources.GetRes().MainLangIndex == 0)
                lists = Resources.GetRes().Requests.Where(x => x.IsEnable == 1).Select(x => x.RequestName0).ToArray();
            else if (Resources.GetRes().MainLangIndex == 1)
                lists = Resources.GetRes().Requests.Where(x => x.IsEnable == 1).Select(x => x.RequestName1).ToArray();
            else if (Resources.GetRes().MainLangIndex == 2)
                lists = Resources.GetRes().Requests.Where(x => x.IsEnable == 1).Select(x => x.RequestName2).ToArray();
            

            krptcbList1.SetValues(lists, false);
            krptcbList2.SetValues(lists, false);
            krptcbList3.SetValues(lists, false);
            krptcbList4.SetValues(lists, false);
            krptcbList5.SetValues(lists, false);
           
          
            //填充列表
            if (!string.IsNullOrWhiteSpace(current))
            {
                string[] currentList = current.Split('&');

                if (currentList.Length > 0)
                {
                    krptcbList1.Text = currentList[0];
                }
                if (currentList.Length > 1)
                {
                    krptcbList2.Text = currentList[1];
                }
                if (currentList.Length > 2)
                {
                    krptcbList3.Text = currentList[2];
                }
                if (currentList.Length > 3)
                {
                    krptcbList4.Text = currentList[3];
                }
                if (currentList.Length > 4)
                {
                    krptcbList5.Text = currentList[4];
                }

            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptBtnSave_Click(object sender, EventArgs e)
        {
            List<string> requests = new List<string>();


            if (Resources.GetRes().Requests.Any(x => (Resources.GetRes().MainLangIndex == 0 ? x.RequestName0 == krptcbList1.Text : (Resources.GetRes().MainLangIndex == 1 ? x.RequestName1 == krptcbList1.Text : (Resources.GetRes().MainLangIndex == 2 ? x.RequestName2 == krptcbList1.Text : false)))))
            {
                requests.Add(krptcbList1.Text);
            }
            else if (!string.IsNullOrWhiteSpace(krptcbList1.Text))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("NotFoundProperty_Number"), krptlbSinger1.Text, Resources.GetRes().GetString("RequestName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Resources.GetRes().Requests.Any(x => (Resources.GetRes().MainLangIndex == 0 ? x.RequestName0 == krptcbList2.Text : (Resources.GetRes().MainLangIndex == 1 ? x.RequestName1 == krptcbList2.Text : (Resources.GetRes().MainLangIndex == 2 ? x.RequestName2 == krptcbList2.Text : false)))))
            {
                requests.Add(krptcbList2.Text);
            }
            else if (!string.IsNullOrWhiteSpace(krptcbList2.Text))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("NotFoundProperty_Number"), krptlbSinger2.Text, Resources.GetRes().GetString("RequestName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Resources.GetRes().Requests.Any(x => (Resources.GetRes().MainLangIndex == 0 ? x.RequestName0 == krptcbList3.Text : (Resources.GetRes().MainLangIndex == 1 ? x.RequestName1 == krptcbList3.Text : (Resources.GetRes().MainLangIndex == 2 ? x.RequestName2 == krptcbList3.Text : false)))))
            {
                requests.Add(krptcbList3.Text);
            }
            else if (!string.IsNullOrWhiteSpace(krptcbList3.Text))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("NotFoundProperty_Number"), krptlbSinger3.Text, Resources.GetRes().GetString("RequestName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Resources.GetRes().Requests.Any(x => (Resources.GetRes().MainLangIndex == 0 ? x.RequestName0 == krptcbList4.Text : (Resources.GetRes().MainLangIndex == 1 ? x.RequestName1 == krptcbList4.Text : (Resources.GetRes().MainLangIndex == 2 ? x.RequestName2 == krptcbList4.Text : false)))))
            {
                requests.Add(krptcbList4.Text);
            }
            else if (!string.IsNullOrWhiteSpace(krptcbList4.Text))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("NotFoundProperty_Number"), krptlbSinger4.Text, Resources.GetRes().GetString("RequestName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Resources.GetRes().Requests.Any(x => (Resources.GetRes().MainLangIndex == 0 ? x.RequestName0 == krptcbList5.Text : (Resources.GetRes().MainLangIndex == 1 ? x.RequestName1 == krptcbList5.Text : (Resources.GetRes().MainLangIndex == 2 ? x.RequestName2 == krptcbList5.Text : false)))))
            {
                requests.Add(krptcbList5.Text);
            }
            else if (!string.IsNullOrWhiteSpace(krptcbList5.Text))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("NotFoundProperty_Number"), krptlbSinger5.Text, Resources.GetRes().GetString("RequestName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            

            if (requests.Count == 0)
                ReturnValue = "";
            else
                ReturnValue = string.Join("&", requests.Distinct().ToArray());

            ReturnValue = ReturnValue.TrimEnd('&');

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            
        }
    }
}
