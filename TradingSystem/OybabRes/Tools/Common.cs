using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Reports;
using System.Globalization;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.IO.Ports;
using Oybab.Res.View.Models;
using System.Management;
using System.Windows.Forms;

namespace Oybab.Res.Tools
{
    /// <summary>
    /// 常用
    /// </summary>
    public sealed class Common
    {

        private static Common common = null;
        private Common() { }

        public static Common GetCommon()
        {
            if (null == common)
                common = new Common();
            return common;
        }


        

        /// <summary>
        /// 打开钱箱
        /// </summary>
        public void OpenCashDrawer(string location = null)
        {
           
            if (string.IsNullOrWhiteSpace(location) && string.IsNullOrWhiteSpace(Resources.GetRes().CashDrawer))
                return;

            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("t000")))
                return;
            else if (model.Mode == 0)
                return;

            if (string.IsNullOrWhiteSpace(location))
                location = Resources.GetRes().CashDrawer;

            try
            {
                // 如果是本地
                if (location == "System")
                {
                    // 非本地打印模式不能用
                    if (!Resources.GetRes().IsLocalPrintCustomOrder)
                        return;


                    // 也可以用, 但是命令需要发送到网络上(打算用底部那个直接把命令发送到本地)
                    //try
                    //{
                    //    PrinterMsg.Instance.SendSocketMsg("192.168.1.85", 9100, 1, PrinterCmdUtils.Instance.open_money());
                    //}
                    //catch(Exception ex)
                    //{

                    //}
                    


                    // 循环发送命令到本地
                    try
                    {
                        Printer printer = Resources.GetRes().Printers.Where(x => x.PrintType == 0 && x.IsEnable == 1 && x.IsCashDrawer == 1).FirstOrDefault();
                        if (null != printer)
                        {
                            RawPrinterHelper.Instance.SendByteToPrinterForOpenCashbox(printer.PrinterDeviceName);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }
                else
                {
                    using (System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort())
                    {
                        sp.PortName = location;
                        sp.Open();
                        byte[] byteA = Encoding.UTF8.GetBytes("#$1b#$70#0#$3c#$ff");
                        sp.Write(byteA, 0, byteA.Length);
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }




        /// <summary>
        /// 呼叫
        /// </summary>
        public void CallDevice(string roomNo)
        {

            if (string.IsNullOrWhiteSpace(roomNo) || string.IsNullOrWhiteSpace(Resources.GetRes().CallDevice) || !int.TryParse(roomNo, out int test))
                return;

            int value = int.Parse(roomNo);

            // 高字节在前, 低字节在后
            byte highByte = (byte)((value >> 8) & 0xff);
            byte lowByte = (byte)(value & 0xff);


            byte[] sendByte = new byte[]
          {
                0x3A,
                0x01,
                0x63,
                0x02, // 长度(后面那2个字节的报警号)
                highByte,
                lowByte,
                0x00, // 效验码
                0x0A
          };

            // 效验码: 起始码和效验码之间的所有数据之和(不含效验码);取低字节
            sendByte[6] = (byte)((sendByte[1] + sendByte[2] + sendByte[3] + sendByte[4] + sendByte[5]) & 0xff);



            try
            {
                using (System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort())
                {
                    sp.ReadTimeout = 2000;
                    sp.WriteTimeout = 2000;

                    sp.PortName = Resources.GetRes().CallDevice;

                    sp.BaudRate = 9600;
                    sp.StopBits = System.IO.Ports.StopBits.One;
                    sp.DataBits = 8;

                    sp.Open();
                    sp.Write(sendByte, 0, sendByte.Length);

                    byte[] bytesToReadNew = new byte[5]; // serialPort.ReadBufferSize
                    int bytesReadNew = sp.Read(bytesToReadNew, 0, 5); // serialPort.ReadBufferSize
                    string resultMsgNew = BitConverter.ToString(bytesToReadNew, 0, bytesReadNew).Replace(" ", "-");

                    // 命令返回成功(底部是其他程序逻辑, )
                    //if ("05-11-01-00".Contains(resultMsgNew))
                    //return true;
                    // 返回值  0x2A+地址（0x01）+0x63“c”+校验码+ 0x0A；
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }



        /// <summary>
        /// 设置设备时间
        /// </summary>
        public void SetDeviceTime()
        {

            if (string.IsNullOrWhiteSpace(Resources.GetRes().CallDevice))
                return;

            DateTime time = DateTime.Now;

            // 时间字节
            byte firstYearByte = (byte)int.Parse(time.Year.ToString().Substring(0, 2));
            byte secondYearByte = (byte)int.Parse(time.Year.ToString().Substring(2, 2));
            byte monthByte = (byte)time.Month;
            byte dayByte = (byte)time.Day;
            byte weekByte = (byte)time.DayOfWeek;
            byte hourByte = (byte)time.Hour;
            byte minuteByte = (byte)time.Minute;
            byte secondByte = (byte)time.Second;


            byte[] sendByte = new byte[]
          {
                0x3A,
                0x01,
                0x74,
                0x08, // 长度(8)
                firstYearByte,
                secondYearByte,
                monthByte,
                dayByte,
                weekByte,
                hourByte,
                minuteByte,
                secondByte,
                0x00, // 效验码
                0x0A
          };

            // 效验码: 起始码和效验码之间的所有数据之和(不含效验码);取低字节
            sendByte[12] = (byte)((sendByte[1] + sendByte[2] + sendByte[3] + sendByte[4] + sendByte[5] + sendByte[6] + sendByte[7] + sendByte[8] + sendByte[9] + sendByte[10] + sendByte[11]) & 0xff);



            try
            {
                using (System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort())
                {
                    sp.ReadTimeout = 2000;
                    sp.WriteTimeout = 2000;

                    sp.PortName = "COM3";

                    sp.BaudRate = 9600;
                    sp.StopBits = System.IO.Ports.StopBits.One;
                    sp.DataBits = 8;

                    sp.Open();
                    sp.Write(sendByte, 0, sendByte.Length);

                    byte[] bytesToReadNew = new byte[5]; // serialPort.ReadBufferSize
                    int bytesReadNew = sp.Read(bytesToReadNew, 0, 5); // serialPort.ReadBufferSize
                    string resultMsgNew = BitConverter.ToString(bytesToReadNew, 0, bytesReadNew).Replace(" ", "-");

                    // 命令返回成功(底部是其他程序逻辑, )
                    //if ("05-11-01-00".Contains(resultMsgNew))
                    //return true;
                    // 返回值  0x2A+地址（0x01）+0x63“c”+校验码+ 0x0A；
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }




        /// <summary>
        /// 打开客显
        /// </summary>
        public void OpenPriceMonitor(string price, string name = null)
        {
            if (string.IsNullOrWhiteSpace(Resources.GetRes().PriceMonitor) && string.IsNullOrWhiteSpace(name))
                return;
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    name = Resources.GetRes().PriceMonitor;

                // 清理
                if (null == price)
                {

                    CustomerDisplay.Instance.DisplayData(null, name, 2400, StopBits.One, 8, CustomerDispiayType.Clear);
                }
                else
                {
                    price = price.Replace("-", "");
                    CustomerDisplay.Instance.DisplayData(price, name, 2400, StopBits.One, 8, CustomerDispiayType.Clear);
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }




     
        
        /// <summary>
        /// 读取还原
        /// </summary>
        public void ReadBak()
        {
            Restore.Instance.ReadBak();
        }

        /// <summary>
        /// 写入还原
        /// </summary>
        public void SetBak()
        {
            Restore.Instance.WriteBak();
        }


        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="adminNo"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ResultModel Load(string adminNo, string password)
        {
            ResultModel result = new ResultModel();

            //自动校准服务器时间到客户端
            if (Resources.GetRes().AutoSyncClientTime)
            {
                if (Resources.GetRes().SERVER_ADDRESS != "127.0.0.1" && Resources.GetRes().SERVER_ADDRESS != "::1")
                    SyncTime.GetSyncTime().SyncServerTimeToClient();
            }

           

            // 如果登录的IP是本地, 查看有没有服务, 有就查是否启动了, 没启动就启动它
            if (Resources.GetRes().SERVER_ADDRESS == "127.0.0.1" || Resources.GetRes().SERVER_ADDRESS == "::1")
            {
                try
                {
                    // 获取本地的服务
                    ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "OybabTradingSystemService");
                    if (ctl != null)
                    {
                        // 如果服务停止了, 则启动它
                        if (ctl.Status == ServiceControllerStatus.Stopped)
                        {
                            ctl.Start();
                            ctl.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 15));
                        }
                    }
                }
                catch
                {
                }
            }


            //新请求
            if (!Resources.GetRes().IsSessionExists())
            {
                result = OperatesService.GetOperates().NewRequest(adminNo, password);
            }

           

            return result;
        }




        /// <summary>
        /// 格式化
        /// </summary>
        /// <returns></returns>
        public string GetFormat()
        {
            return "\n";
        }


        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            return Oybab.Res.Server.OperatesService.GetOperates().ServiceClose();
        }



        private int Count = 0; //错误次数

        /// <summary>
        /// 检测并提示用户!
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="close"></param>
        public void CheckAndAlert(Action<string> alert, Action close = null)
        {
            do
            {
                //链接到服务器检测会话
                bool IsSuccess = false;
                string message = null;

                try 
	            {
                    IsSuccess = Res.Server.OperatesService.GetOperates().ServiceSession(true);
	            }
	            catch (Exception ex)
	            {
                    if (ex is OybabException)
                        message = ex.Message;
		            ExceptionPro.ExpLog(ex);
                }

                if (OperatesService.GetOperates().IsExpired || OperatesService.GetOperates().IsAdminUsing)
                {
                    Count = 3;
                    Session.Instance.ChangeInterval(false);
                }
                else if (!IsSuccess)
                {
                    ++Count;
                    Session.Instance.ChangeInterval(false);
                }
                else
                {
                    Count = 0;
                    Session.Instance.ChangeInterval(true);
                }

                if (Count >= 3)
                {
                    if (null != alert)
                        alert(message);
                }
                else
                {
                    if (null != close)
                        close();
                    break;
                }
            } while (1 == 1);
        }


        /// <summary>
        /// 检测并提示用户(非循环)!
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="close"></param>
        public void CheckAndAlertOnce(Action<string> alert, Action close = null, bool IsAuto = true)
        {
            //连接到服务器检测会话
            bool IsSuccess = false;
            string message = null;

            try 
	        {
                IsSuccess = Res.Server.OperatesService.GetOperates().ServiceSession(true);
	        }
	        catch (Exception ex)
	        {
                if (ex is OybabException)
                    message = ex.Message;
                ExceptionPro.ExpLog(ex);
            }

            if (OperatesService.GetOperates().IsExpired || OperatesService.GetOperates().IsAdminUsing)
            {
                Count = 3;
                Session.Instance.ChangeInterval(false);
            }
            else if (!IsSuccess)
            {
                ++Count;
                Session.Instance.ChangeInterval(false);
            }
            else
            {
                Count = 0;
                Session.Instance.ChangeInterval(true);
            }


            if (Count >= 3)
            {
                if (null != alert)
                    alert(message);
            }
            else
            {
                if (null != close)
                    close();
            }
        }


        /// <summary>
        /// 是否允许该平台1为m100:电脑,2为m200:平板3为m300:手机
        /// </summary>
        /// <returns></returns>
        public bool IsAllowPlatform(int code)
        {
            Admin model = Resources.GetRes().AdminModel;

            string platformCode = "?";
            if (code == 1)
                platformCode = "m100";
            else if (code == 2)
                platformCode = "m200";
            else if (code == 3)
                platformCode = "m300";


            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains(platformCode)))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }

        /// <summary>
        /// 是否允许打开财务日志
        /// </summary>
        /// <returns></returns>
        public bool IsAllowFinancceLog()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("3500")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }


        /// <summary>
        /// 是否允许打开支出管理
        /// </summary>
        /// <returns></returns>
        public bool IsAllowImportManager()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("1200")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }


        /// <summary>
        /// 是否允许收入交易管理
        /// </summary>
        /// <returns></returns>
        public bool IsIncomeTradingManage()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p100")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }

       
        /// <summary>
        /// 是否允许临时修改出售金额
        /// </summary>
        /// <returns></returns>
        public bool IsTemporaryChangePrice()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p200")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }


        /// <summary>
        /// 是否允许修改支出金额
        /// </summary>
        /// <returns></returns>
        public bool IsChangeCostPrice()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p400")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }



        /// <summary>
        /// 是否允许修改出售金额
        /// </summary>
        /// <returns></returns>
        public bool IsChangeUnitPrice()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p500")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }



        /// <summary>
        /// 是否允许替换包厢
        /// </summary>
        /// <returns></returns>
        public bool IsReplaceRoom()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p350")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;

        }

