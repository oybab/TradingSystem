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
    internal sealed partial class RegTimeWindow : KryptonForm
    {
        private bool IsModalMode = false;
        public RegTimeWindow(bool IsModalMode)
        {
            this.IsModalMode = IsModalMode;
            InitializeComponent();
            if (IsModalMode)
                this.StartPosition = FormStartPosition.CenterParent;

            this.Text = Resources.GetRes().GetString("Register");
            krplRequestCode.Text = Resources.GetRes().GetString("MachineNo");
            krplRegCode.Text = Resources.GetRes().GetString("RegisterNo");
            krpbReg.Text = Resources.GetRes().GetString("Register");
            krpbClose.Text = Resources.GetRes().GetString("Close");
            krpbHelp.Text = Resources.GetRes().GetString("Help");
            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptRegCode.Location = new Point(krptRegCode.Location.X, krptRegCode.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.RegTime.ico"));

            krplRequestCodeValue.Text = "0";

            this.Shown += (sender, args) =>
            {
                if (!IsInit)
                {
                    IsInit = true;
                    Init();
                }
            };

        }

        private bool IsInit = false;

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {

            if (string.IsNullOrWhiteSpace(Resources.GetRes().RegTimeRequestCode))
            {

                if (IsModalMode)
                    StartLoad(this, null);
                else
                {
                    //先不让用户单击按钮
                    krpbReg.Enabled = false;
                    krpbClose.Enabled = false;
                    krpbHelp.Enabled = false;
                }

                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        bool result = OperatesService.GetOperates().ServiceRequestTimeCode();

                        //如果验证成功
                        //修改成功
                        this.BeginInvoke(new Action(() =>
                        {
                            if (result)
                            {
                                krplRequestCodeValue.Text = Resources.GetRes().RegTimeRequestCode;
                            }
                            else
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("MachineNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (IsModalMode)
                        StopLoad(this, null);
                    else
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            krpbReg.Enabled = true;
                            krpbClose.Enabled = true;
                            krpbHelp.Enabled = true;
                        }));
                    }

                });
            }
            else
            {
                krplRequestCodeValue.Text = Resources.GetRes().RegTimeRequestCode;
            }
        }

        /// <summary>
        /// 注册时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbReg_Click(object sender, EventArgs e)
        {
            

            //判断是否空
            if (krptRegCode.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplRegCode.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (IsModalMode)
                    StartLoad(this, null);
                else
                {
                    //先不让用户单击按钮
                    krpbReg.Enabled = false;
                    krpbClose.Enabled = false;
                    krpbHelp.Enabled = false;
                }


                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        bool result = OperatesService.GetOperates().ServiceRegTime(krptRegCode.Text.Trim());

                        //如果验证成功
                        //修改成功
                        this.BeginInvoke(new Action(() =>
                        {
                            if (result)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("RegisterSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                System.Environment.Exit(0);
                            }
                            else
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Register")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (IsModalMode)
                        StopLoad(this, null);
                    else
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            krpbReg.Enabled = true;
                            krpbClose.Enabled = true;
                            krpbHelp.Enabled = true;
                        }));
                    }
                });
            }

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
        /// 关闭
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!IsModalMode)
            {
                Common.GetCommon().Close();
                System.Environment.Exit(0);
            }
        }



        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

        private void krpbHelp_Click(object sender, EventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "help.chm")))
                    System.Diagnostics.Process.Start(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "help.chm"), "联系我们.htm");
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }

        /// <summary>
        /// 回车注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptRegCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbReg_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
                
        }
    }
}
