using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Oybab.Report
{
    internal sealed class PrinterMsg
    {
        #region Instance
        private PrinterMsg() { }
        private static readonly Lazy<PrinterMsg> lazy = new Lazy<PrinterMsg>(() => new PrinterMsg());
        internal static PrinterMsg Instance { get { return lazy.Value; } }
        #endregion Instance


        internal void SendSocketMsg(String ip, int port, int times, byte[] data)
        {
            byte[] mData;
            if (times == 1)
            {
                mData = new byte[data.Length];
                Array.Copy(data, 0, mData, 0, data.Length);
            }
            else
            {
                mData = new byte[data.Length * times];
                byte[][] m = new byte[times][];
                for (int i = 0; i < times; i++)
                {
                    m[i] = data;
                }
                Array.Copy(PrinterCmdUtils.Instance.byteMerger(m), 0, mData, 0, PrinterCmdUtils.Instance.byteMerger(m).Length);
            }

            #region 同步 Socket
            Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);

            mySocket.Connect(ipEndPoint);

            mySocket.Send(mData);
            mySocket.Close();
            #endregion

        }
    }



    /// <summary>
    /// 通过网络
    /// </summary>
    internal sealed class PrinterCmdUtils
    {

        #region Instance
        private PrinterCmdUtils() { }
        private static readonly Lazy<PrinterCmdUtils> lazy = new Lazy<PrinterCmdUtils>(() => new PrinterCmdUtils());
        internal static PrinterCmdUtils Instance { get { return lazy.Value; } }
        #endregion Instance

        internal const byte ESC = 27;    // 换码
        internal const byte FS = 28;    // 文本分隔符
        internal const byte GS = 29;    // 组分隔符
        internal const byte DLE = 16;    // 数据连接换码
        internal const byte EOT = 4;    // 传输结束
        internal const byte ENQ = 5;    // 询问字符
        internal const byte SP = 32;    // 空格
        internal const byte HT = 9;    // 横向列表
        internal const byte LF = 10;    // 打印并换行（水平定位）
        internal const byte CR = 13;    // 归位键
        internal const byte FF = 12;    // 走纸控制（打印并回到标准模式（在页模式下） ）
        internal const byte CAN = 24;    // 作废（页模式下取消打印数据 ）

        /**
         * 打印纸一行最大的字节
         */
        private const int LINE_BYTE_SIZE = 32;
        /**
         * 分隔符
         */
        private const String SEPARATOR = "$";
        private StringBuilder sb = new StringBuilder();

        /**
         * 打印机初始化
         * 
         * @return
         */
        internal byte[] init_printer()
        {
            byte[] result = new byte[2];
            result[0] = ESC;
            result[1] = 64;
            return result;
        }

        /**
         * 打开钱箱
         * 
         * @return
         */
        internal byte[] open_money()
        {
            byte[] result = new byte[5];
            result[0] = ESC;
            result[1] = 112;
            result[2] = 48;
            result[3] = 64;
            result[4] = 0;
            return result;
        }

        /**
         * 换行
         * 
         * @param lineNum要换几行
         * @return
         */
        internal byte[] nextLine(int lineNum)
        {
            byte[] result = new byte[lineNum];
            for (int i = 0; i < lineNum; i++)
            {
                result[i] = LF;
            }

            return result;
        }


        /**
         * 绘制下划线（1点宽）
         * 
         * @return
         */
        internal byte[] underlineWithOneDotWidthOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 45;
            result[2] = 1;
            return result;
        }

        /**
         * 绘制下划线（2点宽）
         * 
         * @return
         */
        internal byte[] underlineWithTwoDotWidthOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 45;
            result[2] = 2;
            return result;
        }

        /**
         * 取消绘制下划线
         * 
         * @return
         */
        internal byte[] underlineOff()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 45;
            result[2] = 0;
            return result;
        }


        /**
         * 选择加粗模式
         * 
         * @return
         */
        internal byte[] boldOn()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 69;
            result[2] = 0xF;
            return result;
        }

        /**
         * 取消加粗模式
         * 
         * @return
         */
        internal byte[] boldOff()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 69;
            result[2] = 0;
            return result;
        }


        /**
         * 左对齐
         * 
         * @return
         */
        internal byte[] alignLeft()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 0;
            return result;
        }

        /**
         * 居中对齐
         * 
         * @return
         */
        internal byte[] alignCenter()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 1;
            return result;
        }

        /**
         * 右对齐
         * 
         * @return
         */
        internal byte[] alignRight()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 97;
            result[2] = 2;
            return result;
        }

        /**
         * 水平方向向右移动col列
         * 
         * @param col
         * @return
         */
        internal byte[] set_HT_position(byte col)
        {
            byte[] result = new byte[4];
            result[0] = ESC;
            result[1] = 68;
            result[2] = col;
            result[3] = 0;
            return result;
        }

        /**
         * 字体变大为标准的n倍
         * 
         * @param num
         * @return
         */
        internal byte[] fontSizeSetBig(int num)
        {
            byte realSize = 0;
            switch (num)
            {
                case 1:
                    realSize = 0;
                    break;
                case 2:
                    realSize = 17;
                    break;
                case 3:
                    realSize = 34;
                    break;
                case 4:
                    realSize = 51;
                    break;
                case 5:
                    realSize = 68;
                    break;
                case 6:
                    realSize = 85;
                    break;
                case 7:
                    realSize = 102;
                    break;
                case 8:
                    realSize = 119;
                    break;
            }
            byte[] result = new byte[3];
            result[0] = 29;
            result[1] = 33;
            result[2] = realSize;
            return result;
        }


        /**
         * 字体取消倍宽倍高
         * 
         * @return
         */
        internal byte[] fontSizeSetSmall()
        {
            byte[] result = new byte[3];
            result[0] = ESC;
            result[1] = 33;

            return result;
        }

        /**
         * 进纸并全部切割
         * 
         * @return
         */
        internal byte[] feedPaperCutAll()
        {
            byte[] result = new byte[4];
            result[0] = GS;
            result[1] = 86;
            result[2] = 65;
            result[3] = 0;
            return result;
        }

        /**
         * 进纸并切割（左边留一点不切）
         * 
         * @return
         */
        internal byte[] feedPaperCutPartial()
        {
            byte[] result = new byte[4];
            result[0] = GS;
            result[1] = 86;
            result[2] = 66;
            result[3] = 0;
            return result;
        }

        internal byte[] bmpToByte(Bitmap bmp)
        {
            int h = bmp.Height / 24 + 1;
            int w = bmp.Width;
            byte[][] all = new byte[4 + 2 * h + h * w][];

            all[0] = new byte[] { 0x1B, 0x33, 0x00 };

            Color pixelColor;
            // ESC * m nL nH 点阵图  
            byte[] escBmp = new byte[] { 0x1B, 0x2A, 0x21, (byte)(w % 256), (byte)(w / 256) };

            // 每行进行打印  
            for (int i = 0; i < h; i++)
            {
                all[i * (w + 2) + 1] = escBmp;
                for (int j = 0; j < w; j++)
                {
                    byte[] data = new byte[] { 0x00, 0x00, 0x00 };
                    for (int k = 0; k < 24; k++)
                    {
                        if (((i * 24) + k) < bmp.Height)
                        {
                            pixelColor = bmp.GetPixel(j, (i * 24) + k);
                            if (pixelColor.R == 0)
                            {
                                data[k / 8] += (byte)(128 >> (k % 8));
                            }
                        }
                    }
                    all[i * (w + 2) + j + 2] = data;
                }
                //换行  
                all[(i + 1) * (w + 2)] = nextLine(1);
            }
            all[h * (w + 2) + 1] = nextLine(2);
            all[h * (w + 2) + 2] = feedPaperCutAll();
            all[h * (w + 2) + 3] = open_money();

            return byteMerger(all);
        }

        // ------------------------切纸-----------------------------
        internal byte[] byteMerger(byte[] byte_1, byte[] byte_2)
        {
            byte[] byte_3 = new byte[byte_1.Length + byte_2.Length];
            System.Array.Copy(byte_1, 0, byte_3, 0, byte_1.Length);
            System.Array.Copy(byte_2, 0, byte_3, byte_1.Length, byte_2.Length);
            return byte_3;
        }

        internal byte[] byteMerger(byte[][] byteList)
        {
            int Length = 0;
            for (int i = 0; i < byteList.Length; i++)
            {
                Length += byteList[i].Length;
            }
            byte[] result = new byte[Length];

            int index = 0;
            for (int i = 0; i < byteList.Length; i++)
            {
                byte[] nowByte = byteList[i];
                for (int k = 0; k < byteList[i].Length; k++)
                {
                    result[index] = nowByte[k];
                    index++;
                }
            }
            return result;
        }

        internal byte[][] byte20Merger(byte[] bytes)
        {
            int size = bytes.Length / 10 + 1;
            byte[][] result = new byte[size][];
            for (int i = 0; i < size; i++)
            {
                byte[] by = new byte[((i + 1) * 10) - (i * 10)];
                //从bytes中的第 i * 10 个位置到第 (i + 1) * 10 个位置;
                System.Array.Copy(bytes, i * 10, by, 0, (i + 1) * 10);
                result[i] = by;
            }
            return result;
        }
    }


    /// <summary>
    /// 本地
    /// </summary>
    public sealed class RawPrinterHelper
    {

        #region Instance
        private RawPrinterHelper() { }
        private static readonly Lazy<RawPrinterHelper> lazy = new Lazy<RawPrinterHelper>(() => new RawPrinterHelper());
        public static RawPrinterHelper Instance { get { return lazy.Value; } }
        #endregion Instance

        internal const byte ESC = 27;    // 换码

        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        private bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.
            //di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        internal bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }

        internal bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;

            // How many characters are in the string?
            // Fix from Nicholas Piasecki:
            // dwCount = szString.Length;
            dwCount = (szString.Length + 1) * Marshal.SystemMaxDBCSCharSize;

            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

        public bool SendByteToPrinterForOpenCashbox(string printerName)
        {
            
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            

            byte[] result = new byte[5];
            result[0] = ESC;
            result[1] = 112;
            result[2] = 48;
            result[3] = 64;
            result[4] = 0;

            int nLength = result.Length;
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);

            Marshal.Copy(result, 0, pUnmanagedBytes, nLength);


            Marshal.AllocCoTaskMem(nLength);

            SendBytesToPrinter(printerName, pUnmanagedBytes, nLength);
            Marshal.FreeCoTaskMem(pUnmanagedBytes);

            return true;
        }
    }


}


