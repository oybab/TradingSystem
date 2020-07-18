using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Oybab.ServerManager.Exceptions;

namespace Oybab.ServerManager.Operate
{
    /// <summary>
    /// 检测U-KEY
    /// </summary>
    internal sealed class Detection : Form
    {
        private static Action Execute;
        private static DateTime CheckTime = DateTime.MinValue;//用来防止事件多次调用
        private static Detection detection;


       
        /// <summary>
        /// 启动
        /// </summary>
        internal static void Start(Action execute)
        {
            Detection.Execute = execute;
            Thread t = new Thread(runForm);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();

            
        }

       
        /// <summary>
        /// 停止
        /// </summary>
        internal static void Stop()
        {
            Execute = null;
            if (detection != null)
            {
                detection.Invoke(new MethodInvoker(detection.endForm));
            }
        }
        /// <summary>
        /// 运行
        /// </summary>
        private static void runForm()
        {
            Application.Run(new Detection());
        }

      


        /// <summary>
        /// 关闭
        /// </summary>
        private void endForm()
        {
            this.Close();
        }


        protected override void SetVisibleCore(bool value)
        {
            // 设置窗口
            if (detection == null) CreateHandle();
            detection = this;
            value = false;
            base.SetVisibleCore(value);
        }

        private bool isDeviceDetect;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x219)
            {
                // 插入U盘
                if (m.WParam.ToInt32() == 0x8000)
                {
                    isDeviceDetect = true;

                }
                // 拔出U盘(只在安全拔出时可用)
                else if (m.WParam.ToInt32() == 0x8004)
                {
                    isDeviceDetect = true;
                }

                if (DateTime.Now > CheckTime)
                {
                    CheckTime = DateTime.Now.AddSeconds(5);
                    new Action(() =>
                    {
                        System.Threading.Thread.Sleep(1000 * 5);
                        // 如果没检测到U盘插入就查KEY, 否则忽略
                        if (!isDeviceDetect)
                            Execute();
                        else
                            isDeviceDetect = false;
                    }).BeginInvoke(null, null);
                }

            }
            base.WndProc(ref m);
        }
    }
}
