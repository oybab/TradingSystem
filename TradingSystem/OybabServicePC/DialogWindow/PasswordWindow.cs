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
using Oybab.Res.Tools;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PasswordWindow : KryptonForm
    {
        public PasswordWindow()
        {
            InitializeComponent();
            this.Text = Resources.GetRes().GetString("ChangePassword");
            krplPassword.Text = Resources.GetRes().GetString("OldPassword");
            krplNewPassword.Text = Resources.GetRes().GetString("NewPassword");
            krplConfirmPassword.Text = Resources.GetRes().GetString("ConfirmPassword");
            krpbChangePassword.Text = Resources.GetRes().GetString("Change");
            krpbClose.Text = Resources.GetRes().GetString("Close");
            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptPassword.Location = new Point(krptPassword.Location.X, krptPassword.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2()));
                krptNewPassword.Location = new Point(krptNewPassword.Location.X, krptNewPassword.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")));
                krptConfirmPassword.Location = new Point(krptConfirmPassword.Location.X, krptConfirmPassword.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix"))).RecalcMagnification2());
            }
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangePassword.ico"));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChangePassword_Click(object sender, EventArgs e)
        {

            //判断是否空
            if (krptPassword.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplPassword.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (krptNewPassword.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplNewPassword.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (krptConfirmPassword.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplConfirmPassword.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!krptNewPassword.Text.Trim().Equals(krptConfirmPassword.Text.Trim()))
            {
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("TwoNewPasswordNotEqual"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (krptNewPassword.Text.Trim().Length < 6)
                {
                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("NewPasswordSizeLimit"), 6), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                StartLoad(this, null);


                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        
                        ResultModel result = OperatesService.GetOperates().ChangePWD(krptPassword.Text.Trim(), krptNewPassword.Text.Trim());

                        //如果验证成功
                        //修改成功
                        this.BeginInvoke(new Action(() =>
                        {
                            if (result.Result)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("ChangeSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Close();
                            }
                            else
                            {
                                //验证成功,修改失败
                                if (result.ValidResult)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("ChangeFaild"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                //验证失败
                                else
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("PwdError"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }));
                        }));
                    }
                    StopLoad(this, null);
                });
            }

        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbReset_Click(object sender, EventArgs e)
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
        /// 回车修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptConfirmPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbChangePassword_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
    }
}
