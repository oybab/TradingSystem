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
    internal sealed partial class ExpiredTimeWindow : KryptonForm
    {
        public string ReturnValue { get; private set; } //返回值
        private bool IsNoLimit = false;
        public ExpiredTimeWindow(DateTime time, bool IsNoLimit)
        {
            InitializeComponent();
            this.IsNoLimit = IsNoLimit;

            this.Text = Resources.GetRes().GetString("ExpiredTime");
            krplExpiredTime.Text = Resources.GetRes().GetString("ExpiredTime");
            krpbChange.Text = Resources.GetRes().GetString("Change");

            krpcbIsNoLimit.Text = Resources.GetRes().GetString("Yes");
            krplNoLimit.Text = Resources.GetRes().GetString("NoLimit");
            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplExpiredTime.Location = new Point(krplExpiredTime.Location.X, krplExpiredTime.Location.Y - int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangeTime.ico"));

            if (IsNoLimit)
            {
                krpcbIsNoLimit.Checked = true;
                krptbExpired.Value = DateTime.Now;
                krptbExpired.Enabled = false;
            }
            else
            {
                krpcbIsNoLimit.Checked = false;
                krptbExpired.Value = time;
                krptbExpired.Enabled = true;
            }
            
        }

        /// <summary>
        /// 修改过期时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChange_Click(object sender, EventArgs e)
        {

            if (krpcbIsNoLimit.Checked)
            {
                ReturnValue = "";
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                DateTime endDateTime = DateTime.Now;

                try
                {
                    endDateTime = krptbExpired.Value;
                }
                catch (Exception ex)
                {
                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyError"), Resources.GetRes().GetString("ExpiredTime")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ExceptionPro.ExpLog(ex);
                    return;
                }


                ReturnValue = endDateTime.ToString("yyyy-MM-dd HH:mm");
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            

            
        }

        /// <summary>
        /// 回车修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptbExpired_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbChange_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// 切换无限制状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbIsNoLimit_CheckedChanged(object sender, EventArgs e)
        {
            if (krpcbIsNoLimit.Checked)
            {
                krptbExpired.Enabled = false;
            }
            else
            {
                krptbExpired.Enabled = true;
            }
        }

    }
}
