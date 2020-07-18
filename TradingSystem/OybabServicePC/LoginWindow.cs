using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Oybab.ServicePC.Pattern;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.ServicePC.DialogWindow;

namespace Oybab.ServicePC
{
    internal sealed partial class LoginWindow : KryptonForm
    {
        public LoginWindow()
        {
            InitializeComponent();
            krpManagerLogin.GlobalPaletteMode = PaletteModeManager.Custom;
            krpManagerLogin.GlobalPalette = PaletteBlue.GetSelf();
            //加载文字
            ConfigString.GetConfigString().Config();
            this.Visible = false;
            //加载配置文件
            Config.GetConfig().GetConfigs(true);

            this.Tag = "Main";
            SetLang();


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Login.ico"));
            krpbSetting.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangeSet.png"));
            krpbSetting.StateCommon.Back.ImageStyle = PaletteImageStyle.Stretch;
            krpbSetting.StateCommon.Back.Draw = InheritBool.True;

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptAdminNo.Location = new Point(krptAdminNo.Location.X, krptAdminNo.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptPassword.Location = new Point(krptPassword.Location.X, krptPassword.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }

            // 这个先在这里预先执行一下免得后续无法得到值
            this.GetMagnification();

            Common.GetCommon().ReadBak();

            krptAdminNo.Text = Resources.GetRes().LastLoginAdminNo;

            if (!string.IsNullOrWhiteSpace(krptAdminNo.Text))
                krptPassword.Select();


            this.Shown += (x, y) =>
            {
                if (!_isLoaded) {

                    _isLoaded = true;

                    this.WindowState = FormWindowState.Normal;

                    System.Windows.Size size = new System.Windows.Size();
                    System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromRectangle(new System.Drawing.Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height));

                    size.Width = screen.WorkingArea.Width;
                    size.Height = screen.WorkingArea.Height;
                    
                    Res.Resources.GetRes().setSize(size);

                }
                
            };

            
        }
        private bool _isLoaded = false;

        private void SetLang()
        {
            

            PaletteBlue.GetSelf().Reload();
            //重新加载文字
            ConfigString.GetConfigString().Config();


            //加载语言
            this.Font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            this.Text = Resources.GetRes().GetString("UserLogin");
            krplAdminNo.Text = Resources.GetRes().GetString("AdminNo");
            krplPassword.Text = Resources.GetRes().GetString("Password");
            krpbLogin.Text = Resources.GetRes().GetString("Login");

            krpbExit.Text = Resources.GetRes().GetString("Exit");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbLogin_Click(object sender, EventArgs e)
        {
            //先不让用户单击按钮
            krpbLogin.Enabled = false;
            krpbExit.Enabled = false;


            //判断是否空
            if (krptAdminNo.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplAdminNo.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (krptPassword.Text.Trim().Equals(""))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), krplPassword.Text), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
               
                //查询当前输入的密码
                try
                {
                    //检查路径和数据库
                    //获取密码

                    ResultModel result = Common.GetCommon().Load(krptAdminNo.Text.Trim(), krptPassword.Text.Trim());

                    //如果验证成功
                    //修改成功
                    if (result.Result)
                    {
                        if (!result.ValidResult)
                            this.Close();


                        Resources.GetRes().LastLoginAdminNo = krptAdminNo.Text;
                        Common.GetCommon().SetBak();

                        // 没有权限就不要进了
                        if (!Common.GetCommon().IsAllowPlatform(1))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("PermissionDenied"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.Close();
                            return;
                        }
                        // 如果快过期了则提示
                        else if (Resources.GetRes().ExpiredRemainingDays != -1 && Resources.GetRes().ExpiredRemainingDays <= 30)
                        {
                            var confirm = KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("SoftwareSoonExpired"), Resources.GetRes().ExpiredRemainingDays, Common.GetCommon().GetFormat()), Resources.GetRes().GetString("Login"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (confirm == DialogResult.Yes)
                            {
                                RegTimeWindow reg = new RegTimeWindow(false);
                                reg.Show();
                                lock (Program.TheLock)
                                {
                                    this.Tag = "NotMain";
                                    reg.Tag = "Main";
                                }
                                this.Hide();
                                return;
                            }
                        }
                        if (Resources.GetRes().ExpiredRemainingDays != -1 && Resources.GetRes().ExpiredRemainingDays <= 7)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("SoftwareExpiredWarn"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        if (result.IsAdminUsing)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("Exception_AdminUsing"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                       

                        MainWindow main = new MainWindow();
                        main.Show();

                        // 把这个从Main改为NotMain, 然后再把Main窗口的Tag改为Main, 这样就能防止打开多个实例
                        // 不上锁竟然还不行呢...
                        lock(Program.TheLock)
                        {
                            this.Tag = "NotMain";
                            main.Tag = "Main";
                        }
                        this.Hide();
                        return;
                    }
                    else
                    {
                        // 过期
                        if (result.IsExpired)
                        {
                            var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("SoftwareExpired"), Resources.GetRes().GetString("Login"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (confirm == DialogResult.Yes)
                            {
                                RegTimeWindow reg = new RegTimeWindow(false);
                                reg.Show();
                                this.Hide();
                                return;
                            }
                            else
                            {
                                this.Close();
                            }
                        }
                        //验证成功,只是登录失败
                        if (result.ValidResult)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Login")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        //验证失败
                        else
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("PwdError"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex,new Action<string>((message)=>{
                        KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                
            }
            krpbLogin.Enabled = true;
            krpbExit.Enabled = true;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 关闭应用程序
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Common.GetCommon().Close();
            System.Environment.Exit(0);
        }

        /// <summary>
        /// 回车登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptPassword_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                krpbLogin_Click(null, null);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
                

            
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSetting_Click(object sender, EventArgs e)
        {
            SettingsWindow setting = new SettingsWindow(true);
            setting.ChangeLang += (sender2, args) =>
            {
                SetLang();
            };
            setting.ShowDialog(this);
        }

    }
}
