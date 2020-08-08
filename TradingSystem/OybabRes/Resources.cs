using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using log4net;
using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Tools;
using Oybab.Res.Server.Model;
using Oybab.Res.Server;
using Newtonsoft.Json;
using System.Windows;
using Oybab.ServerManager.Model.Models;

namespace Oybab.Res
{
    /// <summary>
    /// 国际化类
    /// </summary>
    public sealed class Resources
    {
        private static Resources res = null;

        public static Resources GetRes()
        {
            if (null == res)
            {
                res = new Resources();
            }

            return res;
        }

        private CultureInfo ci = null;
        private ResourceManager rm = null;

        // 当前语言的索引
        private int _currentLangIndex = 2; // default en-US
        public int CurrentLangIndex {
            private set
            {
                _currentLangIndex = value;
                MainLangIndex = CurrentLangIndex;
            }
            get { return _currentLangIndex; }
        }


        public Dictionary<int, Lang> AllLangList { private set; get; } = new Dictionary<int, Lang>();

        public Dictionary<int, Lang> MainLangList { private set; get; } = new Dictionary<int, Lang>();

        public class Lang
        {
            public CultureInfo Culture { get; set; }
            public int LangIndex { get; set; }
            public int MainLangIndex { get; set; } = -1;
            public string LangName { get; set; }
        }



        /// <summary>
        /// 初始化所有语言
        /// </summary>
        private void InitialAllLang()
        {
            int orderIndex = 0;
            AllLangList.Clear();


            // zh-CN
            CultureInfo ci = new CultureInfo("zh-CN");
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, MainLangIndex = -1, LangName = GetString("LangName", ci) }); // index 0