        /// <summary>
        /// 是否允许取消订单
        /// </summary>
        /// <returns></returns>
        public bool IsCancelOrder(bool IsNew = false)
        {
            if (IsNew)
            {
                return true;
            }
            else
            {
                Admin model = Resources.GetRes().AdminModel;

                if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p300")))
                    return false;
                else if (model.Mode == 0)
                    return false;

                return true;
            }

        }





        /// <summary>
        /// 是否允许添加内部账单(主页)
        /// </summary>
        /// <returns></returns>
        public bool IsAddInnerBill()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("1000")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }


        /// <summary>
        /// 是否允许添加外部账单
        /// </summary>
        /// <returns></returns>
        public bool IsAddOuterBill()
        {

            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p900")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }



        /// <summary>
        /// 是否允许删除商品
        /// </summary>
        /// <returns></returns>
        public bool IsDeleteProduct(bool IsNew = false)
        {
            if (IsNew)
            {
                return true;
            }
            else
            {
                Admin model = Resources.GetRes().AdminModel;

                if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p800")))
                    return false;
                else if (model.Mode == 0)
                    return false;

                return true;
            }

        }


        /// <summary>
        /// 是否允许减少产品数量
        /// </summary>
        /// <returns></returns>
        public bool IsDecreaseProductCount()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p850")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }


        /// <summary>
        /// 是否允许退回金钱
        /// </summary>
        /// <returns></returns>
        public bool IsReturnMoney()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p880")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }



        /// <summary>
        /// 是否允许修改不限时时间
        /// </summary>
        /// <param name="IsNullOrder"></param>
        /// <returns></returns>
        public bool IsChangeUnlimitedTime(bool IsNullOrder)
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 2)
                return true;
            else
                return false;

        }


        

        /// <summary>
        /// 允许通过会员号码绑定会员
        /// </summary>
        /// <returns></returns>
        public bool IsBindMemberByNo()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p600")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }

        /// <summary>
        /// 允许通过供应商号码绑定供应商
        /// </summary>
        /// <returns></returns>
        public bool IsBindSupplierByNo()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("p700")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }


        /// <summary>
        /// 是否允许更改语言
        /// </summary>
        /// <returns></returns>
        public bool IsAllowChangeLanguage()
        {
            Admin model = Resources.GetRes().AdminModel;

            if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("o100")))
                return false;
            else if (model.Mode == 0)
                return false;

            return true;
        }




        /// <summary>
        /// 通过KEY返回值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal string GetStrFromKey(System.Windows.Input.Key key) {
            if (key >= System.Windows.Input.Key.D0 && key <= System.Windows.Input.Key.D9)
                return key.ToString().TrimStart("D");
            else if (key >= System.Windows.Input.Key.NumPad0 && key <= System.Windows.Input.Key.NumPad9)
                return key.ToString().TrimStart("NumPad");
            else if (key == System.Windows.Input.Key.OemPeriod || key == System.Windows.Input.Key.Decimal)
                return ".";
            return key.ToString();
        }




        /// <summary>
        /// 隐藏功能时复制名称
        /// </summary>
        public void CopyForHide(DataGridViewCell Cell0, DataGridViewCell Cell1, DataGridViewCell Cell2, bool IsNew, bool IsSameBefore, bool MultipleLanguage)
        {
            if (!MultipleLanguage)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                {
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell1.Value.ToString())) || (!IsNew && IsSameBefore))
                        Cell1.Value = Cell0.Value;
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell2.Value.ToString())) || (!IsNew && IsSameBefore))
                        Cell2.Value = Cell0.Value;
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell0.Value.ToString())) || (!IsNew && IsSameBefore))
                        Cell0.Value = Cell1.Value;
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell2.Value.ToString())) || (!IsNew && IsSameBefore))
                        Cell2.Value = Cell1.Value;
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell0.Value.ToString())) || (!IsNew && IsSameBefore))
                        Cell0.Value = Cell2.Value;
                    if ((IsNew && string.IsNullOrWhiteSpace(Cell1.Value.ToString())) || (!IsNew && IsSameBefore))
                        Cell1.Value = Cell2.Value;
                }
            }
        }

    }

}
