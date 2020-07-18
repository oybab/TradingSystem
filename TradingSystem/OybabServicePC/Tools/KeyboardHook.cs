using Oybab.Res.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Oybab.ServicePC.Tools
{

    internal sealed class KeyPressedEventArgs : System.EventArgs
    {

        public KeyPressedEventArgs(string text)
        {
            mText = text;
        }
        public string Text { get { return mText; } }

        private readonly string mText;
    }
    internal sealed partial class KeyboardHook: IMessageFilter,  IDisposable
    {
        public bool PreFilterMessage(ref Message m)
        {


            if (m.Msg == KeyboardHook.WM_INPUT)
            {
                if (ProcessInputCommand(mHookDeviceId, m.LParam))
                {
                    MessageT message;

                    bool result = PeekMessage(out message, IntPtr.Zero, WM_KEYDOWN, WM_KEYUP, PM_REMOVE);

                    return result;
                }
            }

            return false;
        }


            private readonly Regex DeviceNamePattern = new Regex(@"#([^#]+)");
            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            /// <summary>
            /// Set the device to use in keyboard hook
            /// </summary>
            /// <param name="deviceId">Name of device</param>
            /// <returns>true if device is found</returns>
            public bool SetDeviceFilter(string deviceId)
            {

                Dictionary<string, IntPtr> devices = FindAllKeyboardDevices();
                return devices.TryGetValue(deviceId, out mHookDeviceId);
            }


        /// <summary>
        /// Add this KeyboardHook to a window
        /// </summary>
        /// <param name="window">The window to add to</param>
        public void AddHook(System.Windows.Forms.Form window)
            {
                if (window == null)
                    throw new ArgumentNullException("window");


                IntPtr hwnd = window.Handle;

                Application.AddMessageFilter(this);


                RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];

                rid[0].usUsagePage = 0x01;
                rid[0].usUsage = 0x06;
                rid[0].dwFlags = RIDEV_INPUTSINK;
                rid[0].hwndTarget = hwnd;

                if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
                    throw new ApplicationException("Failed to register raw input device(s).");
            }

            /// <summary>
            /// Remove this keyboard hook from window (if it is added)
            /// </summary>
            public void RemoveHook()
            {

                RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];

                rid[0].usUsagePage = 0x01;
                rid[0].usUsage = 0x06;
                rid[0].dwFlags = 0x00000001;
                rid[0].hwndTarget = IntPtr.Zero;

                RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0]));

                Application.RemoveMessageFilter(this);

            }

            public void Dispose()
            {
                RemoveHook();
            }



            private IntPtr mHookDeviceId;

            private const int WM_INPUT = 0x00FF;



            /// <summary>
            /// Get a list of keyboard devices available
            /// </summary>
            /// <returns>Collection of devices available</returns>
            public ICollection<string> GetKeyboardDevices()
            {
                return FindAllKeyboardDevices().Keys;
            }

            private Dictionary<string, IntPtr> FindAllKeyboardDevices()
            {
                Dictionary<string, IntPtr> deviceNames = new Dictionary<string, IntPtr>();
                uint deviceCount = 0;
                int dwSize = (Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

                if (GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
                {
                    IntPtr pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));

                    try
                    {
                        GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                        for (int i = 0; i < deviceCount; i++)
                        {
                            uint pcbSize = 0;

                            var rid = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                                                            new IntPtr((pRawInputDeviceList.ToInt32() + (dwSize * i))),
                                                            typeof(RAWINPUTDEVICELIST));
                            GetRawInputDeviceInfo(rid.hDevice, RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                            
                            if (pcbSize > 0)
                            {
                                IntPtr pData = Marshal.AllocHGlobal((int)pcbSize);
                                try
                                {
                                    uint result = GetRawInputDeviceInfo(rid.hDevice, RIDI_DEVICENAME, pData, ref pcbSize);
                                    string deviceName = Marshal.PtrToStringAnsi(pData);


                                    // The list will include the "root" keyboard and mouse devices
                                    // which appear to be the remote access devices used by Terminal
                                    // Services or the Remote Desktop - we're not interested in these
                                    // so the following code with drop into the next loop iteration
                                    if (deviceName.ToUpper().Contains("ROOT"))
                                        continue;


                                    // If the device is identified as a keyboard or HID device,
                                    // Check if it is the one we're looking for
                                    if (rid.dwType == RIM_TYPEKEYBOARD || rid.dwType == RIM_TYPEHID)
                                    {
                                        Match match = DeviceNamePattern.Match(deviceName);
                                        if (match.Success)
                                        {
                                            if (!deviceNames.ContainsKey(deviceName))
                                                deviceNames.Add(deviceName, rid.hDevice);


                                        }
                                    }
                                }
                                finally
                                {
                                    Marshal.FreeHGlobal(pData);
                                }
                            }
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pRawInputDeviceList);
                    }
                }
                return deviceNames;
            }

           

            /// <summary>
            /// Processes WM_INPUT messages to retrieve information about any
            /// keyboard events that occur.
            /// </summary>
            /// <param name="deviceId">Device to process</param>
            /// <param name="lParam">The WM_INPUT message to process.</param>
            private bool ProcessInputCommand(IntPtr deviceId, IntPtr lParam)
            {
                uint dwSize = 0;

                try
                {
                    // First call to GetRawInputData sets the value of dwSize
                    // dwSize can then be used to allocate the appropriate amount of memory,
                    // storing the pointer in "buffer".
                    GetRawInputData(lParam, RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));

                    IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
                    try
                    {
                        // Check that buffer points to something, and if so,
                        // call GetRawInputData again to fill the allocated memory
                        // with information about the input
                        if (buffer != IntPtr.Zero &&
                            GetRawInputData(lParam, RID_INPUT, buffer, ref dwSize, (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
                        {
                            // Store the message information in "raw", then check
                            // that the input comes from a keyboard device before
                            // processing it to raise an appropriate KeyPressed event.

                            RAWINPUT raw = (RAWINPUT)Marshal.PtrToStructure(buffer, typeof(RAWINPUT));

                            if (raw.header.hDevice != deviceId)
                                return false;

                            if (raw.header.dwType != RIM_TYPEKEYBOARD)
                                return false;


                        if (raw.keyboard.Message == WM_KEYUP && raw.keyboard.Message == WM_KEYUP)
                            return true;
                        if (raw.keyboard.Message != WM_KEYDOWN && raw.keyboard.Message != WM_SYSKEYDOWN)
                                return false;

                            // On most keyboards, "extended" keys such as the arrow or page 
                            // keys return two codes - the key's own code, and an "extended key" flag, which
                            // translates to 255. This flag isn't useful to us, so it can be
                            // disregarded.
                            if (raw.keyboard.VKey > VK_LAST_KEY)
                                return false;

                            if (KeyPressed != null)
                            {
                                string scannedText = null;
                                lock (mLocalBuffer)
                                {
                                    if (GetKeyboardState(mKeyboardState))
                                    {
                                        if (ToUnicode(raw.keyboard.VKey, raw.keyboard.MakeCode, mKeyboardState, mLocalBuffer, 64, 0) > 0)
                                        {
                                            if (mLocalBuffer.Length > 0)
                                            {
                                                scannedText = mLocalBuffer.ToString();
                                            }
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(scannedText))
                                    KeyPressed(this, new KeyPressedEventArgs(scannedText));
                            }
                            return true;
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(buffer);
                    }
                }
                catch (Exception err)
                {
                    ExceptionPro.ExpLog(err, null, false, "Scanner error");
                }
                return false;
            }
            private readonly StringBuilder mLocalBuffer = new StringBuilder(64);
            private readonly byte[] mKeyboardState = new byte[256];
        }

        internal sealed partial class KeyboardHook
        {
            private const int RIDEV_INPUTSINK = 0x00000100;
            private const int RIDEV_REMOVE = 0x00000001;
            private const int RID_INPUT = 0x10000003;

            private const int FAPPCOMMAND_MASK = 0xF000;
            private const int FAPPCOMMAND_MOUSE = 0x8000;
            private const int FAPPCOMMAND_OEM = 0x1000;

            private const int RIM_TYPEMOUSE = 0;
            private const int RIM_TYPEKEYBOARD = 1;
            private const int RIM_TYPEHID = 2;

            private const int RIDI_DEVICENAME = 0x20000007;

            private const int WM_KEYDOWN = 0x0100;
            private const int WM_KEYUP = 257;
            private const int WM_SYSKEYDOWN = 0x0104;
            //private const int WM_INPUT = 0x00FF;
            private const int VK_OEM_CLEAR = 0xFE;
            private const int VK_LAST_KEY = VK_OEM_CLEAR; // this is a made up value used as a sentinal

            private const int PM_REMOVE = 0x01;

            [StructLayout(LayoutKind.Sequential)]
            private struct RAWINPUTDEVICELIST
            {
                public IntPtr hDevice;

                [MarshalAs(UnmanagedType.U4)]
                public int dwType;
            }

            [StructLayout(LayoutKind.Explicit)]
            private struct RAWINPUT
            {
                [FieldOffset(0)]
                public RAWINPUTHEADER header;

                [FieldOffset(16)]
                public RAWMOUSE mouse;

                [FieldOffset(16)]
                public RAWKEYBOARD keyboard;

                [FieldOffset(16)]
                public RAWHID hid;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct RAWINPUTHEADER
            {
                [MarshalAs(UnmanagedType.U4)]
                public int dwType;

                [MarshalAs(UnmanagedType.U4)]
                public int dwSize;

                public IntPtr hDevice;

                [MarshalAs(UnmanagedType.U4)]
                public int wParam;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct RAWHID
            {
                [MarshalAs(UnmanagedType.U4)]
                public int dwSizHid;

                [MarshalAs(UnmanagedType.U4)]
                public int dwCount;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct BUTTONSSTR
            {
                [MarshalAs(UnmanagedType.U2)]
                public ushort usButtonFlags;

                [MarshalAs(UnmanagedType.U2)]
                public ushort usButtonData;
            }

            [StructLayout(LayoutKind.Explicit)]
            private struct RAWMOUSE
            {
                [MarshalAs(UnmanagedType.U2)]
                [FieldOffset(0)]
                public ushort usFlags;

                [MarshalAs(UnmanagedType.U4)]
                [FieldOffset(4)]
                public uint ulButtons;

                [FieldOffset(4)]
                public BUTTONSSTR buttonsStr;

                [MarshalAs(UnmanagedType.U4)]
                [FieldOffset(8)]
                public uint ulRawButtons;

                [FieldOffset(12)]
                public int lLastX;

                [FieldOffset(16)]
                public int lLastY;

                [MarshalAs(UnmanagedType.U4)]
                [FieldOffset(20)]
                public uint ulExtraInformation;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct RAWKEYBOARD
            {
                [MarshalAs(UnmanagedType.U2)]
                public ushort MakeCode;

                [MarshalAs(UnmanagedType.U2)]
                public ushort Flags;

                [MarshalAs(UnmanagedType.U2)]
                public ushort Reserved;

                [MarshalAs(UnmanagedType.U2)]
                public ushort VKey;

                [MarshalAs(UnmanagedType.U4)]
                public uint Message;

                [MarshalAs(UnmanagedType.U4)]
                public uint ExtraInformation;
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct RAWINPUTDEVICE
            {
                [MarshalAs(UnmanagedType.U2)]
                public ushort usUsagePage;

                [MarshalAs(UnmanagedType.U2)]
                public ushort usUsage;

                [MarshalAs(UnmanagedType.U4)]
                public int dwFlags;

                public IntPtr hwndTarget;
            }

            [DllImport("User32.dll")]
            private static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

            [DllImport("User32.dll")]
            private static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

            [DllImport("User32.dll")]
            private static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

            [DllImport("User32.dll")]
            private static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetKeyboardState(byte[] lpKeyState);
            [DllImport("user32.dll")]
            private static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)] StringBuilder pwszBuff,
                                                 int cchBuff, uint wFlags);
            [DllImport("user32")]
            private static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);





            [StructLayout(LayoutKind.Sequential)]
            public struct MessageT
            {
                public IntPtr hWnd;
                public uint Msg;
                public IntPtr wParam;
                public IntPtr lParam;
                public uint Time;
                public System.Drawing.Point Point;
            }

            [DllImport("User32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool PeekMessage(out MessageT message, IntPtr hWnd, uint filterMin, uint filterMax, uint flags);
        }


    
    
    
}
