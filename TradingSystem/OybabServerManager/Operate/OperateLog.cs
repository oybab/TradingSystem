using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oybab.DAL;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Model.Models;
using Oybab.ServerManager.Model.Service;
using Oybab.ServerManager.Res;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Oybab.ServerManager.Operate
{
    internal sealed class OperateLog
    {
        #region Instance
        private OperateLog() { }

        private static readonly Lazy<OperateLog> _instance = new Lazy<OperateLog>(() => new OperateLog());
        public static OperateLog Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance

        internal List<Log> AccessLogs = new List<Log>();
        private DateTime NextCheck = DateTime.Now; // 下次查询时间
        internal void StartRecordAccessLog()
        {
            NextCheck = DateTime.Now;
            Task.Factory.StartNew(new Action(() =>
            {
                for (; ; )
                {

                    if (DateTime.Now >= this.NextCheck)
                    {
                        RecordAll(true);
                    }
                    else
                    {
                        // 如果时间还没过期, 并且睡眠已经结束, 那就让它多睡1分钟
                        System.Threading.Thread.Sleep(1000 * 60 * 1);
                    }



                }
            }), TaskCreationOptions.LongRunning);
        }


        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="IsAuto"></param>
        internal void RecordAll(bool IsAuto = false)
        {
            try
            {
                lock (AccessLogs)
                {
                    if (AccessLogs.Count > 0 && DBOperate.GetDBOperate().IsDataReady)
                    {
                        int successSaveCount = 0;
                        using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                        {
                            ctx.Configuration.ProxyCreationEnabled = false;
                            ((IObjectContextAdapter)ctx).ObjectContext.CommandTimeout = Resources.GetRes().TIME_OUT_SHORT;//无用

                            TransactionOptions option = new TransactionOptions();
                            option.Timeout = TimeSpan.FromSeconds(Resources.GetRes().TIME_OUT_SHORT);//无用
                            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option, EnterpriseServicesInteropOption.Automatic))
                            {

                                ctx.Configuration.AutoDetectChangesEnabled = false;
                                ctx.Configuration.ValidateOnSaveEnabled = false;

                                foreach (var item in AccessLogs)
                                {
                                    ctx.Logs.Add(item);
                                }

                                successSaveCount = ctx.SaveChanges();
                                scope.Complete();
                            }

                            AccessLogs.Clear();

                        }

                    }

                    NextCheck = DateTime.Now.AddMinutes(5);
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log"></param>
        internal void AddRecord(long OperateId, long? OperateSubId, string OperateName, string session = null, ToServerService toServer = null, string model = null)
        {
            Log log = new Log();
            Client client = Resources.GetRes().Services.Where(x => x.SessionId == session).FirstOrDefault();
            if (null != client)
            {
                log.AdminId = client.AdminId;
                log.DeviceId = client.DeviceId;
            }

            if (null != toServer)
            {
                // 复制SessionId序列化后再恢复是因为, 序列化中不需要包括它, 节省数据库空间, 恢复它是为了防止它变空以后后续操作(如推送通知需要sessionId)收到影响
                string sessionId = toServer.SessionId;
                toServer.SessionId = null;
                model = JsonConvert.SerializeObject(toServer);
                toServer.SessionId = sessionId;
            }

            log.OperateId = OperateId;
            log.OperateSubId = OperateSubId;
            log.OperateName = OperateName;

            log.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            if (null != model)
            {
                log.Model = model;
            }


            lock (AccessLogs)
            {
                AccessLogs.Add(log);
            }
        }



        internal double LastBalanceHash = 0;
        /// <summary>
        /// 添加日志(余额变动)
        /// </summary>
        /// <param name="log"></param>
        internal void AddRecord(long OperateId, long? OperateSubId, string OperateName, long BalanceType, string session = null, ToServerService toServer = null, string model = null)
        {
            Log log = new Log();
            Client client = Resources.GetRes().Services.Where(x => x.SessionId == session).FirstOrDefault();
            if (null != client)
            {
                log.AdminId = client.AdminId;
                log.DeviceId = client.DeviceId;
            }


            if (null != toServer)
            {
                // 复制SessionId序列化后再恢复是因为, 序列化中不需要包括它, 节省数据库空间, 恢复它是为了防止它变空以后后续操作(如推送通知需要sessionId)收到影响
                string sessionId = toServer.SessionId;
                toServer.SessionId = null;
                model = JsonConvert.SerializeObject(toServer);
                toServer.SessionId = sessionId;
            }

            log.OperateId = OperateId;
            log.OperateSubId = OperateSubId;
            log.OperateName = OperateName;
            log.BalanceType = BalanceType;

            double currentHash = RefreshBalanceHash(false);

            if (currentHash != LastBalanceHash)
            {
                log.IsBalanceChange = 1;
                log.Balance = JsonConvert.SerializeObject(Resources.GetRes().BALANCES.Where(x => x.HideType != 1).ToList().Select(x =>
                  {
                      return x.FastCopy().ReChangeBalance();
                  }).ToList());
                LastBalanceHash = currentHash;
            }

            log.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            if (null != model)
            {
                log.Model = model;
            }

            lock (AccessLogs)
            {
                AccessLogs.Add(log);
            }
        }


        /// <summary>
        /// 设置哈希
        /// </summary>
        /// <returns></returns>
        internal double RefreshBalanceHash(bool ResetLastHash = true)
        {
            double hash = Resources.GetRes().BALANCES.Where(x => x.HideType != 1).Sum(x => x.BalancePrice);
            if (ResetLastHash)
                LastBalanceHash = hash;
            return hash;
        }



    }

}
