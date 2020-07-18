using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Oybab.Res.Tools
{
    /// <summary>
    /// 任务栏
    /// </summary>
    public class Taskbar
    {
         #region Instance
        private Taskbar() { }
        private static readonly Lazy<Taskbar> lazy = new Lazy<Taskbar>(() => new Taskbar());
        public static Taskbar Instance { get { return lazy.Value; } }
        #endregion Instance

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumThreadWindows(int threadId, EnumThreadProc pfnEnum, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parentHwnd, IntPtr childAfterHwnd, IntPtr className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out int lpdwProcessId);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        private const string VistaStartMenuCaption = "Start";
        private static IntPtr vistaStartMenuWnd = IntPtr.Zero;
        private delegate bool EnumThreadProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// 显示任务栏
        /// </summary>
        public void Show()
        {
            if (Resources.GetRes().AutoHideTaskbar)
                SetState(true);
        }

        /// <summary>
        /// 隐藏任务栏
        /// </summary>
        public void Hide()
        {
            if (Resources.GetRes().AutoHideTaskbar)
                SetState(false);
        }


        private void SetState(bool IsShow)
        {
			// 获取任务栏窗口
			IntPtr taskBarWnd = FindWindow("Shell_TrayWnd", null);

			// 先尝试WinXp模式
			IntPtr startWnd = FindWindowEx(taskBarWnd, IntPtr.Zero, "Button", "Start");

            if (startWnd == IntPtr.Zero)
            {
                // 尝试另外一种模式, 来自 CodeProject by Earl Waylon Flinn
                startWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, "Start");
            }

			if (startWnd == IntPtr.Zero)
			{
				// 好,现在尝试WinVista简单模式...
				startWnd = FindWindow("Button", null);

				if (startWnd == IntPtr.Zero)
				{
					// 无改动, 我们则需要复杂模式...
					startWnd = GetVistaStartMenuWnd(taskBarWnd);
				}
			}
			
			ShowWindow(taskBarWnd, IsShow ? SW_SHOW : SW_HIDE);
			ShowWindow(startWnd, IsShow ? SW_SHOW : SW_HIDE);
		}

        /// <summary>
        /// 返回Vista以上系统的窗口句柄
        /// </summary>
        /// <param name="taskBarWnd"></param>
        /// <returns></returns>
        private IntPtr GetVistaStartMenuWnd(IntPtr taskBarWnd)
        {
            // 获取拥有开始菜单的进程
            int procId;
            GetWindowThreadProcessId(taskBarWnd, out procId);

            Process p = Process.GetProcessById(procId);
            if (p != null)
            {
                // 枚举进程所有线程
                foreach (ProcessThread t in p.Threads)
                {
                    EnumThreadWindows(t.Id, MyEnumThreadWindowsProc, IntPtr.Zero);
                }
            }
            return vistaStartMenuWnd;
        }

        /// <summary>
        /// 回调方法调用来自 'GetVistaStartMenuWnd' - 'EnumThreadWindows'.
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="lParam">参数</param>
        /// <returns>返回真继续枚举,假则停止</returns>
        private bool MyEnumThreadWindowsProc(IntPtr hWnd, IntPtr lParam)
        {
            StringBuilder buffer = new StringBuilder(256);
            if (GetWindowText(hWnd, buffer, buffer.Capacity) > 0)
            {
                Console.WriteLine(buffer);
                if (buffer.ToString() == VistaStartMenuCaption)
                {
                    vistaStartMenuWnd = hWnd;
                    return false;
                }
            }
            return true;
        }

    }
}
