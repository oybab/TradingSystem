using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Oybab.ServicePC;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Tools;

namespace Oybab.ServicePC
{
    static class Program
    {

        private static System.Threading.Mutex mutex;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool createdNew;
            mutex = new System.Threading.Mutex(true, Res.Resources.GetRes().GetSoftServicePCName(), out createdNew);
            System.Threading.EventWaitHandle eventWaitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset, Res.Resources.GetRes().GetSoftServicePCName() + Res.Resources.GetRes().GetSoftServicePCName());



            if (createdNew)
            {

                // Spawn a thread which will be waiting for our event
                var thread = new System.Threading.Thread(
                    () =>
                    {
                        while (eventWaitHandle.WaitOne())
                        {
                            lock (TheLock)
                            {
                                Form defaultForm = Application.OpenForms.Cast<Form>().Where(x => null != x.Tag && x.Tag.ToString() == "Main").FirstOrDefault();

                                if (null != defaultForm)
                                    defaultForm.BeginInvoke((Action)(() => BringToForeground(defaultForm)));
                            }
                        }
                    });

                // It is important mark it as background otherwise it will prevent app from exiting.
                thread.IsBackground = true;

                thread.Start();

                //注册错误
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                //加载日志
                Resources.GetRes().LoadLog();
                //字体效果(为XP开启)
                FontSmoothing.OpenFontEffect();

                Resources.GetRes().SetDeviceType((long)1);
                Application.Run(new LoginWindow());
                mutex.ReleaseMutex();
            }
            // Notify other instance so it could bring itself to foreground.
            eventWaitHandle.Set();

            // Terminate this instance.
            Environment.Exit(0);
        }

        internal static object TheLock = new object();


        /// <summary>Brings main window to foreground.</summary>
        private static void BringToForeground(Form window)
        {
            if (window.WindowState == FormWindowState.Minimized || window.Visible == false)
            {
                window.Show();
                window.WindowState = FormWindowState.Normal;
            }

            // According to some sources these steps gurantee that an app will be brought to foreground.
            window.Activate();
            window.TopMost = true;
            window.TopMost = false;
            window.Focus();
        }





        /// <summary>
        /// 处理不可预期的错误(WindowUI)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ExceptionPro.ExpLog(e.Exception, new Action<string>((message) =>
            {
                KryptonMessageBox.Show(Form.ActiveForm, message, Resources.GetRes().GetString("ErrorBig"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        /// <summary>
        /// 处理不可预期的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionPro.ExpLog(e.ExceptionObject as Exception, new Action<string>((message) =>
            {
                KryptonMessageBox.Show(Form.ActiveForm, message, Resources.GetRes().GetString("ErrorBig"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

    }
}
