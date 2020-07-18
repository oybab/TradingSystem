using ComponentFactory.Krypton.Toolkit;
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
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Tools;
using System.Threading.Tasks;
using Oybab.Res.Server;
using Newtonsoft.Json;
using Oybab.Res.Server.Model;
using Oybab.ServerManager.Model.Models;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class AboutWindow : KryptonForm
    {
        public AboutWindow()
        {
            InitializeComponent();

            this.Text = Resources.GetRes().GetString("About");
            krplSoft.Text = Resources.GetRes().GetString("SoftServiceName");
            krplVersionNo.Text = Resources.GetRes().GetString("VersionNo");

            krplCompanyName.Text = Resources.GetRes().GetString("CompanyAbbreviation");
            krplLicense.Text = Resources.GetRes().GetString("License");
            krpbDonate.Text = Resources.GetRes().GetString("Donate");
            krpbSourceCode.Text = Resources.GetRes().GetString("SourceCode");

            krpbDisable.Text = Resources.GetRes().GetString("Enable");

            int day = Resources.GetRes().ExpiredRemainingDays;
            if (day > 99)
                day = 99;

            krplRemainingTime.Text = Resources.GetRes().GetString("RemainingDays");
            krplRemainingTimeValue.Text = string.Format(": {0}. ", day);

            krplDeviceCount.Text = Resources.GetRes().GetString("Device");
            krplDeviceCountValue.Text = string.Format(": {0}. ", Resources.GetRes().DeviceCount);

            krplTableCount.Text = Resources.GetRes().GetString("Table");
            krplTableCountValue.Text = string.Format(": {0}.", Resources.GetRes().RoomCount);


            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            krplVersion.Text = assembly.GetName().Version.ToString();//获取主版本号  

            assembly = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krppImage.StateCommon.Image = Image.FromStream(assembly.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC." + Resources.GetRes().GetString("CompanyLogoImage")));
            krppImage.StateCommon.ImageStyle = PaletteImageStyle.Stretch;
            this.Icon = new Icon(assembly.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.About.ico"));

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplVersion.Location = new Point(100.RecalcMagnification2(), krplVersion.Top);
            }


            krpbSourceCode.DoubleRightClick += krpbHelp_MouseDoubleClick;
            krplDeviceCountValue.DoubleRightClick += krpbLabel_MouseDoubleClick;
            krplTableCountValue.DoubleRightClick += krpbLabel_MouseDoubleClick;



           
            InitLock();
        }

        /// <summary>
        /// 打开官网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krplUrl_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:service@oybab.net");
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbDonate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://oybab.net/tradingsystem/donate/" + Resources.GetRes().MainLang.Culture.Name);
        }

        /// <summary>
        /// 打开帮助
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://oybab.net/tradingsystem/sourcecode/" + Resources.GetRes().MainLang.Culture.Name);
        }

        /// <summary>
        /// 打开注册时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbHelp_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RegTimeWindow reg = new RegTimeWindow(true);
            reg.StartLoad += (x, y) =>
            {
                this.StartLoad(x, null);
            };
            reg.StopLoad += (x, y) =>
            {
                this.StopLoad(x, null);
            };
            reg.ShowDialog(this.Parent);
        }



        /// <summary>
        /// 打开注册数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbLabel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RegCountWindow reg = new RegCountWindow(true);
            reg.StartLoad += (x, y) =>
            {
                this.StartLoad(x, null);
            };
            reg.StopLoad += (x, y) =>
            {
                this.StopLoad(x, null);
            };
            reg.ShowDialog(this.Parent);
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
        /// 初始化锁住
        /// </summary>
        /// <param name="value"></param>
        private void InitLock()
        {
            // 点歌系统就显示呼叫
            if (Resources.GetRes().ExtendInfo.SliderMode == "2")
            {

                krpbDisable.Text = Resources.GetRes().GetString("Disabled");
                krpbDisable.StateCommon.Content.ShortText.Color1 = Color.Red;
            }
            else
            {
                krpbDisable.Text = Resources.GetRes().GetString("Enabled");
                krpbDisable.StateCommon.Content.ShortText.Color1 = Color.Empty;
            }

        }


        /// <summary>
        /// 发送
        /// </summary>
        private void Send(List<long> RoomsId, string ErrMsgName, int SendType, ExtendInfo info, Action success = null)
        {

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {

                   

                    bool result = OperatesService.GetOperates().ServiceSend(RoomsId, SendType, JsonConvert.SerializeObject(info));

                    // 如果成功则提示
                    if (result)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), ErrMsgName), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (null != success)
                            {
                                success();
                            }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName));
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
        private void krpbDisable_Click(object sender, EventArgs e)
        {
            string ErrMsgName = Resources.GetRes().GetString("Disable");

            int sendType = 17; // ExtInfo

            ExtendInfo info = new ExtendInfo();

            

            if (Resources.GetRes().ExtendInfo.SliderMode == "2")
            {
                ErrMsgName = Resources.GetRes().GetString("Enable");

                info.SliderMode = "1";
            }
            else
            {
                info.SliderMode = "2";
            }


            var confirm = KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("ConfirmOperate"), ErrMsgName), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                // 所有设备ID都发送
                Send(Resources.GetRes().Devices.Select(x => x.DeviceId).ToList(), ErrMsgName, sendType, info, () =>
                {
                    InitLock();
                });
            }
        }


        private int DisableCount = 0;
        /// <summary>
        /// 打开禁用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krplSoft_MouseDown(object sender, MouseEventArgs e)
        {
            // 点歌系统就显示呼叫
            if (Resources.GetRes().IsRequired("Vod"))
            {
                if (DisableCount >= 100 || (DisableCount >= 3 && Resources.GetRes().ExtendInfo.SliderMode != "2"))
                    krpbDisable.Visible = true;
                ++DisableCount;
            }
                

        }

        /// <summary>
        /// 访问官网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krplCompanyWebsite_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://oybab.net/tradingsystem/company/" + Resources.GetRes().MainLang.Culture.Name);
        }

        private void krplLicense_LinkClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://oybab.net/tradingsystem/license/" + Resources.GetRes().MainLang.Culture.Name);
        }
    }


    internal class RButton : KryptonButton
    {
        public delegate void MouseDoubleRightClick(object sender, MouseEventArgs e);
        public event MouseDoubleRightClick DoubleRightClick;
        protected override void WndProc(ref Message m)
        {
            const Int32 WM_RBUTTONDBLCLK = 0x0206;
            if (m.Msg == WM_RBUTTONDBLCLK)
                DoubleRightClick(this, null);
            base.WndProc(ref m);
        }
    }



    internal class RLabel : KryptonLabel
    {
        public delegate void MouseDoubleRightClick(object sender, MouseEventArgs e);
        public event MouseDoubleRightClick DoubleRightClick;
        protected override void WndProc(ref Message m)
        {
            const Int32 WM_RBUTTONDBLCLK = 0x0206;
            if (m.Msg == WM_RBUTTONDBLCLK)
                DoubleRightClick(this, null);
            base.WndProc(ref m);
        }
    }
}
