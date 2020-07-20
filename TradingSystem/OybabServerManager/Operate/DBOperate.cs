using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using Oybab.DAL;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Model.Models;
using Oybab.ServerManager.Res;
using System.Data.Entity.Core.EntityClient;
using System.Reflection;

namespace Oybab.ServerManager.Operate
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public sealed class DBOperate
    {

        private static DBOperate dbOperate = null;

        private DBOperate() { }
        public static DBOperate GetDBOperate()
        {
            if (null == dbOperate)
                dbOperate = new DBOperate();
            return dbOperate;
        }

        //数据库连接串
        internal string CONS = null;
        private string CONS_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ts.db");
        internal bool IsDataReady = false;
        internal bool IsNotSuccessLoadData = false;
        internal bool IsNotFoundDatabase = false;

        /// <summary>
        /// 加载数据
        /// </summary>
        internal void LoadData()
        {
            IsDataReady = false;
            IsNotSuccessLoadData = false;
            IsNotFoundDatabase = false;

            try
            {
                Config.GetConfig().GetConfigs();

                LoadSqlConn();


                bool IsDBKeyChange = false;
                bool IsUIDChange = false;

                if (string.IsNullOrWhiteSpace(Resources.GetRes().DB_KEY))
                {
                    Resources.GetRes().DB_KEY = "".GenereteRandomCode(32, 2);
                    IsDBKeyChange = true;
                }
                if (string.IsNullOrWhiteSpace(Resources.GetRes().UID))
                {
                    Resources.GetRes().UID = "".GenereteRandomCode(32, 1);
                    IsUIDChange = true;
                }


                if (IsDBKeyChange || IsUIDChange)
                    Config.GetConfig().SetConfig(Config.GetConfig().ReadConfig().GetLines().ToList());


                //检查数据库文件是否存在

                if (!File.Exists(CONS_PATH))
                {
                    IsNotFoundDatabase = true;



                    DbOperator dbOperator = new DbOperator();
                    string adminPassword = "123456";
                    


                   
                    adminPassword = Key.GetKeys().Encryption(adminPassword);

                    dbOperator.CreateNewDB(CONS_PATH, Resources.GetRes().DB_KEY, adminPassword);

                    IsNotFoundDatabase = false;


                    ExceptionPro.ExpInfoLog("Successflully created a new database!");
                }



                InitialConn(Resources.GetRes().DB_KEY);

#if !DEBUG
                //检查数据库文件密码是否为空
                bool IsInvalidDB = false;
                try
                {
                    using (System.Data.SQLite.SQLiteConnection _con = new System.Data.SQLite.SQLiteConnection("Data Source=" + CONS_PATH + ";Password=;"))
                    {
                        _con.Open();
                        DataTable tables = _con.GetSchema("Tables");
                        IsInvalidDB = true;
                    }
                }
                catch
                {
                    IsInvalidDB = false;
                }

                if (IsInvalidDB)
                {
                    IsNotFoundDatabase = true;
                    throw new OybabException("Invalid database!");
                }
#endif

                //备份
                Backup.Instance.BackupFile();

                //加载缓存
                LoadCache();

            }
            catch (Exception ex)
            {

                ExceptionPro.ExpLog(ex, null, false, "Exception_DatabaseLoadFailed");
            }
        }

        
        /// <summary>
        /// 清空数据
        /// </summary>
        internal void ClearData()
        {
            IsDataReady = false;
            IsNotFoundDatabase = false;
            IsNotSuccessLoadData = false;
           

            Resources.GetRes().ROOMS = null;
            Resources.GetRes().PRODUCT_TYPES = null;
            Resources.GetRes().PRODUCTS = null;

            Resources.GetRes().ROOMS_Model = new List<RoomModel>();
            Resources.GetRes().TAKEOUT_Model = new List<TakeoutModel>();

            Resources.GetRes().ADMINS = null;
            Resources.GetRes().MEMBERS = null;
            Resources.GetRes().SUPPLIERS = null;
            Resources.GetRes().PRINTERS = null;
            Resources.GetRes().DEVICES = null;
            Resources.GetRes().REQUESTS = null;
            Resources.GetRes().PPRS = null;
            Resources.GetRes().BALANCES = null;


        }


        /// <summary>
        /// 加载
        /// </summary>
        private void LoadSqlConn()
        {
            try
            {
                var dataSet = ConfigurationManager.GetSection("system.data") as System.Data.DataSet;
                List<DataRow> removeList = new List<DataRow>();
                foreach (DataRow item in dataSet.Tables[0].Rows)
                {
                    if (item.ItemArray.Contains("System.Data.SQLite") || item.ItemArray.Contains("System.Data.SQLite.EF6"))
                    {
                        removeList.Add(item);
                    }
                }

                foreach (var item in removeList)
                {
                    dataSet.Tables[0].Rows.Remove(item);
                }

            }
            catch (System.Data.ConstraintException) { }


            try
            {
                var dataSet = ConfigurationManager.GetSection("system.data") as System.Data.DataSet;

                dataSet.Tables[0].Rows.Add("SQLite Data Provider (Entity Framework 6)"
                , ".Net Framework Data Provider for SQLite (Entity Framework 6)"
                , "System.Data.SQLite.EF6"
                , "System.Data.SQLite.EF6.SQLiteProviderFactory, System.Data.SQLite.EF6, Version=1.0.98.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139");
            }
            catch (System.Data.ConstraintException) { }

           
            
        }

   

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pass"></param>
        private void InitialConn(string pass)
        {
            try
            {

                //写入EF对象
                EntityConnectionStringBuilder entity = new EntityConnectionStringBuilder();
                entity.Provider = "System.Data.SQLite.EF6";


                entity.ProviderConnectionString = "data source=" + CONS_PATH + ";password=" + pass + ";baseschemaname=main;Version=3";//;Timeout=600000;Synchronous=Off
                entity.Metadata = string.Format("res://{0}/TradingSystemModel.csdl|res://{0}/TradingSystemModel.ssdl|res://{0}/TradingSystemModel.msl", typeof(tsEntities).Assembly.FullName);

                CONS = entity.ConnectionString;
            }
            catch (Exception ex)
            {
                throw new OybabException("ConnStrError: " + ex);
            }
        }

        /// <summary>
        /// 加载缓存
        /// </summary>
        internal void LoadCache(bool LazyLoadingEnabled = true, bool PrepareData = true)
        {
            try
            {
                // 先获取Database表并升级
                Database database = null;
                using (var ctx = new tsEntities(CONS, false))
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    ctx.Configuration.LazyLoadingEnabled = false;
                    database = ctx.Databases.FirstOrDefault();
                }

                // 升级数据库
                DBUpdate.Instance.UpdateDatabase(database);


                // 获取数据
                using (var ctx = new tsEntities(CONS, false))
                {
                    ctx.Configuration.ProxyCreationEnabled = false;
                    ctx.Configuration.LazyLoadingEnabled = false;


                    //加载包厢
                    LoadRooms(ctx);
                    //加载包厢(用来控制) 和 菜单(如果开启了菜单)
                    LoadRoomsOrders(ctx);

                    //加载外卖
                    LoadTakeouts(ctx);

                    // 加载产品和产品类型
                    LoadProductTypes(ctx);
                    LoadProducts(ctx);

                    // 加载管理员
                    LoadAdmin(ctx);
                    // 加载会员
                    LoadMember(ctx);
                    // 加载供应商
                    LoadSupplier(ctx);
                    // 加载设备
                    LoadDevices(ctx);
                    // 加载请求
                    LoadRequests(ctx);

                    // 加载打印机和打印机绑定
                    LoadPrinters(ctx);
                    LoadPprs(ctx, PrepareData);

                    // 加载余额
                    LoadBalance(ctx);

                    
                }
               
                IsDataReady = true;
            }
            catch (Exception ex)
            {
                IsNotSuccessLoadData = true;
                throw new OybabException("Exception_LoadCacheError: ", ex);
            }

        }

        
       

        /// <summary>
        /// 加载包厢
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadRooms(tsEntities ctx)
        {
            Resources.GetRes().ROOMS = ctx.Rooms.ToList();
        }

        /// <summary>
        /// 获取外卖
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadTakeouts(tsEntities ctx)
        {
            Resources.GetRes().TAKEOUT_Model = new List<TakeoutModel>();
        }


       
        /// <summary>
        /// 加载产品类型
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadProductTypes(tsEntities ctx)
        {
            Resources.GetRes().PRODUCT_TYPES = ctx.ProductTypes.ToList();
        }


        /// <summary>
        /// 加载产品
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadProducts(tsEntities ctx)
        {
            Resources.GetRes().PRODUCTS = ctx.Products.ToList();
        }



       

        /// <summary>
        /// 加载包厢与订单关系
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadRoomsOrders(tsEntities ctx)
        {

            Resources.GetRes().ROOMS_Model = new List<RoomModel>();

            long time = long.Parse(DateTime.Now.AddYears(-1).ToString("yyyyMMddHHmmss"));
            
            //加载订单及订单详细

            foreach (var item in Resources.GetRes().ROOMS.Where(x=>x.HideType == 0))
            {
                Resources.GetRes().ROOMS_Model.Add(new RoomModel() { HideType = item.HideType, Order = item.Order, RoomId = item.RoomId, RoomNo = item.RoomNo, PayOrder = ctx.Orders.Include("tb_orderdetail").Include("tb_orderpay").Where(x => x.AddTime > time && x.RoomId == item.RoomId && x.State == 0).FirstOrDefault(), OrderSession = Guid.NewGuid().ToString() });
            }
        }


        /// <summary>
        /// 加载管理员
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadAdmin(tsEntities ctx)
        {
            Resources.GetRes().ADMINS = ctx.Admins.ToList();
        }


        /// <summary>
        /// 加载会员
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadMember(tsEntities ctx)
        {
            Resources.GetRes().MEMBERS = ctx.Members.ToList();
        }


        /// <summary>
        /// 加载供应商
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadSupplier(tsEntities ctx)
        {
            Resources.GetRes().SUPPLIERS = ctx.Suppliers.ToList();
        }

        /// <summary>
        /// 加载设备
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadDevices(tsEntities ctx)
        {
            Resources.GetRes().DEVICES = ctx.Devices.ToList();
        }


        /// <summary>
        /// 加载打印机
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadPrinters(tsEntities ctx)
        {
            Resources.GetRes().PRINTERS = ctx.Printers.ToList();
        }


        /// <summary>
        /// 加载请求
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadRequests(tsEntities ctx)
        {
            Resources.GetRes().REQUESTS = ctx.Requests.ToList();
        }

        /// <summary>
        /// 加载打印机和产品打印绑定
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadPprs(tsEntities ctx, bool PrepareData)
        {
            Resources.GetRes().PPRS = ctx.Pprs.ToList();

        }




        /// <summary>
        /// 加载余额
        /// </summary>
        /// <param name="ctx"></param>
        private void LoadBalance(tsEntities ctx)
        {
            Resources.GetRes().BALANCES = ctx.Balances.ToList();

            OperateLog.Instance.RefreshBalanceHash();
        }





        private sealed class DbOperator
        {


            internal void CreateNewDB(string Conn, string dbNewPassword, string adminNewPassword)
            {
                ExceptionPro.ExpInfoLog("Database not found, creating a new database!");


                string osPath = Path.GetTempFileName();
                ExportFile("Oybab.ServerManager.Resources.Database.ts.db", osPath);

                // 如果有密码需要先创建密码
                ChangePassword(osPath, dbNewPassword, "", adminNewPassword);

                //现在返回(暂时先先复制再返回新文件路径, 免得无法读取,提示什么进程正在使用之类的错误).

                File.Copy(osPath, Conn, false);
            }


            /// <summary>
            /// 输出
            /// </summary>
            /// <param name="assemblyName"></param>
            /// <param name="dllPath"></param>
            private void ExportFile(string assemblyName, string dllPath)
            {
                using (Stream stm = Assembly.GetExecutingAssembly().GetManifestResourceStream(assemblyName))
                {
                    // Copy the assembly to the temporary file
                    try
                    {
                        using (Stream outFile = File.Create(dllPath))
                        {
                            int sz = 4096;
                            byte[] buf = new byte[sz];
                            while (true)
                            {
                                int nRead = stm.Read(buf, 0, sz);
                                if (nRead < 1)
                                    break;
                                outFile.Write(buf, 0, nRead);
                            }
                        }
                    }
                    catch
                    {
                        // This may happen if another process has already created and loaded the file.
                        // Since the directory includes the version number of this assembly we can
                        // assume that it's the same bits, so we just ignore the excecption here and
                        // load the DLL.
                    }
                }
            }


            /// <summary>
            /// 修改密码
            /// </summary>
            /// <param name="DbFilePath"></param>
            /// <param name="newPassword"></param>
            /// <param name="OriginalPassword"></param>
            /// <param name="InnerPassword"></param>
            private void ChangePassword(string DbFilePath, string newPassword, string OriginalPassword, string InnerPassword)
            {
                System.Data.SQLite.SQLiteConnection _con = new System.Data.SQLite.SQLiteConnection();
                _con.ConnectionString = "Data Source=" + DbFilePath;
                if (OriginalPassword.Length > 0)
                {
                    _con.ConnectionString += ";Password=" + OriginalPassword;
                }
                try
                {
                    _con.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Connect database failed! :" + ex.Message);
                }


                try
                {
                    System.Data.Common.DbCommand comm = _con.CreateCommand();

                        comm.CommandText = "update tb_admin set Password = @Password where adminId = 1";

                    comm.CommandType = System.Data.CommandType.Text;
                    comm.Parameters.Add(new System.Data.SQLite.SQLiteParameter("@Password", InnerPassword));
                    int result = comm.ExecuteNonQuery();

                    if (result == 0)
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    throw new Exception("Change admin password failed! :" + ex.Message);
                }

                //
                //先修改内部密码
                try
                {
                    _con.ChangePassword(newPassword);
                }
                catch (Exception ex)
                {
                    throw new Exception("Encruption database failed!" + ex.Message);
                }
                _con.Close();
            }



        }

    }
}
