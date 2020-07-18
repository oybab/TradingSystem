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
using Oybab.ServerManager.Exceptions;
using log4net;
using Oybab.ServerManager.Model.Models;
using Oybab.DAL;
using Oybab.ServerManager.Operate;
using System.Threading;
using Oybab.ServerManager.Res;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;

namespace Oybab.ServerManager
{
    /// <summary>
    /// 资源类
    /// </summary>
    public sealed class Resources
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static Resources res = null;
        public static Resources GetRes()
        {
            if (null == res)
                res = new Resources();
            return res;
        }
        private Resources()
        {
        }

        private CultureInfo ci = null;
        private ResourceManager rm = null;


        // 当前语言的索引
        public int CurrentLangIndex { private set; get; } = 2; // default en-US


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
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, LangName = GetString("LangName", ci) }); // index 0


            // ug-CN
            ++orderIndex;
            ci = new CultureInfo("ug-CN");
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, LangName = GetString("LangName", ci) }); // index 1


            //en-US
            ++orderIndex;
            ci = new CultureInfo("en-US");
            AllLangList.Add(orderIndex, new Lang() { Culture = ci, LangIndex = orderIndex, LangName = GetString("LangName", ci) }); // index 2




            var currentLang = AllLangList.Where(x => x.Value.Culture.Name == System.Globalization.CultureInfo.CurrentCulture.Name).Select(x => x.Value).FirstOrDefault();
            if (null != currentLang)
                CurrentLangIndex = currentLang.LangIndex;
            else
                CurrentLangIndex = 1; // english for default

        }

        /// <summary>
        /// 初始化主要语言
        /// </summary>
        private void InitialMainLang(string mainLangLists)
        {
            int orderIndex = 0;
            MainLangList.Clear();
            foreach (var item in mainLangLists.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                int index = int.Parse(item);
                CultureInfo ci = AllLangList[index].Culture;
                MainLangList.Add(index, new Lang() { Culture = ci, LangIndex = index, MainLangIndex = orderIndex, LangName = GetString("LangName", ci) }); 
                ++orderIndex;
            }
        }



        // 对比主语言里有没有该语言, 没有, 则返回主语言第一个语言.
        internal Lang GetMainLangByLangIndex(int index)
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
        internal void Initial()
        {

            
            rm = new ResourceManager("Oybab.ServerManager.Resource", Assembly.GetExecutingAssembly());
            InitialAllLang();
            InitialMainLang(PrintInfo.MainLangList);
            ci = AllLangList[CurrentLangIndex].Culture;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

        }


    


        /// <summary>
        /// 返回对应的语言
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetString(string name)
        {
            try
            {
                return rm.GetString(name, ci);
            }
            catch (Exception ex)
            {
                throw new OybabException("ReadResourceStrError" + ":" + name, ex);
            }
        }



        /// <summary>
        /// 返回对应的语言
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal string GetString(string name, CultureInfo ci)
        {
            try
            {
                return rm.GetString(name, ci);
            }
            catch (Exception ex)
            {
                throw new OybabException("ReadResourceStrError"+ ":" + name, ex);
            }
        }




        //一些公用字段
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



        



        //一些缓存

        //包厢
        internal List<Room> ROOMS;
        internal List<RoomModel> ROOMS_Model;
        // 外卖
        internal List<TakeoutModel> TAKEOUT_Model;
        //菜单类别和菜单
        internal List<ProductType> PRODUCT_TYPES;
        internal List<Product> PRODUCTS;

        // 管理员
        internal List<Admin> ADMINS;
        // 会员
        internal List<Member> MEMBERS;
        // 供应商
        internal List<Supplier> SUPPLIERS;
        // 打印
        internal List<Printer> PRINTERS;
        // 设备
        internal List<Device> DEVICES;
        // 请求
        internal List<Request> REQUESTS;

        // 打印和产品绑定表
        internal List<Ppr> PPRS;

        // 余额
        internal List<Balance>  BALANCES;




        // 用到的常量
        internal string SOFT_SERVICE_MOBILE_NAME = "TradingSystemMobilesV1.0";
        internal string SOFT_SERVICE_TABLET_NAME = "TradingSystemTabletV1.0";
        internal string SOFT_SERVICE_PC_NAME = "TradingSystemPCV1.0";
        internal string SOFT_SERVICE_NAME = "TradingSystemV1.0";
        internal string KEY_X86_M = "dbc5742201f5f885dea7e932eb7319f6";
        internal string KEY_X64_M = "11d3099b6ce514943421734ea33789f9";
        internal string KEY_TC_M = "228c14223a06c858d18e811359177234";
        public string IPAddress { internal set; get; } = "localhost";
        internal string DB_KEY = "";
        internal string UID = "";
        internal string KEY = "";
        internal int TIME_OUT_SHORT = 30;
        internal int TIME_OUT_LONG = 300;

        internal int DB_BACKUP_TIME = 4; // 小时
        internal int DB_BACKUP_RETRY = 5; // 分钟

        // Config配置文件
        internal string BackupFolderPath;// 数据库备份路径
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
                InitialMainLang(value.MainLangList);
                _printInfo = value;
            }
        }
        internal ExtendInfo ExtendInfo = new ExtendInfo();// 附加信息
        

        internal bool IsFireAlarmEnable;  // 火警是否启用


        // 用到的全局变量
        internal int ROOM_COUNT = 0;// 雅座数量
        internal int SERVICE_COUNT = 0;// 可用服务数量(可连接设备数量)
        internal int MINUTES_INTERNAL_TIME = 1; // 订单雅座间隔时间:订单雅座循环计算时间间隔/计算最小单位(分钟) 1小时的5分钟的价格为最低价格单位. 后来改为了1分钟
        internal int HOURS_INTERNAL_TIME = 1; // 订单雅座间隔时间:订单雅座循环计算时间间隔/计算最小单位(小时) 1天的1小时的价格为最低价格单位
        internal List<Client> Services = new List<Client>();
        internal List<Client> ServicesUsedByOther = new List<Client>();
        internal List<PasswordErrorModel> PasswordErrorList = new List<PasswordErrorModel>();
        private DateTime NextCheck = DateTime.MinValue;//锁下次查询时间
        private Thread handleThread;//处理线程
        //internal string Config;//配置文件(客户端相同的配置文件内容, 用于覆盖客户端)
        internal int RemoveKeyCount = -1;// 拔出KEY次数, 多次拔出就不能再识别(超过2次)
        internal int IsExpired = -1; // -1未检测, 1过期, 0未过期
        internal int ExpiredRemainingDays = -1; // 过期剩余时间
        internal string RegTimeRequestCode = ""; // 时间申请码
        internal string RegCountRequestCode = ""; // 数量申请码

        public string App { internal get; set; }
        internal bool IsCreateMainCheckThread { get; set; }

        /// <summary>
        /// 创建定时检查线程
        /// </summary>
        public void CreateThread()
        {
            // 初始化
            Initial();

            //启动自动更新
            Update.UpdateIt("Server");

           

            IsCreateMainCheckThread = true;

            OperateLog.Instance.StartRecordAccessLog();


            // 检测插入
            Detection.Start(new Action(() =>
            {
                //检测KEY
                Resources.GetRes().KeyCheck(true);
            }));



            //启动检查线程
            Resources.GetRes().handleThread = new Thread(() =>
            {
                for (; ; )
                {
                    //如果3分钟以前查过一次则继续查
                    if (DateTime.Now >= Resources.GetRes().NextCheck)
                    {
                        Resources.GetRes().KeyCheck();
                    }
                    //每3分钟查一次
                    System.Threading.Thread.Sleep(1000 * 60 * 3);
                }
            });
            


            //启动24小时清空线程
            Task.Factory.StartNew(new Action(() =>
            {
                for (; ; )
                {
                    //每25小时执行一次(之所以不用24, 是因为怕服务器有定时重启, 反正1天之内不重启再执行这个----但是貌似不需要这个逻辑, 因为到期了锁会自动停止,有机会测试一下是不是真那样.这个暂时为了保险保留,先确定再决定)
                    System.Threading.Thread.Sleep(1000 * 60 * 60 * 25);

                    Key.GetKeys().Clear(true);


                    Key.GetKeys().Check(false, true);

                }
            }));

            // 备份数据库
            Session.Instance.StartSession((param) =>
            {

                // 强制再执行备份一次.(虽然强制, 内部还是会有判断)
                Backup.Instance.BackupFile(true);
            });

            Resources.GetRes().handleThread.IsBackground = true;
            Resources.GetRes().handleThread.Start();

        }

       

        /// <summary>
        /// 关闭检查线程
        /// </summary>
        public void CloseThread()
        {

            // 关闭记录
            OperateLog.Instance.RecordAll();


            //关闭查KEY(可能会导致异常:Detection.Stop();在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke。
            try
            {
                Detection.Stop();
            }
            catch { }


            //关闭线程
            if (null != Resources.GetRes().handleThread)
            {
                Resources.GetRes().handleThread.Abort();
                Resources.GetRes().handleThread = null;
            }

            //关闭锁
            if (Key.GetKeys().Check())
            {
                Key.GetKeys().Close();
            }

        }



        internal void KeyCheck(bool AutoDetect = false)
        {
            NextCheck = DateTime.Now.AddMinutes(3);
            bool keyCheck = false;
            //检测KEY
            try
            {
                if (RemoveKeyCount < 2)
                    keyCheck = Key.GetKeys().Check();
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Checking KEY failed!");
            }
            try
            {
                //检测KEY
                if (!keyCheck)
                {
                    if (RemoveKeyCount != -1 && AutoDetect)
                        ++RemoveKeyCount;

                    ExceptionPro.ExpInfoLog("Check KEY failed!");

                    // 清空数据和客户端
                    ClearDataAndService();

                    Key.GetKeys().Clear(true);

                }
                else
                {
                    // 首次成功打开时把次数重置为0
                    if (RemoveKeyCount == -1)
                        RemoveKeyCount = 0;

                    if (AutoDetect)
                    {
                        ExceptionPro.ExpInfoLog("Check KEY succeeded!");
                    }
                    

                    // 自动检查过程中(非自动检测U盘模式), 如果有失效的, 则去除掉
                    if (!AutoDetect)
                    {

                        // 清理失去连接的连接
                        CleanDisconnectService();

                        // 清理一下别的地方登陆帐号
                        CleanUsedByOtherService();

                        // 清理密码错误队列
                        CleanPasswordErrorList();

                    }



                    // 如果KEY过期,则清空
                    if (Resources.GetRes().IsExpired == 1)
                    {
                        ClearDataAndService();
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Check KEY operation failed.");
            }
        }


        /// <summary>
        /// 清空数据和客户端
        /// </summary>
        private void ClearDataAndService() {
            // 先记录一下日志
            OperateLog.Instance.RecordAll();
            //如果服务列表存在则清空
            if (Resources.GetRes().Services.Count > 0)
            {
                
                // 清空前先把日志保存掉

                Resources.GetRes().Services.Clear();

            }
            // 清空数据
            if(DBOperate.GetDBOperate().IsDataReady)
                DBOperate.GetDBOperate().ClearData();
        }


        /// <summary>
        /// 清理失去连接的
        /// </summary>
        private void CleanDisconnectService()
        {
            //KEY存在则刷新客户端,如有客户端2次没相应,则去掉它.如果还不到一次则增加未响应次数.如果正常则清零

            List<Client> removeServiceCount = new List<Client>();

            foreach (var item in Resources.GetRes().Services)
            {
                if ((DateTime.Now - item.CheckDate).TotalSeconds >= 60 * 3)
                {

                    int countLimit = 1;
                    if (item.DeviceType == 2 || item.DeviceType == 3) // 去掉了平板(又重新加上了), 毕竟现在平板用的情况不多,都是针对触摸屏. item.DeviceType == 2 || 
                    {

                        countLimit = 10 * 4; // 手机给了时间为未响应时间超出120分钟(每次3分钟* 40 = 120);
                    }

                    if (item.LostCount >= countLimit)
                    {
                        removeServiceCount.Add(item);
                    }
                    else
                    {
                        ++item.LostCount;
                    }

                }
                else
                {
                    item.LostCount = 0;
                }

                // 手机因为可以直接退出, 所以每40分钟多给一次机会防止误操作(改为每13分钟,免得手机频繁出问题导致无法进入)
                if (item.DeviceType == 3 && item.OldRemoveCount > 0 && DateTime.Now >= item.OldRemoveCheckData.AddMinutes(40 / 3))
                {
                    --item.OldRemoveCount;
                    item.OldRemoveCheckData = DateTime.Now;
                }
                // 非手机(PC1,平板2),每30分钟查一次
                else if (item.DeviceType < 3 && item.OldRemoveCount > 0 && DateTime.Now >= item.OldRemoveCheckData.AddMinutes(30))
                {
                    --item.OldRemoveCount;
                    item.OldRemoveCheckData = DateTime.Now;
                }
            }



            //移出失效的
            foreach (var item in removeServiceCount)
            {
                lock (Resources.GetRes().Services)
                {
                    CloseService(item);
                }
            }


        }

        /// <summary>
        /// 关闭当前客户端
        /// </summary>
        /// <param name="service"></param>
        internal void CloseService(Client service)
        {
            try
            {
                // 如果有老的通讯, 先终止掉它
                if (null != service.ClientChannel)
                {
                    ServiceOperate.GetServiceOperate().AbortOldChannel(service.ClientChannel);
                }
                else if (null != service.SignalRClientSessionId)
                {
                    var hub = GlobalHost.ConnectionManager.GetHubContext<ServiceHub>();
                    //hub.Clients.Client(service.SignalRClientSessionId).Close();

                    // SingnalR这个设计....不能手动关闭连接...
                }

                service.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

            Resources.GetRes().Services.Remove(service);
        }

        /// <summary>
        /// 清理其他人用的账户
        /// </summary>
        private void CleanUsedByOtherService()
        {
            List<Client> removeUsedServiceCount = new List<Client>();

            foreach (var item in Resources.GetRes().ServicesUsedByOther)
            {
                if ((DateTime.Now - item.CheckDate).TotalSeconds >= 60 * 3)
                {

                    // 这里之所以保留原始逻辑, 即便不按原始逻辑检测, 到时session也会失效
                    int countLimit = 1;


                    if (item.LostCount >= countLimit)
                    {
                        removeUsedServiceCount.Add(item);
                    }
                    else
                    {
                        ++item.LostCount;
                    }

                }
                else
                {
                    item.LostCount = 0;
                }
                
            }



            //移出失效的
            foreach (var item in removeUsedServiceCount)
            {
                lock (Resources.GetRes().ServicesUsedByOther)
                {
                    // 如果有老的通讯, 先终止掉它
                    if (null != item.ClientChannel)
                        ServiceOperate.GetServiceOperate().AbortOldChannel(item.ClientChannel);

                    item.Dispose();

                    Resources.GetRes().ServicesUsedByOther.Remove(item);
                }
            }
        }




        /// <summary>
        /// 清理密码错误队列
        /// </summary>
        private void CleanPasswordErrorList()
        {
            List<PasswordErrorModel> removePasswordErrorCount = new List<PasswordErrorModel>();

            foreach (var item in Resources.GetRes().PasswordErrorList)
            {
                if ((DateTime.Now - item.LastErrorData).TotalSeconds >= 60 * 5)
                {
                    if (item.ErrorCount <= 1)
                        removePasswordErrorCount.Add(item);
                    else
                        --item.ErrorCount;
                }
                

            }



            //移出失效的
            foreach (var item in removePasswordErrorCount)
            {
                Resources.GetRes().PasswordErrorList.Remove(item);
            }
        }


       


       


        /// <summary>
        /// 初始化服务器(获取服务端IP) (打算不获取了. 因为服务器IP万能就可以, 又支持私网或者公网IP都应该可以,你不愿意你自己调防火墙)
        /// </summary>
        public void InitialServer()
        {
            try
            {

                bool IsSuccessGetIP = false;


                using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServerConfig.txt"), Encoding.UTF8))
                {
                    string line = null;
                    string temp = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        //获取消费类型

                        if (!IsSuccessGetIP && line.Trim().StartsWith("IPAddress"))
                        {
                            try
                            {
                                temp = line.Trim().Split('=')[1];
                                if (System.Text.RegularExpressions.Regex.Match(temp, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$").Success)
                                {
                                    Resources.GetRes().IPAddress = temp;
                                    IsSuccessGetIP = true;
                                }
                                else if (string.IsNullOrWhiteSpace(temp)) // 默认localhost
                                {
                                    IsSuccessGetIP = true;
                                }
                                else
                                {
                                    ExceptionPro.ExpInfoLog("Unknow IPAddress param.");
                                }
                            }
                            catch (Exception)
                            {
                                ExceptionPro.ExpInfoLog("Unknow IPAddress param.");
                            }
                        }
                    }


                }

             
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }
        
    }

}
