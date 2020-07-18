using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Oybab.Res.Tools
{
    /// 客显  
    /// </summary>  
    internal sealed class CustomerDisplay
    {

        #region Instance
        private CustomerDisplay() { }
        private static readonly Lazy<CustomerDisplay> lazy = new Lazy<CustomerDisplay>(() => new CustomerDisplay());
        public static CustomerDisplay Instance { get { return lazy.Value; } }
        #endregion Instance



        #region 属性  
        /// <summary>  
        /// 客显发送类型  
        /// </summary>  
        private CustomerDispiayType DispiayType { get; set; }
        #endregion 属性  

        

        #region Method  
        #region 公共方法  

        /// <summary>  
        /// 数据信息展现  
        /// </summary>  
        /// <param name="data">发送的数据（清屏可以为null或者空）</param>  
        internal void DisplayData(string data, string _spPortName, int _spBaudRate, StopBits _spStopBits, int _spDataBits, CustomerDispiayType DispiayType)
        {
            using (SerialPort serialPort = new SerialPort())
            {
                serialPort.PortName = _spPortName;
                serialPort.BaudRate = _spBaudRate;
                serialPort.StopBits = _spStopBits;
                serialPort.DataBits = _spDataBits;
                this.DispiayType = DispiayType;
                serialPort.Open();


                char esc = (char)27;

                serialPort.Write(esc + @"@");
                serialPort.Write(esc + @"s" + Convert.ToInt32(this.DispiayType).ToString());

                //发送数据  
                if (!string.IsNullOrEmpty(data))
                {
                    serialPort.Write(((char)27).ToString() + ((char)81).ToString() + ((char)65).ToString() + data + ((char)13).ToString());
                }else
                {
                    serialPort.Write(((char)27).ToString() + ((char)81).ToString() + ((char)65).ToString() + "0" + ((char)13).ToString());
                }

            }
        }

        public void DisplayData(object price, string priceMonitor, int v1, object one, int v2, CustomerDispiayType total)
        {
            throw new NotImplementedException();
        }

        #endregion --公共方法  
        #endregion --Method  
    }

    /// <summary>  
    /// 客显类型  
    /// </summary>  
    internal enum CustomerDispiayType
    {
        /// <summary>  
        /// 清屏  
        /// </summary>  
        Clear,
        /// <summary>  
        /// 单价  
        /// </summary>  
        Price,
        /// <summary>  
        /// 合计  
        /// </summary>  
        Total,
        /// <summary>  
        /// 收款  
        /// </summary>  
        Recive,
        /// <summary>  
        /// 找零  
        /// </summary>  
        Change
    }
}