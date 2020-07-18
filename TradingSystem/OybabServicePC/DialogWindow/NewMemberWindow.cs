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
    internal sealed partial class NewMemberWindow : KryptonForm
    {

        public NewMemberWindow()
        {
            InitializeComponent();
           

            krpbAdd.Text = Resources.GetRes().GetString("Add");
            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptMemberNo.Location = new Point(krptMemberNo.Location.X, krptMemberNo.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            
            this.Text = Resources.GetRes().GetString("AddMember");
            krplMemberNo.Text = Resources.GetRes().GetString("MemberNo");
            krplCardNo.Text = Resources.GetRes().GetString("CardNo");
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.AddMember.ico"));
            
           


        }




        internal void ScanCardNo(string cardNo)
        {
            if (null != cardNo)
            {
                this.krptCardNo.Text = cardNo;
            }
        }



        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAdd_Click(object sender, EventArgs e)
        {


            //判断是否空
            if (krptMemberNo.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplMemberNo.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (krptCardNo.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplCardNo.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (null != AddMember)
                {
                    AddMember(new Tuple<string, string>(krptMemberNo.Text.Trim(), krptCardNo.Text.Trim()), null);
                }
            }
            

        }


        /// <summary>
        /// 添加会员
        /// </summary>
        public event EventHandler AddMember;

      

        private void krptMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbAdd_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        
    }
}