            // ug-CN
            ++orderIndex;
            ci = new CultureInfo("ug-CN");
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, MainLangIndex = -1, LangName = GetString("LangName", ci) }); // index 1


            //en-US
            ++orderIndex;
            ci = new CultureInfo("en-US");
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, MainLangIndex = -1, LangName = GetString("LangName", ci) }); // index 2




            
        }

        /// <summary>
        /// 初始化主要语言
        /// </summary>
        internal void InitialMainLang(string mainLangLists)
        {
            int orderIndex = 0;
            MainLangList.Clear();
            foreach (var item in mainLangLists.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                int index = int.Parse(item);
                CultureInfo ci = AllLangList[index].Culture;
                MainLangList.Add(index, new Lang() { Culture = ci, LangIndex = index, MainLangIndex = orderIndex, LangName = GetString("LangName", ci) }); // index 0  
                ++orderIndex;
            }

            MainLangIndex = CurrentLangIndex;
        }


        private int _mainLangIndex;
        /// <summary>
        /// 获取主语言索引
        /// </summary>
        public int MainLangIndex
        {
            internal set {

                if (MainLangList.ContainsKey(CurrentLangIndex))
                    _mainLangIndex = MainLangList[CurrentLangIndex].MainLangIndex;
                // 否则返回主语言索引
                else
                    _mainLangIndex = MainLangList[0].MainLangIndex;
            }
            get
            {
                return _mainLangIndex;
            }
        }

        /// <summary>
        /// 主语言名称
        /// </summary>
        public Lang MainLang
        {
            internal set { }
            get
            {
                return MainLangList.FirstOrDefault(x => x.Value.MainLangIndex == MainLangIndex).Value;
            }
        }


        /// <summary>
        /// 根据语言名返回索引
        /// </summary>
        /// <param name="LangName"></param>
        /// <returns></returns>
        public Lang GetLangByLangName(string LangName)
        {
            if (AllLangList.Any(x => x.Value.LangName == LangName))
                return AllLangList.FirstOrDefault(x => x.Value.LangName == LangName).Value;
            else
                return null;
        }

        // 根据语言索引返回主语言
        public Lang GetLangByLangIndex(int index)
        {
            if (AllLangList.Any(x => x.Value.LangIndex == index))
            {
                return AllLangList.FirstOrDefault(x => x.Value.LangIndex == index).Value;
            }
            return null;
        }


        /// <summary>
        /// 根据主语言名返回索引
        /// </summary>
        /// <param name="LangName"></param>
        /// <returns></returns>
        public Lang GetMainLangByLangName(string LangName)
        {
            if (MainLangList.Any(x => x.Value.LangName == LangName))
                return MainLangList.FirstOrDefault(x => x.Value.LangName == LangName).Value;
            else
                return MainLang;
        }


        // 根据主语言索引返回主语言
        public Lang GetMainLangByMainLangIndex(int index)
        {
            if (MainLangList.Any(x=>x.Value.MainLangIndex == index))
            {
                return MainLangList.FirstOrDefault(x=> x.Value.MainLangIndex == index).Value;
            }
            return MainLangList[0];
        }
        // 对比主语言里有没有该语言, 没有, 则返回主语言第一个语言.
        public Lang GetMainLangByLangIndex(int index)
        {
            if (MainLangList.ContainsKey(index))
            {
                return MainLangList[index];
            }
            return MainLangList[0];
        }

        /// <summary>
        /// 初始化加载项
        /// </summary>
        private Resources() {
            
            SERVER_ADDRESS = "127.0.0.1";
            DefaultOrderLang = -1;
            DefaultPrintLang = -1;
            ShorDay = 7;
            DefaultDay = 40;
            LongDay = 100;

            rm = new ResourceManager("Oybab.Res.Resource", Assembly.GetExecutingAssembly());
            InitialAllLang();
            InitialMainLang(PrintInfo.MainLangList);
            ci = AllLangList[CurrentLangIndex].Culture;
            


            // 解决WPF 实时渲染报错
            if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
            {
                // 注册JSON全局配置
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
            }

            // 调试模式下才显示鼠标
#if DEBUG
            DisplayCursor = true;
#endif


        }
        /// <summary>
        /// 重新加载
        /// </summary>
        public void ReloadResources(int index)
        {
            CurrentLangIndex = index;
            InitialMainLang(PrintInfo.MainLangList);
            SetCulture();
            ci = AllLangList[CurrentLangIndex].Culture;

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
                return rm.GetString(name, ci);
            }
            catch (Exception ex)
            {
                throw new OybabException(Resources.GetRes().GetString("ReadResourceStrError") + ":" + name, ex);
            }
        }

        /// <summary>
        /// 设置语言
        /// </summary>
        private void SetCulture()
        {
            string name = AllLangList[CurrentLangIndex].Culture.Name;
            if (!CheckCulture(name))
                name = AllLangList[1].Culture.Name;
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(name);
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// 检查当前语言是否存在
        /// </summary>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        private bool CheckCulture(string cultureName)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => culture.Name == cultureName);
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
                return rm.GetString(name, ci);
            }
            catch (Exception ex)
            {
                throw new OybabException(Resources.GetRes().GetString("ReadResourceStrError") + ":" + name, ex);
            }
        }





        /// <summary>
        /// 会话是否存在
        /// </summary>
        /// <returns></returns>
        internal bool IsSessionExists()
        {
            return (!string.IsNullOrWhiteSpace(Res.Resources.GetRes().SERVER_SESSION));
        }


        /// <summary>
        /// 加载日志
        /// </summary>
        public void LoadLog()
        {
            try
            {
                Logger.Create();
            }
            catch (Exception ex)
            {

                throw new OybabException(Resources.GetRes().GetString("Exception_LogError"), ex);
            }
        }


        //一些公用字段
        //public string KEY_NAME;//用户名称
        private string _kEY_NAME_0;//用户名称(中文)
        public string KEY_NAME_0
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
        public string KEY_NAME_1
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
        public string KEY_NAME_2
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


        /// <summary>
        /// 返回PC版名称
        /// </summary>
        /// <returns></returns>
        public string GetSoftServicePCName()
        {
            return Resources.GetRes().SOFT_SERVICE_PC_NAME;
        }



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

        // 设备类型.1.PC/2.平板/3.手机
        internal long DevicesType { get; set; }
        public void SetDeviceType(long time)
        {
            DevicesType = time;
        }

        /// <summary>
        /// 通知
        /// </summary>
        public Dictionary<long, bool> CallNotifications = new Dictionary<long, bool>();


        internal int AnimateTime = 200;


        internal string ROOT = "";

        /// <summary>
        /// 根目录:OS
        /// </summary>
        internal string ROOT_FOLDER = "OS";
        /// <summary>
        /// 产品目录:Products
        /// </summary>
        internal string PRODUCTS_FOLDER = "Products";
        /// <summary>
        /// 歌曲目录:Bars
        /// </summary>
        internal string BARS_FOLDER = "Bars";
        /// <summary>
        /// 产品类型目录:ProductTypes
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


        private PrintInfo _printInfo = new PrintInfo();
        /// <summary>
        /// 打印信息
        /// </summary>
        public PrintInfo PrintInfo
        {
            get { return _printInfo; }
            internal set
            {
                InitialMainLang(value.MainLangList);
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
        /// 客显
        /// </summary>
        public string PriceMonitor { get; internal set; }
        /// <summary>
        /// 条码阅读器
        /// </summary>
        public string BarcodeReader { get; internal set; }
        /// <summary>
        /// 卡片阅读器
        /// </summary>
        public string CardReader { get; internal set; }
        /// <summary>
        /// 呼叫器
        /// </summary>
        public string CallDevice { get; internal set; }


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


        /// <summary>
        /// 是否显示第二屏幕
        /// </summary>
        public bool DisplaySecondMonitor { get; internal set; }


        // 持久化

        /// <summary>
        /// 上次登录管理员编号
        /// </summary>
        public string LastLoginAdminNo { get; set; }



        // 窗口大小
        public Size Size { get; internal set; }
        /// <summary>
        /// 设置窗口大小
        /// </summary>
        /// <param name="size"></param>
        public void setSize(Size size)
        {
            Size = size;
        }






        // 需求
        internal List<string> Demands = new List<string>(); // 标准
        //private string[] Demands = new string[] { "Vod" }; // 点歌系统
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
