using Newtonsoft.Json;
using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server.Model;
using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Oybab.TradingSystemX
{
    public class Resources
    {
        #region Instance
        /// <summary>
        /// For Controls
        /// </summary>
        private static Resources _instance;
        private Resources()
        {
            DefaultOrderLang = -1;
            DefaultPrintLang = -1;
            ShorDay = 3;
            DefaultDay = 7;
            LongDay = 35;

            ReloadResources();


            // 注册JSON全局配置
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
        public static Resources Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Resources();
                return _instance;
            }
        }
        #endregion

      


      

        /// <summary>
        /// 重新加载
        /// </summary>
        public void ReloadResources(int langIndex = -1)
        {
            Oybab.TradingSystemX.Res.Instance.LoadResources(PrintInfo.MainLangList, langIndex);
        }





        /// <summary>
        /// 返回对应的语言
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name)
        {
            try
            {
                return Oybab.TradingSystemX.Res.Instance.GetString(name);
            }
            catch (Exception ex)
            {
                throw new OybabException(Resources.Instance.GetString("ReadResourceStrError") + ":" + name, ex);
            }
        }



        /// <summary>
        /// 返回对应的语言
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name, CultureInfo ci)
        {
            try
            {
                return Oybab.TradingSystemX.Res.Instance.GetString(name, ci);
            }
            catch (Exception ex)
            {
                throw new OybabException(Resources.Instance.GetString("ReadResourceStrError") + ":" + name, ex);
            }
        }




        /// <summary>
        /// 会话是否存在
        /// </summary>
        /// <returns></returns>
        internal bool IsSessionExists()
        {
            return (!string.IsNullOrWhiteSpace(Resources.Instance.SERVER_SESSION));
        }





        //一些公用字段
        //public string KEY_NAME;//用户名称
        private string _kEY_NAME_0;//用户名称(中文)
        internal string KEY_NAME_0
        {
            set { _kEY_NAME_0 = value; }
            get
            {
                if (!string.IsNullOrWhiteSpace(PrintInfo.Name0))
                    return PrintInfo.Name0;
                else
                    return _kEY_NAME_0;
            }
        }
        private string _kEY_NAME_1;//用户名称(维文)
        internal string KEY_NAME_1
        {
            set { _kEY_NAME_1 = value; }
            get
            {
                if (!string.IsNullOrWhiteSpace(PrintInfo.Name1))
                    return PrintInfo.Name1;
                else
                    return _kEY_NAME_1;
            }
        }
        private string _kEY_NAME_2;//用户名称(英文)
        internal string KEY_NAME_2
        {
            set { _kEY_NAME_2 = value; }
            get
            {
                if (!string.IsNullOrWhiteSpace(PrintInfo.Name2))
                    return PrintInfo.Name2;
                else
                    return _kEY_NAME_2;
            }
        }

        internal string SOFT_SERVICE_NAME = "TradingSystemV1.1";
        internal string SOFT_SERVICE_TABLET_NAME = "TradingSystemTabletV1.1";
        internal string SOFT_SERVICE_MOBILE_NAME = "TradingSystemMobilesV1.1";
        internal string SOFT_SERVICE_PC_NAME = "TradingSystemPCV1.1";



        public int LongDay { get; private set; } // 查询最长时间限制
        public int DefaultDay { get; private set; } // 查询一般时间限制
        public int ShorDay { get; private set; } // 查询最短时间限制


     

        //服务器会话
        internal string SERVER_SESSION = null;

        // 服务器地址
        public string SERVER_ADDRESS { get; internal set; }
        // 服务器端口
        internal string SERVER_PORT = "19998";



        public List<Room> Rooms { get; set; }
        public List<RoomModel> RoomsModel { get; set; }
        public List<ProductType> ProductTypes { get; set; }
        public List<Product> Products { get; set; }

        public List<Admin> Admins { get; set; }
        public List<Device> Devices { get; set; }
        public List<Request> Requests { get; set; }
        public List<Printer> Printers { get; set; }

        public List<Ppr> Pprs { get; set; }

        public Admin AdminModel { get; set; }

        public Device DeviceModel { get; set; }

        public List<Balance> Balances { get; set; }

        public int Mode { get; set; }

        public bool IsFireAlarmEnable { get; internal set; } // 火警是否启用

        // 到期和注册相关
        public bool IsExpired { get; internal set; }
        public int ExpiredRemainingDays { get; internal set; }
        public string RegTimeRequestCode { get; internal set; }
        public string RegCountRequestCode { get; internal set; }

        // 设备和雅座数量
        public int DeviceCount { get; internal set; }
        public int RoomCount { get; internal set; }

        // 雅座按时间计算(分钟)
        public int MinutesIntervalTime { get; internal set; }

        // 雅座按时间计算(小时)
        public int HoursIntervalTime { get; internal set; }

        // 是否是手机
        internal long DeviceType { get; set; }
        public void SetTime(long time)
        {
            DeviceType = time;
        }

        /// <summary>
        /// 通知
        /// </summary>
        public Dictionary<long, bool> CallNotifications = new Dictionary<long, bool>();


        internal int AnimateTime = 200;

        /// <summary>
        /// 根目录
        /// </summary>
        internal string ROOT_FOLDER = "OS";
        /// <summary>
        /// 歌曲目录
        /// </summary>
        internal string PRODUCTS_FOLDER = "Products";
        /// <summary>
        /// 产品类型目录
        /// </summary>
        internal string PRODUCT_TYPES_FOLDER = "ProductTypes";


        /// <summary>
        /// 默认打印语言
        /// </summary>
        public int DefaultPrintLang { get; set; }

        /// <summary>
        /// 默认条码打印尺寸
        /// </summary>
        public int DefaultBarcodePrintSize { get; set; }
        /// <summary>
        /// 默认订单语言
        /// </summary>
        public int DefaultOrderLang { get; set; }

        /// <summary>
        /// 自动校准服务器时间到客户端时间
        /// </summary>
        internal bool AutoSyncClientTime = true;
        /// <summary>
        /// 本地打印客户订单
        /// </summary>
        public bool IsLocalPrintCustomOrder { get; internal set; }

        // 打印信息
        private PrintInfo _printInfo = new PrintInfo();
        /// <summary>
        /// 打印信息
        /// </summary>
        internal PrintInfo PrintInfo
        {
            get { return _printInfo; }
            set
            {
                ReloadResources();
                _printInfo = value;
            }
        }

        /// <summary>
        /// 附加信息
        /// </summary>
        public ExtendInfo ExtendInfo { get; internal set; }

        /// <summary>
        /// 钱箱
        /// </summary>
        public string CashDrawer { get; internal set; }
        /// <summary>
        /// 条码阅读器
        /// </summary>
        public string BarcodeReader { get; internal set; }

        public string App { internal get; set; }

        /// <summary>
        /// 是否自动隐藏任务栏
        /// </summary>
        internal bool AutoHideTaskbar { get; set; }

        /// <summary>
        /// 是否自动改分辨率
        /// </summary>
        internal bool AutoChangeScreenSize { get; set; }

        /// <summary>
        /// 是否显示鼠标
        /// </summary>
        public bool DisplayCursor { get; internal set; }


        // 持久化

        /// <summary>
        /// 上次登录管理员编号
        /// </summary>
        public string LastLoginAdminNo { get; set; }

        /// <summary>
        /// 上次登录密码
        /// </summary>
        internal string LastLoginPassword { get; set; }
        /// <summary>
        /// 上次是否选中[保存密码]
        /// </summary>
        internal bool IsSavePassword { get; set; }








        // 需求
        private string[] Demands = new string[] { }; // 标准
        // private string[] Demands = new string[] { "Dentist" }; // 牙医
        /// <summary>
        /// 需求包含需求
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsRequired(string value)
        {
            if (Demands.Contains(value))
                return true;
            else
                return false;
        }

    }
}
