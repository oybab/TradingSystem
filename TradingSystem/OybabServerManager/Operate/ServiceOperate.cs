using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Transactions;
using Oybab.DAL;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Model.Models;
using Oybab.ServerManager.Model.Service;
using Oybab.ServerManager.Model.Service.Common;
using Oybab.ServerManager.Model.Service.Import;
using Oybab.ServerManager.Model.Service.ImportDetail;
using Oybab.ServerManager.Model.Service.Order;
using Oybab.ServerManager.Model.Service.OrderDetail;
using Oybab.ServerManager.Model.Service.Product;
using Oybab.ServerManager.Model.Service.ProductType;
using Oybab.ServerManager.Model.Service.Room;
using Oybab.ServerManager.Model.Service.Admin;
using Oybab.ServerManager.Model.Service.Member;
using Oybab.ServerManager.Model.Service.Printer;
using Oybab.ServerManager.Model.Service.Device;
using Oybab.ServerManager.Model.Service.Takeout;
using Oybab.ServerManager.Model.Service.TakeoutDetail;
using Oybab.ServerManager.Model.Service.MemberPay;
using System.ServiceModel.Channels;
using Oybab.ServerManager.Model.Service.Supplier;
using Oybab.ServerManager.Model.Service.SupplierPay;
using Oybab.ServerManager.Model.Service.AdminLog;
using Oybab.ServerManager.Model.Service.AdminPay;
using Oybab.ServerManager.Model.Service.Log;
using Oybab.ServerManager.Model.Service.BalancePay;
using Oybab.ServerManager.Model.Service.Request;
using System.Threading.Tasks;
using Oybab.ServerManager.Model.Service.Balance;
using Microsoft.AspNet.SignalR;
using Oybab.Report;
using Oybab.Report.Model;

namespace Oybab.ServerManager.Operate
{
    /// <summary>
    /// 服务操作
    /// </summary>
    public sealed class ServiceOperate
    {
        private static ServiceOperate serviceOperates = null;
        private ServiceOperate() {}
        public static ServiceOperate GetServiceOperate()
        {
            if (null == serviceOperates)
                serviceOperates = new ServiceOperate();
            return serviceOperates;
        }


        



        private BackgroundJobSchedueller CallbackTask { get; set; } = new BackgroundJobSchedueller();


        #region Common


        /// <summary>
        /// 服务新情求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceNewRequest ServiceNewRequest(ToServerServiceNewRequest toServer, string SignalRSessionId, string IpAddress)
        {
            try
            {
#if !DEBUG
                // 2秒等待免得不停发送请求
                System.Threading.Thread.Sleep(2000);

            
                if (Math.Abs((toServer.Time - DateTime.Now).TotalMinutes) > 3)
                {
                    return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.ServerClientTimeMisalignment };
                }


                // 目前暂时针对PC版和平板版做个不一致提醒要求, 后续增加手机版版本对比. (目前暂时用=对比,后续再考虑是否用<来代替)
                if (null != toServer.CurrentVersion && toServer.CurrentVersion != GetVersion())
                {
                    return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.ServerClientVersionMisalignment };
                }
#endif

                //查询KEY
                if (!Res.Key.GetKeys().Check())
                {
                    return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.KeyCheckFaild };
                }

                // 是否过期
                if (Resources.GetRes().IsExpired == 1)
                {
                    return new ToClientServiceNewRequest() { IsExpired = true, RegTimeRequestCode = Resources.GetRes().RegTimeRequestCode };
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    // 未找到数据库
                    if (DBOperate.GetDBOperate().IsNotFoundDatabase)
                    {
                        return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.DatabaseNotFound };
                    }
                    // 数据库加载失败
                    else if (DBOperate.GetDBOperate().IsNotSuccessLoadData)
                    {
                        return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.DatabaseLoadFailed };
                    }
                    // 数据未准备好
                    else
                    {
                        return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.DataNotReady };
                    }
                }

                // 查询密码错误次数
                if (Resources.GetRes().PasswordErrorList.Any(x => x.AdminNo == toServer.AdminNo))
                {
                    PasswordErrorModel errPasswordModel = Resources.GetRes().PasswordErrorList.Where(x => x.AdminNo == toServer.AdminNo).FirstOrDefault();
                    
                    if (errPasswordModel.ErrorCount >= 5 || errPasswordModel.TotalErrorCount >= 5)
                    {
                        OperateLog.Instance.AddRecord(-1, null, "PasswordError#" + OperateType.None, null, toServer);
                        return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.PasswordErrorCountLimit };
                    }
                    
                }

                int oldRemoveCount = 0;
                DateTime firstLogin = DateTime.Now;
                DateTime lastLogin = DateTime.Now;

               
                //检查IP(设备)
                string ip = GetIp(IpAddress);
                Device device = null;

                // 本地IP打开的, 直接绿灯. 让他直接获取第一个设备
                if (ip == "127.0.0.1" || ip == "::1") 
                {
                    device = Resources.GetRes().DEVICES.FirstOrDefault();
                }
                else
                {
                    // 如果没找到同样IP
                    if (Resources.GetRes().DEVICES.Where(x => x.IpAddress == ip).Count() == 0)
                    {
                        // 获取通用地址IP, 并同类型的设备
                        device = Resources.GetRes().DEVICES.Where(x => x.IpAddress == "*" && x.DeviceType == toServer.DeviceType && toServer.DeviceType != 0).FirstOrDefault();

                        if (null == device)
                        {
                            device = Resources.GetRes().DEVICES.Where(x => x.IpAddress == "*" && x.DeviceType == 0).FirstOrDefault();
                        }

                        // 如果依然没找到则IP无效
                        if (null == device)
                        {
                            return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.IPInvalid };
                        }
                        // 找到了,但是没开启的,则设备为禁用
                        else
                        {
                            if (device.IsEnable == 0)
                            {
                                return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.UnknownDevice };
                            }
                        }     
                    }
                    // 如果找到了同类型的设备则获取一个已开启的
                    else
                    {
                        IEnumerable<Device> devices = Resources.GetRes().DEVICES.Where(x => x.IpAddress == ip);

                        foreach (var item in devices)
                        {
                            if (item.IsEnable == 1)
                            {
                                if (item.DeviceType == toServer.DeviceType && toServer.DeviceType != 0)
                                {
                                    device = item;
                                    break;
                                }
                                else if (item.DeviceType == 0)
                                {
                                    device = item;
                                    break;
                                }
                            }
                        }

                        if (null == device)
                        {
                            return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.UnknownDevice };
                        }
                    }
                }


             

                //获取数据库中的密码
                bool result = false;
                bool validResult = false;
                long adminId = 0;
                bool changePWD = false;



                Admin admin = Resources.GetRes().ADMINS.Where(x => x.AdminNo == toServer.AdminNo).FirstOrDefault();

                if (null != admin)
                {

                    string no = null;
                    
                    if (!string.IsNullOrWhiteSpace(toServer.PWD) && toServer.PWD.Length >= 6)
                    {
                        try
                        {
                            string password = toServer.PWD;
                            if (null != admin.Salt)
                            {
                                password = (toServer.PWD + admin.Salt).CreateMD5();
                            }

                            no = Res.Key.GetKeys().Encryption(password);
                        }
                        catch (Exception ex)
                        {
                            ExceptionPro.ExpLog(ex, null, false, "Key encryption failed.");
                            return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.KeyFaild };
                        }
                    }

                    if (admin.Password == no)
                    {
                        if (admin.IsEnable == 0)
                        {
                            return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.UnknownAdmin };
                        }
                        else
                        {
                            validResult = true;
                            adminId = admin.AdminId;
                        }
                    }

                }
                else
                {
                    return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.UnknownAdmin };
                }
                  

                //如果验证失败,也试试UID方式
                if (!validResult && toServer.AdminNo == "1000")
                {
                    if (Resources.GetRes().UID.Substring(0, 8) + Resources.GetRes().DB_KEY.Substring(0, 8) == toServer.PWD)
                    {
                        adminId = 1;
                        changePWD = true;
                        validResult = true;
                    }
                }

                // 如果密码错误, 则查询密码错误队列
                if (!validResult)
                {
                    PasswordErrorModel errPasswordModel = Resources.GetRes().PasswordErrorList.Where(x => x.AdminNo == toServer.AdminNo).FirstOrDefault();
                    if (null == errPasswordModel)
                    {
                        errPasswordModel = new PasswordErrorModel() { AdminNo = toServer.AdminNo, ErrorCount = 1, LastErrorData = DateTime.Now };
                        Resources.GetRes().PasswordErrorList.Add(errPasswordModel);
                    }
                    else
                    {
                        ++errPasswordModel.ErrorCount;
                        ++errPasswordModel.TotalErrorCount;
                        errPasswordModel.LastErrorData = DateTime.Now;
                    }
                }
                else
                {
                    PasswordErrorModel errPasswordModel = Resources.GetRes().PasswordErrorList.Where(x => x.AdminNo == toServer.AdminNo).FirstOrDefault();
                    if (null != errPasswordModel)
                    {
                        Resources.GetRes().PasswordErrorList.Remove(errPasswordModel); 
                    }
                }


                string SessionId = null;
                Client newClient = null;
                bool IsAdminUsing = false;
                if (validResult)
                {



                    // 针对第一次请求检查IP并查看列表中是否存在这IP. 如果覆盖次数没超过2次移出它, 如果超过了提示超过最大连接限制
                    Client client = Resources.GetRes().Services.Where(x => x.IP == ip && x.AdminId == admin.AdminId && x.DeviceId == device.DeviceId && x.ClientIP == toServer.CI && x.ClientMAC == toServer.CM).FirstOrDefault();
                    if (null != client)
                    {
                        oldRemoveCount = client.OldRemoveCount + 1;
                        firstLogin = client.FirstLogin;

                        lock (Resources.GetRes().Services)
                        {
                            Resources.GetRes().CloseService(client);
                        }
                    }


                    //查看是否超出限制
                    if (Resources.GetRes().Services.Count >= Resources.GetRes().SERVICE_COUNT)
                    {
                        return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.CountOutOfLimit };
                    }



                    //创建新Session
                    SessionId = Guid.NewGuid().ToString();

                    newClient = new Client() { SessionId = SessionId, AdminId = adminId, CheckDate = DateTime.Now, IP = ip, OldRemoveCount = oldRemoveCount, IsConnected = true, ClientChannel = OperationContext.Current?.Channel, ClientCallback = OperationContext.Current?.GetCallbackChannel<IServiceCallback>(), DeviceId = device.DeviceId, RoomId = device.RoomId, DeviceType = toServer.DeviceType, Mode = admin.Mode, FirstLogin = firstLogin, LastLogin = lastLogin, ClientMAC = toServer.CM, ClientIP = toServer.CI, IsLocalPrintCustomOrder = toServer.IsLocalPrintCustomOrder, NotificationCaches = new List<NotificationCache>(), SignalRClientSessionId = SignalRSessionId };



                   
                    // 查看是否当前管理员已在用, 有就提示, 没有则加入队列
                    if (Resources.GetRes().Services.Where(x => x.AdminId == admin.AdminId).Count() > 0)
                    {
                        // 如果是1999用户guest用户就忽略,不然提示管理员已存在(主要是允许1999客户账户重复使用, 这是为了允许其他程序和IP以及Room绑定使用的)
                        if (admin.AdminNo == "1999" && admin.Mode == 0)
                        {
                            // 如果是通用IP或者不是电脑版则提示无效设备
                            if (device.IpAddress == "*" || device.DeviceType != 1 || null == device.RoomId)
                            {
                                return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.UnknownDevice };
                            }

                            if (Resources.GetRes().Services.Where(x => null != x.RoomId && x.RoomId == device.RoomId).Count() > 0)
                                return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.DeviceExists };
                        }
                        else
                        {
                            lock(Resources.GetRes().Services)
                            {
                                Client service = Resources.GetRes().Services.Where(x => x.AdminId == admin.AdminId).FirstOrDefault();
                                
                                Resources.GetRes().CloseService(service);

                                Resources.GetRes().ServicesUsedByOther.Add(service);


                                // 下次用DeviceId
                                if (service.ClientMAC != toServer.CM)
                                    IsAdminUsing = true;
                            }
                        }
                    }

                    // 如果当前用户1999guest用户, 并且IP是星并不是电脑就无效设备
                    else if (admin.AdminNo == "1999" && admin.Mode == 0)
                    {
                        // 如果是通用IP或者不是电脑版则提示无效设备
                        if (device.IpAddress == "*" || device.DeviceType != 1 || null == device.RoomId)
                        {
                            return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.UnknownDevice };
                        }

                        if (Resources.GetRes().Services.Where(x => null != x.RoomId && x.RoomId == device.RoomId).Count() > 0)
                            return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.DeviceExists };

                    }



                    lock (Resources.GetRes().Services)
                    {
                        Resources.GetRes().Services.Add(newClient);
                        
                    }

                    result = true;
                }

                //返回它
                
                if (result)
                {

                    OperateLog.Instance.AddRecord(admin.AdminId, null, "Login#" + OperateType.None, SessionId, null, JsonConvert.SerializeObject(newClient));

                    ToClientServiceNewRequest s = new ToClientServiceNewRequest() { SessionId = SessionId, ChangePassword = changePWD, Name_0 = Resources.GetRes().KEY_NAME_0, Name_1 = Resources.GetRes().KEY_NAME_1, Name_2 = Resources.GetRes().KEY_NAME_2, ExpiredRemaningDays = Resources.GetRes().ExpiredRemainingDays, RegTimeRequestCode = Resources.GetRes().RegTimeRequestCode, Products = JsonConvert.SerializeObject(Resources.GetRes().PRODUCTS), ProductTypes = JsonConvert.SerializeObject(Resources.GetRes().PRODUCT_TYPES), Rooms = JsonConvert.SerializeObject(Resources.GetRes().ROOMS), RoomsModel = JsonConvert.SerializeObject(Resources.GetRes().ROOMS_Model), Result = result, ValidResult = validResult, Admin = JsonConvert.SerializeObject(admin), Device = JsonConvert.SerializeObject(device), Admins = JsonConvert.SerializeObject(Resources.GetRes().ADMINS), Services = JsonConvert.SerializeObject(Resources.GetRes().Services), Balances = JsonConvert.SerializeObject(Resources.GetRes().BALANCES.Select(x=> { return x.FastCopy(false, true); })), Devices = JsonConvert.SerializeObject(Resources.GetRes().DEVICES), Requests = JsonConvert.SerializeObject(Resources.GetRes().REQUESTS), Printers = JsonConvert.SerializeObject(Resources.GetRes().PRINTERS), Pprs = JsonConvert.SerializeObject(Resources.GetRes().PPRS), IsExpired = (Resources.GetRes().IsExpired != 0), DeviceCount = Resources.GetRes().SERVICE_COUNT, RoomCount = Resources.GetRes().ROOM_COUNT, MinutesIntervalTime = Resources.GetRes().MINUTES_INTERNAL_TIME, HoursIntervalTime = Resources.GetRes().HOURS_INTERNAL_TIME, PrintInfo = JsonConvert.SerializeObject(Resources.GetRes().PrintInfo), ExtendInfo = JsonConvert.SerializeObject(Resources.GetRes().ExtendInfo), IsFireAlarmEnable = Resources.GetRes().IsFireAlarmEnable, IsAdminUsing = IsAdminUsing };

                    return s;
                }
                else
                {
                    return new ToClientServiceNewRequest() { ValidResult = validResult, Result = result };
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "New request operation failed.");
                //返回它
                return new ToClientServiceNewRequest() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceChangePWD ServiceChangePWD(ToServerServiceChangePWD toServer)
        {

            try
            {
                // 2秒等待免得不停发送请求
                System.Threading.Thread.Sleep(2000);

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceChangePWD() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                    client.CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceChangePWD() { ExceptionType = ServiceExceptionType.DataNotReady };
                }




                // Admin 更新日期不一致, 不能更新
                //解析出来
                Admin model = toServer.Admin.DeserializeObject<Admin>();
                Admin serverModel = Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceChangePWD() { ExceptionType = ServiceExceptionType.UpdateModel };
                }


                string oldPWD;
                string newPWD;
                string oldSalt = null;
                string newSalt = null;
                try
                {
                    string OldPWD = toServer.OldPWD;
                    if (null != serverModel.Salt)
                    {
                        OldPWD = (toServer.OldPWD + serverModel.Salt).CreateMD5();
                        oldSalt = serverModel.Salt;
                    }

                    newSalt = "".GenereteRandomCode(32, 1);
                    toServer.NewPWD = (toServer.NewPWD + newSalt).CreateMD5();



                    oldPWD = Res.Key.GetKeys().Encryption(OldPWD);
                    newPWD = Res.Key.GetKeys().Encryption(toServer.NewPWD);
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Key encryption failed.");
                    return new ToClientServiceChangePWD() { ExceptionType = ServiceExceptionType.KeyFaild };
                }

                //连接数据库
                bool validResult = false;
                bool result = false;

                try
                {
                    Admin newModel = Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId && x.Password == oldPWD).FirstOrDefault();

                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS))
                    {
                        if (null != newModel)
                        {
                            validResult = true;
                            newModel.Password = newPWD;
                            if (null != newSalt)
                                newModel.Salt = newSalt;

                            newModel.ClearReferences();

                            ctx.Entry(newModel).State = System.Data.Entity.EntityState.Modified; // System.Data.Entity.EntityState.Modified;
                            int results = ctx.SaveChanges();
                            if (results > 0)
                            {
                                result = true;
                                model = newModel;
                            }
                        }

                        //如果验证失败,也试试UID方式
                        if (!validResult && (null != model && model.AdminNo == "1000"))
                        {
                            if (Resources.GetRes().UID.Substring(0,8) + Resources.GetRes().DB_KEY.Substring(0, 8) == toServer.OldPWD)
                            {
                                validResult = true;
                                model.Password = newPWD;
                                if (null != newSalt)
                                    model.Salt = newSalt;

                                model.ClearReferences();

                                ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                                int results = ctx.SaveChanges();
                                if (results > 0)
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (serverModel.Salt != oldSalt)
                    {
                        serverModel.Salt = oldSalt;
                        serverModel.Password = toServer.OldPWD;
                    }

                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceChangePWD() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                // 记录日志
                if (result)
                {
                    Resources.GetRes().ADMINS.Remove(Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault());
                    Resources.GetRes().ADMINS.Add(model);

                    OperateLog.Instance.AddRecord(model.AdminId, null, "Password#" + OperateType.Edit);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Admin, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                return new ToClientServiceChangePWD() { Result = result, ValidResult = validResult, Admin = JsonConvert.SerializeObject(model) };

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Change password operation failed.");
                //返回它
                return new ToClientServiceChangePWD() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 获取UID
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceUID ServiceUID(ToServerServiceUID toServer)
        {
            try
            {
                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceUID() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }

                return new ToClientServiceUID() { UID = Resources.GetRes().UID };
            }
            catch (Exception ex)
            {

                ExceptionPro.ExpLog(ex, null, false, "Get UID operation failed.");
                //返回它
                return new ToClientServiceUID() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 写入设置
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSetCon ServiceSetCon(ToServerServiceSetCon toServer)
        {
            try
            {
                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceSetCon() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceSetCon() { ExceptionType = ServiceExceptionType.DataNotReady };
                }


                bool Result = Config.GetConfig().SetConfig(toServer.Config.DeserializeObject<List<string>>());

                // 记录日志
                if (Result)
                {
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "Setting#" + OperateType.Edit, toServer.SessionId, toServer);

                    // 发送给其他客户端, 配置变更提醒
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = toServer.Config, ModelType = ModelType.Config, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                return new ToClientServiceSetCon(){ Result = Result};

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Set config operation failed.");
                //返回它
                return new ToClientServiceSetCon() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        /// <summary>
        /// 发送操作
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSend ServiceSend(ToServerServiceSend toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceSend() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceSend() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                // 记录日志
                if (Result)
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "Send#" + OperateType.None, toServer.SessionId, toServer);

                switch (toServer.SendType)
                {
                    case SendType.ShutdownServer:
                        new Action(() => {
                            System.Threading.Thread.Sleep(3000);
                            OperatePC.GetOperates().Shutdown();
                        }).BeginInvoke(null, null);
                        return new ToClientServiceSend() { Result = true };
                    case SendType.RestartServer:
                        new Action(() => {
                            System.Threading.Thread.Sleep(3000);
                            OperatePC.GetOperates().Resart();
                        }).BeginInvoke(null, null);
                        return new ToClientServiceSend() { Result = true };
                    case SendType.Call:

                        //解析出来(这里解析出了设备ID,但是刚开始我定义的变量名为RoomsId,以后有机会修改掉)
                        List<long> DeviceIds = toServer.RoomsId.DeserializeObject<List<long>>();

                        Client service = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                        long? RoomId = Resources.GetRes().DEVICES.Where(x => x.DeviceId == service.DeviceId).Select(x => x.RoomId).FirstOrDefault();
                        NotificateSend(toServer.SendType, service.SessionId, DeviceIds, toServer.Message, RoomId, toServer.Model, toServer.ModelExt, toServer.AdminId);
                        //操作
                        Result = true;

                        //返回它
                        return new ToClientServiceSend() { Result = Result };
                    case SendType.FireOn:
                    case SendType.FireOff:

                        //解析出来(这里解析出了设备ID,但是刚开始我定义的变量名为RoomsId,以后有机会修改掉)
                        List<long> DeviceIds2 = toServer.RoomsId.DeserializeObject<List<long>>();

                        Client service2 = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                        long? RoomId2 = Resources.GetRes().DEVICES.Where(x => x.DeviceId == service2.DeviceId).Select(x => x.RoomId).FirstOrDefault();

                        Resources.GetRes().IsFireAlarmEnable = (toServer.SendType == SendType.FireOn ? true : false);
                        NotificateSend(toServer.SendType, service2.SessionId, DeviceIds2, toServer.Message, RoomId2, toServer.Model, toServer.ModelExt, toServer.AdminId);
                        //操作
                        Result = true;

                        //返回它
                        return new ToClientServiceSend() { Result = Result };

                    case SendType.ExtendInfo:

                        //解析出来(这里解析出了设备ID,但是刚开始我定义的变量名为RoomsId,以后有机会修改掉)
                        List<long> DeviceIds3 = toServer.RoomsId.DeserializeObject<List<long>>();

                        Client service3 = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                        long? RoomId3 = Resources.GetRes().DEVICES.Where(x => x.DeviceId == service3.DeviceId).Select(x => x.RoomId).FirstOrDefault();

                        Resources.GetRes().ExtendInfo = toServer.Model.DeserializeObject<ExtendInfo>();
                        NotificateSend(toServer.SendType, service3.SessionId, DeviceIds3, toServer.Message, RoomId3, toServer.Model, toServer.ModelExt, toServer.AdminId);
                        //操作
                        Result = true;

                        //返回它
                        return new ToClientServiceSend() { Result = Result };
                    case SendType.Message:

                        //解析出来(这里解析出了设备ID,但是刚开始我定义的变量名为RoomsId,以后有机会修改掉)
                        List<long> DeviceIds4 = toServer.RoomsId.DeserializeObject<List<long>>();

                        Client service4 = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                        long? RoomId4 = Resources.GetRes().DEVICES.Where(x => x.DeviceId == service4.DeviceId).Select(x => x.RoomId).FirstOrDefault();


                        NotificateSend(toServer.SendType, service4.SessionId, DeviceIds4, toServer.Message, RoomId4, toServer.Model, toServer.ModelExt, toServer.AdminId);
                        //操作
                        Result = true;

                        //返回它
                        return new ToClientServiceSend() { Result = Result };
                    case SendType.OpenCashDrawer:

                        Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                        Admin model = Resources.GetRes().ADMINS.Where(x => x.AdminId == client.AdminId).FirstOrDefault();

                        if (model.Mode == 1 && (null == model.Menu || !model.Menu.Contains("t000")))
                            return new ToClientServiceSend() { Result = false };
                        else if (model.Mode == 0)
                            return new ToClientServiceSend() { Result = false };


                        try
                        {
                            // 如果是本地
                            if (toServer.Message == "System")
                            {

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
                                    Printer printer = Resources.GetRes().PRINTERS.Where(x => x.PrintType == 0 && x.IsEnable == 1 && x.IsCashDrawer == 1).FirstOrDefault();
                                    if (null != printer)
                                    {
                                        RawPrinterHelper.Instance.SendByteToPrinterForOpenCashbox(printer.PrinterDeviceName);
                                        Result = true;
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
                                    sp.PortName = toServer.Message;
                                    sp.Open();
                                    byte[] byteA = Encoding.UTF8.GetBytes("#$1b#$70#0#$3c#$ff");
                                    sp.Write(byteA, 0, byteA.Length);
                                    System.Threading.Thread.Sleep(100);
                                }
                                Result = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionPro.ExpLog(ex);
                        }




                        //返回它
                        return new ToClientServiceSend() { Result = Result };
                    default:
                        return new ToClientServiceSend() { Result = false };
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Send operation failed.");
                //返回它
                return new ToClientServiceSend() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 打印操作
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServicePrint ServicePrint(ToServerServicePrint toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServicePrint() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServicePrint() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                // 记录日志
                if (Result)
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "Print#" + OperateType.None, toServer.SessionId);

                switch (toServer.ModelType)
                {
                    case ModelType.Import:

                        // 解析出来
                        Import import = toServer.Model.DeserializeObject<Import>();
                        // 打印
                        Print.Instance.PrintImport(null, import, null == import.tb_importdetail ? null : import.tb_importdetail.ToList(), toServer.Lang);

                        return new ToClientServicePrint() { Result = true };

                    case ModelType.Order:

                        // 解析出来
                        Order order = toServer.Model.DeserializeObject<Order>();
                        // 打印
                        Print.Instance.PrintOrderAfterCheckout(null, order, null == order.tb_orderdetail ? null : order.tb_orderdetail.ToList(), toServer.Lang);

                        return new ToClientServicePrint() { Result = true };


                    case ModelType.Takeout:

                        // 解析出来
                        Takeout takeout = toServer.Model.DeserializeObject<Takeout>();
                        // 打印
                        Print.Instance.PrintOrderAfterCheckout(null, takeout, null == takeout.tb_takeoutdetail ? null : takeout.tb_takeoutdetail.ToList(), toServer.Lang);

                        return new ToClientServicePrint() { Result = true };

                    case ModelType.Statistic:

                        // 解析出来
                        SummaryModelPackage package = toServer.Model.DeserializeObject<SummaryModelPackage>();

                        if (toServer.StatisticType == StatisticType.Summary)
                        {
                            // 打印
                            Print.Instance.PrintSummary(null, package, toServer.Lang);

                            return new ToClientServicePrint() { Result = true };
                        }
                        else
                        {
                            return new ToClientServicePrint() { Result = false };
                        }


                    default:
                        return new ToClientServicePrint() { Result = false };
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Print operation failed.");
                //返回它
                return new ToClientServicePrint() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 锁住
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceLock ServiceLock(ToServerServiceLock toServer)
        {
            try
            {

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceLock() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }

                bool result = false;

                try
                {
                    Res.Key.GetKeys().LockKey();

                    //如果成功,则暂时不关闭KEY
                    if (result)
                    {
                        OperateLog.Instance.AddRecord(0, null, "LK#" + OperateType.None, toServer.SessionId);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Key LK operation failed.");
                    return new ToClientServiceLock() { ExceptionType = ServiceExceptionType.KeyFaild };
                }

                return new ToClientServiceLock() { IsSuccessLock = result };
            }
            catch (Exception ex)
            {

                ExceptionPro.ExpLog(ex, null, false, "LK operation failed.");
                //返回它
                return new ToClientServiceLock() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceClose ServiceClose(ToServerServiceClose toServer)
        {
            try
            {
                // 记录日志
                OperateLog.Instance.AddRecord(toServer.AdminId, null, "Logout#" + OperateType.None, toServer.SessionId);

                //移出会话
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() != 0)
                {
                    lock (Resources.GetRes().Services)
                    {
                        Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                        if (null != client)
                        {
                            // 不这样操作, 直接关闭的话会影响正常退出
                            Task.Factory.StartNew(() =>
                            {
                                System.Threading.Thread.Sleep(5000);
                                Resources.GetRes().CloseService(client);
                            });
                        }
                    }
                }


                // 把日志写入到数据库文件中
                Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(1000);
                    // 为了防止某些小规模用户关闭系统就关机, 导致日志没能写入完或者出现数据库极端丢失或损失情况, 每次用户退出后把日志给写入到数据库中. (就算是大规模用户也不会有特别大的问题, 毕竟又不是用户会频繁的退出)
                    OperateLog.Instance.RecordAll();

                    // 放弃, 如果频繁备份大数据库, 可能会影响性能
                    //// 强制备份一下数据库(如果不存在任何PC版设备连接)
                    //if (Resources.GetRes().Services.Count() == 0)
                    //    Backup.Instance.BackupFile(false, true);
                });




                return new ToClientServiceClose() { Result = true };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Close operation failed.");
                //返回它
                return new ToClientServiceClose() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 获取备份
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetbak ServiceGetbak(ToServerServiceGetbak toServer)
        {
            try
            {
                // 以后做这个功能时,  先判断是否是管理员, 然后继续



                return new ToClientServiceGetbak() { Result = true };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get backup operation failed.");
                //返回它
                return new ToClientServiceGetbak() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 设置备份
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSetbak ServiceSetbak(ToServerServiceSetbak toServer)
        {
            try
            {
                // 以后做这个功能时,  先判断是否是管理员, 然后继续



                return new ToClientServiceSetbak() { Result = true };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Set backup operation failed.");
                //返回它
                return new ToClientServiceSetbak() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 获取时间请求码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceRequestTimeCode ServiceTimeRequestCode(ToServerServiceRequestTimeCode toServer)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(toServer.SessionId))
                {
                    //查询SESSION
                    if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                    {
                        return new ToClientServiceRequestTimeCode() { ExceptionType = ServiceExceptionType.SessionExpired };
                    }
                    else
                    {
                        //更新响应时间
                        Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                    }
                }

               
                //查看Key是否插入
                if (!Res.Key.GetKeys().LastCheck())
                {
                    return new ToClientServiceRequestTimeCode() { ExceptionType = ServiceExceptionType.KeyCheckFaild };
                }

                if (string.IsNullOrWhiteSpace(Resources.GetRes().RegTimeRequestCode))
                {
                    ++regOperateLimit;
                    if (regOperateLimit > 10)
                    {
                        return new ToClientServiceRequestTimeCode() { ExceptionType = ServiceExceptionType.ApplicationValidFaild };
                    }


                    Res.Key.GetKeys().Request();
                }


                // 记录日志
                if (string.IsNullOrWhiteSpace(Resources.GetRes().RegTimeRequestCode))
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "TimeRequestCode#" + OperateType.Get, toServer.SessionId);

                return new ToClientServiceRequestTimeCode() { RequestCode = Resources.GetRes().RegTimeRequestCode };

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Request time code operation failed.");
                //返回它
                return new ToClientServiceRequestTimeCode() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }





        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSetContent ServiceSetContent(ToServerServiceSetContent toServer)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(toServer.SessionId))
                {
                    //查询SESSION
                    if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                    {
                        return new ToClientServiceSetContent() { ExceptionType = ServiceExceptionType.SessionExpired };
                    }
                    else
                    {
                        //更新响应时间
                        Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                    }
                }


                //查看Key是否插入
                if (!Res.Key.GetKeys().LastCheck())
                {
                    return new ToClientServiceSetContent() { ExceptionType = ServiceExceptionType.KeyCheckFaild };
                }

                bool Result = false;
                string content = null;

                if (!string.IsNullOrWhiteSpace(toServer.Key) && !string.IsNullOrWhiteSpace(toServer.Value) && !string.IsNullOrWhiteSpace(toServer.Token))
                {
                    if (toServer.Token == "Vod0")
                    {

                        if (System.IO.File.Exists(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "VScroll.dll")))
                        {
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "VScroll.dll"), Encoding.UTF8))
                            {
                                content = sr.ReadToEnd();

                                Result = true;


                            }
                        }
                    }
                    else if (toServer.Token == "Vod10" || toServer.Token == "Vod11")
                    {

                        
                        using (System.IO.StreamWriter sr = new System.IO.StreamWriter(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "VScroll.dll"), false, Encoding.UTF8))
                        {
                            sr.Write(toServer.Value);

                            Result = true;

                        }

                        if (Result && toServer.Token == "Vod11")
                        {
                            // 获取本地的服务
                            System.ServiceProcess.ServiceController ctl = System.ServiceProcess.ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "OybabVodPlayerService");
                            if (ctl != null)
                            {
                                // 如果服务停止了, 则启动它
                                if (ctl.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                                {
                                    ctl.Stop();
                                    ctl.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);
                                    ctl.Start();
                                    ctl.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);
                                }
                            }



                        }

                    }
                    else if (toServer.Token == "Vod00" || toServer.Token == "Vod01")
                    {
                        if (System.IO.File.Exists(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "VScroll.dll")))
                        {
                            System.IO.File.Delete(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "VScroll.dll"));

                            if (toServer.Token == "Vod01")
                            {
                                // 获取本地的服务
                                System.ServiceProcess.ServiceController ctl = System.ServiceProcess.ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "OybabVodPlayerService");
                                if (ctl != null)
                                {
                                    // 如果服务停止了, 则启动它
                                    if (ctl.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                                    {
                                        ctl.Stop();
                                        ctl.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);
                                        ctl.Start();
                                        ctl.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running);
                                    }
                                }
                            }
                        }

                        Result = true;

                       
                    }
                }
                


                // 记录日志
                OperateLog.Instance.AddRecord(toServer.AdminId, null, "SetContent#" + OperateType.Save, toServer.SessionId);

                return new ToClientServiceSetContent() { Result = Result, Content  = content };

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Set content operation failed.");
                //返回它
                return new ToClientServiceSetContent() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        private int regOperateLimit = 0;
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceRegTime ServiceRegTime(ToServerServiceRegTime toServer)
        {
            try
            {
                // 2秒等待免得不停发送请求
                System.Threading.Thread.Sleep(2000);

                if (!string.IsNullOrWhiteSpace(toServer.SessionId))
                {
                    //查询SESSION
                    if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                    {
                        return new ToClientServiceRegTime() { ExceptionType = ServiceExceptionType.SessionExpired };
                    }
                    else
                    {
                        //更新响应时间
                        Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                    }
                }

                //查看Key是否插入
                if (!Res.Key.GetKeys().LastCheck())
                {
                    return new ToClientServiceRegTime() { ExceptionType = ServiceExceptionType.KeyCheckFaild };
                }

                ++regOperateLimit;
                if (regOperateLimit > 10)
                {
                    return new ToClientServiceRegTime() { ExceptionType = ServiceExceptionType.ApplicationValidFaild };
                }

                bool result = false;
                if (Res.Key.GetKeys().SetRegCode(toServer.RegCode))
                {
                    result = true;

                    Res.Key.GetKeys().Clear(true);
                    if (DBOperate.GetDBOperate().IsDataReady)
                    {
                        Res.Key.GetKeys().Check(false, true);
                    }
                    else
                    {
                        Res.Key.GetKeys().Check(false);
                    }
                }
                else
                    result = false;

                // 记录日志
                if (result)
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "RegTime#" + OperateType.Edit, toServer.SessionId);
                else
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "RegTime#" + OperateType.None, toServer.SessionId);

                return new ToClientServiceRegTime() { Result = result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Reg time operation failed.");
                //返回它
                return new ToClientServiceRegTime() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }









        /// <summary>
        /// 获取数量请求码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceRequestCountCode ServiceCountRequestCode(ToServerServiceRequestCountCode toServer)
        {
            try
            {

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceRequestCountCode() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }
                

                //查看Key是否插入
                if (!Res.Key.GetKeys().LastCheck())
                {
                    return new ToClientServiceRequestCountCode() { ExceptionType = ServiceExceptionType.KeyCheckFaild };
                }

                if (string.IsNullOrWhiteSpace(Resources.GetRes().RegCountRequestCode))
                {
                    Res.Key.GetKeys().GetCountRequest();
                }
                else
                {
                    ++regOperateLimit;
                    if (regOperateLimit > 10)
                    {
                        return new ToClientServiceRequestCountCode() { ExceptionType = ServiceExceptionType.ApplicationValidFaild };
                    }
                }

                // 记录日志
                if (string.IsNullOrWhiteSpace(Resources.GetRes().RegCountRequestCode))
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "CountRequestCode#" + OperateType.Get, toServer.SessionId);

                return new ToClientServiceRequestCountCode() { RequestCode = Resources.GetRes().RegCountRequestCode };

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Request count code operation failed.");
                //返回它
                return new ToClientServiceRequestCountCode() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 设置数量
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceRegCount ServiceRegCount(ToServerServiceRegCount toServer)
        {
            try
            {
                // 2秒等待免得不停发送请求
                System.Threading.Thread.Sleep(2000);

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceRegCount() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }
                

                //查看Key是否插入
                if (!Res.Key.GetKeys().LastCheck())
                {
                    return new ToClientServiceRegCount() { ExceptionType = ServiceExceptionType.KeyCheckFaild };
                }


                ++regOperateLimit;
                if (regOperateLimit > 10)
                {
                    return new ToClientServiceRegCount() { ExceptionType = ServiceExceptionType.ApplicationValidFaild };
                }

                bool result = false;
                if (Res.Key.GetKeys().RegCount(toServer.RegCode))
                {
                    result = true;

                    Res.Key.GetKeys().Clear(true);
                    if (DBOperate.GetDBOperate().IsDataReady)
                    {
                        Res.Key.GetKeys().Check(false, true);
                    }
                    else
                    {
                        Res.Key.GetKeys().Check(false);
                    }
                }
                else
                    result = false;

                // 记录日志
                if (result)
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "RegCount#" + OperateType.Edit, toServer.SessionId);
                else
                    OperateLog.Instance.AddRecord(toServer.AdminId, null, "RegCount#" + OperateType.None, toServer.SessionId);

                 return new ToClientServiceRegCount() { Result = result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Reg count operation failed.");
                //返回它
                return new ToClientServiceRegCount() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetModel ServiceGetModel(ToServerServiceGetModel toServer)
        {
            try
            {

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetModel() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }



                return new ToClientServiceGetModel() { Result = false };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get model operation failed.");
                //返回它
                return new ToClientServiceGetModel() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        /// <summary>
        /// 刷新会话
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSession ServiceSession(ToServerServiceSession toServer, string SignalRSessionId, string ipAddress)
        {
            try
            {
                //检查IP
                string ip = GetIp(ipAddress);

                // 获取Client
                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                //查询SESSION
                if (null == client)
                {
                    if (Resources.GetRes().ServicesUsedByOther.Where(x => x.SessionId == toServer.SessionId).Count() > 0)
                    {
                        lock (Resources.GetRes().Services)
                        {
                            Client clientOld = Resources.GetRes().ServicesUsedByOther.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                            Resources.GetRes().ServicesUsedByOther.Remove(clientOld);
                        }
                        return new ToClientServiceSession() { ExceptionType = ServiceExceptionType.AdminUsing };
                    }
                    else
                    {
                        return new ToClientServiceSession() { ExceptionType = ServiceExceptionType.Relogin };
                    }
                }

              


                //既然找到同样的SESSION,也对比一下IP是否一样,不一样的话....
                if (client.IP != ip)
                {
                    if (Resources.GetRes().DEVICES.Where(x => x.DeviceId == client.DeviceId).FirstOrDefault().IpAddress != "*")
                    {
                        client.IsConnected = false;
                        return new ToClientServiceSession() { ExceptionType = ServiceExceptionType.SessionInvalid };
                    }

                    client.IP = ip;
                }

                string cache = null;

                IContextChannel oldChannel = null;


                lock (client.Lock)
                {

                    // 如果Callback失效, 则修正
                    if (!client.IsConnected || toServer.IsNew || client.ClientChannel != OperationContext.Current?.Channel)
                    {
                        oldChannel = client.ClientChannel;

                        // 如果有老的通讯, 终止掉它
                        if (null != oldChannel)
                            AbortOldChannel(oldChannel);
                        

                        // 想了那么久发现, 拉和推两个模式相互结合才是最完美的  (补充: 只针对WCF才是, SignalR只能拉, 因为它没有真实的Callback, 至少目前为止是这样的)
                        try
                        {
                            client.ClientChannel = OperationContext.Current?.Channel;
                            client.ClientCallback = OperationContext.Current?.GetCallbackChannel<IServiceCallback>();
                            client.SignalRClientSessionId = SignalRSessionId;


                            //如果有没连接的
                            if (client.NotificationCaches.Count > 0)
                            {
                                cache = JsonConvert.SerializeObject(client.NotificationCaches);
                                if (!toServer.IsSignalRMode)
                                    client.ClientCallback.ServiceModelUpdateNotification(new ToClientServiceModelUpdateNotification() { Model = cache, ModelType = ModelType.CallBack, OperateType = OperateType.Get });
                                else
                                    SignalRCallback.Instance.ServiceModelUpdateNotification(client.SignalRClientSessionId, new ToClientServiceModelUpdateNotification() { Model = cache, ModelType = ModelType.CallBack, OperateType = OperateType.Get });
                                client.NotificationCaches.Clear();
                            }


                            // SignalR模式下强制刷新一下所以列表, 因为它是不可靠的(也就是说可能会丢失)
                            if (toServer.IsNew && toServer.IsSignalRMode)
                            {
                                List<OrderNotificationModel> roomModes = new List<OrderNotificationModel>();
                                foreach (var item in Resources.GetRes().ROOMS_Model)
                                {
                                    roomModes.Add(new OrderNotificationModel() { RoomId = item.RoomId, Order = item.PayOrder, OrderSessionId = item.OrderSession });
                                }

                                ToClientServiceOrderUpdateNotification orderUpdate = new ToClientServiceOrderUpdateNotification() { OrderNotification = JsonConvert.SerializeObject(roomModes) };

                                SignalRCallback.Instance.ServiceOrderUpdateNotification(client.SignalRClientSessionId, orderUpdate);
                            }



                            client.IsConnected = true;

                        }
                        catch
                        {
                            return new ToClientServiceSession() { Result = false };
                        }
                    }
                  
                }

     


                // 暂时去掉这些. 免得同步重复(我指拉和推)
                string rooms = null;
                List<RoomModel> models = new List<RoomModel>();
                //// 说明只获取一个即可
                if (null != toServer.RoomsId && toServer.RoomsId.Length > 0)
                {
                    foreach (var item in Resources.GetRes().ROOMS_Model)
                    {
                        if (toServer.RoomsId.Contains(item.RoomId))
                        {
                            RoomModel model = new RoomModel();
                            model.Order = item.Order;
                            model.HideType = item.HideType;
                            model.RoomId = item.RoomId;
                            model.RoomNo = item.RoomNo;
                            model.PayOrder = item.PayOrder;
                            model.OrderSession = item.OrderSession;
                            models.Add(model);
                        }
                    }

                    rooms = JsonConvert.SerializeObject(models, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });


                }
                //更新会话,并加密数据并返回
                Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;

                //返回它
                return new ToClientServiceSession() { Result = true, RoomsModel = rooms};
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Session operation failed.");
                //返回它
                return new ToClientServiceSession() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



       

        /// <summary>
        /// 终止老通讯
        /// </summary>
        /// <param name="oldChannel"></param>
        internal void AbortOldChannel(IContextChannel oldChannel)
        {
            try
            {
                oldChannel.Abort();
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }

        #endregion Common


        #region Room

        /// <summary>
        /// 新增包厢
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddRoom ServiceAddRoom(ToServerServiceAddRoom toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddRoom() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddRoom() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Room model = toServer.Room.DeserializeObject<Room>();
                string RoomOrderSession = null;


                // 不能超出雅座数量限制
                if (Resources.GetRes().ROOMS.Where(x => x.HideType == 0 || x.HideType == 2).Count() >= Resources.GetRes().ROOM_COUNT && (model.HideType == 0 || model.HideType == 2))
                {
                    return new ToClientServiceAddRoom() { ExceptionType = ServiceExceptionType.RoomCountOutOfLimit };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.ClearReferences();

                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;
                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddRoom() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {
                    
                    Id = model.RoomId;
                    Resources.GetRes().ROOMS.Add(model);

                    RoomModel roomModel = null;
                    if (model.HideType == 0)
                    {
                        roomModel = new RoomModel() { HideType = model.HideType, Order = model.Order, RoomId = model.RoomId, RoomNo = model.RoomNo, OrderSession = RoomOrderSession = Guid.NewGuid().ToString() };

                        Resources.GetRes().ROOMS_Model.Add(roomModel);
                    }

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.RoomId, null, "Room#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Room, OperateType = OperateType.Add, ModelRef = JsonConvert.SerializeObject(roomModel) }, toServer.SessionId);
                    
                }

                //返回它
                return new ToClientServiceAddRoom() { Room = JsonConvert.SerializeObject(model), Result = Result, RoomStateSession = RoomOrderSession };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add room operation failed.");
                //返回它
                return new ToClientServiceAddRoom() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改包厢
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditRoom ServiceEditRoom(ToServerServiceEditRoom toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditRoom() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditRoom() { ExceptionType = ServiceExceptionType.DataNotReady };
                }


                //解析出来
                Room model = toServer.Room.DeserializeObject<Room>();
                string RoomOrderSession = null;

                // 有订单关联, 不能编辑
                if (Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId && x.PayOrder != null).Count() > 0)
                {
                    return new ToClientServiceEditRoom() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                }
                // Room 更新日期不一致, 不能更新
                Room serverModel = Resources.GetRes().ROOMS.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditRoom() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                // 不能超出雅座数量限制
                if (Resources.GetRes().ROOMS.Where(x => x.HideType == 0 || x.HideType == 2).Count() >= Resources.GetRes().ROOM_COUNT && Resources.GetRes().ROOMS.Where(x => x.RoomId == model.RoomId).FirstOrDefault().HideType == 1 && (model.HideType == 0 || model.HideType == 2))
                {
                    return new ToClientServiceEditRoom() { ExceptionType = ServiceExceptionType.RoomCountOutOfLimit };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.ClearReferences();

                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditRoom() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().ROOMS.Remove(Resources.GetRes().ROOMS.Where(x => x.RoomId == model.RoomId).FirstOrDefault());
                    Resources.GetRes().ROOMS.Add(model);

                    RoomModel roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                    if (null != roomModel)
                        Resources.GetRes().ROOMS_Model.Remove(roomModel);

                    if (model.HideType == 0)
                    {
                        roomModel = new RoomModel() { HideType = model.HideType, Order = model.Order, RoomId = model.RoomId, RoomNo = model.RoomNo, OrderSession = RoomOrderSession = Guid.NewGuid().ToString() };
                        Resources.GetRes().ROOMS_Model.Add(roomModel);
                    }
                    else if (model.HideType == 1)
                    {
                        roomModel = new RoomModel() { HideType = model.HideType, Order = model.Order, RoomId = model.RoomId, RoomNo = model.RoomNo, OrderSession = null };
                    }

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.RoomId, null, "Room#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Room, OperateType = OperateType.Edit, ModelRef = JsonConvert.SerializeObject(roomModel) }, toServer.SessionId);

                }

                //返回它
                return new ToClientServiceEditRoom() {  Result = Result, Room = JsonConvert.SerializeObject(model), RoomStateSession = RoomOrderSession };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit room operation failed.");
                //返回它
                return new ToClientServiceEditRoom() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除包厢
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelRoom ServiceDelRoom(ToServerServiceDelRoom toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelRoom() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelRoom() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Room model = toServer.Room.DeserializeObject<Room>();

                // 正在开启的包厢不能删除
                if (Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId && x.PayOrder != null).Count() > 0)
                {
                    return new ToClientServiceDelRoom() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                }
                // Room 更新日期不一致, 不能更新
                Room serverModel = Resources.GetRes().ROOMS.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelRoom() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        //订单中有此包厢,说明有关系无法删除
                        if (ctx.Orders.Any(x => x.RoomId == model.RoomId) || ctx.Devices.Any(x => x.RoomId == model.RoomId))
                        {
                            return new ToClientServiceDelRoom() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                        }

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;
                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelRoom() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().ROOMS.Remove(Resources.GetRes().ROOMS.Where(x => x.RoomId == model.RoomId).FirstOrDefault());
                    RoomModel roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                    if (null != roomModel)
                        Resources.GetRes().ROOMS_Model.Remove(roomModel);


                    // 记录日志
                    OperateLog.Instance.AddRecord(model.RoomId, null, "Room#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Room, OperateType = OperateType.Delete }, toServer.SessionId);

                    
                }

                //返回它
                return new ToClientServiceDelRoom() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete room operation failed.");
                //返回它
                return new ToClientServiceDelRoom() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Room


        #region Product

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetAllProduct ServiceGetAllProduct(ToServerServiceGetAllProduct toServer)
        {
            try
            {
                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetAllProduct() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetAllProduct() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                if (toServer.ProductId != 0)
                {
                    // 返回它
                    return new ToClientServiceGetAllProduct() { Result = true, Products = JsonConvert.SerializeObject(Resources.GetRes().PRODUCTS.Where(x=>x.ProductId == toServer.ProductId).FirstOrDefault()) };
                }
                else
                {
                    // 返回它
                    return new ToClientServiceGetAllProduct() { Result = true, Products = JsonConvert.SerializeObject(Resources.GetRes().PRODUCTS) };
                }

                
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get All product operation failed.");
                //返回它
                return new ToClientServiceGetAllProduct() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 新增产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddProduct ServiceAddProduct(ToServerServiceAddProduct toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddProduct() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddProduct() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Product model = toServer.Product.DeserializeObject<Product>();


                List<Ppr> pprsList = toServer.Pprs.DeserializeObject<List<Ppr>>();
                try
                {

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


                            model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                            model.ClearReferences();

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Added;



                            foreach (var item in pprsList)
                            {
                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                            }
                            

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddProduct() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {
                    Id = model.ProductId;
                    Resources.GetRes().PRODUCTS.Add(model);

                    foreach (var item in pprsList)
                    {
                        Resources.GetRes().PPRS.Add(item);
                    }


                    // 记录日志
                    OperateLog.Instance.AddRecord(model.ProductId, null, "Product#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Product, OperateType = OperateType.Add }, toServer.SessionId);
                    if (pprsList.Count > 0)
                    {
                        NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(pprsList), ModelRef = JsonConvert.SerializeObject(model), ModelType = ModelType.Ppr, OperateType = OperateType.Add }, toServer.SessionId);
                    }
                }

                //返回它
                return new ToClientServiceAddProduct() { Product = JsonConvert.SerializeObject(model), Pprs = JsonConvert.SerializeObject(pprsList), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add product operation failed.");
                //返回它
                return new ToClientServiceAddProduct() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditProduct ServiceEditProduct(ToServerServiceEditProduct toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditProduct() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditProduct() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Product model = toServer.Product.DeserializeObject<Product>();

                // Product 更新日期不一致, 不能更新
                Product serverModel = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == model.ProductId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditProduct() { ExceptionType = ServiceExceptionType.UpdateModel };
                }


                List<Ppr> pprsList = toServer.Pprs.DeserializeObject<List<Ppr>>();
                List<Ppr> pprsFinal = new List<Ppr>();

                List<Ppr> newPpr = new List<Ppr>();
                List<Ppr> deletePpr = new List<Ppr>();
                List<Ppr> oldPpr = Resources.GetRes().PPRS.Where(x => x.ProductId == model.ProductId).ToList();
                List<long> keepPpr = new List<long>();

                try
                {

                    keepPpr = oldPpr.Select(x => x.PprId).Where(x => pprsList.Select(y => y.PprId).Contains(x)).ToList<long>();

                    //先删除关联,不要删除无改动的
                    foreach (var item in oldPpr.Where(x => !pprsList.Select(y => y.PprId).Contains(x.PprId)))
                    {
                        deletePpr.Add(item);
                    }


                    //增加新关联
                    foreach (var item in pprsList)
                    {
                        //已删除的别重复添加
                        if (!deletePpr.Select(x => x.PprId).Contains(item.PprId) && !keepPpr.Contains(item.PprId))
                        {
                            Ppr ppr = new Ppr();
                            ppr.ProductId = model.ProductId;
                            ppr.PrinterId = item.PrinterId;
                            newPpr.Add(ppr);
                        }
                    }


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

                            model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            model.ClearReferences();

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;

                            foreach (var item in deletePpr)
                            {
                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                            }


                            foreach (var item in newPpr)
                            {
                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditProduct() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().PRODUCTS.Remove(Resources.GetRes().PRODUCTS.Where(x => x.ProductId == model.ProductId).FirstOrDefault());
                    Resources.GetRes().PRODUCTS.Add(model);


                    // 删除老关联
                    foreach (var item in deletePpr)
                    {
                        Resources.GetRes().PPRS.Remove(item);
                    }

                    // 添加新的打印和产品关系
                    foreach (var item in newPpr)
                    {
                        Resources.GetRes().PPRS.Add(item);
                    }

                    pprsFinal = Resources.GetRes().PPRS.Where(x => x.ProductId == model.ProductId).ToList();


                    // 记录日志
                    OperateLog.Instance.AddRecord(model.ProductId, null, "Product#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Product, OperateType = OperateType.Edit }, toServer.SessionId);


                    if (newPpr.Count > 0 || deletePpr.Count > 0)
                        NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(pprsFinal), ModelRef = JsonConvert.SerializeObject(model), ModelType = ModelType.Ppr, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditProduct() { Result = Result, Product = JsonConvert.SerializeObject(model), Pprs = JsonConvert.SerializeObject(pprsFinal) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit product operation failed.");
                //返回它
                return new ToClientServiceEditProduct() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelProduct ServiceDelProduct(ToServerServiceDelProduct toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelProduct() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelProduct() { ExceptionType = ServiceExceptionType.DataNotReady };
                }


                //解析出来
                Product model = toServer.Product.DeserializeObject<Product>();

                // Product 更新日期不一致, 不能更新
                Product serverModel = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == model.ProductId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelProduct() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                List<Ppr> pprsList = toServer.Pprs.DeserializeObject<List<Ppr>>();

                if (null != pprsList)
                    pprsList = Resources.GetRes().PPRS.Where(x => x.ProductId == model.ProductId).ToList();// model.tb_Ppr.ToList();

                try
                {
                   using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;
                        ((IObjectContextAdapter)ctx).ObjectContext.CommandTimeout = Resources.GetRes().TIME_OUT_SHORT;//无用


                        if (ctx.OrderDetails.Any(x => x.ProductId == model.ProductId) || ctx.ImportDetails.Any(x => x.ProductId == model.ProductId) || ctx.TakeoutDetails.Any(x => x.ProductId == model.ProductId))
                        {
                            return new ToClientServiceDelProduct() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                        }


                        TransactionOptions option = new TransactionOptions();
                        option.Timeout = TimeSpan.FromSeconds(Resources.GetRes().TIME_OUT_SHORT);//无用
                        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option, EnterpriseServicesInteropOption.Automatic))
                        {

                            ctx.Configuration.AutoDetectChangesEnabled = false;
                            ctx.Configuration.ValidateOnSaveEnabled = false;



                            model.ClearReferences();

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;



                            foreach (var item in pprsList)
                            {
                                item.ClearReferences();
                                ctx.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;

                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelProduct() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().PRODUCTS.Remove(Resources.GetRes().PRODUCTS.Where(x => x.ProductId == model.ProductId).FirstOrDefault());

                    // 把相关关系也去掉
                    List<Ppr> pprs = Resources.GetRes().PPRS.Where(x => x.ProductId == model.ProductId).ToList();
                    foreach (var item in pprs)
                    {
                        Resources.GetRes().PPRS.Remove(item);
                    }


                    // 记录日志
                    OperateLog.Instance.AddRecord(model.ProductId, null, "Product#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Product, OperateType = OperateType.Delete }, toServer.SessionId);
                    if (pprsList.Count > 0)
                    {
                        NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(pprsList), ModelRef = JsonConvert.SerializeObject(model), ModelType = ModelType.Ppr, OperateType = OperateType.Delete }, toServer.SessionId);
                    }
                }

                //返回它
                return new ToClientServiceDelProduct() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete product operation failed.");
                //返回它
                return new ToClientServiceDelProduct() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Product


        #region ProductType

        /// <summary>
        /// 新增产品类型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddProductType ServiceAddProductType(ToServerServiceAddProductType toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddProductType() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddProductType() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                ProductType model = toServer.ProductType.DeserializeObject<ProductType>();

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.ClearReferences();

                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;
                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddProductType() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.ProductTypeId;
                    Resources.GetRes().PRODUCT_TYPES.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.ProductTypeId, null, "ProductType#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.ProductType, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddProductType() { ProductType = JsonConvert.SerializeObject(model), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add product type operation failed.");
                //返回它
                return new ToClientServiceAddProductType() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改产品类型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditProductType ServiceEditProductType(ToServerServiceEditProductType toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditProductType() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditProductType() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                ProductType model = toServer.ProductType.DeserializeObject<ProductType>();

                // ProductType 更新日期不一致, 不能更新
                ProductType serverModel = Resources.GetRes().PRODUCT_TYPES.Where(x => x.ProductTypeId == model.ProductTypeId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditProductType() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.ClearReferences();

                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditProductType() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().PRODUCT_TYPES.Remove(Resources.GetRes().PRODUCT_TYPES.Where(x => x.ProductTypeId == model.ProductTypeId).FirstOrDefault());
                    Resources.GetRes().PRODUCT_TYPES.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.ProductTypeId, null, "ProductType#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.ProductType, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditProductType() { Result = Result, ProductType = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit product type operation failed.");
                //返回它
                return new ToClientServiceEditProductType() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除产品类型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelProductType ServiceDelProductType(ToServerServiceDelProductType toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelProductType() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelProductType() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                ProductType model = toServer.ProductType.DeserializeObject<ProductType>();

                // 不能删除产品中用的产品类型
                if (Resources.GetRes().PRODUCTS.Any(x => x.ProductTypeId == model.ProductTypeId))
                {
                    return new ToClientServiceDelProductType() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                }

                // ProductType 更新日期不一致, 不能更新
                ProductType serverModel = Resources.GetRes().PRODUCT_TYPES.Where(x => x.ProductTypeId == model.ProductTypeId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelProductType() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                
                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;
                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelProductType() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().PRODUCT_TYPES.Remove(Resources.GetRes().PRODUCT_TYPES.Where(x => x.ProductTypeId == model.ProductTypeId).FirstOrDefault());

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.ProductTypeId, null, "ProductType#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.ProductType, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelProductType() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete product type operation failed.");
                //返回它
                return new ToClientServiceDelProductType() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion ProductType


        #region Order

        /// <summary>
        /// 新建订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceNewOrder ServiceAddOrder(ToServerServiceNewOrder toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceNewOrder() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceNewOrder() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Order model = toServer.Order.DeserializeObject<Order>();
                List<OrderDetail> details = toServer.OrderDetails.DeserializeObject<List<OrderDetail>>();
                List<OrderPay> pays = toServer.OrderPays.DeserializeObject<List<OrderPay>>();

                RoomModel roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                //查找包厢能否使用(包厢状态不同说明模型有改动, 包厢已有订单,说明还未结账或模型改动)
                if (null != roomModel && roomModel.OrderSession != toServer.RoomStateSession)
                {
                    return new ToClientServiceNewOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                }
                else
                {
                    // 防止同一5秒内同时操作
                    lock (roomModel)
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        if (roomModel.LastOperateSessionId != toServer.SessionId && (dateTimeNow - roomModel.LastOperateTime).TotalSeconds <= 5)
                        {
                            return new ToClientServiceNewOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else if (roomModel.LastOperateSessionId == toServer.SessionId && (dateTimeNow - roomModel.LastOperateTime).TotalSeconds <= 3)
                        {
                            return new ToClientServiceNewOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else
                        {
                            roomModel.LastOperateTime = dateTimeNow;
                            roomModel.LastOperateSessionId = toServer.SessionId;
                        }
                    }
                }

                // 要开启的订单不能超过当前设置的包厢数
                if (Resources.GetRes().ROOMS_Model.Where(x=> null != x.PayOrder).Count() >= Resources.GetRes().ROOM_COUNT)
                {
                    return new ToClientServiceNewOrder() { ExceptionType = ServiceExceptionType.RoomCountOutOfLimit };
                }

                // 先检查产品数量是否少于它要购买的数量
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();
                
                foreach (var item in details)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1)
                    {
                        if (product.BalanceCount < item.Count)
                        {
                            // 如果有父级
                            if (null != product.ProductParentId)
                            {
                                Product productParent = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                if (null != productParent && productParent.IsBindCount == 1)
                                {
                                    double ParentRemove = 0;
                                    double ProductAdd = 0;

                                    lock (product)
                                    {
                                        double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); 
                                        ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3);
                                        ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                        ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 
                                    }

                                    // 从父级中去掉
                                    lock (productParent)
                                        productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);

                                    if (productsChange.ContainsKey(productParent.ProductId))
                                        productsChange[productParent.ProductId].CountChange = Math.Round(productsChange[productParent.ProductId].CountChange - ParentRemove, 3);
                                    else
                                        productsChange.Add(productParent.ProductId, new ProductWithCount() { Product = productParent, CountChange = -ParentRemove });


                                    // 给产品增加零的
                                    lock (product)
                                        product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);

                                    if (productsChange.ContainsKey(product.ProductId))
                                        productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + ProductAdd, 3);
                                    else
                                        productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = ProductAdd });

                                }
                            }



                        }

                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - item.Count, 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = -item.Count });

                    }
                }

                

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                long UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                try
                {
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


                            model.AddTime = UpdateTime;


                            if (Resources.GetRes().ROOMS.Where(x=>x.RoomId == roomModel.RoomId).FirstOrDefault().IsPayByTime != 0)
                                model.RoomPriceCalcTime = model.EndTime.Value;
                            else
                                model.RoomPriceCalcTime = model.AddTime;

                            
                            model.AdminId = client.AdminId;
                            model.DeviceId = client.DeviceId;
                            model.Mode = client.Mode;


                            model.ClearReferences();

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Added;


                            foreach (var item in details)
                            {
                                item.ClearReferences();

                                item.AdminId = client.AdminId;
                                item.DeviceId = client.DeviceId;
                                item.AddTime = UpdateTime;
                                item.Mode = client.Mode;

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                            }

                            foreach (var item in productsChange)
                            {
                                item.Value.Product.ClearReferences();

                                item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                item.Value.Product.UpdateTime = UpdateTime;

                                ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                            }



                            // 新增支付
                            foreach (var item in pays)
                            {
                                // 如果会员信息存在, 则更新会员信息
                                if (item.State == 0 && null != item.MemberId)
                                {
                                    // 更新会员余额
                                    Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                                    item.tb_member = memberPay;
                                    memberPay.BalancePrice = Math.Round(memberPay.BalancePrice - item.Price, 2);
                                    item.BalancePrice = memberPay.BalancePrice;
                                    memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice + item.RemovePrice, 2);
                                    memberPay.SpendPrice = Math.Round(memberPay.SpendPrice + item.Price, 2);
                                    memberPay.UpdateTime = UpdateTime;

                                    memberPay.ClearReferences();


                                    ctx.Entry(memberPay).State = System.Data.Entity.EntityState.Modified;

                                }

                                if (item.State == 0 && null != item.BalanceId)
                                {
                                    // 更新里面的余额信息
                                    lock (Resources.GetRes().BALANCES)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                        if (null != item.tb_balance)
                                        {
                                            item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                            item.BalancePrice = item.tb_balance.BalancePrice;
                                            //item.tb_balance.UpdateTime = UpdateTime;

                                            item.tb_balance.ClearReferences();
                                            ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                        }
                                    }
                                }

                                item.ClearReferences();

                                item.AdminId = client.AdminId;
                                item.DeviceId = client.DeviceId;
                                item.AddTime = UpdateTime;
                                item.Mode = client.Mode;
                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        lock (item.Value)
                            item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                    }


                    foreach (var item in pays)
                    {
                        if (item.State == 0 && null != item.MemberId)
                        {
                            // 更新会员余额
                            Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                            memberPay.BalancePrice = Math.Round(memberPay.BalancePrice + item.Price, 2);

                            memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice - item.RemovePrice, 2);
                            memberPay.SpendPrice = Math.Round(memberPay.SpendPrice - item.Price, 2);
                        }

                        // 更新里面的余额信息
                        if (item.State == 0 && null != item.BalanceId)
                        {
                            lock (Resources.GetRes().BALANCES)
                            {
                                if (null != item.tb_balance)
                                {
                                    item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                    item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                }
                            }
                        }
                    }
                    

                    return new ToClientServiceNewOrder() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Result = true;
                    Id = model.OrderId;

                    roomModel.OrderSession = session = Guid.NewGuid().ToString();

                    Order oldOrder = roomModel.PayOrder;
                    roomModel.PayOrder = model;

                    foreach (var item in details)
                    {
                        roomModel.PayOrder.tb_orderdetail.Add(item);
                    }

                    foreach (var item in pays)
                    {
                        if (null != item.tb_member)
                        {
                            Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.tb_member.MemberId).FirstOrDefault());
                            Resources.GetRes().MEMBERS.Add(item.tb_member);
                        }

                        roomModel.PayOrder.tb_orderpay.Add(item);
                    }

                    // 记录日志
                    if (Result && null != pays && pays.Count > 0)
                        OperateLog.Instance.AddRecord(model.OrderId, null, "Order#" + OperateType.Add, 1, toServer.SessionId, toServer);
                    else
                        OperateLog.Instance.AddRecord(model.OrderId, null, "Order#" + OperateType.Add, toServer.SessionId, toServer);

                    // 成功了给客户端发送更新订单通知
                    NotificateToServiceUpdateOrder(JsonConvert.SerializeObject(new List<OrderNotificationModel>() { new OrderNotificationModel() { RoomId = roomModel.RoomId, Order = roomModel.PayOrder, OrderSessionId = roomModel.OrderSession, ProductsChange = productsChange } }), toServer.SessionId);

                    // 打印
                    Print.Instance.PrintOrderAfterBuy(client, model, details, oldOrder);
                }



                //返回它
                return new ToClientServiceNewOrder() { Order = JsonConvert.SerializeObject(model), OrderDetails = JsonConvert.SerializeObject(details), OrderPays = JsonConvert.SerializeObject(pays), Result = Result, RoomSessionId = session, UpdateTime = UpdateTime };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add order operation failed.");
                //返回它
                return new ToClientServiceNewOrder() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 编辑订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditOrder ServiceEditOrder(ToServerServiceEditOrder toServer)
        {
            try
            {
                bool Result = false;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Order model = toServer.Order.DeserializeObject<Order>();
                ICollection<OrderPay> OrderPays = new List<OrderPay>();
                ICollection<OrderPay> DelOrderPays = new List<OrderPay>();
                ICollection<OrderDetail> OrderDetails = null;
                RoomModel roomModel = null;

                Member member = model.tb_member;
                
                if (!toServer.Rechecked)
                {
                    roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                    //查找包厢能否使用(包厢状态不同说明模型有改动, 包厢已有订单,说明还未结账或模型改动)
                    if (null != roomModel && roomModel.OrderSession != toServer.RoomStateSession)///if (Resources.GetRes().ROOMS_Model.Any(x => (x.RoomId == model.RoomId && x.OrderSession != toServer.RoomStateSession)))//|| (x.RoomId == model.RoomId && null != x.Order))
                    {
                        return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                    }
                    else
                    {
                        // 防止同一5秒内同时操作
                        lock (roomModel)
                        {
                            DateTime dateTimeNow = DateTime.Now;
                            if (roomModel.LastOperateSessionId != toServer.SessionId && (dateTimeNow - roomModel.LastOperateTime).TotalSeconds <= 5)
                            {
                                return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                            }
                            else if (roomModel.LastOperateSessionId == toServer.SessionId && (dateTimeNow - roomModel.LastOperateTime).TotalSeconds <= 3)
                            {
                                return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                            }
                            else
                            {
                                roomModel.LastOperateTime = dateTimeNow;
                                roomModel.LastOperateSessionId = toServer.SessionId;
                            }
                        }
                    }
                }
                


                // 判断会员UpdateTime,以免覆盖
                if (null != member && Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault().UpdateTime != member.UpdateTime)
                {
                    return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.UpdateRefModel };
                }





               

                if (!toServer.Rechecked && model.State == 1)
                {
                    OrderPays = toServer.OrderPays.DeserializeObject<List<OrderPay>>();
                    OrderDetails = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId && null != x.PayOrder && null != x.PayOrder.tb_orderdetail).Select(x => x.PayOrder.tb_orderdetail).FirstOrDefault();
                }
                else if (!toServer.Rechecked && model.State == 2)
                {
                    OrderPays = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId && null != x.PayOrder && null != x.PayOrder.tb_orderpay).Select(x => x.PayOrder.tb_orderpay).FirstOrDefault();
                    OrderDetails = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId && null != x.PayOrder && null != x.PayOrder.tb_orderdetail).Select(x => x.PayOrder.tb_orderdetail).FirstOrDefault();
                }
                else
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;
                        ((IObjectContextAdapter)ctx).ObjectContext.CommandTimeout = Resources.GetRes().TIME_OUT_SHORT;//无用
                        ctx.Configuration.AutoDetectChangesEnabled = false;
                        ctx.Configuration.ValidateOnSaveEnabled = false;

                        Order ctxOrdersWhereFirstOrDefault = ctx.Orders.Where(x => x.OrderId == model.OrderId).FirstOrDefault();
                        //查找该外卖信息是否已修改过
                        if (ctxOrdersWhereFirstOrDefault.UpdateTime != model.UpdateTime)
                        {
                            return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                        }



                        OrderDetails = ctx.OrderDetails.Where(x => x.OrderId == model.OrderId).ToList();
                        OrderPays = ctx.OrderPays.Where(x => x.OrderId == model.OrderId).ToList();
                    }
                }


                // 结账没关系. 反正结帐前是确认好的, 也已经购买好的. 但是如果取消, 就得还原产品
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();

                if (null != OrderDetails && model.State == 2)
                {
                    foreach (var item in OrderDetails)
                    {
                        Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                        if (product.IsBindCount == 1 && item.State != 1 && item.State != 3)
                        {
                            lock (product)
                                product.BalanceCount = Math.Round(product.BalanceCount +item.Count, 3);

                            if (productsChange.ContainsKey(product.ProductId))
                                productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + item.Count, 3);
                            else
                                productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = item.Count });

                        }
                    }
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                long UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                
                try
                {
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



                            if (model.State == 2)
                            {

                                if (toServer.Rechecked)
                                {
                                    model.ReCheckedCount = model.ReCheckedCount + 1;
                                }
                                else
                                {
                                    model.FinishTime = UpdateTime;
                                    model.FinishAdminId = client.AdminId;
                                    model.FinishDeviceId = client.DeviceId;
                                }

                                model.UpdateTime = UpdateTime;

                                model.ClearReferences();

                                ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            }
                            else if (!toServer.Rechecked && model.State == 1)
                            {
                                // 如果订单完成了
                                if (model.State == 1)
                                {
                                    model.FinishTime = UpdateTime;
                                    model.FinishAdminId = client.AdminId;
                                    model.FinishDeviceId = client.DeviceId;
                                }


                                model.ClearReferences();


                                // 如果会员信息存在, 则更新会员信息
                                if (model.State == 1 && null != member)
                                {

                                    member.UpdateTime = UpdateTime;

                                    member.ClearReferences();


                                    ctx.Entry(member).State = System.Data.Entity.EntityState.Modified;

                                    model.MemberId = member.MemberId;


                                }
                            }


                                ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;



                            if (!toServer.Rechecked && model.State == 1)
                            {
                                // 新增支付
                                foreach (var item in OrderPays)
                                {
                                    // 如果会员信息存在, 则更新会员信息
                                    if (item.State == 0 && null != item.MemberId)
                                    {
                                        // 更新会员余额
                                        Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();

                                        memberPay.BalancePrice = Math.Round(memberPay.BalancePrice - item.Price, 2);
                                        item.tb_member = memberPay;
                                        item.BalancePrice = memberPay.BalancePrice;
                                        memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice + item.RemovePrice, 2);
                                        memberPay.SpendPrice = Math.Round(memberPay.SpendPrice + item.Price, 2);
                                        memberPay.UpdateTime = UpdateTime;

                                        memberPay.ClearReferences();


                                        ctx.Entry(memberPay).State = System.Data.Entity.EntityState.Modified;

                                    }

                                    if (item.State == 0 && null != item.BalanceId)
                                    {
                                        // 更新里面的余额信息
                                        lock (Resources.GetRes().BALANCES)
                                        {
                                            item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                            if (null != item.tb_balance)
                                            {
                                                item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                                item.BalancePrice = item.tb_balance.BalancePrice;
                                                //item.tb_balance.UpdateTime = UpdateTime;

                                                item.tb_balance.ClearReferences();
                                                ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                            }
                                        }
                                    }

                                    item.ClearReferences();

                                    item.AdminId = client.AdminId;
                                    item.DeviceId = client.DeviceId;
                                    item.AddTime = UpdateTime;
                                    item.Mode = client.Mode;
                                    ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                                }
                            }
                            else if ((toServer.Rechecked && model.State == 2) || (!toServer.Rechecked && model.State == 2 && null != roomModel && null != roomModel.PayOrder && roomModel.PayOrder.State == 0))
                            {


                                foreach (var item in productsChange)
                                {
                                    item.Value.Product.ClearReferences();

                                    item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                    item.Value.Product.UpdateTime = UpdateTime;

                                    ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                                }

                                if (null != OrderDetails)
                                {
                                    foreach (var item in OrderDetails)
                                    {
                                        if (item.State != 3)
                                        {
                                            item.ClearReferences();

                                            item.State = 3;
                                            item.AdminId = client.AdminId;
                                            item.DeviceId = client.DeviceId;
                                            item.UpdateTime = UpdateTime;
                                            item.Mode = client.Mode;
                                            ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                        }
                                    }
                                }

                                // 新增支付
                                if (null != OrderPays)
                                {
                                    foreach (var item in OrderPays)
                                {
                                    if (item.State != 3)
                                    {

                                        if (null != item.MemberId)
                                        {

                                            // 如果会员信息存在, 则更新会员信息
                                            Member payMember = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();

                                            item.tb_member = payMember;
                                            payMember.BalancePrice = Math.Round(payMember.BalancePrice + item.Price, 2);
                                            payMember.FavorablePrice = Math.Round(payMember.FavorablePrice - item.RemovePrice, 2); // 增加了账单里的优惠给会员
                                            payMember.SpendPrice = Math.Round(payMember.SpendPrice - item.Price, 2);


                                            payMember.UpdateTime = UpdateTime;

                                            payMember.ClearReferences();


                                            ctx.Entry(payMember).State = System.Data.Entity.EntityState.Modified;



                                        }

                                        if (null != item.BalanceId)
                                        {
                                            // 更新里面的余额信息
                                            lock (Resources.GetRes().BALANCES)
                                            {
                                                item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                                if (null != item.tb_balance)
                                                {
                                                    item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                                    item.BalancePrice = item.tb_balance.BalancePrice;
                                                    //item.tb_balance.UpdateTime = UpdateTime;

                                                    item.tb_balance.ClearReferences();
                                                    ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;
                                                }

                                            }
                                        }

                                        item.ClearReferences();

                                        item.State = 3;
                                        item.AdminId = client.AdminId;
                                        item.DeviceId = client.DeviceId;
                                        item.AddTime = UpdateTime;
                                        item.Mode = client.Mode;
                                        ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                        DelOrderPays.Add(item);
                                    }
                                }
                                }

                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                        lock (item.Value)
                            item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                    }

                    if (model.State == 1)
                    {
                        foreach (var item in OrderPays)
                        {
                            if (null != item.MemberId)
                            {
                                // 更新会员余额
                                Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                                memberPay.BalancePrice = Math.Round(memberPay.BalancePrice + item.Price, 2);

                                memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice - item.RemovePrice, 2);
                                memberPay.SpendPrice = Math.Round(memberPay.SpendPrice - item.Price, 2);
                            }

                            // 更新里面的余额信息
                            if (null != item.BalanceId)
                            {
                                lock (Resources.GetRes().BALANCES)
                                {
                                    if (null != item.tb_balance)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                        item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                    }
                                }
                            }
                        }
                    }
                    else if (model.State == 2)
                    {
                        foreach (var item in DelOrderPays)
                        {
                            if (null != item.MemberId)
                            {
                                // 更新会员余额
                                Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                                memberPay.BalancePrice = Math.Round(memberPay.BalancePrice - item.Price, 2);

                                memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice + item.RemovePrice, 2);
                                memberPay.SpendPrice = Math.Round(memberPay.SpendPrice + item.Price, 2);
                            }

                            // 更新里面的余额信息
                            if (null != item.BalanceId)
                            {
                                lock (Resources.GetRes().BALANCES)
                                {
                                    if (null != item.tb_balance)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                        item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                    }
                                }
                            }
                        }
                    }



                    return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.DataFaild };
                }



                if (Result)
                {
                    roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId && null != x.PayOrder && x.PayOrder.OrderId == model.OrderId).FirstOrDefault();
                    ICollection<OrderDetail> detailsOld = null;
                    ICollection<OrderPay> paysOld = null;
                    Order FinishedOrder = null;

                    if (!toServer.Rechecked && null != roomModel)
                    {

                        if (null != roomModel.PayOrder)
                        {
                            detailsOld = roomModel.PayOrder.tb_orderdetail;
                            paysOld = roomModel.PayOrder.tb_orderpay;
                        }
                        // This is why
                        roomModel.PayOrder = model;

                        if (null != detailsOld)
                            roomModel.PayOrder.tb_orderdetail = detailsOld;
                        if (null != paysOld)
                            roomModel.PayOrder.tb_orderpay = paysOld;

                       


                        roomModel.OrderSession = session = Guid.NewGuid().ToString();



                        // 1结账,2取消
                        if (roomModel.PayOrder.State == 1 || roomModel.PayOrder.State == 2)
                        {
                            // 如果会员信息存在, 则更新会员信息
                            if (roomModel.PayOrder.State == 1 && null != member)
                            {
                                Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault());
                                Resources.GetRes().MEMBERS.Add(member);
                                roomModel.PayOrder.tb_member = model.tb_member = member;


                            }

                            FinishedOrder = model;

                            roomModel.PayOrder = null;
                        }
                    }

                    if (model.State == 1)
                    {
                        foreach (var item in OrderPays)
                        {
                            if (null != item.tb_member)
                            {
                                Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.tb_member.MemberId).FirstOrDefault());
                                Resources.GetRes().MEMBERS.Add(item.tb_member);
                            }

                        }
                    }


                    // 记录日志
                    if (Result)
                        OperateLog.Instance.AddRecord(model.OrderId, null, "Order#" + OperateType.Edit, model.State, toServer.SessionId, toServer);
                    else
                        OperateLog.Instance.AddRecord(model.OrderId, null, "Order#" + OperateType.Edit, toServer.SessionId, toServer);


                    if (null != roomModel)
                        NotificateToServiceUpdateOrder(JsonConvert.SerializeObject(new List<OrderNotificationModel>() { new OrderNotificationModel() { RoomId = roomModel.RoomId, Order = roomModel.PayOrder, FinishedOrder = FinishedOrder, OrderSessionId = roomModel.OrderSession, ProductsChange = productsChange } }), toServer.SessionId); // 这个可能会导致获取不到会员信息 Order = roomModel.PayOrder, 但是不能用 Order = model,因为这更会导致所有跟会员无关的交易信息产生问题.除非需要在客户端推送接收里写好判断逻辑(暂时就这么放弃了, 会员信息如果无法及时刷新, 反正会提示错误. 重新搜索会员即可)

                    // 结账了则打印
                    if (!toServer.Rechecked && model.State == 1)
                        Print.Instance.PrintOrderAfterCheckout(client, model, null == detailsOld ? null : detailsOld.ToList());
                }

                //返回它
                return new ToClientServiceEditOrder() { Result = Result, RoomStateSession = session, Order = JsonConvert.SerializeObject(model), UpdateTime = UpdateTime };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit order operation failed.");
                //返回它
                return new ToClientServiceEditOrder() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 替换订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceReplaceOrder ServiceReplaceOrder(ToServerServiceReplaceOrder toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                RoomModel oldRoom = Resources.GetRes().ROOMS_Model.Where(x => (x.RoomId == toServer.OldRoomId && x.OrderSession == toServer.OldRoomSession)).FirstOrDefault();
                RoomModel newRoom = Resources.GetRes().ROOMS_Model.Where(x => (x.RoomId == toServer.NewRoomId && x.OrderSession == toServer.NewRoomSession)).FirstOrDefault();


                Order oldOrder = toServer.OldOrder.DeserializeObject<Order>();
                Order newOrder = toServer.NewOrder.DeserializeObject<Order>();

                ICollection<OrderDetail> oldOrderDetails = null;
                ICollection<OrderDetail> newOrderDetails = null;
                ICollection<OrderPay> oldOrderPays = null;
                ICollection<OrderPay> newOrderPays = null;



                //查找该订单信息是否已修改过
                if (null == oldRoom || toServer.OldRoomSession != oldRoom.OrderSession)//|| (x.RoomId == model.RoomId && null != x.Order))
                {
                    return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModel, OldRoomSession = "-1" };
                }
                else if (null == newRoom || toServer.NewRoomSession != newRoom.OrderSession)
                {
                    return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModel, NewRoomSession = "-1" };
                }
                else
                {
                    // 防止同一5秒内同时操作
                    lock (oldRoom)
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        if (oldRoom.LastOperateSessionId != toServer.SessionId && (dateTimeNow - oldRoom.LastOperateTime).TotalSeconds <= 5)
                        {
                            return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else if (oldRoom.LastOperateSessionId == toServer.SessionId && (dateTimeNow - oldRoom.LastOperateTime).TotalSeconds <= 3)
                        {
                            return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else
                        {
                            oldRoom.LastOperateTime = dateTimeNow;
                            oldRoom.LastOperateSessionId = toServer.SessionId;
                        }
                    }

                    // 防止同一5秒内同时操作
                    lock (newRoom)
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        if (newRoom.LastOperateSessionId != toServer.SessionId && (dateTimeNow - newRoom.LastOperateTime).TotalSeconds <= 5)
                        {
                            return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else if (newRoom.LastOperateSessionId == toServer.SessionId && (dateTimeNow - newRoom.LastOperateTime).TotalSeconds <= 3)
                        {
                            return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else
                        {
                            newRoom.LastOperateTime = dateTimeNow;
                            newRoom.LastOperateSessionId = toServer.SessionId;
                        }
                    }
                }






                if (null != oldRoom.PayOrder && null != oldRoom.PayOrder.tb_orderdetail)
                    oldOrderDetails = oldRoom.PayOrder.tb_orderdetail;
                if (null != newRoom.PayOrder && null != newRoom.PayOrder.tb_orderdetail)
                    newOrderDetails = newRoom.PayOrder.tb_orderdetail;

                if (null != oldRoom.PayOrder && null != oldRoom.PayOrder.tb_orderpay)
                    oldOrderPays = oldRoom.PayOrder.tb_orderpay;
                if (null != newRoom.PayOrder && null != newRoom.PayOrder.tb_orderpay)
                    newOrderPays = newRoom.PayOrder.tb_orderpay;




                try
                {
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


                            if (null != oldOrder && oldOrder.OrderId > 0)
                            {
                                oldOrder.ClearReferences();

                                ctx.Entry(oldOrder).State = System.Data.Entity.EntityState.Modified;
                            }


                            if (null != newOrder && newOrder.OrderId > 0)
                            {
                                newOrder.ClearReferences();

                                ctx.Entry(newOrder).State = System.Data.Entity.EntityState.Modified;
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {

                    //删除老的房间中的订单并替换新的
                    if (null != newOrder && newOrder.OrderId > 0)
                        newRoom.PayOrder = newOrder;
                    else
                        newRoom.PayOrder = null;

                    //删除新的订单中的订单并替换老的
                    if (null != oldOrder && oldOrder.OrderId > 0)
                        oldRoom.PayOrder = oldOrder;
                    else
                        oldRoom.PayOrder = null;

                    // 恢复对应的订单详情
                    if (null != oldOrderDetails)
                        newRoom.PayOrder.tb_orderdetail = oldOrderDetails;

                    if (null != newOrderDetails)
                        oldRoom.PayOrder.tb_orderdetail = newOrderDetails;

                    if (null != oldOrderPays)
                        newRoom.PayOrder.tb_orderpay = oldOrderPays;

                    if (null != newOrderPays)
                        oldRoom.PayOrder.tb_orderpay = newOrderPays;

                    oldRoom.OrderSession = Guid.NewGuid().ToString();
                    newRoom.OrderSession = Guid.NewGuid().ToString();

                    // 记录日志
                    if (null != newOrder && newOrder.OrderId > 0)
                        OperateLog.Instance.AddRecord(newOrder.OrderId, null, "Order#" + OperateType.Replace, toServer.SessionId, toServer);
                    if (null != oldOrder && oldOrder.OrderId > 0)
                        OperateLog.Instance.AddRecord(oldOrder.OrderId, null, "Order#" + OperateType.Replace, toServer.SessionId, toServer);

                    NotificateToServiceUpdateOrder(JsonConvert.SerializeObject(new List<OrderNotificationModel>() { new OrderNotificationModel() { RoomId = oldRoom.RoomId, Order = oldRoom.PayOrder, OrderSessionId = oldRoom.OrderSession, IsChangeRoom = true }, new OrderNotificationModel() { RoomId = newRoom.RoomId, Order = newRoom.PayOrder, OrderSessionId = newRoom.OrderSession, IsChangeRoom = true } }), toServer.SessionId);
                }


                //返回它
                return new ToClientServiceReplaceOrder() { Result = Result, NewRoomSession = newRoom.OrderSession, OldRoomSession = oldRoom.OrderSession };// , OldOrder = JsonConvert.SerializeObject(oldOrder), NewOrder = JsonConvert.SerializeObject(newOrder)
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Replace order operation failed.");
                //返回它
                return new ToClientServiceReplaceOrder() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 查找订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetOrders ServiceGetOrders(ToServerServiceGetOrders toServer)
        {
            try
            {
                bool Result = false;
                List<Order> Orders;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetOrders() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetOrders() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        IQueryable<Order> Statement = null;
                            
                            
                        if (toServer.IsIncludeRef)
                            Statement = ctx.Orders.Include("tb_member").Include("tb_orderdetail").Include("tb_orderpay");
                        else
                            Statement = ctx.Orders.Include("tb_member");



                        if (toServer.StartTime > 0)
                            Statement = Statement.Where(x => x.StartTime >= toServer.StartTime);
                        if (toServer.EndTime > 0)
                            Statement = Statement.Where(x => x.EndTime <= toServer.EndTime);

                        if (toServer.AddTimeStart > 0)
                            Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                        if (toServer.AddTimeEnd > 0)
                            Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                        if (toServer.FinishTime > 0)
                            Statement = Statement.Where(x => x.FinishTime <= toServer.FinishTime);

                        if (toServer.AdminId > 0)
                            Statement = Statement.Where(x => x.AdminId == toServer.AdminId);
                        if (toServer.FinishAdminId > 0)
                            Statement = Statement.Where(x => x.FinishAdminId == toServer.FinishAdminId);

                        if (!string.IsNullOrWhiteSpace(toServer.MemberNo))
                        {
                            Member member = Resources.GetRes().MEMBERS.Where(x => x.MemberNo == toServer.MemberNo).FirstOrDefault();
                            if (null == member)
                            {
                                return new ToClientServiceGetOrders() { Orders = JsonConvert.SerializeObject(new List<Order>()), Result = true };
                            }

                            Statement = Statement.Where(x => x.MemberId == member.MemberId);
                        }
                        if (!string.IsNullOrWhiteSpace(toServer.CardNo))
                        {
                            Member member = Resources.GetRes().MEMBERS.Where(x => x.CardNo == toServer.CardNo).FirstOrDefault();
                            if (null == member)
                            {
                                return new ToClientServiceGetOrders() { Orders = JsonConvert.SerializeObject(new List<Order>()), Result = true };
                            }

                            Statement = Statement.Where(x => x.MemberId == member.MemberId);
                        }

                        if (toServer.RoomId > 0)
                            Statement = Statement.Where(x => x.RoomId == toServer.RoomId);
                        if (toServer.State > -1)
                            Statement = Statement.Where(x => x.State == toServer.State);

                        Orders = Statement.ToList();
                        Result = true;

                    }

                    
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetOrders() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetOrders() {  Orders = JsonConvert.SerializeObject(Orders), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get order operation failed.");
                //返回它
                return new ToClientServiceGetOrders() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Order


        #region OrderDetail


        /// <summary>
        /// 增加订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddOrderDetail ServiceAddOrderDetail(ToServerServiceAddOrderDetail toServer)
        {
            throw new NotSupportedException("This method too old for now, it hasn't been used for a long time, need to renew and test first!");
            try
            {
                bool Result = false;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddOrderDetail() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddOrderDetail() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Order model = toServer.Order.DeserializeObject<Order>();
                List<OrderDetail> details = toServer.OrderDetails.DeserializeObject<List<OrderDetail>>();

                //查找包厢能否使用(包厢状态不同说明模型有改动, 包厢已有订单,说明还未结账或模型改动)
                RoomModel roomModel2 = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                if (null != roomModel2 && roomModel2.OrderSession != toServer.RoomStateSession)
                {
                    return new ToClientServiceAddOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                }
                else
                {
                    // 防止同一5秒内同时操作
                    lock (roomModel2)
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        if (roomModel2.LastOperateSessionId != toServer.SessionId && (dateTimeNow - roomModel2.LastOperateTime).TotalSeconds <= 5)
                        {
                            return new ToClientServiceAddOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else if (roomModel2.LastOperateSessionId == toServer.SessionId && (dateTimeNow - roomModel2.LastOperateTime).TotalSeconds <= 3)
                        {
                            return new ToClientServiceAddOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else
                        {
                            roomModel2.LastOperateTime = dateTimeNow;
                            roomModel2.LastOperateSessionId = toServer.SessionId;
                        }
                    }
                }


                // 要开启的订单不能超过当前设置的包厢数
                if (toServer.IsNewOrder && Resources.GetRes().ROOMS_Model.Where(x => null != x.PayOrder).Count() >= Resources.GetRes().ROOM_COUNT)
                {
                    return new ToClientServiceAddOrderDetail() { ExceptionType = ServiceExceptionType.RoomCountOutOfLimit };
                }

                 Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
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

                            if (toServer.IsNewOrder)
                            {
                                model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                model.RoomPriceCalcTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                model.AdminId = client.AdminId;
                                model.DeviceId = client.DeviceId;
                                model.Mode = client.Mode;

                                model.ClearReferences();

                                ctx.Entry(model).State = System.Data.Entity.EntityState.Added;

                            }
                            else
                            {
                                model.ClearReferences();


                                if (model.IsPayByTime != 0 && roomModel2.PayOrder.EndTime != model.EndTime)
                                    model.RoomPriceCalcTime = model.EndTime.Value;

                                ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            }

                            foreach (var item in details)
                            {
                                item.AdminId = client.AdminId;
                                item.DeviceId = client.DeviceId;
                                item.Mode = client.Mode;
                                item.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;


                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddOrderDetail() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    RoomModel roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();


                    ICollection<OrderDetail> detailsOld = null;
                    ICollection<OrderPay> paysOld = null;

                    if (null != roomModel.PayOrder)
                    {
                        detailsOld = roomModel.PayOrder.tb_orderdetail;
                        paysOld = roomModel.PayOrder.tb_orderpay;
                    }

                    roomModel.PayOrder = model;

                    if (null != detailsOld)
                        roomModel.PayOrder.tb_orderdetail = detailsOld;
                    if (null != paysOld)
                        roomModel.PayOrder.tb_orderpay = paysOld;


                    roomModel.OrderSession = session = Guid.NewGuid().ToString();

                    //roomModel.Order = model;如果使用,先好好看其他方法中的具体用法, 免得覆盖了里面本该有的订单详细信息
                    foreach (var item in details)
                    {
                        roomModel.PayOrder.tb_orderdetail.Add(item);
                    }

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.OrderId, null, "OrderDetail#" + OperateType.Add, toServer.SessionId, toServer);

                    // 成功了给客户端发送更新订单明细有增加通知(顾客验证模式)
                    NotificateToServicAddOrderDetails(JsonConvert.SerializeObject(model), model.RoomId, roomModel.OrderSession, toServer.SessionId);
                }

                

                //返回它
                return new ToClientServiceAddOrderDetail() { Result = Result, OrderDetails = JsonConvert.SerializeObject(details), OrderSessionId = session };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add order detail operation failed.");
                //返回它
                return new ToClientServiceAddOrderDetail() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        /// <summary>
        /// 修改订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSaveOrderDetail ServiceSaveOrderDetail(ToServerServiceSaveOrderDetail toServer)
        {


            try
            {
                bool Result = false;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceSaveOrderDetail() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceSaveOrderDetail() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Order model = toServer.Order.DeserializeObject<Order>();
                List<OrderDetail> detailsAdd = toServer.OrderDetailsAdd.DeserializeObject<List<OrderDetail>>();
                List<OrderPay> paysAdd = toServer.OrderPaysAdd.DeserializeObject<List<OrderPay>>();
                List<OrderDetail> detailsEdit = toServer.OrderDetailsEdit.DeserializeObject<List<OrderDetail>>();
                List<OrderDetail> detailsConfirm = toServer.OrderDetailsConfirm.DeserializeObject<List<OrderDetail>>();

                //查找包厢能否使用(包厢状态不同说明模型有改动, 包厢已有订单,说明还未结账或模型改动
                RoomModel roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                if (null != roomModel && roomModel.OrderSession != toServer.RoomStateSession)
                {
                    return new ToClientServiceSaveOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                }
                else
                {
                    // 防止同一5秒内同时操作
                    lock (roomModel)
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        if (roomModel.LastOperateSessionId != toServer.SessionId && (dateTimeNow - roomModel.LastOperateTime).TotalSeconds <= 5)
                        {
                            return new ToClientServiceSaveOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else if (roomModel.LastOperateSessionId == toServer.SessionId && (dateTimeNow - roomModel.LastOperateTime).TotalSeconds <= 3)
                        {
                            return new ToClientServiceSaveOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else
                        {
                            roomModel.LastOperateTime = dateTimeNow;
                            roomModel.LastOperateSessionId = toServer.SessionId;
                        }
                    }
                }



                // 先检查产品数量是否少于它要购买的数量
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();
                // 新增的部分
                foreach (var item in detailsAdd)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1)
                    {
                        if (product.BalanceCount < item.Count)
                        {
                            // 如果有父级
                            if (null != product.ProductParentId)
                            {
                                Product productParent = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                if (null != productParent && productParent.IsBindCount == 1)
                                {
                                    double ParentRemove = 0;
                                    double ProductAdd = 0;

                                    lock (product)
                                    {
                                        double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); 
                                        ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                        ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                        ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3);
                                    }

                                    // 从父级中去掉
                                    lock (productParent)
                                        productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);

                                    if (productsChange.ContainsKey(productParent.ProductId))
                                        productsChange[productParent.ProductId].CountChange = Math.Round(productsChange[productParent.ProductId].CountChange - ParentRemove, 3);
                                    else
                                        productsChange.Add(productParent.ProductId, new ProductWithCount() { Product = productParent, CountChange = -ParentRemove });


                                    // 给产品增加零的
                                    lock (product)
                                        product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);

                                    if (productsChange.ContainsKey(product.ProductId))
                                        productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + ProductAdd, 3);
                                    else
                                        productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = ProductAdd });

                                }
                            }
                        }
                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - item.Count, 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = -item.Count });
                    }
                }
                // 编辑的部分
                foreach (var item in detailsEdit)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1)
                    {
                        OrderDetail oldDetail = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault().PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == item.OrderDetailId).FirstOrDefault();

                        if (product.BalanceCount < -oldDetail.Count + item.Count)
                        {
                            // 如果有父级
                            if (null != product.ProductParentId)
                            {
                                Product productParent = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                if (null != productParent && productParent.IsBindCount == 1)
                                {
                                    double ParentRemove = 0;
                                    double ProductAdd = 0;

                                    lock (product)
                                    {
                                        double NeedChangeFromParent = Math.Round((-oldDetail.Count + item.Count) - product.BalanceCount, 3);
                                        ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3);
                                        ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                        ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3);
                                    }

                                    // 从父级中去掉
                                    lock (productParent)
                                        productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);

                                    if (productsChange.ContainsKey(productParent.ProductId))
                                        productsChange[productParent.ProductId].CountChange = Math.Round(productsChange[productParent.ProductId].CountChange - ParentRemove, 3);
                                    else
                                        productsChange.Add(productParent.ProductId, new ProductWithCount() { Product = productParent, CountChange = -ParentRemove });


                                    // 给产品增加零的
                                    lock (product)
                                        product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);

                                    if (productsChange.ContainsKey(product.ProductId))
                                        productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + ProductAdd);
                                    else
                                        productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = ProductAdd });

                                }
                            }

                        }
                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount - (-oldDetail.Count + item.Count), 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - (-oldDetail.Count + item.Count), 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = -(-oldDetail.Count + item.Count) });
                    }
                }
                // 确认的部分
                foreach (var item in detailsConfirm)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1)
                    {
                        if (product.BalanceCount < item.Count)
                        {
                            // 如果有父级
                            if (null != product.ProductParentId)
                            {
                                Product productParent = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                if (null != productParent && productParent.IsBindCount == 1)
                                {
                                    double ParentRemove = 0;
                                    double ProductAdd = 0;

                                    lock (product)
                                    {
                                        double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); 
                                        ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3);
                                        ParentRemove = (int)Math.Ceiling(ParentRemove); // 0.1 to 1. 1.1 to 2

                                        ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 
                                    }

                                    // 从父级中去掉
                                    lock (productParent)
                                        productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);

                                    if (productsChange.ContainsKey(productParent.ProductId))
                                        productsChange[productParent.ProductId].CountChange = Math.Round(productsChange[productParent.ProductId].CountChange - ParentRemove, 3);
                                    else
                                        productsChange.Add(productParent.ProductId, new ProductWithCount() { Product = productParent, CountChange = -ParentRemove });


                                    // 给产品增加零的
                                    lock (product)
                                        product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);

                                    if (productsChange.ContainsKey(product.ProductId))
                                        productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + ProductAdd, 3);
                                    else
                                        productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = ProductAdd });

                                }
                            }
                        }

                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - item.Count, 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = -item.Count });
                    }
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                long UpdateTime = 0;

                try
                {
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

                            // 时间不同, 说明到期时间不同(一般多见于按时间收费上的改动)
                            if (model.IsPayByTime != 0 && roomModel.PayOrder.EndTime != model.EndTime)
                                model.RoomPriceCalcTime = model.EndTime.Value;


                            model.ClearReferences();

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;


                            foreach (var item in detailsAdd)
                            {
                                item.AdminId = client.AdminId;
                                item.DeviceId = client.DeviceId;
                                item.Mode = client.Mode;
                                item.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));


                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;

                            }

                            foreach (var item in detailsEdit)
                            {
                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;

                            }

                            foreach (var item in detailsConfirm)
                            {
                                item.ClearReferences();

                                item.ConfirmAdminId = client.AdminId;
                                item.ConfirmDeviceId = client.DeviceId;
                                item.ConfirmTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;

                            }

                            UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            foreach (var item in productsChange)
                            {
                                item.Value.Product.ClearReferences();

                                item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                item.Value.Product.UpdateTime = UpdateTime;

                                ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                            }


                            // 新增支付
                            foreach (var item in paysAdd)
                            {
                                // 如果会员信息存在, 则更新会员信息
                                if (item.State == 0 && null != item.MemberId)
                                {
                                    // 更新会员余额
                                    Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();

                                    memberPay.BalancePrice = Math.Round(memberPay.BalancePrice - item.Price, 2);
                                    item.tb_member = memberPay;
                                    item.BalancePrice = memberPay.BalancePrice;
                                    memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice + item.RemovePrice, 2);
                                    memberPay.SpendPrice = Math.Round(memberPay.SpendPrice + item.Price, 2);
                                    memberPay.UpdateTime = UpdateTime;

                                    memberPay.ClearReferences();


                                    ctx.Entry(memberPay).State = System.Data.Entity.EntityState.Modified;

                                }

                                if (item.State == 0 && null != item.BalanceId)
                                {
                                    // 更新里面的余额信息
                                    lock (Resources.GetRes().BALANCES)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                        if (null != item.tb_balance)
                                        {
                                            item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                            item.BalancePrice = item.tb_balance.BalancePrice;
                                            //item.tb_balance.UpdateTime = UpdateTime;

                                            item.tb_balance.ClearReferences();
                                            ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                        }
                                    }
                                }

                                item.ClearReferences();

                                item.AdminId = client.AdminId;
                                item.DeviceId = client.DeviceId;
                                item.AddTime = UpdateTime;
                                item.Mode = client.Mode;
                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                            }



                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
 
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                        lock(item.Value)
                            item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount  - item.Value.CountChange, 3);
                    }

                    foreach (var item in paysAdd)
                    {
                        if (null != item.MemberId)
                        {
                            // 更新会员余额
                            Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                            memberPay.BalancePrice = Math.Round(memberPay.BalancePrice + item.Price, 2);

                            memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice - item.RemovePrice, 2);
                            memberPay.SpendPrice = Math.Round(memberPay.SpendPrice - item.Price, 2);
                        }

                        // 更新里面的余额信息
                        if (null != item.BalanceId)
                        {
                            lock (Resources.GetRes().BALANCES)
                            {
                                if (null != item.tb_balance)
                                {
                                    item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                    item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                }
                            }
                        }
                    }

                    return new ToClientServiceSaveOrderDetail() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {

                    ICollection<OrderDetail> detailsOld = null;
                    ICollection<OrderPay> paysOld = null;
                    if (null != roomModel.PayOrder)
                    {
                        detailsOld = roomModel.PayOrder.tb_orderdetail;
                        paysOld = roomModel.PayOrder.tb_orderpay;
                    }

                    Order oldOrder = roomModel.PayOrder;
                    // This is why
                    roomModel.PayOrder = model;


                    if (null != detailsOld)
                        roomModel.PayOrder.tb_orderdetail = detailsOld;
                    if (null != paysOld)
                        roomModel.PayOrder.tb_orderpay = paysOld;

                    roomModel.OrderSession = session = Guid.NewGuid().ToString();
                    
                    foreach (var item in detailsAdd)
                    {
                        roomModel.PayOrder.tb_orderdetail.Add(item);
                    }

                    foreach (var item in detailsEdit)
                    {
                        roomModel.PayOrder.tb_orderdetail.Remove(roomModel.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == item.OrderDetailId).FirstOrDefault());
                        roomModel.PayOrder.tb_orderdetail.Add(item);
                    }

                    foreach (var item in detailsConfirm)
                    {
                        roomModel.PayOrder.tb_orderdetail.Remove(roomModel.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == item.OrderDetailId).FirstOrDefault());
                        roomModel.PayOrder.tb_orderdetail.Add(item);
                    }


                    foreach (var item in paysAdd)
                    {
                        if (null != item.tb_member)
                        {
                            Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.tb_member.MemberId).FirstOrDefault());
                            Resources.GetRes().MEMBERS.Add(item.tb_member);
                        }

                        roomModel.PayOrder.tb_orderpay.Add(item);
                    }

                    
                    // 记录日志
                    if (Result && null != paysAdd && paysAdd.Count > 0)
                        OperateLog.Instance.AddRecord(model.OrderId, null, "OrderDetail#" + OperateType.Save, 1, toServer.SessionId, toServer);
                    else
                        OperateLog.Instance.AddRecord(model.OrderId, null, "OrderDetail#" + OperateType.Save, toServer.SessionId, toServer);

                    // 成功了给客户端发送更新订单通知
                    NotificateToServiceUpdateOrder(JsonConvert.SerializeObject(new List<OrderNotificationModel>() { new OrderNotificationModel() { RoomId = roomModel.RoomId, Order = roomModel.PayOrder, OrderSessionId = roomModel.OrderSession, ProductsChange = productsChange} }), toServer.SessionId);


                    

                    // 打印

                    List<OrderDetail> details = null;
                    if (null != detailsAdd && detailsAdd.Count > 0)
                        details = detailsAdd;
                    else if (null != detailsConfirm && detailsConfirm.Count > 0)
                        details = detailsConfirm;

                    // 去掉了, 只是加钱就没法打印 if (null != details)
                        Print.Instance.PrintOrderAfterBuy(client, model, details, oldOrder);
                }

                //返回它
                return new ToClientServiceSaveOrderDetail() { Result = Result , OrderDetailAdd = JsonConvert.SerializeObject(detailsAdd), OrderDetailEdit = JsonConvert.SerializeObject(detailsEdit), OrderDetailConfirm = JsonConvert.SerializeObject(detailsConfirm), OrderPayAdd = JsonConvert.SerializeObject(paysAdd), OrderSessionId = session, UpdateTime = UpdateTime };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Save order detail operation failed.");
                //返回它
                return new ToClientServiceSaveOrderDetail() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelOrderDetail ServiceDelOrderDetail(ToServerServiceDelOrderDetail toServer)
        {
            try
            {
                bool Result = false;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelOrderDetail() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelOrderDetail() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Order model = toServer.Order.DeserializeObject<Order>();
                List<OrderDetail> details = toServer.OrderDetails.DeserializeObject<List<OrderDetail>>();


                RoomModel roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();
                //查找包厢能否使用(包厢状态不同说明模型有改动, 包厢已有订单,说明还未结账或模型改动)
                if (null != roomModel && roomModel.OrderSession != toServer.RoomStateSession)
                {
                    return new ToClientServiceDelOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                }
                else
                {
                    // 防止同一5秒内同时操作
                    lock (roomModel)
                    {
                        DateTime dateTimeNow = DateTime.Now;
                        if (roomModel.LastOperateSessionId != toServer.SessionId && (dateTimeNow - roomModel.LastOperateTime).TotalSeconds <= 5)
                        {
                            return new ToClientServiceDelOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else if (roomModel.LastOperateSessionId == toServer.SessionId && (dateTimeNow - roomModel.LastOperateTime).TotalSeconds <= 3)
                        {
                            return new ToClientServiceDelOrderDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModelForSameTimeOperate };
                        }
                        else
                        {
                            roomModel.LastOperateTime = dateTimeNow;
                            roomModel.LastOperateSessionId = toServer.SessionId;
                        }
                    }
                }

                // 先检查产品数量是否少于它要购买的数量
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();

                foreach (var item in details)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1 && item.State != 1)
                    {
                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + item.Count, 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = item.Count });

                    }
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                long UpdateTime = 0;

                try
                {
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


                            model.ClearReferences();

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;



                            foreach (var item in details)
                            {

                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            }

                            UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            foreach (var item in productsChange)
                            {
                                item.Value.Product.ClearReferences();

                                item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                item.Value.Product.UpdateTime = UpdateTime;

                                ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                        lock (item.Value)
                            item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                    }

                    return new ToClientServiceDelOrderDetail() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {
                    roomModel = Resources.GetRes().ROOMS_Model.Where(x => x.RoomId == model.RoomId).FirstOrDefault();

                    ICollection<OrderDetail> detailsOld = null;
                    ICollection<OrderPay> paysOld = null;

                    if (null != roomModel.PayOrder)
                    {
                        detailsOld = roomModel.PayOrder.tb_orderdetail;
                        paysOld = roomModel.PayOrder.tb_orderpay;
                    }

                    roomModel.PayOrder = model;

                    if (null != detailsOld)
                        roomModel.PayOrder.tb_orderdetail = detailsOld;
                    if (null != paysOld)
                        roomModel.PayOrder.tb_orderpay = paysOld;

                    roomModel.OrderSession = session = Guid.NewGuid().ToString();

                    foreach (var item in details)
                    {
                        roomModel.PayOrder.tb_orderdetail.Remove(roomModel.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == item.OrderDetailId).FirstOrDefault());
                        roomModel.PayOrder.tb_orderdetail.Add(item);
                    }

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.OrderId, null, "OrderDetail#" + OperateType.Delete, toServer.SessionId, toServer);

                    // 成功了给客户端发送更新订单通知
                    NotificateToServiceUpdateOrder(JsonConvert.SerializeObject(new List<OrderNotificationModel>() { new OrderNotificationModel() { RoomId = roomModel.RoomId, Order = roomModel.PayOrder, OrderSessionId = roomModel.OrderSession, ProductsChange = productsChange } }), toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelOrderDetail() { Result = Result, OrderSessionId = session, UpdateTime = UpdateTime, Order = JsonConvert.SerializeObject(model), OrderDetails = JsonConvert.SerializeObject(details) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete order detail operation failed.");
                //返回它
                return new ToClientServiceDelOrderDetail() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找订单明细(按订单)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetOrderDetail ServiceGetOrderDetail(ToServerServiceGetOrderDetail toServer)
        {
            try
            {
                bool Result = false;
                List<OrderDetail> OrderDetails;
                List<OrderPay> OrderPays;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetOrderDetail() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetOrderDetail() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        OrderDetails = ctx.OrderDetails.Where(x => x.OrderId == toServer.OrderId).ToList();
                        OrderPays = ctx.OrderPays.Include("tb_member").Where(x => x.OrderId == toServer.OrderId).ToList();
                        Result = true;

                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetOrderDetail() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetOrderDetail() { OrderDetails = JsonConvert.SerializeObject(OrderDetails), OrderPays = JsonConvert.SerializeObject(OrderPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get order detail operation failed.");
                //返回它
                return new ToClientServiceGetOrderDetail() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }







        /// <summary>
        /// 查找订单支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetOrderPay ServiceGetOrderPay(ToServerServiceGetOrderPay toServer)
        {
            try
            {
                bool Result = false;
                List<OrderPay> OrderPays = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetOrderPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetOrderPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;


                        if (toServer.AddTimeStart != 0 && toServer.AddTimeEnd != 0)
                        {

                            IQueryable<OrderPay> Statement = ctx.OrderPays;

                            if (toServer.AddTimeStart > 0)
                                Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);


                            OrderPays = Statement.ToList();
                            Result = true;
                        }

                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetOrderPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetOrderPay() { OrderPays = JsonConvert.SerializeObject(OrderPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get order pay operation failed.");
                //返回它
                return new ToClientServiceGetOrderPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        #endregion Order


        #region Takeout

        /// <summary>
        /// 新建外卖
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceNewTakeout ServiceAddTakeout(ToServerServiceNewTakeout toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceNewTakeout() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceNewTakeout() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Takeout model = toServer.Takeout.DeserializeObject<Takeout>();
                Member member = model.tb_member;
                List<TakeoutDetail> details = toServer.TakeoutDetails.DeserializeObject<List<TakeoutDetail>>();
                List<TakeoutPay> pays = toServer.TakeoutPays.DeserializeObject<List<TakeoutPay>>();

                // 判断会员UpdateTime,以免覆盖
                if (null != member && Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault().UpdateTime != member.UpdateTime)
                {
                    return new ToClientServiceNewTakeout() { ExceptionType = ServiceExceptionType.UpdateRefModel };
                }


                // 先检查产品数量是否少于它要购买的数量
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();

                foreach (var item in details)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1)
                    {
                        if (product.BalanceCount < item.Count)
                        {
                            // 如果有父级
                            if (null != product.ProductParentId)
                            {
                                Product productParent = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                if (null != productParent && productParent.IsBindCount == 1)
                                {
                                    double ParentRemove = 0;
                                    double ProductAdd = 0;

                                    lock (product)
                                    {
                                        double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3);
                                        ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                        ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                        ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3);
                                    }

                                    // 从父级中去掉
                                    lock (productParent)
                                        productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);

                                    if (productsChange.ContainsKey(productParent.ProductId))
                                        productsChange[productParent.ProductId].CountChange = Math.Round(productsChange[productParent.ProductId].CountChange - ParentRemove, 3);
                                    else
                                        productsChange.Add(productParent.ProductId, new ProductWithCount() { Product = productParent, CountChange = -ParentRemove });


                                    // 给产品增加零的
                                    lock (product)
                                        product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);

                                    if (productsChange.ContainsKey(product.ProductId))
                                        productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + ProductAdd, 3);
                                    else
                                        productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = ProductAdd });

                                }
                            }

                        }

                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - item.Count, 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = -item.Count });
                    }
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                long UpdateTime= long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                try
                {
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

                            model.AdminId = client.AdminId;
                            model.DeviceId = client.DeviceId;
                            model.Mode = client.Mode;
                            model.AddTime = UpdateTime;

                            model.ClearReferences();

                            if (model.State == 1)
                            {
                                model.FinishAdminId = client.AdminId;
                                model.FinishDeviceId = client.DeviceId;
                                model.FinishTime = UpdateTime;
                            }



                            ctx.Entry(model).State = System.Data.Entity.EntityState.Added;

                            // 如果会员信息存在, 则更新会员信息
                            if (model.State == 1 && null != member)
                            {
                                member.UpdateTime = UpdateTime;

                                member.ClearReferences();


                                ctx.Entry(member).State = System.Data.Entity.EntityState.Modified;

                                model.MemberId = member.MemberId;

                            }


                            foreach (var item in details)
                            {
                                item.AdminId = client.AdminId;
                                item.DeviceId = client.DeviceId;
                                item.Mode = client.Mode;
                                item.AddTime = UpdateTime;

                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;

                            }

                            foreach (var item in productsChange)
                            {
                                item.Value.Product.ClearReferences();

                                item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                item.Value.Product.UpdateTime = UpdateTime;

                                ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                            }


                            // 新增支付
                            foreach (var item in pays)
                            {
                                // 如果会员信息存在, 则更新会员信息
                                if (item.State == 0 && null != item.MemberId)
                                {
                                    // 更新会员余额
                                    Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();

                                    memberPay.BalancePrice = Math.Round(memberPay.BalancePrice - item.Price, 2);
                                    item.tb_member = memberPay;
                                    item.BalancePrice = memberPay.BalancePrice;
                                    memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice + item.RemovePrice, 2);
                                    memberPay.SpendPrice = Math.Round(memberPay.SpendPrice + item.Price, 2);
                                    memberPay.UpdateTime = UpdateTime;

                                    memberPay.ClearReferences();


                                    ctx.Entry(memberPay).State = System.Data.Entity.EntityState.Modified;

                                }

                                if (null != item.BalanceId)
                                {
                                    // 更新里面的余额信息
                                    lock (Resources.GetRes().BALANCES)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                        if (null != item.tb_balance)
                                        {
                                            item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                            item.BalancePrice = item.tb_balance.BalancePrice;
                                            //item.tb_balance.UpdateTime = UpdateTime;

                                            item.tb_balance.ClearReferences();
                                            ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                        }
                                    }
                                }

                                item.ClearReferences();

                                item.AdminId = client.AdminId;
                                item.DeviceId = client.DeviceId;
                                item.AddTime = UpdateTime;
                                item.Mode = client.Mode;
                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                            }

                            
                            

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                        lock (item.Value)
                            item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                    }

                    foreach (var item in pays)
                    {
                        if (null != item.MemberId)
                        {
                            // 更新会员余额
                            Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                            memberPay.BalancePrice = Math.Round(memberPay.BalancePrice + item.Price, 2);

                            memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice - item.RemovePrice, 2);
                            memberPay.SpendPrice = Math.Round(memberPay.SpendPrice - item.Price, 2);
                        }

                        // 更新里面的余额信息
                        if (null != item.BalanceId)
                        {
                            lock (Resources.GetRes().BALANCES)
                            {
                                if (null != item.tb_balance)
                                {
                                    item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                    item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                }
                            }
                        }
                    }



                    return new ToClientServiceNewTakeout() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Result = true;
                    Id = model.TakeoutId;

                    TakeoutModel takeoutModel = new TakeoutModel();
                    

                    takeoutModel.Takeout = model;
                    session = takeoutModel.TakeoutSession = Guid.NewGuid().ToString();


                    foreach (var item in details)
                    {
                        takeoutModel.Takeout.tb_takeoutdetail.Add(item);
                    }

                    foreach (var item in pays)
                    {
                        if (null != item.tb_member)
                        {
                            Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.tb_member.MemberId).FirstOrDefault());
                            Resources.GetRes().MEMBERS.Add(item.tb_member);
                        }

                        takeoutModel.Takeout.tb_takeoutpay.Add(item);
                    }


                    // 如果会员信息存在, 则更新会员信息
                    if (model.State == 1 && null != member)
                    {
                        Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault());
                        Resources.GetRes().MEMBERS.Add(member);
                        takeoutModel.Takeout.tb_member = model.tb_member = member;

                    
                    }


                    // 记录日志
                    if (Result)
                        OperateLog.Instance.AddRecord(model.TakeoutId, null, "Takeout#" + OperateType.Add, 1, toServer.SessionId, toServer);
                    else
                        OperateLog.Instance.AddRecord(model.TakeoutId, null, "Takeout#" + OperateType.Add, toServer.SessionId, toServer);

                    // 成功了给服务端发送更新订单通知
                    NotificateToServiceUpdateTakeout(JsonConvert.SerializeObject(new List<TakeoutNotificationModel>() { new TakeoutNotificationModel() { Takeout = takeoutModel.Takeout, TakeoutSessionId = takeoutModel.TakeoutSession, ProductsChange = productsChange } }), toServer.SessionId);

                    if (model.State == 1)
                    {
                        // 打印
                        Print.Instance.PrintOrderAfterCheckout(client, model, details);
                    }
                }


                //返回它
                return new ToClientServiceNewTakeout() { Takeout = JsonConvert.SerializeObject(model), TakeoutDetails = JsonConvert.SerializeObject(details), TakeoutPays = JsonConvert.SerializeObject(pays), Result = Result, TakeoutSessionId = session, UpdateTime = UpdateTime };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add takeout operation failed.");
                //返回它
                return new ToClientServiceNewTakeout() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 编辑外卖
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditTakeout ServiceEditTakeout(ToServerServiceEditTakeout toServer)
        {
            try
            {
                bool Result = false;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditTakeout() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditTakeout() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Takeout model = toServer.Takeout.DeserializeObject<Takeout>();
                List<TakeoutPay> TakeoutPays = new List<TakeoutPay>();
                List<TakeoutPay> DelTakeoutPays = new List<TakeoutPay>();
                ICollection<TakeoutDetail> TakeoutDetails = null;

                Member member = model.tb_member;

                if (!toServer.Rechecked)
                {
                    //查找该外卖信息是否已修改过
                    if (Resources.GetRes().TAKEOUT_Model.Any(x => (null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId && x.TakeoutSession != toServer.TakeoutStateSession)))
                    {
                        return new ToClientServiceEditTakeout() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                    }
                }

                // 判断会员UpdateTime,以免覆盖
                if (null != member && Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault().UpdateTime != member.UpdateTime)
                {
                    return new ToClientServiceEditTakeout() { ExceptionType = ServiceExceptionType.UpdateRefModel };
                }


                
                if (!toServer.Rechecked)
                {
                    TakeoutDetails = Resources.GetRes().TAKEOUT_Model.Where(x => null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId).Select(x => x.Takeout.tb_takeoutdetail).FirstOrDefault();
                }
                else if (toServer.Rechecked && model.State == 2)
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;
                        ((IObjectContextAdapter)ctx).ObjectContext.CommandTimeout = Resources.GetRes().TIME_OUT_SHORT;//无用
                        ctx.Configuration.AutoDetectChangesEnabled = false;
                        ctx.Configuration.ValidateOnSaveEnabled = false;

                        Takeout ctxTakeoutsWhereFirstOrDefault = ctx.Takeouts.Where(x => x.TakeoutId == model.TakeoutId).FirstOrDefault();
                        //查找该外卖信息是否已修改过
                        if (ctxTakeoutsWhereFirstOrDefault.UpdateTime != model.UpdateTime)
                        {
                            return new ToClientServiceEditTakeout() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                        }


                        TakeoutDetails = ctx.TakeoutDetails.Where(x => x.TakeoutId == model.TakeoutId).ToList();
                        TakeoutPays = ctx.TakeoutPays.Where(x => x.TakeoutId == model.TakeoutId).ToList();
                    }
                }


                // 结账没关系. 反正结帐前是确认好的, 也已经购买好的. 但是如果取消, 就得还原产品
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();

               
                if (null != TakeoutDetails && model.State == 2)
                {
                    foreach (var item in TakeoutDetails)
                    {
                        Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                        if (product.IsBindCount == 1 && item.State != 1 && item.State != 3)
                        {
                            lock (product)
                                product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);

                            if (productsChange.ContainsKey(product.ProductId))
                                productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + item.Count, 3);
                            else
                                productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = item.Count });

                        }
                    }
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                long UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

             
                try
                {
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

                            if (toServer.Rechecked && model.State == 2)
                            {

                                

                                model.ReCheckedCount = model.ReCheckedCount + 1;
                                model.UpdateTime = UpdateTime;

                                model.ClearReferences();

                                ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            }
                            else if(!toServer.Rechecked && model.State == 1)
                            {
                                // 如果订单完成了
                                if (model.State == 1)
                                {
                                    model.FinishTime = UpdateTime;
                                    model.FinishAdminId = client.AdminId;
                                    model.FinishDeviceId = client.DeviceId;

                                }

                                // 如果会员信息存在, 则更新会员信息
                                if (model.State == 1 && null != model.tb_member)
                                {

                                    member.UpdateTime = UpdateTime;

                                    member.ClearReferences();

                                    ctx.Entry(member).State = System.Data.Entity.EntityState.Modified;

                                    model.MemberId = member.MemberId;
                                }

                                ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            }

                                if (!toServer.Rechecked && model.State == 1)
                                {
                                    // 新增支付
                                    foreach (var item in TakeoutPays)
                                    {
                                    // 如果会员信息存在, 则更新会员信息
                                    if (item.State == 0 && null != item.MemberId)
                                    {
                                        // 更新会员余额
                                        Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();

                                        memberPay.BalancePrice = Math.Round(memberPay.BalancePrice - item.Price, 2);
                                        item.tb_member = memberPay;
                                        item.BalancePrice = memberPay.BalancePrice;
                                        memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice + item.RemovePrice, 2);
                                        memberPay.SpendPrice = Math.Round(memberPay.SpendPrice + item.Price, 2);
                                        memberPay.UpdateTime = UpdateTime;

                                        memberPay.ClearReferences();


                                        ctx.Entry(memberPay).State = System.Data.Entity.EntityState.Modified;

                                    }

                                    if (null != item.BalanceId)
                                    {
                                        // 更新里面的余额信息
                                        lock (Resources.GetRes().BALANCES)
                                        {
                                            item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                            if (null != item.tb_balance)
                                            {
                                                item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                                item.BalancePrice = item.tb_balance.BalancePrice;
                                                //item.tb_balance.UpdateTime = UpdateTime;

                                                item.tb_balance.ClearReferences();
                                                ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                            }
                                        }
                                    }

                                        item.ClearReferences();

                                        item.AdminId = client.AdminId;
                                        item.DeviceId = client.DeviceId;
                                        item.AddTime = UpdateTime;
                                        item.Mode = client.Mode;
                                        ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                                    }

                                }
                                else if (toServer.Rechecked && model.State == 2)
                                {


                                foreach (var item in productsChange)
                                {
                                    item.Value.Product.ClearReferences();

                                    item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                    item.Value.Product.UpdateTime = UpdateTime;

                                    ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                                }

                                if (null != TakeoutDetails)
                                {
                                    foreach (var item in TakeoutDetails)
                                    {
                                        if (item.State != 3)
                                        {
                                            item.ClearReferences();

                                            item.State = 3;
                                            item.AdminId = client.AdminId;
                                            item.DeviceId = client.DeviceId;
                                            item.UpdateTime = UpdateTime;
                                            item.Mode = client.Mode;
                                            ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                        }
                                    }
                                }
                                    // 新增支付
                                    foreach (var item in TakeoutPays)
                                    {
                                        if (item.State != 3)
                                        {

                                            // 更新里面的余额信息
                                            lock (Resources.GetRes().BALANCES)
                                            {

                                                if (null != item.MemberId)
                                                {
                                                    // 如果会员信息存在, 则更新会员信息
                                                    Member payMember = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.tb_member.MemberId).FirstOrDefault();

                                                    item.tb_member = payMember;
                                                    payMember.BalancePrice = Math.Round(payMember.BalancePrice + item.Price, 2);
                                                    payMember.FavorablePrice = Math.Round(payMember.FavorablePrice - item.RemovePrice, 2); // 增加了账单里的优惠给会员
                                                    payMember.SpendPrice = Math.Round(payMember.SpendPrice - item.Price, 2);


                                                    payMember.UpdateTime = UpdateTime;

                                                    payMember.ClearReferences();


                                                    ctx.Entry(payMember).State = System.Data.Entity.EntityState.Modified;
                                                }


                                                

                                                if (null != item.BalanceId)
                                                {

                                                item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                                item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                                    item.BalancePrice = item.tb_balance.BalancePrice;
                                                    //item.tb_balance.UpdateTime = UpdateTime;

                                                    item.tb_balance.ClearReferences();
                                                    ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                                }
                                            }

                                            item.ClearReferences();

                                            item.State = 3;
                                            item.AdminId = client.AdminId;
                                            item.DeviceId = client.DeviceId;
                                            item.AddTime = UpdateTime;
                                            item.Mode = client.Mode;
                                            ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                            DelTakeoutPays.Add(item);
                                        }
                                    }

                                }
                            

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");


                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                        lock (item.Value)
                            item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                    }



                    if (model.State == 1)
                    {
                        foreach (var item in TakeoutPays)
                        {
                            if (null != item.MemberId)
                            {
                                // 更新会员余额
                                Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                                memberPay.BalancePrice = Math.Round(memberPay.BalancePrice + item.Price, 2);

                                memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice - item.RemovePrice, 2);
                                memberPay.SpendPrice = Math.Round(memberPay.SpendPrice - item.Price, 2);
                            }

                            // 更新里面的余额信息
                            if (null != item.BalanceId)
                            {
                                lock (Resources.GetRes().BALANCES)
                                {
                                    if (null != item.tb_balance)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                        item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                    }
                                }
                            }
                        }
                    }
                    else if (model.State == 2)
                    {
                        foreach (var item in DelTakeoutPays)
                        {
                            if (null != item.MemberId)
                            {
                                // 更新会员余额
                                Member memberPay = Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.MemberId).FirstOrDefault();
                                memberPay.BalancePrice = Math.Round(memberPay.BalancePrice - item.Price, 2);

                                memberPay.FavorablePrice = Math.Round(memberPay.FavorablePrice + item.RemovePrice, 2);
                                memberPay.SpendPrice = Math.Round(memberPay.SpendPrice + item.Price, 2);
                            }

                            // 更新里面的余额信息
                            if (null != item.BalanceId)
                            {
                                lock (Resources.GetRes().BALANCES)
                                {
                                    if (null != item.tb_balance)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                        item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                    }
                                }
                            }
                        }
                    }



                    return new ToClientServiceEditTakeout() { ExceptionType = ServiceExceptionType.DataFaild };
                }



                if (Result)
                {
                    // 貌似返回的就是正常的, 所以暂时不需要 Takeout FinishedOrder = null;
                    TakeoutModel takeoutModel = Resources.GetRes().TAKEOUT_Model.Where(x => null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId && null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId).FirstOrDefault();
                    if (!toServer.Rechecked && null != takeoutModel)
                    {
                        ICollection<TakeoutDetail> detailsOld = null;
                        ICollection<TakeoutPay> paysOld = null;

                        if (null != takeoutModel.Takeout)
                        {
                            detailsOld = takeoutModel.Takeout.tb_takeoutdetail;
                            paysOld = takeoutModel.Takeout.tb_takeoutpay;
                        }

                        takeoutModel.Takeout = model;

                        if (null != detailsOld)
                            takeoutModel.Takeout.tb_takeoutdetail = detailsOld;
                        if (null != paysOld)
                            takeoutModel.Takeout.tb_takeoutpay = paysOld;

                        takeoutModel.TakeoutSession = session = Guid.NewGuid().ToString();

                        // 1结账,2取消
                        if (takeoutModel.Takeout.State == 1 || takeoutModel.Takeout.State == 2)
                        {
                            // 如果会员信息存在, 则更新会员信息
                            if (takeoutModel.Takeout.State == 1 && null != member)
                            {
                                Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault());
                                Resources.GetRes().MEMBERS.Add(member);
                                takeoutModel.Takeout.tb_member = model.tb_member = member;

                                
                            }
                        }
                    }

                    if (model.State == 1)
                    {
                        foreach (var item in TakeoutPays)
                        {
                            if (null != item.tb_member)
                            {
                                Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == item.tb_member.MemberId).FirstOrDefault());
                                Resources.GetRes().MEMBERS.Add(item.tb_member);
                            }

                        }
                    }

                    // 记录日志
                    if (Result)
                        OperateLog.Instance.AddRecord(model.TakeoutId, null, "Takeout#" + OperateType.Edit, model.State, toServer.SessionId, toServer);
                    else
                        OperateLog.Instance.AddRecord(model.TakeoutId, null, "Takeout#" + OperateType.Edit, toServer.SessionId, toServer);

                    if (null != takeoutModel)
                        NotificateToServiceUpdateTakeout(JsonConvert.SerializeObject(new List<TakeoutNotificationModel>() { new TakeoutNotificationModel() { Takeout = takeoutModel.Takeout, TakeoutSessionId = takeoutModel.TakeoutSession, ProductsChange = productsChange } }), toServer.SessionId);
                }


                //返回它
                return new ToClientServiceEditTakeout() { Result = Result, TakeoutStateSession = session, Takeout = JsonConvert.SerializeObject(model), UpdateTime = UpdateTime };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit takeout operation failed.");
                //返回它
                return new ToClientServiceEditTakeout() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        /// <summary>
        /// 查找外卖
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetTakeouts ServiceGetTakeout(ToServerServiceGetTakeouts toServer)
        {
            try
            {
                bool Result = false;
                List<Takeout> Takeouts = new List<Takeout>();

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetTakeouts() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetTakeouts() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    if (toServer.IsFromCacheOnly)
                    {
                        foreach (var item in Resources.GetRes().TAKEOUT_Model)
                        {
                            Takeouts.Add(item.Takeout);
                        }
                        Result = true;
                    }
                    else
                    {
                        using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                        {
                            ctx.Configuration.ProxyCreationEnabled = false;

                            IQueryable<Takeout> Statement = null;


                            if (toServer.IsIncludeRef)
                                Statement = ctx.Takeouts.Include("tb_member").Include("tb_takeoutdetail").Include("tb_takeoutpay");
                            else
                                Statement = ctx.Takeouts.Include("tb_member");

                            if (!string.IsNullOrWhiteSpace(toServer.Phone))
                                Statement = Statement.Where(x => x.Phone.Contains(toServer.Phone));

                            if (!string.IsNullOrWhiteSpace(toServer.Name))
                                Statement = Statement.Where(x => x.Name0.Contains(toServer.Name) || x.Name1.Contains(toServer.Name) || x.Name2.Contains(toServer.Name));

                            if (toServer.AddTimeStart > 0)
                                Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            if (toServer.FinishTime > 0)
                                Statement = Statement.Where(x => x.FinishTime <= toServer.FinishTime);

                            if (toServer.AdminId > 0)
                                Statement = Statement.Where(x => x.AdminId == toServer.AdminId);
                            if (toServer.FinishAdminId > 0)
                                Statement = Statement.Where(x => x.FinishAdminId == toServer.FinishAdminId);

                            if (!string.IsNullOrWhiteSpace(toServer.MemberNo))
                            {
                                Member member = Resources.GetRes().MEMBERS.Where(x => x.MemberNo == toServer.MemberNo).FirstOrDefault();
                                if (null == member)
                                {
                                    return new ToClientServiceGetTakeouts() { Takeouts = JsonConvert.SerializeObject(new List<Takeout>()), Result = true };
                                }

                                Statement = Statement.Where(x => x.MemberId == member.MemberId);
                            }
                            if (!string.IsNullOrWhiteSpace(toServer.CardNo))
                            {
                                Member member = Resources.GetRes().MEMBERS.Where(x => x.CardNo == toServer.CardNo).FirstOrDefault();
                                if (null == member)
                                {
                                    return new ToClientServiceGetTakeouts() { Takeouts = JsonConvert.SerializeObject(new List<Takeout>()), Result = true };
                                }

                                Statement = Statement.Where(x => x.MemberId == member.MemberId);
                            }

                            if (toServer.State > -1)
                                Statement = Statement.Where(x => x.State == toServer.State);

                            if (toServer.SendAdminId > 0)
                                Statement = Statement.Where(x => x.SendAdminId == toServer.SendAdminId);

                            foreach (var item in Statement.ToList())
                            {
                                Takeouts.Add(item);
                            }
                            Result = true;

                        }
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetTakeouts() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetTakeouts() { Takeouts = JsonConvert.SerializeObject(Takeouts), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get takeout operation failed.");
                //返回它
                return new ToClientServiceGetTakeouts() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Takeout


        #region TakeoutDetail

        /// <summary>
        /// 修改外卖明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSaveTakeoutDetail ServiceSaveTakeoutDetail(ToServerServiceSaveTakeoutDetail toServer)
        {
            throw new NotSupportedException("This method too old for now, it hasn't been used for a long time, need to renew and test first!");
            try
            {
                bool Result = false;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceSaveTakeoutDetail() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceSaveTakeoutDetail() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Takeout model = toServer.Takeout.DeserializeObject<Takeout>();
                List<TakeoutDetail> detailsAdd = toServer.TakeoutDetailsAdd.DeserializeObject<List<TakeoutDetail>>();
                List<TakeoutDetail> detailsEdit = toServer.TakeoutDetailsEdit.DeserializeObject<List<TakeoutDetail>>();
                List<TakeoutDetail> detailsConfirm = toServer.TakeoutDetailsConfirm.DeserializeObject<List<TakeoutDetail>>();

                //查找包厢能否使用(包厢状态不同说明模型有改动, 包厢已有订单,说明还未结账或模型改动)
                if (Resources.GetRes().TAKEOUT_Model.Any(x => (null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId && x.TakeoutSession != toServer.TakeoutStateSession)))//|| (x.RoomId == model.RoomId && null != x.Order))
                {
                    return new ToClientServiceSaveTakeoutDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                }



                // 先检查产品数量是否少于它要购买的数量
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();
                // 新增的部分
                foreach (var item in detailsAdd)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1)
                    {
                        if (product.BalanceCount < item.Count)
                        {
                            // 如果有父级
                            if (null != product.ProductParentId)
                            {
                                Product productParent = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                if (null != productParent && productParent.IsBindCount == 1)
                                {
                                    double ParentRemove = 0;
                                    double ProductAdd = 0;

                                    lock (product)
                                    {
                                        double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); 
                                        ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                        ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                        ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 
                                    }

                                    // 从父级中去掉
                                    lock (productParent)
                                        productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);

                                    if (productsChange.ContainsKey(productParent.ProductId))
                                        productsChange[productParent.ProductId].CountChange = Math.Round(productsChange[productParent.ProductId].CountChange - ParentRemove, 3);
                                    else
                                        productsChange.Add(productParent.ProductId, new ProductWithCount() { Product = productParent, CountChange = -ParentRemove });


                                    // 给产品增加零的
                                    lock (product)
                                        product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);

                                    if (productsChange.ContainsKey(product.ProductId))
                                        productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + ProductAdd, 3);
                                    else
                                        productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = ProductAdd });

                                }
                            }
                        }

                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - item.Count, 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = -item.Count });
                    }
                }
                // 编辑的部分
                foreach (var item in detailsEdit)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1)
                    {
                        TakeoutDetail oldDetail = Resources.GetRes().TAKEOUT_Model.Where(x => null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId).FirstOrDefault().Takeout.tb_takeoutdetail.Where(x => x.TakeoutDetailId == item.TakeoutDetailId).FirstOrDefault();


                        if (product.BalanceCount < -oldDetail.Count + item.Count)
                        {
                            // 如果有父级
                            if (null != product.ProductParentId)
                            {
                                Product productParent = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                if (null != productParent && productParent.IsBindCount == 1)
                                {
                                    double ParentRemove = 0;
                                    double ProductAdd = 0;

                                    lock (product)
                                    {
                                        double NeedChangeFromParent = Math.Round((-oldDetail.Count + item.Count) - product.BalanceCount, 3);
                                        ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                        ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                        ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 
                                    }

                                    // 从父级中去掉
                                    lock (productParent)
                                        productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);

                                    if (productsChange.ContainsKey(productParent.ProductId))
                                        productsChange[productParent.ProductId].CountChange = Math.Round(productsChange[productParent.ProductId].CountChange - ParentRemove, 3);
                                    else
                                        productsChange.Add(productParent.ProductId, new ProductWithCount() { Product = productParent, CountChange = -ParentRemove });


                                    // 给产品增加零的
                                    lock (product)
                                        product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);

                                    if (productsChange.ContainsKey(product.ProductId))
                                        productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + ProductAdd, 3);
                                    else
                                        productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = ProductAdd });

                                }
                            }

                        }
                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount - (-oldDetail.Count + item.Count), 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - (-oldDetail.Count + item.Count), 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = -(-oldDetail.Count + item.Count) });
                    }
                }
                // 确认的部分
                foreach (var item in detailsConfirm)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1)
                    {
                        if (product.BalanceCount < item.Count)
                        {
                            // 如果有父级
                            if (null != product.ProductParentId)
                            {
                                Product productParent = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                if (null != productParent && productParent.IsBindCount == 1)
                                {
                                    double ParentRemove = 0;
                                    double ProductAdd = 0;

                                    lock (product)
                                    {
                                        double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3);
                                        ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3);
                                        ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                        ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3);
                                    }

                                    // 从父级中去掉
                                    lock (productParent)
                                        productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);

                                    if (productsChange.ContainsKey(productParent.ProductId))
                                        productsChange[productParent.ProductId].CountChange = Math.Round(productsChange[productParent.ProductId].CountChange - ParentRemove, 3);
                                    else
                                        productsChange.Add(productParent.ProductId, new ProductWithCount() { Product = productParent, CountChange = -ParentRemove });


                                    // 给产品增加零的
                                    lock (product)
                                        product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);

                                    if (productsChange.ContainsKey(product.ProductId))
                                        productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + ProductAdd);
                                    else
                                        productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = ProductAdd });

                                }
                            }
                        }
                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - item.Count, 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = -item.Count });
                    }
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                long UpdateTime = 0;

                try
                {
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

                            model.ClearReferences();

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;


                            foreach (var item in detailsAdd)
                            {
                                item.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                item.DeviceId = client.DeviceId;
                                item.AdminId = client.AdminId;
                                item.Mode = client.Mode;

                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Added;

                                
                            }

                            foreach (var item in detailsEdit)
                            {
                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            }

                            foreach (var item in detailsConfirm)
                            {
                                item.ClearReferences();

                                item.ConfirmAdminId = client.AdminId;
                                item.ConfirmDeviceId = client.DeviceId;
                                item.ConfirmTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                            }

                            UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            foreach (var item in productsChange)
                            {
                                item.Value.Product.ClearReferences();

                                item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                item.Value.Product.UpdateTime = UpdateTime;

                                ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                        lock (item.Value)
                            item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                    }

                    return new ToClientServiceSaveTakeoutDetail() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {
                    TakeoutModel takeoutModel = Resources.GetRes().TAKEOUT_Model.Where(x => null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId).FirstOrDefault();

                    ICollection<TakeoutDetail> detailsOld = null;
                    ICollection<TakeoutPay> paysOld = null;

                    if (null != takeoutModel.Takeout)
                    {
                        detailsOld = takeoutModel.Takeout.tb_takeoutdetail;
                        paysOld = takeoutModel.Takeout.tb_takeoutpay;
                    }

                    takeoutModel.Takeout = model;

                    if (null != detailsOld)
                        takeoutModel.Takeout.tb_takeoutdetail = detailsOld;
                    if (null != paysOld)
                        takeoutModel.Takeout.tb_takeoutpay = paysOld;

                    takeoutModel.TakeoutSession = session = Guid.NewGuid().ToString();

                    foreach (var item in detailsAdd)
                    {
                        takeoutModel.Takeout.tb_takeoutdetail.Add(item);
                    }

                    foreach (var item in detailsEdit)
                    {
                        takeoutModel.Takeout.tb_takeoutdetail.Remove(takeoutModel.Takeout.tb_takeoutdetail.Where(x => x.TakeoutDetailId == item.TakeoutDetailId).FirstOrDefault());
                        takeoutModel.Takeout.tb_takeoutdetail.Add(item);
                    }

                    foreach (var item in detailsConfirm)
                    {
                        takeoutModel.Takeout.tb_takeoutdetail.Remove(takeoutModel.Takeout.tb_takeoutdetail.Where(x => x.TakeoutDetailId == item.TakeoutDetailId).FirstOrDefault());
                        takeoutModel.Takeout.tb_takeoutdetail.Add(item);
                    }

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.TakeoutId, null, "TakeoutDetail#" + OperateType.Save, toServer.SessionId, toServer);


                    // 成功了给服务端发送更新订单通知
                    NotificateToServiceUpdateTakeout(JsonConvert.SerializeObject(new List<TakeoutNotificationModel>() { new TakeoutNotificationModel() { Takeout = takeoutModel.Takeout, TakeoutSessionId = takeoutModel.TakeoutSession, ProductsChange = productsChange} }), toServer.SessionId);
                }

                //返回它
                return new ToClientServiceSaveTakeoutDetail() {  Result = Result, TakeoutDetailAdd = JsonConvert.SerializeObject(detailsAdd), TakeoutDetailEdit = JsonConvert.SerializeObject(detailsEdit), TakeoutDetailConfirm = JsonConvert.SerializeObject(detailsConfirm), TakeoutSessionId = session, UpdateTime = UpdateTime };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Save takeout detail operation failed.");
                //返回它
                return new ToClientServiceSaveTakeoutDetail() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        //!!!!!!!!!!!!!!!!!!! 注意 !!!!!!!!!!!!!!!!!
        /// <summary>
        /// 删除外卖明细(万一以后用到这个所以保留了, 但记住, 跟Order一样要更改它为修改, 而不是删除)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelTakeoutDetail ServiceDelTakeoutDetail(ToServerServiceDelTakeoutDetail toServer)
        {
            try
            {
                bool Result = false;
                string session = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelTakeoutDetail() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelTakeoutDetail() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Takeout model = toServer.Takeout.DeserializeObject<Takeout>();
                List<TakeoutDetail> details = toServer.TakeoutDetails.DeserializeObject<List<TakeoutDetail>>();

                //查找包厢能否使用(包厢状态不同说明模型有改动, 包厢已有订单,说明还未结账或模型改动)
                if (Resources.GetRes().TAKEOUT_Model.Any(x => (null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId && x.TakeoutSession != toServer.TakeoutStateSession)))//|| (x.RoomId == model.RoomId && null != x.Order))
                {
                    return new ToClientServiceDelTakeoutDetail() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                }


                // 不能删除
                return new ToClientServiceDelTakeoutDetail() { ExceptionType = ServiceExceptionType.ServerFaild };

                // 先检查产品数量是否少于它要购买的数量
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();

                foreach (var item in details)
                {
                    Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                    if (product.IsBindCount == 1 && item.State != 1)
                    {
                        lock (product)
                            product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);

                        if (productsChange.ContainsKey(product.ProductId))
                            productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange + item.Count, 3);
                        else
                            productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = item.Count });

                    }
                }

                long UpdateTime = 0;

                try
                {
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

                            model.ClearReferences();

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            

                            foreach (var item in details)
                            {
                                item.ClearReferences();

                                ctx.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                            }

                            UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            foreach (var item in productsChange)
                            {
                                item.Value.Product.ClearReferences();

                                item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                item.Value.Product.UpdateTime = UpdateTime;

                                ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                        lock (item.Value)
                            item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                    }

                    return new ToClientServiceDelTakeoutDetail() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {
                    TakeoutModel takeoutModel = Resources.GetRes().TAKEOUT_Model.Where(x => null != x.Takeout && x.Takeout.TakeoutId == model.TakeoutId).FirstOrDefault();

                    ICollection<TakeoutDetail> detailsOld = null;
                    ICollection<TakeoutPay> paysOld = null;

                    if (null != takeoutModel.Takeout)
                    {
                        detailsOld = takeoutModel.Takeout.tb_takeoutdetail;
                        paysOld = takeoutModel.Takeout.tb_takeoutpay;
                    }

                    takeoutModel.Takeout = model;

                    if (null != detailsOld)
                        takeoutModel.Takeout.tb_takeoutdetail = detailsOld;
                    if (null != paysOld)
                        takeoutModel.Takeout.tb_takeoutpay = paysOld;

                    takeoutModel.TakeoutSession = session = Guid.NewGuid().ToString();

                    foreach (var item in details)
                    {
                        takeoutModel.Takeout.tb_takeoutdetail.Remove(takeoutModel.Takeout.tb_takeoutdetail.Where(x => x.TakeoutDetailId == item.TakeoutDetailId).FirstOrDefault());
                    }

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.TakeoutId, null, "TakeoutDetail#" + OperateType.Delete, toServer.SessionId, toServer);

                    // 成功了给服务端发送更新订单通知
                    NotificateToServiceUpdateTakeout(JsonConvert.SerializeObject(new List<TakeoutNotificationModel>() { new TakeoutNotificationModel() { Takeout = takeoutModel.Takeout, TakeoutSessionId = takeoutModel.TakeoutSession, ProductsChange = productsChange} }), toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelTakeoutDetail() { Result = Result, TakeoutSessionId = session, UpdateTime = UpdateTime  };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete takeout detail operation failed.");
                //返回它
                return new ToClientServiceDelTakeoutDetail() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找外卖明细(按外卖)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetTakeoutDetail ServiceGetTakeoutDetail(ToServerServiceGetTakeoutDetail toServer)
        {
            try
            {
                bool Result = false;
                List<TakeoutDetail> TakeoutDetails;
                List<TakeoutPay> TakeoutPays;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetTakeoutDetail() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetTakeoutDetail() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        TakeoutDetails = ctx.TakeoutDetails.Where(x => x.TakeoutId == toServer.TakeoutId).ToList();
                        TakeoutPays = ctx.TakeoutPays.Include("tb_member").Where(x => x.TakeoutId == toServer.TakeoutId).ToList();
                        Result = true;

                    }

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetTakeoutDetail() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetTakeoutDetail() { TakeoutDetails = JsonConvert.SerializeObject(TakeoutDetails), TakeoutPays = JsonConvert.SerializeObject(TakeoutPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get takeout detail operation failed.");
                //返回它
                return new ToClientServiceGetTakeoutDetail() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找外卖支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetTakeoutPay ServiceGetTakeoutPay(ToServerServiceGetTakeoutPay toServer)
        {
            try
            {
                bool Result = false;
                List<TakeoutPay> TakeoutPays = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetTakeoutPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetTakeoutPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;


                        if (toServer.AddTimeStart != 0 && toServer.AddTimeEnd != 0)
                        {

                            IQueryable<TakeoutPay> Statement = ctx.TakeoutPays;

                            if (toServer.AddTimeStart > 0)
                                Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);


                            TakeoutPays = Statement.ToList();
                            Result = true;
                        }

                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetTakeoutPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetTakeoutPay() { TakeoutPays = JsonConvert.SerializeObject(TakeoutPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get takeout pay operation failed.");
                //返回它
                return new ToClientServiceGetTakeoutPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        #endregion TakeoutDetail


        #region ImportWithDetails

        /// <summary>
        /// 增加进货及明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceNewImport ServiceAddImportWithDetail(ToServerServiceNewImport toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceNewImport() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceNewImport() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Import model = toServer.Import.DeserializeObject<Import>();
                List<ImportDetail> details = toServer.ImportDetails.DeserializeObject<List<ImportDetail>>();
                List<ImportPay> pays = toServer.ImportPays.DeserializeObject<List<ImportPay>>();
                List<ImportPay> delPays = new List<ImportPay>();


                Supplier supplier = model.tb_supplier;

                // 判断供应商UpdateTime,以免覆盖
                if (null != supplier && Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == supplier.SupplierId).FirstOrDefault().UpdateTime != supplier.UpdateTime)
                {
                    return new ToClientServiceNewImport() { ExceptionType = ServiceExceptionType.UpdateRefModel };
                }


                if (toServer.Rechecked && model.State == 2)
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;
                        ((IObjectContextAdapter)ctx).ObjectContext.CommandTimeout = Resources.GetRes().TIME_OUT_SHORT;//无用
                        ctx.Configuration.AutoDetectChangesEnabled = false;
                        ctx.Configuration.ValidateOnSaveEnabled = false;

                        //查找该订单信息是否已修改过
                        Import ctxImportsWhereFirstOrDefault = ctx.Imports.Where(x => x.ImportId == model.ImportId).FirstOrDefault();
                        if (ctxImportsWhereFirstOrDefault.UpdateTime != model.UpdateTime)
                        {
                            return new ToClientServiceNewImport() { ExceptionType = ServiceExceptionType.RefreshSessionModel };
                        }

                        details = ctx.ImportDetails.Where(x => x.ImportId == model.ImportId).ToList();
                        pays = ctx.ImportPays.Where(x => x.ImportId == model.ImportId).ToList();
                    }
                }
               


                // 先检查产品数量是否少于它要购买的数量
                Dictionary<long, ProductWithCount> productsChange = new Dictionary<long, ProductWithCount>();

                if (null != details && model.State == 2)
                {
                    foreach (var item in details)
                    {
                        Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                        if (product.IsBindCount == 1 && item.State != 1 && item.State != 3)
                        {
                            lock (product)
                                product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);

                            if (productsChange.ContainsKey(product.ProductId))
                                productsChange[product.ProductId].CountChange = Math.Round(productsChange[product.ProductId].CountChange - item.Count, 3);
                            else
                                productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = item.Count });

                        }
                    }
                }
                else
                {
                    foreach (var item in details)
                    {
                        Product product = Resources.GetRes().PRODUCTS.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                        if (product.IsBindCount == 1 || item.TotalPrice != item.OriginalTotalPrice || item.SalePrice != item.OriginalSalePrice)
                        {
                            double? oldCostPrice = null;
                            double? oldPrice = null;
                            double? newCostPrice = null;
                            double? newPrice = null;

                            lock (product)
                            {
                                if (product.IsBindCount == 1)
                                    product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);

                                // 如果总支出价格和应该算出来的价格不一样, 则更改产品支出价格
                                if (item.TotalPrice != item.OriginalTotalPrice)
                                {
                                    oldCostPrice = product.CostPrice;
                                    newCostPrice = product.CostPrice = item.Price;
                                }

                                // 如果产品价格和原始价格不同, 则更改产品价格
                                if (item.SalePrice != item.OriginalSalePrice)
                                {
                                    oldPrice = product.Price;
                                    newPrice = product.Price = item.SalePrice;
                                }
                            }

                            if (productsChange.ContainsKey(product.ProductId))
                            {
                                ProductWithCount pChange = productsChange[product.ProductId];

                                if (product.IsBindCount == 1)
                                    pChange.CountChange = Math.Round(productsChange[product.ProductId].CountChange + item.Count, 3);
                                pChange.OldCostPrice = oldCostPrice;
                                pChange.OldPrice = oldPrice;
                                pChange.NewCostPrice = newCostPrice;
                                pChange.NewPrice = newPrice;
                            }
                            else
                            {
                                double count = 0;
                                if (product.IsBindCount == 1)
                                    count = item.Count;

                                productsChange.Add(product.ProductId, new ProductWithCount() { Product = product, CountChange = count, OldCostPrice = oldCostPrice, OldPrice = oldPrice, NewCostPrice = newCostPrice, NewPrice = newPrice });
                            }
                        }
                    }
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                long UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                try
                {
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

                            if (toServer.Rechecked && model.State == 2)
                            {

                               

                                model.ReCheckedCount = model.ReCheckedCount + 1;
                                model.UpdateTime = UpdateTime;

                                model.ClearReferences();

                                ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            }

                            else if (!toServer.Rechecked && model.State == 0)
                            {
                                model.AddTime = UpdateTime;

                                model.AdminId = client.AdminId;
                                model.DeviceId = client.DeviceId;
                                model.Mode = client.Mode;


                                model.ClearReferences();

                                ctx.Entry(model).State = System.Data.Entity.EntityState.Added;




                                foreach (var item in details)
                                {
                                    item.AdminId = client.AdminId;
                                    item.DeviceId = client.DeviceId;
                                    item.Mode = client.Mode;
                                    item.AddTime = UpdateTime;


                                    item.ClearReferences();

                                    ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                                }



                                // 如果供应商信息存在, 则更新会员信息
                                if (null != supplier)
                                {
                                    supplier.UpdateTime = UpdateTime;

                                    supplier.ClearReferences();


                                    ctx.Entry(supplier).State = System.Data.Entity.EntityState.Modified;

                                    model.SupplierId = supplier.SupplierId;

                                }

                                

                            }

                            if (!toServer.Rechecked && model.State == 0)
                            {

                                foreach (var item in productsChange)
                                {
                                    item.Value.Product.ClearReferences();

                                    item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                    item.Value.Product.UpdateTime = UpdateTime;

                                    //item.Value.Product.UpdateTime = UpdateTime;
                                    ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                                }
                                // 新增支付
                                foreach (var item in pays)
                                {
                                    // 如果会员信息存在, 则更新会员信息
                                    if (item.State == 0 && null != item.SupplierId)
                                    {
                                        // 更新会员余额
                                        Supplier supplierPay = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == item.SupplierId).FirstOrDefault();

                                        supplierPay.BalancePrice = Math.Round(supplierPay.BalancePrice - item.Price, 2);
                                        item.tb_supplier = supplierPay;
                                        item.BalancePrice = supplierPay.BalancePrice;
                                        supplierPay.FavorablePrice = Math.Round(supplierPay.FavorablePrice + item.RemovePrice, 2);
                                        supplierPay.SpendPrice = Math.Round(supplierPay.SpendPrice + item.Price, 2);
                                        supplierPay.UpdateTime = UpdateTime;

                                        supplierPay.ClearReferences();


                                        ctx.Entry(supplierPay).State = System.Data.Entity.EntityState.Modified;

                                    }
                                    if (item.State == 0 && null != item.BalanceId)
                                    {
                                        // 更新里面的余额信息
                                        lock (Resources.GetRes().BALANCES)
                                        {
                                            item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                            if (null != item.tb_balance)
                                            {
                                                item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                                item.BalancePrice = item.tb_balance.BalancePrice;
                                                //item.tb_balance.UpdateTime = UpdateTime;

                                                item.tb_balance.ClearReferences();
                                                ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                            }
                                        }
                                    }

                                    item.ClearReferences();

                                    item.AdminId = client.AdminId;
                                    item.DeviceId = client.DeviceId;
                                    item.AddTime = UpdateTime;
                                    item.Mode = client.Mode;
                                    ctx.Entry(item).State = System.Data.Entity.EntityState.Added;
                                }
                            }

                            else if (toServer.Rechecked && model.State == 2)
                            {

                                foreach (var item in productsChange)
                                {
                                    item.Value.Product.ClearReferences();

                                    item.Value.OriginalUpdateTime = item.Value.Product.UpdateTime;
                                    item.Value.Product.UpdateTime = UpdateTime;

                                    //item.Value.Product.UpdateTime = UpdateTime;
                                    ctx.Entry(item.Value.Product).State = System.Data.Entity.EntityState.Modified;
                                }


                                foreach (var item in details)
                                {
                                    if (item.State != 3)
                                    {
                                        item.ClearReferences();

                                        item.State = 3;
                                        item.AdminId = client.AdminId;
                                        item.DeviceId = client.DeviceId;
                                        item.UpdateTime = UpdateTime;
                                        item.Mode = client.Mode;
                                        ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                    }
                                }

                                // 新增支付
                                foreach (var item in pays)
                                {
                                    if (item.State != 3)
                                    {
                                        if (null != item.SupplierId)
                                        {
                                            // 如果会员信息存在, 则更新会员信息
                                            Supplier paySupplier = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == item.tb_supplier.SupplierId).FirstOrDefault();

                                            item.tb_supplier = paySupplier;
                                            paySupplier.BalancePrice = Math.Round(paySupplier.BalancePrice + item.Price, 2);
                                            paySupplier.FavorablePrice = Math.Round(paySupplier.FavorablePrice - item.RemovePrice, 2); // 增加了账单里的优惠给会员
                                            paySupplier.SpendPrice = Math.Round(paySupplier.SpendPrice - item.Price, 2);


                                            paySupplier.UpdateTime = UpdateTime;

                                            paySupplier.ClearReferences();


                                            ctx.Entry(paySupplier).State = System.Data.Entity.EntityState.Modified;
                                        }



                                       if (null != item.BalanceId)
                                        {
                                            // 更新里面的余额信息
                                            lock (Resources.GetRes().BALANCES)
                                            {
                                                item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                                if (null != item.tb_balance)
                                                {
                                                    item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                                    item.BalancePrice = item.tb_balance.BalancePrice;
                                                    //item.tb_balance.UpdateTime = UpdateTime;

                                                    item.tb_balance.ClearReferences();
                                                    ctx.Entry(item.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                                }
                                            }
                                        }

                                        item.ClearReferences();

                                        item.State = 3;
                                        item.AdminId = client.AdminId;
                                        item.DeviceId = client.DeviceId;
                                        item.AddTime = UpdateTime;
                                        item.Mode = client.Mode;
                                        ctx.Entry(item).State = System.Data.Entity.EntityState.Modified;
                                        delPays.Add(item);
                                    }


                                }
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    // 将数量恢复
                    foreach (var item in productsChange)
                    {
                        item.Value.Product.UpdateTime = item.Value.OriginalUpdateTime;
                        lock (item.Value)
                        {
                            if (item.Value.Product.IsBindCount == 1)
                            {
                                if (model.State == 0 && !toServer.Rechecked)
                                    item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                                else if (model.State == 2 && toServer.Rechecked)
                                    item.Value.Product.BalanceCount = Math.Round(item.Value.Product.BalanceCount - item.Value.CountChange, 3);
                            }
                            if (null != item.Value.OldCostPrice)
                                item.Value.Product.CostPrice = item.Value.OldCostPrice.Value;
                            if (null != item.Value.OldPrice)
                                item.Value.Product.Price = item.Value.OldPrice.Value;
                        }
                    }


                    if (model.State == 0 && !toServer.Rechecked)
                    {
                        foreach (var item in pays)
                        {
                            if (null != item.SupplierId)
                            {
                                // 更新会员余额
                                Supplier supplierPay = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == item.SupplierId).FirstOrDefault();
                                supplierPay.BalancePrice = Math.Round(supplierPay.BalancePrice + item.Price, 2);

                                supplierPay.FavorablePrice = Math.Round(supplierPay.FavorablePrice - item.RemovePrice, 2);
                                supplierPay.SpendPrice = Math.Round(supplierPay.SpendPrice - item.Price, 2);
                            }

                            // 更新里面的余额信息
                            if (null != item.BalanceId)
                            {
                                lock (Resources.GetRes().BALANCES)
                                {
                                    if (null != item.tb_balance)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                        item.tb_balance.BalancePrice = item.tb_balance.BalancePrice + item.Price;
                                    }
                                }
                            }
                        }
                    }
                    else if (model.State == 2 && toServer.Rechecked)
                    {
                        foreach (var item in delPays)
                        {
                            if (null != item.SupplierId)
                            {
                                // 更新会员余额
                                Supplier supplierPay = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == item.SupplierId).FirstOrDefault();
                                supplierPay.BalancePrice = Math.Round(supplierPay.BalancePrice - item.Price, 2);

                                supplierPay.FavorablePrice = Math.Round(supplierPay.FavorablePrice + item.RemovePrice, 2);
                                supplierPay.SpendPrice = Math.Round(supplierPay.SpendPrice + item.Price, 2);
                            }

                            // 更新里面的余额信息
                            if (null != item.BalanceId)
                            {
                                lock (Resources.GetRes().BALANCES)
                                {
                                    if (null != item.tb_balance)
                                    {
                                        item.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == item.BalanceId && (x.IsBind == 1)).FirstOrDefault();
                                        item.tb_balance.BalancePrice = item.tb_balance.BalancePrice - item.Price;
                                    }
                                }
                            }
                        }
                    }





                    return new ToClientServiceNewImport() { ExceptionType = ServiceExceptionType.DataFaild };
                }



                // 记录日志
                if (Result)
                {

                    // 如果会员信息存在, 则更新会员信息
                    if (null != supplier)
                    {
                        Resources.GetRes().SUPPLIERS.Remove(Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == supplier.SupplierId).FirstOrDefault());
                        Resources.GetRes().SUPPLIERS.Add(supplier);
                        model.tb_supplier = supplier;

                    }


                    if (model.State == 1)
                    {
                        foreach (var item in pays)
                        {
                            if (null != item.tb_supplier)
                            {
                                Resources.GetRes().SUPPLIERS.Remove(Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == item.tb_supplier.SupplierId).FirstOrDefault());
                                Resources.GetRes().SUPPLIERS.Add(item.tb_supplier);
                            }

                        }
                    }

                    if (Result)
                        OperateLog.Instance.AddRecord(model.ImportId, null, "Import#" + (model.State == 2 ? OperateType.Edit : OperateType.Add), model.State == 1 ? 2 : 2, toServer.SessionId, toServer);
                    else
                        OperateLog.Instance.AddRecord(model.ImportId, null, "Import#" + OperateType.Add, toServer.SessionId, toServer);

                    // 发送更新产品数量通知
                    NotificateToServiceProductCountUpdate(new ToClientServiceProductCountUpdateNotification() { ProductAndCounts = JsonConvert.SerializeObject(productsChange) }, toServer.SessionId);
                }


                //返回它
                return new ToClientServiceNewImport() { Result = Result, Import = JsonConvert.SerializeObject(model), ImportDetails = JsonConvert.SerializeObject(details), ImportPays = JsonConvert.SerializeObject(pays), UpdateTime = UpdateTime };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add import operation failed.");
                //返回它
                return new ToClientServiceNewImport() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 编辑进货
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditImport ServiceEditImport(ToServerServiceEditImport toServer)
        {
            throw new NotSupportedException("This method too old for now, it hasn't been used for a long time, need to renew and test first!");
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditImport() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }

                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditImport() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Import model = toServer.Import.DeserializeObject<Import>();

                if (!toServer.Rechecked)
                    return new ToClientServiceEditImport() { ExceptionType= ServiceExceptionType.ServerFaild };


                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();
                long UpdateTime = 0;

                try
                {
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


                            Import ctxImportsWhereFirstOrDefault = ctx.Imports.Where(x => x.ImportId == model.ImportId).FirstOrDefault();

                            //只能重新结账才能修改
                            if (ctxImportsWhereFirstOrDefault.UpdateTime != model.UpdateTime)
                            {
                                return new ToClientServiceEditImport() { ExceptionType = ServiceExceptionType.UpdateModel };
                            }


                            model.ReCheckedCount = ctxImportsWhereFirstOrDefault.ReCheckedCount + 1;

                            model.ClearReferences();

                            model.UpdateTime = UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                            ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;



                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    return new ToClientServiceEditImport() { ExceptionType = ServiceExceptionType.DataFaild };
                }



                if (Result)
                {

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.ImportId, null, "Import#" + OperateType.Edit, 2, toServer.SessionId, toServer);

                }

                //返回它
                return new ToClientServiceEditImport() { Result = Result, Import = JsonConvert.SerializeObject(model), UpdateTime = UpdateTime };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit import operation failed.");
                //返回它
                return new ToClientServiceEditImport() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 查找进货
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetImports ServiceGetImports(ToServerServiceGetImports toServer)
        {
            try
            {
                bool Result = false;
                List<Import> Imports;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetImports() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetImports() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        IQueryable<Import> Statement = null;


                        if (toServer.IsIncludeRef)
                            Statement = ctx.Imports.Include("tb_supplier").Include("tb_importdetail").Include("tb_importpay");
                        else
                            Statement = ctx.Imports.Include("tb_supplier");

                        if (toServer.ImportTimeStart > 0 && toServer.ImportTimeEnd > 0)
                            Statement = Statement.Where(x => x.ImportTime >= toServer.ImportTimeStart && x.ImportTime <= toServer.ImportTimeEnd);
                        if (toServer.AddTimeStart > 0 && toServer.AddTimeEnd > 0)
                            Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart && x.AddTime <= toServer.AddTimeEnd);

                        if (!string.IsNullOrWhiteSpace(toServer.SupplierNo))
                        {
                            Supplier supplier = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierNo == toServer.SupplierNo).FirstOrDefault();
                            if (null == supplier)
                            {
                                return new ToClientServiceGetImports() { Imports = JsonConvert.SerializeObject(new List<Import>()), Result = true };
                            }

                            Statement = Statement.Where(x => x.SupplierId == supplier.SupplierId);
                        }
                        if (!string.IsNullOrWhiteSpace(toServer.CardNo))
                        {
                            Supplier supplier = Resources.GetRes().SUPPLIERS.Where(x => x.CardNo == toServer.CardNo).FirstOrDefault();
                            if (null == supplier)
                            {
                                return new ToClientServiceGetImports() { Imports = JsonConvert.SerializeObject(new List<Import>()), Result = true };
                            }

                            Statement = Statement.Where(x => x.SupplierId == supplier.SupplierId);
                        }

                        Imports = Statement.ToList();

                        Result = true;

                    }

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetImports() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetImports() { Imports = JsonConvert.SerializeObject(Imports), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get import operation failed.");
                //返回它
                return new ToClientServiceGetImports() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 查找进货明细(按订单)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetImportDetail ServiceGeImportDetail(ToServerServiceGetImportDetail toServer)
        {
            try
            {
                bool Result = false;
                List<ImportDetail> ImportDetail;
                List<ImportPay> ImportPays;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetImportDetail() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetImportDetail() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        ImportDetail = ctx.ImportDetails.Where(x => x.ImportId == toServer.ImportId).ToList();
                        ImportPays = ctx.ImportPays.Include("tb_supplier").Where(x => x.ImportId == toServer.ImportId).ToList();
                        Result = true;

                    }

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetImportDetail() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetImportDetail() { ImportDetails = JsonConvert.SerializeObject(ImportDetail), ImportPays = JsonConvert.SerializeObject(ImportPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get import detail operation failed.");
                //返回它
                return new ToClientServiceGetImportDetail() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找进货支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetImportPay ServiceGetImportPay(ToServerServiceGetImportPay toServer)
        {
            try
            {
                bool Result = false;
                List<ImportPay> ImportPays = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetImportPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetImportPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;


                        if (toServer.AddTimeStart != 0 && toServer.AddTimeEnd != 0)
                        {

                            IQueryable<ImportPay> Statement = ctx.ImportPays;

                            if (toServer.AddTimeStart > 0)
                                Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);


                            ImportPays = Statement.ToList();
                            Result = true;
                        }

                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetImportPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetImportPay() { ImportPays = JsonConvert.SerializeObject(ImportPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get import pay operation failed.");
                //返回它
                return new ToClientServiceGetImportPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion ImportWithDetails


        #region Admin

        /// <summary>
        /// 新增管理员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddAdmin ServiceAddAdmin(ToServerServiceAddAdmin toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddAdmin() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddAdmin() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Admin model = toServer.Admin.DeserializeObject<Admin>();

                model.Password = "123456";
                model.Salt = "".GenereteRandomCode(32, 1);

                string no;
                try
                {
                    no = Res.Key.GetKeys().Encryption((model.Password + model.Salt).CreateMD5());
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Key encryption failed.");
                    return new ToClientServiceAddAdmin() { ExceptionType = ServiceExceptionType.KeyFaild };
                }

                model.Password = no;

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;

                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddAdmin() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.AdminId;
                    Resources.GetRes().ADMINS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.AdminId, null, "Admin#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Admin, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddAdmin() { Admin = JsonConvert.SerializeObject(model), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add admin operation failed.");
                //返回它
                return new ToClientServiceAddAdmin() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改管理员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditAdmin ServiceEditAdmin(ToServerServiceEditAdmin toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditAdmin() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditAdmin() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Admin model = toServer.Admin.DeserializeObject<Admin>();

                // Admin 更新日期不一致, 不能更新
                Admin serverModel = Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditAdmin() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        Admin oldAdmin = Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault();

                        model.Password = oldAdmin.Password;
                        model.Salt = oldAdmin.Salt;
                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;



                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditAdmin() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().ADMINS.Remove(Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault());
                    Resources.GetRes().ADMINS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.AdminId, null, "Admin#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Admin, OperateType = OperateType.Edit }, toServer.SessionId);

                    Client client = Resources.GetRes().Services.FirstOrDefault(x => x.AdminId == model.AdminId);
                    if (null != client)
                        Resources.GetRes().CloseService(client);
                }

                //返回它
                return new ToClientServiceEditAdmin() { Result = Result, Admin = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit admin operation failed.");
                //返回它
                return new ToClientServiceEditAdmin() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }





        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceResetAdmin ServiceResetAdmin(ToServerServiceResetAdmin toServer)
        {
            try
            {
                // 2秒等待免得不停发送请求
                System.Threading.Thread.Sleep(2000);

                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceResetAdmin() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceResetAdmin() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                Admin model = toServer.Admin.DeserializeObject<Admin>();

                // Admin 更新日期不一致, 不能更新
                Admin serverModel = Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceResetAdmin() { ExceptionType = ServiceExceptionType.UpdateModel };
                }


                string oldPWD = null;
                string oldSalt = null;

                if (null != serverModel.Salt)
                {
                    oldSalt = serverModel.Salt;
                }
                oldPWD = serverModel.Password;


                serverModel.Salt = "".GenereteRandomCode(32, 1);
                serverModel.Password = "123456" ;

                string no = "";
                try
                {
                    no = Res.Key.GetKeys().Encryption((serverModel.Password + serverModel.Salt).CreateMD5());
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Key encryption failed.");
                    return new ToClientServiceResetAdmin() { ExceptionType = ServiceExceptionType.KeyFaild };
                }

                


                try
                {
                    model = Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault();

                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        
                        model.Password = no;
                        model.Salt = serverModel.Salt;

                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));


                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;

                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (serverModel.Salt != oldSalt)
                    {
                        serverModel.Salt = oldSalt;
                        serverModel.Password = oldPWD;
                    }

                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceResetAdmin() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().ADMINS.Remove(Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault());
                    Resources.GetRes().ADMINS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.AdminId, null, "Admin#" + OperateType.Reset, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Admin, OperateType = OperateType.Edit }, toServer.SessionId);

                    Client client = Resources.GetRes().Services.FirstOrDefault(x => x.AdminId == model.AdminId);
                    if (null != client)
                        Resources.GetRes().CloseService(client);
                }

                //返回它
                return new ToClientServiceResetAdmin() { Result = Result, Admin = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Reset admin operation failed.");
                //返回它
                return new ToClientServiceResetAdmin() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelAdmin ServiceDelAdmin(ToServerServiceDelAdmin toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelAdmin() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelAdmin() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Admin model = toServer.Admin.DeserializeObject<Admin>();

                // Admin 更新日期不一致, 不能更新
                Admin serverModel = Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelAdmin() { ExceptionType = ServiceExceptionType.UpdateModel };
                }
                // 不能删除
                return new ToClientServiceDelAdmin() { ExceptionType = ServiceExceptionType.ServerFaild };
                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                         // 订单,进货, 会员中跟当前管理员相关的数据, 说明有关系, 不能删除
                        if (ctx.Orders.Any(x => x.AdminId == model.AdminId) || ctx.Imports.Any(x => x.AdminId == model.AdminId) || ctx.Takeouts.Any(x=>x.AdminId == model.AdminId) || ctx.OrderDetails.Any(x => x.AdminId == model.AdminId) || ctx.ImportDetails.Any(x => x.AdminId == model.AdminId) || ctx.TakeoutDetails.Any(x => x.AdminId == model.AdminId) || ctx.Members.Any(x => x.AdminId == model.AdminId))
                        {
                            return new ToClientServiceDelAdmin() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                        }

                        model.ClearReferences();

                        model.Password = Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault().Password;

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelAdmin() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().ADMINS.Remove(Resources.GetRes().ADMINS.Where(x => x.AdminId == model.AdminId).FirstOrDefault());

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.AdminId, null, "Admin#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Admin, OperateType = OperateType.Delete }, toServer.SessionId);

                    Client client = Resources.GetRes().Services.FirstOrDefault(x => x.AdminId == model.AdminId);
                    if (null != client)
                        Resources.GetRes().CloseService(client);
                }

                //返回它
                return new ToClientServiceDelAdmin() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete admin operation failed.");
                //返回它
                return new ToClientServiceDelAdmin() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Admin


        #region AdminPay


        /// <summary>
        /// 增加管理员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddAdminPay ServiceAddAdminPay(ToServerServiceAddAdminPay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddAdminPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddAdminPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Admin admin = toServer.Admin.DeserializeObject<Admin>();
                AdminPay adminPay = toServer.AdminPay.DeserializeObject<AdminPay>();


                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
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

                            adminPay.ClearReferences();
                            adminPay.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                            adminPay.AddAdminId = client.AdminId;
                            adminPay.DeviceId = client.DeviceId;
                            adminPay.Mode = client.Mode;


                            ctx.Entry(adminPay).State = System.Data.Entity.EntityState.Added;


                            lock (Resources.GetRes().BALANCES)
                            {
                                adminPay.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == adminPay.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                if (null != adminPay.tb_balance)
                                {
                                    adminPay.tb_balance.BalancePrice = adminPay.tb_balance.BalancePrice - adminPay.Price;
                                    adminPay.BalancePrice = adminPay.tb_balance.BalancePrice;
                                    adminPay.ParentBalancePrice = admin.BalancePrice;

                                    adminPay.tb_balance.ClearReferences();
                                    ctx.Entry(adminPay.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                }
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    lock (Resources.GetRes().BALANCES)
                    {
                        adminPay.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == adminPay.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                        if (null != adminPay.tb_balance)
                        {
                            adminPay.tb_balance.BalancePrice = adminPay.tb_balance.BalancePrice + adminPay.Price;
                        }
                    }
                    return new ToClientServiceAddAdminPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    // 记录日志
                    OperateLog.Instance.AddRecord(adminPay.AdminPayId, adminPay.AdminId, "AdminPay#" + OperateType.Add, adminPay.Price > 0 ? 2 : 1, toServer.SessionId, toServer);
                    
                }


                //返回它
                return new ToClientServiceAddAdminPay() { Result = Result, AdminPay = JsonConvert.SerializeObject(adminPay), Admin = JsonConvert.SerializeObject(admin) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add admin pay operation failed.");
                //返回它
                return new ToClientServiceAddAdminPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        /// <summary>
        /// 删除管理员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelAdminPay ServiceDelAdminPay(ToServerServiceDelAdminPay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelAdminPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelAdminPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Admin admin = toServer.Admin.DeserializeObject<Admin>();
                AdminPay adminPay = toServer.AdminPay.DeserializeObject<AdminPay>();



                // 不能删除
                return new ToClientServiceDelAdminPay() { ExceptionType = ServiceExceptionType.ServerFaild };

                try
                {
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

                            adminPay.ClearReferences();

                            ctx.Entry(adminPay).State = System.Data.Entity.EntityState.Deleted;



                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    return new ToClientServiceDelAdminPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {

                    // 记录日志
                    OperateLog.Instance.AddRecord(adminPay.AdminPayId, adminPay.AdminId, "AdminPay#" + OperateType.Delete, toServer.SessionId);
                    
                }

                //返回它
                return new ToClientServiceDelAdminPay() { Result = Result, Admin = JsonConvert.SerializeObject(admin) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete admin pay operation failed.");
                //返回它
                return new ToClientServiceDelAdminPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找管理员支付(按管理员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetAdminPay ServiceGetAdminPay(ToServerServiceGetAdminPay toServer)
        {
            try
            {
                bool Result = false;
                List<AdminPay> AdminPays;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetAdminPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetAdminPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        var Statement = ctx.AdminPays.AsQueryable();

                        if (toServer.AdminId > 0)
                            Statement = Statement.Where(x => x.AdminId == toServer.AdminId);

                        if (toServer.NotAdminId > 0)
                            Statement = Statement.Where(x => x.AdminId != toServer.NotAdminId);

                        if (toServer.AddAdminId > 0)
                            Statement = Statement.Where(x => x.AddAdminId == toServer.AddAdminId);


                        if (toServer.AddTimeStart > 0)
                            Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                        if (toServer.AddTimeEnd > 0)
                            Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);



                        AdminPays = Statement.ToList();
                        Result = true;

                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetAdminPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetAdminPay() { AdminPays = JsonConvert.SerializeObject(AdminPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get admin pay operation failed.");
                //返回它
                return new ToClientServiceGetAdminPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        #endregion AdminPay


        #region Balance

        /// <summary>
        /// 新增余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddBalance ServiceAddBalance(ToServerServiceAddBalance toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddBalance() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddBalance() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Balance model = toServer.Balance.DeserializeObject<Balance>();


                
                //查看数据是否已准备好
                if (Resources.GetRes().BALANCES.Any(x=>x.BalanceName0 == model.BalanceName0 || x.BalanceName1 == model.BalanceName1 || x.BalanceName2 == model.BalanceName2))
                {
                    return new ToClientServiceAddBalance() { IsBalanceExists = true };
                }


                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();


                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddBalance() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.BalanceId;
                    Resources.GetRes().BALANCES.Add(model);
                    OperateLog.Instance.RefreshBalanceHash();

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.BalanceId, null, "Balance#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model.FastCopy(false, true)), ModelType = ModelType.Balance, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddBalance() { Balance = JsonConvert.SerializeObject(model), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add balance operation failed.");
                //返回它
                return new ToClientServiceAddBalance() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditBalance ServiceEditBalance(ToServerServiceEditBalance toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditBalance() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditBalance() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Balance model = toServer.Balance.DeserializeObject<Balance>();

                // Balance 更新日期不一致, 不能更新
                Balance serverModel = Resources.GetRes().BALANCES.Where(x => x.BalanceId == model.BalanceId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditBalance() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                //判断是否已存在
                if (Resources.GetRes().BALANCES.Where(x => x.BalanceId != model.BalanceId && (x.BalanceName0.Equals(model.BalanceName0, StringComparison.OrdinalIgnoreCase) || x.BalanceName1.Equals(model.BalanceName1, StringComparison.OrdinalIgnoreCase) || x.BalanceName2.Equals(model.BalanceName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                {
                    return new ToClientServiceEditBalance() { IsBalanceExists = true };
                }



                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        
                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        // 修改前把当前的余额替换过去(如果存在)
                        lock (Resources.GetRes().BALANCES)
                        {
                            Balance OldBalance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == model.BalanceId).FirstOrDefault();

                            model.BalancePrice = OldBalance.BalancePrice;
                        }



                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;



                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditBalance() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().BALANCES.Remove(Resources.GetRes().BALANCES.Where(x => x.BalanceId == model.BalanceId).FirstOrDefault());
                    Resources.GetRes().BALANCES.Add(model);
                    OperateLog.Instance.RefreshBalanceHash();

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.BalanceId, null, "Balance#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model.FastCopy(false, true)), ModelType = ModelType.Balance, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditBalance() { Result = Result, Balance = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit balance operation failed.");
                //返回它
                return new ToClientServiceEditBalance() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelBalance ServiceDelBalance(ToServerServiceDelBalance toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelBalance() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelBalance() { ExceptionType = ServiceExceptionType.DataNotReady };
                }


                //解析出来
                Balance model = toServer.Balance.DeserializeObject<Balance>();

                // Balance 更新日期不一致, 不能更新
                Balance serverModel = Resources.GetRes().BALANCES.Where(x => x.BalanceId == model.BalanceId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelBalance() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                // 不能删除
                return new ToClientServiceDelBalance() { ExceptionType = ServiceExceptionType.ServerFaild };

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;


                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelBalance() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().BALANCES.Remove(Resources.GetRes().BALANCES.Where(x => x.BalanceId == model.BalanceId).FirstOrDefault());
                    OperateLog.Instance.RefreshBalanceHash();


                    // 记录日志
                    OperateLog.Instance.AddRecord(model.BalanceId, null, "Balance#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Balance, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelBalance() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete balance operation failed.");
                //返回它
                return new ToClientServiceDelBalance() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetBalance ServiceGetBalances(ToServerServiceGetBalance toServer)
        {
            try
            {
                bool Result = false;
                List<Balance> balances = new List<Balance>();

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetBalance() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetBalance() { ExceptionType = ServiceExceptionType.DataNotReady };
                }


                var Statement = Resources.GetRes().BALANCES.AsQueryable();

                if (toServer.BalanceId > 0)
                    Statement = Statement.Where(x => x.BalanceId == toServer.BalanceId);


                foreach (var item in Statement.OrderByDescending(x=>x.BalanceId).ToList())
                {
                    balances.Add(item);
                }
                Result = true;


                //返回它
                return new ToClientServiceGetBalance() { Balances = JsonConvert.SerializeObject(balances), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get balance operation failed.");
                //返回它
                return new ToClientServiceGetBalance() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Balance


        #region BalancePay


        /// <summary>
        /// 增加余额支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddBalancePay ServiceAddBalancePay(ToServerServiceAddBalancePay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddBalancePay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddBalancePay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                BalancePay balancePay = toServer.BalancePay.DeserializeObject<BalancePay>();
                Balance balance = null;
                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
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

                            balancePay.ClearReferences();


                            balancePay.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            balancePay.AdminId = client.AdminId;
                            balancePay.DeviceId = client.DeviceId;
                            balancePay.Mode = client.Mode;

                            lock (Resources.GetRes().BALANCES)
                            {
                                balancePay.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == balancePay.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                if (null != balancePay.tb_balance)
                                {
                                    balancePay.tb_balance.BalancePrice = balancePay.tb_balance.BalancePrice + balancePay.Price;
                                    balancePay.BalancePrice = balancePay.tb_balance.BalancePrice;

                                    balancePay.tb_balance.ClearReferences();
                                    ctx.Entry(balancePay.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                }
                            }

                            ctx.Entry(balancePay).State = System.Data.Entity.EntityState.Added;



                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    lock (Resources.GetRes().BALANCES)
                    {
                        balancePay.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == balancePay.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                        if (null != balancePay.tb_balance)
                        {
                            balancePay.tb_balance.BalancePrice = balancePay.tb_balance.BalancePrice - balancePay.Price;
                        }
                    }
                    return new ToClientServiceAddBalancePay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    if (null != balancePay.tb_balance)
                        balance = balancePay.tb_balance;

                    // 记录日志
                    OperateLog.Instance.AddRecord(balancePay.BalancePayId, null, "BalancePay#" + OperateType.Add, balancePay.Price >= 0 ? 1 : 2, toServer.SessionId, toServer);

                }

                //返回它
                return new ToClientServiceAddBalancePay() { Result = Result, BalancePay = JsonConvert.SerializeObject(balancePay), Balance = JsonConvert.SerializeObject(balance) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add balance pay operation failed.");
                //返回它
                return new ToClientServiceAddBalancePay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }






        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceTransferBalancePay ServiceTransferBalancePay(ToServerServiceTransferBalancePay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceTransferBalancePay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceTransferBalancePay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                BalancePay balancePay1 = toServer.BalancePay1.DeserializeObject<BalancePay>();
                BalancePay balancePay2 = toServer.BalancePay2.DeserializeObject<BalancePay>();
                Balance balance1 = null;
                Balance balance2 = null;
                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
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

                            balancePay1.ClearReferences();
                            balancePay2.ClearReferences();


                            balancePay1.AddTime = balancePay2.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            balancePay1.AdminId = balancePay2.AdminId = client.AdminId;
                            balancePay1.DeviceId = balancePay2.DeviceId = client.DeviceId;
                            balancePay1.Mode = balancePay2.Mode = client.Mode;
                            balancePay2.TransferMode = 1;

                            lock (Resources.GetRes().BALANCES)
                            {
                                balancePay1.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == balancePay1.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                if (null != balancePay1.tb_balance)
                                {
                                    balancePay1.tb_balance.BalancePrice = balancePay1.tb_balance.BalancePrice + balancePay1.Price;
                                    balancePay1.BalancePrice = balancePay1.tb_balance.BalancePrice;
                                    balancePay1.TransferId = balancePay2.BalanceId;

                                    balancePay1.tb_balance.ClearReferences();
                                    ctx.Entry(balancePay1.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                }


                                balancePay2.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == balancePay2.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                if (null != balancePay2.tb_balance)
                                {
                                    balancePay2.tb_balance.BalancePrice = balancePay2.tb_balance.BalancePrice + balancePay2.Price;
                                    balancePay2.BalancePrice = balancePay2.tb_balance.BalancePrice;
                                    balancePay2.TransferId = balancePay1.BalanceId;

                                    balancePay2.tb_balance.ClearReferences();
                                    ctx.Entry(balancePay2.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                }
                            }

                            ctx.Entry(balancePay1).State = System.Data.Entity.EntityState.Added;
                            ctx.Entry(balancePay2).State = System.Data.Entity.EntityState.Added;

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    lock (Resources.GetRes().BALANCES)
                    {
                        balancePay1.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == balancePay1.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                        if (null != balancePay1.tb_balance)
                        {
                            balancePay1.tb_balance.BalancePrice = balancePay1.tb_balance.BalancePrice - balancePay1.Price;
                        }

                        balancePay2.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == balancePay2.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                        if (null != balancePay2.tb_balance)
                        {
                            balancePay1.tb_balance.BalancePrice = balancePay2.tb_balance.BalancePrice - balancePay2.Price;
                        }
                    }
                    return new ToClientServiceTransferBalancePay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    if (null != balancePay1.tb_balance)
                        balance1 = balancePay1.tb_balance;

                    if (null != balancePay2.tb_balance)
                        balance2 = balancePay2.tb_balance;

                    // 记录日志
                    OperateLog.Instance.AddRecord(balancePay1.BalancePayId, null, "BalancePay#" + OperateType.Edit,  1 , toServer.SessionId, toServer);

                }

                //返回它
                return new ToClientServiceTransferBalancePay() { Result = Result, BalancePay1 = JsonConvert.SerializeObject(balancePay1), Balance1 = JsonConvert.SerializeObject(balance1), BalancePay2 = JsonConvert.SerializeObject(balancePay2), Balance2 = JsonConvert.SerializeObject(balance2) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Tranfer blance pay operation failed.");
                //返回它
                return new ToClientServiceTransferBalancePay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }





        /// <summary>
        /// 删除余额支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelBalancePay ServiceDelBalancePay(ToServerServiceDelBalancePay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelBalancePay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelBalancePay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                BalancePay balacePay = toServer.BalancePay.DeserializeObject<BalancePay>();


                // 不能删除
                return new ToClientServiceDelBalancePay() { ExceptionType = ServiceExceptionType.ServerFaild };

                try
                {
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

                            balacePay.ClearReferences();

                            ctx.Entry(balacePay).State = System.Data.Entity.EntityState.Deleted;



                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    return new ToClientServiceDelBalancePay() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {

                    // 记录日志
                    OperateLog.Instance.AddRecord(balacePay.BalancePayId, null, "BalancePay#" + OperateType.Delete, toServer.SessionId, toServer);
                }

                //返回它
                return new ToClientServiceDelBalancePay() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete balance pay operation failed.");
                //返回它
                return new ToClientServiceDelBalancePay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找余额支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetBalancePay ServiceGetBalancePay(ToServerServiceGetBalancePay toServer)
        {
            try
            {
                bool Result = false;
                List<BalancePay> BalancePays = null;
                List<OrderPay> OrderPays = null;
                List<TakeoutPay> TakeoutPays = null;
                List<MemberPay> MemberPays = null;
                List<SupplierPay> SupplierPays = null;
                List<AdminPay> AdminPays = null;
                List<ImportPay> ImportPays = null;
                

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetBalancePay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetBalancePay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        if (toServer.BalanceType == 0 || toServer.BalanceType == 2)
                        {
                            var Statement = ctx.BalancePays.AsQueryable();

                            if (toServer.AdminId > 0)
                                Statement = Statement.Where(x => x.AdminId == toServer.AdminId);
                            if (toServer.NotAdminId > 0)
                                Statement = Statement.Where(x => x.AdminId != toServer.NotAdminId);
                            if (toServer.BalanceId > 0)
                                Statement = Statement.Where(x => x.BalanceId == toServer.BalanceId);
                            if (toServer.AddTimeStart > 0)
                                Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            BalancePays = Statement.ToList();
                        }

                        if (toServer.BalanceType == 1 || toServer.BalanceType == 2)
                        {
                            IQueryable<OrderPay> OrderPayStatement = ctx.OrderPays.AsQueryable();

                            if (toServer.AdminId > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.AdminId == toServer.AdminId);
                            if (toServer.NotAdminId > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.AdminId != toServer.NotAdminId);
                            if (toServer.BalanceId > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.BalanceId == toServer.BalanceId);
                            if (toServer.AddTimeStart > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            OrderPays = OrderPayStatement.ToList();



                            IQueryable<TakeoutPay> TakeoutPayStatement = ctx.TakeoutPays.AsQueryable();

                            if (toServer.AdminId > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.AdminId == toServer.AdminId);
                            if (toServer.NotAdminId > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.AdminId != toServer.NotAdminId);
                            if (toServer.BalanceId > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.BalanceId == toServer.BalanceId);
                            if (toServer.AddTimeStart > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            TakeoutPays = TakeoutPayStatement.ToList();



                            IQueryable<MemberPay> MemberPayStatement = ctx.MemberPays.AsQueryable();

                            if (toServer.AdminId > 0)
                                MemberPayStatement = MemberPayStatement.Where(x => x.AdminId == toServer.AdminId);
                            if (toServer.NotAdminId > 0)
                                MemberPayStatement = MemberPayStatement.Where(x => x.AdminId != toServer.NotAdminId);
                            if (toServer.BalanceId > 0)
                                MemberPayStatement = MemberPayStatement.Where(x => x.BalanceId == toServer.BalanceId);
                            if (toServer.AddTimeStart > 0)
                                MemberPayStatement = MemberPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                MemberPayStatement = MemberPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            MemberPays = MemberPayStatement.ToList();



                            IQueryable<SupplierPay> SupplierPayStatement = ctx.SupplierPays.AsQueryable();

                            if (toServer.AdminId > 0)
                                SupplierPayStatement = SupplierPayStatement.Where(x => x.AdminId == toServer.AdminId);
                            if (toServer.NotAdminId > 0)
                                SupplierPayStatement = SupplierPayStatement.Where(x => x.AdminId != toServer.NotAdminId);
                            if (toServer.BalanceId > 0)
                                SupplierPayStatement = SupplierPayStatement.Where(x => x.BalanceId == toServer.BalanceId);
                            if (toServer.AddTimeStart > 0)
                                SupplierPayStatement = SupplierPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                SupplierPayStatement = SupplierPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            SupplierPays = SupplierPayStatement.ToList();


                            IQueryable<AdminPay> AdminPayStatement = ctx.AdminPays.AsQueryable();

                            if (toServer.AdminId > 0)
                                AdminPayStatement = AdminPayStatement.Where(x => x.AddAdminId == toServer.AdminId);
                            if (toServer.NotAdminId > 0)
                                AdminPayStatement = AdminPayStatement.Where(x => x.AddAdminId != toServer.NotAdminId);
                            if (toServer.BalanceId > 0)
                                AdminPayStatement = AdminPayStatement.Where(x => x.BalanceId == toServer.BalanceId);
                            if (toServer.AddTimeStart > 0)
                                AdminPayStatement = AdminPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                AdminPayStatement = AdminPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            AdminPays = AdminPayStatement.ToList();


                            IQueryable<ImportPay> ImportPayStatement = ctx.ImportPays.AsQueryable();

                            if (toServer.AdminId > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.AdminId == toServer.AdminId);
                            if (toServer.NotAdminId > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.AdminId != toServer.NotAdminId);
                            if (toServer.BalanceId > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.BalanceId == toServer.BalanceId);
                            if (toServer.AddTimeStart > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            ImportPays = ImportPayStatement.ToList();


                            

                        }

                        Result = true;

                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetBalancePay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetBalancePay() { BalancePays = JsonConvert.SerializeObject(BalancePays), OrderPays = JsonConvert.SerializeObject(OrderPays), TakeoutPays = JsonConvert.SerializeObject(TakeoutPays), MemberPays = JsonConvert.SerializeObject(MemberPays), SupplierPays = JsonConvert.SerializeObject(SupplierPays), AdminPays = JsonConvert.SerializeObject(AdminPays), ImportPays = JsonConvert.SerializeObject(ImportPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get balance pay operation failed.");
                //返回它
                return new ToClientServiceGetBalancePay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        #endregion BalancePay


        #region AdminLog

        /// <summary>
        /// 管理员日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddAdminLog ServiceAddAdminLog(ToServerServiceAddAdminLog toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddAdminLog() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddAdminLog() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                AdminLog model = toServer.AdminLog.DeserializeObject<AdminLog>();

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        model.AdminId = client.AdminId;
                        model.DeviceId = client.DeviceId;
                        model.Mode = client.Mode;

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddAdminLog() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.AdminLogId;

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.AdminLogId, null, "AdminLog#" + OperateType.Add, toServer.SessionId);
                    //不重要, 不用通知NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.AdminLog, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddAdminLog() { AdminLog = JsonConvert.SerializeObject(model), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add admin log operation failed.");
                //返回它
                return new ToClientServiceAddAdminLog() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改管理员日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditAdminLog ServiceEditAdminLog(ToServerServiceEditAdminLog toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditAdminLog() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditAdminLog() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                AdminLog model = toServer.AdminLog.DeserializeObject<AdminLog>();

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;



                        // AdminLog 更新日期不一致, 不能更新
                        if (ctx.AdminLogs.Any(x => x.AdminLogId == model.AdminLogId && x.UpdateTime != model.UpdateTime))
                        {
                            return new ToClientServiceEditAdminLog() { ExceptionType = ServiceExceptionType.UpdateModel };
                        }

                        model.ClearReferences();


                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));



                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditAdminLog() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    // 记录日志
                    OperateLog.Instance.AddRecord(model.AdminLogId, null, "AdminLog#" + OperateType.Edit, toServer.SessionId);
                    //不重要, 不用通知NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.AdminLog, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditAdminLog() { Result = Result, AdminLog = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit admin log operation failed.");
                //返回它
                return new ToClientServiceEditAdminLog() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除管理员日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelAdminLog ServiceDelAdminLog(ToServerServiceDelAdminLog toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelAdminLog() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelAdminLog() { ExceptionType = ServiceExceptionType.DataNotReady };
                }




                //解析出来
                AdminLog model = toServer.AdminLog.DeserializeObject<AdminLog>();

                // 不能删除
                return new ToClientServiceDelAdminLog() { ExceptionType = ServiceExceptionType.ServerFaild };

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        // AdminLog 更新日期不一致, 不能更新
                        if (ctx.AdminLogs.Any(x => x.AdminLogId == model.AdminLogId && x.UpdateTime != model.UpdateTime))
                        {
                            return new ToClientServiceDelAdminLog() { ExceptionType = ServiceExceptionType.UpdateModel };
                        }

                        model.ClearReferences();


                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelAdminLog() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    // 记录日志
                    OperateLog.Instance.AddRecord(model.AdminLogId, null, "AdminLog#" + OperateType.Delete, toServer.SessionId);
                    //不重要, 不用通知NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.AdminLog, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelAdminLog() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete admin log operation failed.");
                //返回它
                return new ToClientServiceDelAdminLog() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 查找管理员日志(按管理员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetAdminLog ServiceGetAdminLog(ToServerServiceGetAdminLog toServer)
        {
            try
            {
                bool Result = false;
                List<AdminLog> AdminLogs;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetAdminLog() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetAdminLog() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        var Statement = ctx.AdminLogs.AsQueryable();

                        if (toServer.AdminId > 0)
                            Statement = Statement.Where(x => x.AdminId == toServer.AdminId);
                        if (toServer.AddTimeStart > 0)
                            Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                        if (toServer.AddTimeEnd > 0)
                            Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);



                        AdminLogs = Statement.ToList();
                        Result = true;

                    }

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetAdminLog() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetAdminLog() { AdminLogs = JsonConvert.SerializeObject(AdminLogs), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get admin log operation failed.");
                //返回它
                return new ToClientServiceGetAdminLog() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion AdminLog


        #region Member

        /// <summary>
        /// 新增会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddMember ServiceAddMember(ToServerServiceAddMember toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddMember() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddMember() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Member model = toServer.Member.DeserializeObject<Member>();


                //// 获取所有会员中的序号最大的那个
                //Member lastMember = Resources.GetRes().MEMBERS.OrderByDescending(x => x.MemberNo).FirstOrDefault();
                //if (null == lastMember)
                //    model.MemberNo = "1000";
                //else
                //    model.MemberNo = (int.Parse(lastMember.MemberNo) + 1).ToString();



                // 判断已存在
                if (Resources.GetRes().MEMBERS.Any(x=>x.MemberNo == model.MemberNo))
                {
                    return new ToClientServiceAddMember() {  IsMemberExists = true };
                }
                if (Resources.GetRes().MEMBERS.Any(x => null!= x.CardNo && x.CardNo == model.CardNo))
                {
                    return new ToClientServiceAddMember() { IsCardExists = true };
                }
                if (Resources.GetRes().MEMBERS.Any(x => x.IsEnable == 1 && (null != x.Phone && x.Phone == model.Phone)))
                {
                    return new ToClientServiceAddMember() { IsCardExists = true };
                }


                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.AdminId = client.AdminId;
                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();


                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddMember() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.MemberId;
                    Resources.GetRes().MEMBERS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.MemberId, null, "Member#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Member, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddMember() { Member = JsonConvert.SerializeObject(model), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add member operation failed.");
                //返回它
                return new ToClientServiceAddMember() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditMember ServiceEditMember(ToServerServiceEditMember toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditMember() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditMember() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Member model = toServer.Member.DeserializeObject<Member>();

                // Member 更新日期不一致, 不能更新
                Member serverModel = Resources.GetRes().MEMBERS.Where(x => x.MemberId == model.MemberId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditMember() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                //判断是否已存在
                if (Resources.GetRes().MEMBERS.Where(x => x.MemberId != model.MemberId && (x.MemberNo.Equals(model.MemberNo, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                {
                    return new ToClientServiceEditMember() { IsMemberExists = true };
                }

                //判断是否已存在
                if (Resources.GetRes().MEMBERS.Where(x => null != x.CardNo && x.MemberId != model.MemberId && x.CardNo == model.CardNo).Count() > 0)
                {
                    return new ToClientServiceEditMember() { IsCardExists = true };
                }

                //判断是否已存在
                if (Resources.GetRes().MEMBERS.Where(x => x.IsEnable == 1 &&  null != x.Phone && x.MemberId != model.MemberId && x.Phone == model.Phone).Count() > 0)
                {
                    return new ToClientServiceEditMember() { IsCardExists = true };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        
                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;



                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditMember() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == model.MemberId).FirstOrDefault());
                    Resources.GetRes().MEMBERS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.MemberId, null, "Member#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Member, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditMember() { Result = Result, Member = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit member operation failed.");
                //返回它
                return new ToClientServiceEditMember() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelMember ServiceDelMember(ToServerServiceDelMember toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelMember() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelMember() { ExceptionType = ServiceExceptionType.DataNotReady };
                }


                

                //解析出来
                Member model = toServer.Member.DeserializeObject<Member>();

                // Member 更新日期不一致, 不能更新
                Member serverModel = Resources.GetRes().MEMBERS.Where(x => x.MemberId == model.MemberId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelMember() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                // 不能删除
                return new ToClientServiceDelMember() { ExceptionType = ServiceExceptionType.ServerFaild };

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        // 如果某订单或外卖有会员信息, 不能删除
                        if (ctx.Orders.Any(x => x.MemberId == model.MemberId) || ctx.Takeouts.Any(x => x.MemberId == model.MemberId) || ctx.MemberPays.Any(x => x.MemberId == model.MemberId))
                        {
                            return new ToClientServiceDelMember() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                        }

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelMember() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == model.MemberId).FirstOrDefault());

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.MemberId, null, "Member#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Member, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelMember() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete member operation failed.");
                //返回它
                return new ToClientServiceDelMember() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetMember ServiceGetMembers(ToServerServiceGetMember toServer)
        {
            try
            {
                bool Result = false;
                List<Member> members = new List<Member>();

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetMember() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetMember() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                var Statement = Resources.GetRes().MEMBERS.AsQueryable();

                if (toServer.MemberId > 0)
                    Statement = Statement.Where(x => x.MemberId == toServer.MemberId);


                if (!string.IsNullOrWhiteSpace(toServer.MemberNo))
                {
                    if (toServer.SingleMemberNo)
                    {
                        if (toServer.MemberId == -1)
                        {
                            Statement = Statement.Where(x => x.MemberNo == toServer.MemberNo || x.CardNo == toServer.MemberNo || x.Phone == toServer.MemberNo);
                        }
                        else
                        {
                            Statement = Statement.Where(x => x.MemberNo == toServer.MemberNo);
                        }
                    }
                    else
                    {
                        if (toServer.MemberId == -1)
                        {
                            Statement = Statement.Where(x => x.MemberNo.Contains(toServer.MemberNo) || (null != x.CardNo && x.CardNo.Contains(toServer.MemberNo)) || (null != x.Phone && x.Phone.Contains(toServer.MemberNo)));
                        }
                        else
                        {
                            Statement = Statement.Where(x => x.MemberNo.Contains(toServer.MemberNo));
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(toServer.CardNo))
                    Statement = Statement.Where(x => x.CardNo == toServer.CardNo);
                

                if (!string.IsNullOrWhiteSpace(toServer.Phone))
                    Statement = Statement.Where(x => x.Phone.Contains(toServer.Phone));

                if (!string.IsNullOrWhiteSpace(toServer.Name))
                    Statement = Statement.Where(x => x.MemberName0.Contains(toServer.Name) || x.MemberName1.Contains(toServer.Name) || x.MemberName2.Contains(toServer.Name));


                foreach (var item in Statement.OrderByDescending(x=>x.MemberId).Take(50).ToList())
                {
                    members.Add(item);
                }
                Result = true;


                //返回它
                return new ToClientServiceGetMember() { Members = JsonConvert.SerializeObject(members), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get member operation failed.");
                //返回它
                return new ToClientServiceGetMember() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Member


        #region MemberPay


        /// <summary>
        /// 增加会员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddMemberPay ServiceAddMemberPay(ToServerServiceAddMemberPay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddMemberPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddMemberPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Member member = toServer.Member.DeserializeObject<Member>();
                MemberPay memberPay = toServer.MemberPay.DeserializeObject<MemberPay>();

                // 如果发现用户被更新说明, 用户信息也许有误.
                Member serverModel = Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != member.UpdateTime)
                {
                    return new ToClientServiceAddMemberPay() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
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

                            member.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            memberPay.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            memberPay.AdminId = client.AdminId;
                            memberPay.DeviceId = client.DeviceId;
                            memberPay.Mode = client.Mode;

                            member.ClearReferences();

                            memberPay.ClearReferences();


                            ctx.Entry(member).State = System.Data.Entity.EntityState.Modified;

                            ctx.Entry(memberPay).State = System.Data.Entity.EntityState.Added;


                            lock (Resources.GetRes().BALANCES)
                            {
                                memberPay.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == memberPay.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                if (null != memberPay.tb_balance)
                                {
                                    memberPay.tb_balance.BalancePrice = memberPay.tb_balance.BalancePrice + memberPay.Price;
                                    memberPay.BalancePrice = memberPay.tb_balance.BalancePrice;
                                    memberPay.ParentBalancePrice = member.BalancePrice;

                                    memberPay.tb_balance.ClearReferences();
                                    ctx.Entry(memberPay.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                }
                            }

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    lock (Resources.GetRes().BALANCES)
                    {
                        memberPay.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == memberPay.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                        if (null != memberPay.tb_balance)
                        {
                            memberPay.tb_balance.BalancePrice = memberPay.tb_balance.BalancePrice - memberPay.Price;
                        }
                    }
                    return new ToClientServiceAddMemberPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault());
                    Resources.GetRes().MEMBERS.Add(member);

                    // 记录日志

                    OperateLog.Instance.AddRecord(memberPay.MemberPayId, memberPay.MemberId, "MemberPay#" + OperateType.Add, memberPay.Price >= 0 ? 1 : 2, toServer.SessionId, toServer);
                }

                

                //返回它
                return new ToClientServiceAddMemberPay() { Result = Result, MemberPay = JsonConvert.SerializeObject(memberPay), Member = JsonConvert.SerializeObject(member) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add member pay operation failed.");
                //返回它
                return new ToClientServiceAddMemberPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        /// <summary>
        /// 删除会员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelMemberPay ServiceDelMemberPay(ToServerServiceDelMemberPay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelMemberPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelMemberPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Member member = toServer.Member.DeserializeObject<Member>();
                MemberPay memberPay = toServer.MemberPay.DeserializeObject<MemberPay>();

                // Member 更新日期不一致, 不能更新
                Member serverModel = Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != member.UpdateTime)
                {
                    return new ToClientServiceDelMemberPay() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                // 不能删除
                return new ToClientServiceDelMemberPay() { ExceptionType = ServiceExceptionType.ServerFaild };

                try
                {
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

                            
                            member.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                            member.ClearReferences();

                            memberPay.ClearReferences();


                            ctx.Entry(member).State = System.Data.Entity.EntityState.Modified;

                            ctx.Entry(memberPay).State = System.Data.Entity.EntityState.Deleted;

                            

                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    return new ToClientServiceDelMemberPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {
                    Resources.GetRes().MEMBERS.Remove(Resources.GetRes().MEMBERS.Where(x => x.MemberId == member.MemberId).FirstOrDefault());
                    Resources.GetRes().MEMBERS.Add(member);

                    // 记录日志
                    OperateLog.Instance.AddRecord(memberPay.MemberPayId, member.MemberId, "MemberPay#" + OperateType.Delete, toServer.SessionId, toServer);
                }

                //返回它
                return new ToClientServiceDelMemberPay() { Result = Result, Member = JsonConvert.SerializeObject(member) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete member pay operation failed.");
                //返回它
                return new ToClientServiceDelMemberPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找会员支付(按会员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetMemberPay ServiceGetMemberPay(ToServerServiceGetMemberPay toServer)
        {
            try
            {
                bool Result = false;
                List<MemberPay> MemberPays = null;
                List<OrderPay> OrderPays = null;
                List<TakeoutPay> TakeoutPays = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetMemberPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetMemberPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        
                        if (toServer.BalanceType == 0 || toServer.BalanceType == 2)
                        {
                            var Statement = ctx.MemberPays.AsQueryable();

                            if (toServer.MemberId > 0)
                                Statement = Statement.Where(x => x.MemberId == toServer.MemberId);

                            if (toServer.AddAdminId > 0)
                                Statement = Statement.Where(x => x.AdminId == toServer.AddAdminId);


                            if (toServer.AddTimeStart > 0)
                                Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);



                            MemberPays = Statement.ToList();
                        }



                        if (toServer.BalanceType == 1 || toServer.BalanceType == 2)
                        {
                            IQueryable<OrderPay> OrderPayStatement = ctx.OrderPays.AsQueryable();

                            if (toServer.MemberId > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.MemberId == toServer.MemberId);
                            if (toServer.AddAdminId > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.AdminId == toServer.AddAdminId);
                            if (toServer.AddTimeStart > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                OrderPayStatement = OrderPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);

                            OrderPays = OrderPayStatement.ToList();



                            IQueryable<TakeoutPay> TakeoutPayStatement = ctx.TakeoutPays.AsQueryable();

                            if (toServer.MemberId > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.MemberId == toServer.MemberId);
                            if (toServer.AddAdminId > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.AdminId == toServer.AddAdminId);
                            if (toServer.AddTimeStart > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                TakeoutPayStatement = TakeoutPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);

                            TakeoutPays = TakeoutPayStatement.ToList();
                        }




                        Result = true;

                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetMemberPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetMemberPay() { MemberPays = JsonConvert.SerializeObject(MemberPays), OrderPays = JsonConvert.SerializeObject(OrderPays), TakeoutPays = JsonConvert.SerializeObject(TakeoutPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get member pay operation failed.");
                //返回它
                return new ToClientServiceGetMemberPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        #endregion MemberPay


        #region Supplier

        /// <summary>
        /// 新增供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddSupplier ServiceAddSupplier(ToServerServiceAddSupplier toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddSupplier() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddSupplier() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Supplier model = toServer.Supplier.DeserializeObject<Supplier>();


                //// 获取所有供应商中的序号最大的那个
                //Supplier lastSupplier = Resources.GetRes().SUPPLIERS.OrderByDescending(x => x.SupplierNo).FirstOrDefault();
                //if (null == lastSupplier)
                //    model.SupplierNo = "1000";
                //else
                //    model.SupplierNo = (int.Parse(lastSupplier.SupplierNo) + 1).ToString();




                //查看数据是否已准备好
                if (Resources.GetRes().SUPPLIERS.Any(x => x.SupplierNo == model.SupplierNo))
                {
                    return new ToClientServiceAddSupplier() { IsSupplierExists = true };
                }
                if (Resources.GetRes().SUPPLIERS.Any(x => null != x.CardNo && x.CardNo == model.CardNo))
                {
                    return new ToClientServiceAddSupplier() { IsCardExists = true };
                }


                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.AdminId = client.AdminId;
                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();


                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddSupplier() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.SupplierId;
                    Resources.GetRes().SUPPLIERS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.SupplierId, null, "Supplier#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Supplier, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddSupplier() { Supplier = JsonConvert.SerializeObject(model), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add supplier operation failed.");
                //返回它
                return new ToClientServiceAddSupplier() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditSupplier ServiceEditSupplier(ToServerServiceEditSupplier toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditSupplier() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditSupplier() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Supplier model = toServer.Supplier.DeserializeObject<Supplier>();

                // Supplier 更新日期不一致, 不能更新
                Supplier serverModel = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == model.SupplierId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditSupplier() { ExceptionType = ServiceExceptionType.UpdateModel };
                }


                //判断是否已存在
                if (Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId != model.SupplierId && (x.SupplierNo.Equals(model.SupplierNo, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                {
                    return new ToClientServiceEditSupplier() { IsSupplierExists = true };
                }
                if (Resources.GetRes().SUPPLIERS.Where(x => null != x.CardNo && x.SupplierId != model.SupplierId && x.CardNo == model.CardNo).Count() > 0)
                {
                    return new ToClientServiceEditSupplier() { IsCardExists = true };
                }


                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;


                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;



                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditSupplier() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().SUPPLIERS.Remove(Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == model.SupplierId).FirstOrDefault());
                    Resources.GetRes().SUPPLIERS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.SupplierId, null, "Supplier#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Supplier, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditSupplier() { Result = Result, Supplier = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit supplier operation failed.");
                //返回它
                return new ToClientServiceEditSupplier() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelSupplier ServiceDelSupplier(ToServerServiceDelSupplier toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelSupplier() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelSupplier() { ExceptionType = ServiceExceptionType.DataNotReady };
                }




                //解析出来
                Supplier model = toServer.Supplier.DeserializeObject<Supplier>();

                // Member 更新日期不一致, 不能更新
                Supplier serverModel = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == model.SupplierId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelSupplier() { ExceptionType = ServiceExceptionType.UpdateModel };
                }


                // 不能删除
                return new ToClientServiceDelSupplier() { ExceptionType = ServiceExceptionType.ServerFaild };


                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        // 如果某订单或外卖有会员信息, 不能删除
                        if (ctx.Imports.Any(x => x.SupplierId == model.SupplierId) || ctx.SupplierPays.Any(x => x.SupplierId == model.SupplierId))
                        {
                            return new ToClientServiceDelSupplier() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                        }

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelSupplier() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().SUPPLIERS.Remove(Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == model.SupplierId).FirstOrDefault());

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.SupplierId, null, "Supplier#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Supplier, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelSupplier() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete supplier operation failed.");
                //返回它
                return new ToClientServiceDelSupplier() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetSupplier ServiceGetSupplier(ToServerServiceGetSupplier toServer)
        {
            try
            {
                bool Result = false;
                List<Supplier> suppliers = new List<Supplier>();

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetSupplier() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetSupplier() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                var Statement = Resources.GetRes().SUPPLIERS.AsQueryable();

                if (toServer.SupplierId > 0)
                    Statement = Statement.Where(x => x.SupplierId == toServer.SupplierId);


                if (!string.IsNullOrWhiteSpace(toServer.SupplierNo))
                {
                    if (toServer.SingleSupplierNo)
                    {
                        if (toServer.SupplierId == -1)
                        {
                            Statement = Statement.Where(x => x.SupplierNo == toServer.SupplierNo || x.CardNo == toServer.SupplierNo || x.Phone == toServer.SupplierNo);
                        }
                        else
                        {
                            Statement = Statement.Where(x => x.SupplierNo == toServer.SupplierNo);
                        }
                    }
                    else
                    {
                        if (toServer.SupplierId == -1)
                        {
                            Statement = Statement.Where(x => x.SupplierNo.Contains(toServer.SupplierNo) || (null != x.CardNo && x.CardNo.Contains(toServer.SupplierNo)) || (null != x.Phone && x.Phone.Contains(toServer.SupplierNo)));
                        }
                        else
                        {
                            Statement = Statement.Where(x => x.SupplierNo.Contains(toServer.SupplierNo));
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(toServer.CardNo))
                    Statement = Statement.Where(x => x.CardNo == toServer.CardNo);


                if (!string.IsNullOrWhiteSpace(toServer.Phone))
                    Statement = Statement.Where(x => x.Phone.Contains(toServer.Phone));

                if (!string.IsNullOrWhiteSpace(toServer.Name))
                    Statement = Statement.Where(x => x.SupplierName0.Contains(toServer.Name) || x.SupplierName1.Contains(toServer.Name) || x.SupplierName2.Contains(toServer.Name));


                foreach (var item in Statement.OrderByDescending(x=>x.SupplierId).Take(50).ToList())
                {
                    suppliers.Add(item);
                }
                Result = true;


                //返回它
                return new ToClientServiceGetSupplier() { Suppliers = JsonConvert.SerializeObject(suppliers), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get supplier operation failed.");
                //返回它
                return new ToClientServiceGetSupplier() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Supplier


        #region SupplierPay


        /// <summary>
        /// 增加供应商支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddSupplierPay ServiceAddSupplierPay(ToServerServiceAddSupplierPay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddSupplierPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddSupplierPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Supplier supplier = toServer.Supplier.DeserializeObject<Supplier>();
                SupplierPay supplierPay = toServer.SupplierPay.DeserializeObject<SupplierPay>();

                // 如果发现用户被更新说明, 用户信息也许有误.
                Supplier serverModel = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == supplier.SupplierId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != supplier.UpdateTime)
                {
                    return new ToClientServiceAddSupplierPay() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                Client client = Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault();

                try
                {
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

                            supplier.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            supplierPay.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            supplierPay.AdminId = client.AdminId;
                            supplierPay.DeviceId = client.DeviceId;
                            supplierPay.Mode = client.Mode;

                            supplier.ClearReferences();

                            supplierPay.ClearReferences();

                            ctx.Entry(supplier).State = System.Data.Entity.EntityState.Modified;

                            ctx.Entry(supplierPay).State = System.Data.Entity.EntityState.Added;


                            lock (Resources.GetRes().BALANCES)
                            {
                                supplierPay.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == supplierPay.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                                if (null != supplierPay.tb_balance)
                                {
                                    supplierPay.tb_balance.BalancePrice = supplierPay.tb_balance.BalancePrice - supplierPay.Price;
                                    supplierPay.BalancePrice = supplierPay.tb_balance.BalancePrice;
                                    supplierPay.ParentBalancePrice = supplier.BalancePrice;

                                    supplierPay.tb_balance.ClearReferences();
                                    ctx.Entry(supplierPay.tb_balance).State = System.Data.Entity.EntityState.Modified;

                                }
                            }
                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    lock (Resources.GetRes().BALANCES)
                    {
                        supplierPay.tb_balance = Resources.GetRes().BALANCES.Where(x => x.BalanceId == supplierPay.BalanceId && (x.IsBind == 1)).FirstOrDefault();

                        if (null != supplierPay.tb_balance)
                        {
                            supplierPay.tb_balance.BalancePrice = supplierPay.tb_balance.BalancePrice + supplierPay.Price;
                        }
                    }

                    return new ToClientServiceAddSupplierPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().SUPPLIERS.Remove(Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == supplier.SupplierId).FirstOrDefault());
                    Resources.GetRes().SUPPLIERS.Add(supplier);


                    // 记录日志

                    OperateLog.Instance.AddRecord(supplierPay.SupplierPayId, supplierPay.SupplierId, "SupplierPay#" + OperateType.Add, supplierPay.Price > 0 ? 2 : 1, toServer.SessionId, toServer);
                }



                //返回它
                return new ToClientServiceAddSupplierPay() { Result = Result, SupplierPay = JsonConvert.SerializeObject(supplierPay), Supplier = JsonConvert.SerializeObject(supplier) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add supplier pay operation failed.");
                //返回它
                return new ToClientServiceAddSupplierPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        /// <summary>
        /// 删除供应商支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelSupplierPay ServiceDelSupplierPay(ToServerServiceDelSupplierPay toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelSupplierPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelSupplierPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Supplier supplier = toServer.Supplier.DeserializeObject<Supplier>();
                SupplierPay supplierPay = toServer.SupplierPay.DeserializeObject<SupplierPay>();

                // SupplierPay 更新日期不一致, 不能更新
                Supplier serverModel = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == supplier.SupplierId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != supplier.UpdateTime)
                {
                    return new ToClientServiceDelSupplierPay() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                // 不能删除
                return new ToClientServiceDelSupplierPay() { ExceptionType = ServiceExceptionType.ServerFaild };

                try
                {
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


                            supplier.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                            supplier.ClearReferences();

                            supplierPay.ClearReferences();


                            ctx.Entry(supplier).State = System.Data.Entity.EntityState.Modified;

                            ctx.Entry(supplierPay).State = System.Data.Entity.EntityState.Deleted;



                            ctx.SaveChanges();
                            scope.Complete();
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");

                    return new ToClientServiceDelSupplierPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }


                if (Result)
                {
                    Resources.GetRes().SUPPLIERS.Remove(Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == supplier.SupplierId).FirstOrDefault());
                    Resources.GetRes().SUPPLIERS.Add(supplier);

                    // 记录日志
                    OperateLog.Instance.AddRecord(supplierPay.SupplierPayId, supplierPay.SupplierId, "SupplierPay#" + OperateType.Delete, toServer.SessionId, toServer);
                    //NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(supplierPay), ModelRef = JsonConvert.SerializeObject(supplier), ModelType = ModelType.SupplierPay, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelSupplierPay() { Result = Result, Supplier = JsonConvert.SerializeObject(supplier) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete supplier pay operation failed.");
                //返回它
                return new ToClientServiceDelSupplierPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }




        /// <summary>
        /// 查找供应商支付(按供应商)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetSupplierPay ServiceGetSupplierPay(ToServerServiceGetSupplierPay toServer)
        {
            try
            {
                bool Result = false;
                List<SupplierPay> supplierPays = null;
                List<ImportPay> ImportPays = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetSupplierPay() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetSupplierPay() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        if (toServer.BalanceType == 0 || toServer.BalanceType == 2)
                        {
                            var Statement = ctx.SupplierPays.AsQueryable();

                            if (toServer.SupplierId > 0)
                                Statement = Statement.Where(x => x.SupplierId == toServer.SupplierId);

                            if (toServer.AddAdminId > 0)
                                Statement = Statement.Where(x => x.AdminId == toServer.AddAdminId);


                            if (toServer.AddTimeStart > 0)
                                Statement = Statement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                Statement = Statement.Where(x => x.AddTime <= toServer.AddTimeEnd);

                            supplierPays = Statement.ToList();
                        }


                        if (toServer.BalanceType == 1 || toServer.BalanceType == 2)
                        {
                            IQueryable<ImportPay> ImportPayStatement = ctx.ImportPays.AsQueryable();

                            if (toServer.SupplierId > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.SupplierId == toServer.SupplierId);
                            if (toServer.AddAdminId > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.AdminId == toServer.AddAdminId);
                            if (toServer.AddTimeStart > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                ImportPayStatement = ImportPayStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);

                            ImportPays = ImportPayStatement.ToList();

                            
                        }
                        Result = true;

                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetSupplierPay() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetSupplierPay() { SupplierPays = JsonConvert.SerializeObject(supplierPays), ImportPays = JsonConvert.SerializeObject(ImportPays), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get supplier pay operation failed.");
                //返回它
                return new ToClientServiceGetSupplierPay() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        #endregion SupplierPay


        #region Printer

        /// <summary>
        /// 新增打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddPrinter ServiceAddPrinter(ToServerServiceAddPrinter toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddPrinter() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddPrinter() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Printer model = toServer.Printer.DeserializeObject<Printer>();

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddPrinter() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.PrinterId;
                    Resources.GetRes().PRINTERS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.PrinterId, null, "Printer#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Printer, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddPrinter() { Printer = JsonConvert.SerializeObject(model), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add printer operation failed.");
                //返回它
                return new ToClientServiceAddPrinter() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditPrinter ServiceEditPrinter(ToServerServiceEditPrinter toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditPrinter() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditPrinter() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Printer model = toServer.Printer.DeserializeObject<Printer>();

                // Printer 更新日期不一致, 不能更新
                Printer serverModel = Resources.GetRes().PRINTERS.Where(x => x.PrinterId == model.PrinterId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditPrinter() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditPrinter() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().PRINTERS.Remove(Resources.GetRes().PRINTERS.Where(x => x.PrinterId == model.PrinterId).FirstOrDefault());
                    Resources.GetRes().PRINTERS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.PrinterId, null, "Printer#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Printer, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditPrinter() { Result = Result, Printer = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit printer operation failed.");
                //返回它
                return new ToClientServiceEditPrinter() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelPrinter ServiceDelPrinter(ToServerServiceDelPrinter toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelPrinter() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelPrinter() { ExceptionType = ServiceExceptionType.DataNotReady };
                }




                //解析出来
                Printer model = toServer.Printer.DeserializeObject<Printer>();

                // Printer 更新日期不一致, 不能更新
                Printer serverModel = Resources.GetRes().PRINTERS.Where(x => x.PrinterId == model.PrinterId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelPrinter() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                // 不能删除产品中用的产品类型
                if (Resources.GetRes().PPRS.Any(x => x.PrinterId == model.PrinterId))
                {
                    return new ToClientServiceDelPrinter() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                }

                
                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelPrinter() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().PRINTERS.Remove(Resources.GetRes().PRINTERS.Where(x => x.PrinterId == model.PrinterId).FirstOrDefault());
                    // 记录日志
                    OperateLog.Instance.AddRecord(model.PrinterId, null, "Printer#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Printer, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelPrinter() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete printer operation failed.");
                //返回它
                return new ToClientServiceDelPrinter() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Printer


        #region Device

        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddDevice ServiceAddDevice(ToServerServiceAddDevice toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddDevice() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddDevice() { ExceptionType = ServiceExceptionType.DataNotReady };
                }


                //解析出来
                Device model = toServer.Device.DeserializeObject<Device>();

                // 不能超出设备数量限制
                if (model.IsEnable == 1 && Resources.GetRes().DEVICES.Where(x => x.IsEnable == 1).Count() >= Resources.GetRes().SERVICE_COUNT)
                {
                    return new ToClientServiceAddDevice() { ExceptionType = ServiceExceptionType.DeviceCountOutOfLimit };
                }


                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddDevice() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.DeviceId;
                    Resources.GetRes().DEVICES.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.DeviceId, null, "Device#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Device, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddDevice() { Device = JsonConvert.SerializeObject(model) , Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add device operation failed.");
                //返回它
                return new ToClientServiceAddDevice() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditDevice ServiceEditDevice(ToServerServiceEditDevice toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditDevice() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditDevice() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Device model = toServer.Device.DeserializeObject<Device>();

                // 不能超出设备数量限制
                if (model.IsEnable == 1 && Resources.GetRes().DEVICES.Where(x => x.IsEnable == 1 && x.DeviceId != model.DeviceId).Count() >= Resources.GetRes().SERVICE_COUNT)
                {
                    return new ToClientServiceEditDevice() { ExceptionType = ServiceExceptionType.DeviceCountOutOfLimit };
                }

                // Device 更新日期不一致, 不能更新
                Device serverModel = Resources.GetRes().DEVICES.Where(x => x.DeviceId == model.DeviceId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditDevice() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;

                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditDevice() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().DEVICES.Remove(Resources.GetRes().DEVICES.Where(x => x.DeviceId == model.DeviceId).FirstOrDefault());
                    Resources.GetRes().DEVICES.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.DeviceId, null, "Device#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Device, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditDevice() { Result = Result, Device = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit device operation failed.");
                //返回它
                return new ToClientServiceEditDevice() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelDevice ServiceDelDevice(ToServerServiceDelDevice toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelDevice() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelDevice() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Device model = toServer.Device.DeserializeObject<Device>();

                // Device 更新日期不一致, 不能更新
                Device serverModel = Resources.GetRes().DEVICES.Where(x => x.DeviceId == model.DeviceId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelDevice() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        // 订单,进货, 会员中跟当前管理员相关的数据, 说明有关系, 不能删除
                        if (ctx.Orders.Any(x => x.DeviceId == model.DeviceId) || ctx.Imports.Any(x => x.DeviceId == model.DeviceId) || ctx.Takeouts.Any(x => x.DeviceId == model.DeviceId) || ctx.OrderDetails.Any(x => x.DeviceId == model.DeviceId) || ctx.ImportDetails.Any(x => x.DeviceId == model.DeviceId) || ctx.TakeoutDetails.Any(x => x.DeviceId == model.DeviceId))
                        {
                            return new ToClientServiceDelDevice() { ExceptionType = ServiceExceptionType.DataHasRefrence };
                        }

                        model.ClearReferences();

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;



                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelDevice() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().DEVICES.Remove(Resources.GetRes().DEVICES.Where(x => x.DeviceId == model.DeviceId).FirstOrDefault());

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.DeviceId, null, "Device#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Device, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelDevice() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete device operation failed.");
                //返回它
                return new ToClientServiceDelDevice() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Device


        #region Log

        /// <summary>
        /// 查找日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetLog ServiceGetLog(ToServerServiceGetLog toServer)
        {
            try
            {
                bool Result = false;
                List<Log> logs = new List<Log>();
                string balance = null;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceGetLog() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceGetLog() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                try
                {
                    // 如果要查历史订单
                    if (toServer.OperateId > 0)
                    {
                        // 先获取内存中的日志
                        var MemoryStatement = OperateLog.Instance.AccessLogs.AsQueryable();
                        
                        if (toServer.AddTimeStart > 0)
                            MemoryStatement = MemoryStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                        if (toServer.AddTimeEnd > 0)
                            MemoryStatement = MemoryStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                        if (toServer.OperateId > 0)
                            MemoryStatement = MemoryStatement.Where(x => x.OperateId == toServer.OperateId);
                        MemoryStatement = MemoryStatement.Where(x => x.OperateName.StartsWith("Order#") || x.OperateName.StartsWith("OrderDetail#"));

                        List<Log> memoryLogs = MemoryStatement.ToList(); 
                        if (memoryLogs.Count > 0)
                        {
                            foreach (var item in memoryLogs)
                            {
                                Log log = new Log();
                                log.AddTime = item.AddTime;
                                log.AdminId = item.AdminId;
                                log.Balance = item.Balance;
                                log.Other = item.Other;
                                log.BalanceType = item.BalanceType;
                                log.DeviceId = item.DeviceId;
                                log.IsBalanceChange = item.IsBalanceChange;
                                log.LogId = item.LogId;
                                log.OperateId = item.OperateId;
                                log.OperateName = item.OperateName;
                                log.Remark = item.Remark;
                                log.OperateSubId = item.OperateSubId;
                                log.Model = item.Model;

                                logs.Add(log);
                            }
                        }


                        using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                        {
                            ctx.Configuration.ProxyCreationEnabled = false;

                            var DBStatement = ctx.Logs.AsQueryable();

                            if (toServer.AddTimeStart > 0)
                                DBStatement = DBStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                DBStatement = DBStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);
                            if (toServer.OperateId > 0)
                                DBStatement = DBStatement.Where(x => x.OperateId == toServer.OperateId);
                            DBStatement = DBStatement.Where(x => x.OperateName.StartsWith("Order#") || x.OperateName.StartsWith("OrderDetail#"));



                            List<Log> dbLogs = DBStatement.ToList(); 
                            if (dbLogs.Count > 0)
                            {
                                foreach (var item in dbLogs)
                                {
                                    Log log = new Log();
                                    log.AddTime = item.AddTime;
                                    log.AdminId = item.AdminId;
                                    log.Balance = item.Balance;
                                    log.Other = item.Other;
                                    log.BalanceType = item.BalanceType;
                                    log.DeviceId = item.DeviceId;
                                    log.IsBalanceChange = item.IsBalanceChange;
                                    log.LogId = item.LogId;
                                    log.OperateId = item.OperateId;
                                    log.OperateName = item.OperateName;
                                    log.Remark = item.Remark;
                                    log.OperateSubId = item.OperateSubId;
                                    log.Model = item.Model;


                                    logs.Add(log);
                                }
                            }


                        }


                        Result = true;
                    }
                
                    // 只获取余额
                    else if (toServer.IsBalancePrice == 1)
                    {
                        balance = JsonConvert.SerializeObject(Resources.GetRes().BALANCES.Where(x => x.HideType != 1).ToList().Select(x =>
                        {
                            return x.FastCopy().ReChangeBalance();
                        }).ToList());

                        Result = true;
                    }
                    // 获取余额和日志
                    else if (toServer.IsBalancePrice == 2)
                    {

                        balance = JsonConvert.SerializeObject(Resources.GetRes().BALANCES.Where(x => x.HideType != 1).ToList().Select(x =>
                        {
                            return x.FastCopy().ReChangeBalance();
                        }).ToList());


                        // 先获取内存中的日志
                        var MemoryStatement = OperateLog.Instance.AccessLogs.AsQueryable();
                        if (toServer.IsBalanceChange != -1)
                        {
                            MemoryStatement = MemoryStatement.Where(x => x.IsBalanceChange == toServer.IsBalanceChange);
                        }

                        if (toServer.AddTimeStart > 0)
                            MemoryStatement = MemoryStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                        if (toServer.AddTimeEnd > 0)
                            MemoryStatement = MemoryStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);

                        List<Log> memoryLogs = MemoryStatement.ToList(); 
                        if (memoryLogs.Count > 0)
                        {
                            foreach (var item in memoryLogs)
                            {
                                Log log = new Log();
                                log.AddTime = item.AddTime;
                                log.AdminId = item.AdminId;
                                log.Balance = item.Balance;
                                log.Other = item.Other;
                                log.BalanceType = item.BalanceType;
                                log.DeviceId = item.DeviceId;
                                log.IsBalanceChange = item.IsBalanceChange;
                                log.LogId = item.LogId;
                                log.OperateId = item.OperateId;
                                log.OperateName = item.OperateName;
                                log.Remark = item.Remark;
                                log.OperateSubId = item.OperateSubId;

                                logs.Add(log);
                            }
                        }


                        using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                        {
                            ctx.Configuration.ProxyCreationEnabled = false;

                            var DBStatement = ctx.Logs.AsQueryable();
                            if (toServer.IsBalanceChange != -1)
                            {
                                DBStatement = DBStatement.Where(x => x.IsBalanceChange == toServer.IsBalanceChange);
                            }

                            if (toServer.AddTimeStart > 0)
                                DBStatement = DBStatement.Where(x => x.AddTime >= toServer.AddTimeStart);
                            if (toServer.AddTimeEnd > 0)
                                DBStatement = DBStatement.Where(x => x.AddTime <= toServer.AddTimeEnd);



                            List<Log> dbLogs = DBStatement.ToList(); 
                            if (dbLogs.Count > 0)
                            {
                                foreach (var item in dbLogs)
                                {
                                    Log log = new Log();
                                    log.AddTime = item.AddTime;
                                    log.AdminId = item.AdminId;
                                    log.Balance = item.Balance;
                                    log.Other = item.Other;
                                    log.BalanceType = item.BalanceType;
                                    log.DeviceId = item.DeviceId;
                                    log.IsBalanceChange = item.IsBalanceChange;
                                    log.LogId = item.LogId;
                                    log.OperateId = item.OperateId;
                                    log.OperateName = item.OperateName;
                                    log.Remark = item.Remark;
                                    log.OperateSubId = item.OperateSubId;

                                    logs.Add(log);
                                }
                            }


                        }

                        logs.Select(x => x.Model = null).ToList();

                        Result = true;
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceGetLog() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                //返回它
                return new ToClientServiceGetLog() { Logs = JsonConvert.SerializeObject(logs), Result = Result, Balance = balance };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Get log operation failed.");
                //返回它
                return new ToClientServiceGetLog() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }

        #endregion Log


        #region Request

        /// <summary>
        /// 新增请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddRequest ServiceAddRequest(ToServerServiceAddRequest toServer)
        {
            try
            {
                bool Result = false;
                long Id = 0;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceAddRequest() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceAddRequest() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Request model = toServer.Request.DeserializeObject<Request>();

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Added;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceAddRequest() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Id = model.RequestId;
                    Resources.GetRes().REQUESTS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.RequestId, null, "Request#" + OperateType.Add, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Request, OperateType = OperateType.Add }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceAddRequest() { Request = JsonConvert.SerializeObject(model), Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Add request operation failed.");
                //返回它
                return new ToClientServiceAddRequest() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        /// <summary>
        /// 修改请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditRequest ServiceEditRequest(ToServerServiceEditRequest toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceEditRequest() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceEditRequest() { ExceptionType = ServiceExceptionType.DataNotReady };
                }

                //解析出来
                Request model = toServer.Request.DeserializeObject<Request>();

                // Printer 更新日期不一致, 不能更新
                Request serverModel = Resources.GetRes().REQUESTS.Where(x => x.RequestId == model.RequestId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceEditRequest() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;

                        model.UpdateTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        ctx.Entry(model).State = System.Data.Entity.EntityState.Modified;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceEditRequest() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().REQUESTS.Remove(Resources.GetRes().REQUESTS.Where(x => x.RequestId == model.RequestId).FirstOrDefault());
                    Resources.GetRes().REQUESTS.Add(model);

                    // 记录日志
                    OperateLog.Instance.AddRecord(model.RequestId, null, "Request#" + OperateType.Edit, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Request, OperateType = OperateType.Edit }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceEditRequest() { Result = Result, Request = JsonConvert.SerializeObject(model) };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Edit request operation failed.");
                //返回它
                return new ToClientServiceEditRequest() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }



        /// <summary>
        /// 删除请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelRequest ServiceDelRequest(ToServerServiceDelRequest toServer)
        {
            try
            {
                bool Result = false;

                //查询SESSION
                if (Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).Count() == 0)
                {
                    return new ToClientServiceDelRequest() { ExceptionType = ServiceExceptionType.SessionExpired };
                }
                else
                {
                    //更新响应时间
                    Resources.GetRes().Services.Where(x => x.SessionId == toServer.SessionId).FirstOrDefault().CheckDate = DateTime.Now;
                }


                //查看数据是否已准备好
                if (!DBOperate.GetDBOperate().IsDataReady)
                {
                    return new ToClientServiceDelRequest() { ExceptionType = ServiceExceptionType.DataNotReady };
                }




                //解析出来
                Request model = toServer.Request.DeserializeObject<Request>();

                // Printer 更新日期不一致, 不能更新
                Request serverModel = Resources.GetRes().REQUESTS.Where(x => x.RequestId == model.RequestId).FirstOrDefault();
                if (null == serverModel || serverModel.UpdateTime != model.UpdateTime)
                {
                    return new ToClientServiceDelRequest() { ExceptionType = ServiceExceptionType.UpdateModel };
                }

                // 不能删除, 不然会影响以前的记录显示在历史账单中
                return new ToClientServiceDelRequest() { ExceptionType = ServiceExceptionType.ServerFaild };


                try
                {
                    using (tsEntities ctx = new tsEntities(DBOperate.GetDBOperate().CONS, false))
                    {
                        ctx.Configuration.ProxyCreationEnabled = false;



                        //model = ctx.Printers.Where(x => x.PrinterId == model.PrinterId).FirstOrDefault();


                        ctx.Entry(model).State = System.Data.Entity.EntityState.Deleted;


                        int result = ctx.SaveChanges();

                        if (result > 0)
                        {
                            Result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Database operation failed.");
                    return new ToClientServiceDelRequest() { ExceptionType = ServiceExceptionType.DataFaild };
                }

                if (Result)
                {
                    Resources.GetRes().REQUESTS.Remove(Resources.GetRes().REQUESTS.Where(x => x.RequestId == model.RequestId).FirstOrDefault());
                    // 记录日志
                    OperateLog.Instance.AddRecord(model.RequestId, null, "Request#" + OperateType.Delete, toServer.SessionId);
                    NotificateToServiceModelUpdate(new ToClientServiceModelUpdateNotification() { Model = JsonConvert.SerializeObject(model), ModelType = ModelType.Request, OperateType = OperateType.Delete }, toServer.SessionId);
                }

                //返回它
                return new ToClientServiceDelRequest() { Result = Result };
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, "Delete request operation failed.");
                //返回它
                return new ToClientServiceDelRequest() { ExceptionType = ServiceExceptionType.ServerFaild };
            }
        }


        #endregion Request




        #region Notification






        /// <summary>
        /// 服务给客户端发送请求
        /// </summary>
        /// <param name="RoomId"></param>
        /// <param name="Order"></param>
        private void NotificateSend(SendType SendType, string sessionId, List<long> devicesId, string Message, long? RoomId, string Model, string ModelExt, long AdminId)
        {
            CallbackTask.QueueJob(() =>
            {
                foreach (var item in Resources.GetRes().Services.Where(x => x.SessionId != sessionId && devicesId.Contains(x.DeviceId)))
                {
                    item.TheTask.QueueJob(() =>
                    {
                        ToClientServiceSendNotification newToClientServiceSendNotification = new ToClientServiceSendNotification() { SendType = SendType, Message = Message, RoomId = RoomId, Model = Model, ModelExt = ModelExt, AdminId = AdminId };
                        lock (item.Lock)
                        {
                            try
                            {

                                if (item.IsConnected)
                                {
                                    if (null != item.SignalRClientSessionId)
                                    {
                                        SignalRCallback.Instance.ServiceSendNotification(item.SignalRClientSessionId, newToClientServiceSendNotification);
                                    }
                                    else
                                    {
                                        item.ClientCallback.ServiceSendNotification(newToClientServiceSendNotification);
                                    }

                                }
                                else
                                {
                                    item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(newToClientServiceSendNotification), Type = NotificationType.Send });

                                }
                            }
                            catch
                            {

                                item.IsConnected = false;
                                item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(newToClientServiceSendNotification), Type = NotificationType.Send });
                            }
                        }
                    });

                }
            });
        }




        /// <summary>
        /// 服务为更新订单通知所有设备(获取除了当前ServiceClient之外的所有其他的. 并发送订单更新信息.)
        /// </summary>
        /// <param name="OrderNotification"></param>
        /// <param name="sessionId"></param>
        private void NotificateToServiceUpdateOrder(string OrderNotification, string sessionId)
        {
            CallbackTask.QueueJob(() => {
                foreach (var item in Resources.GetRes().Services.Where(x => x.SessionId != sessionId))
                {
                    item.TheTask.QueueJob(() =>
                    {
                        DateTime time = DateTime.Now;
                        ToClientServiceOrderUpdateNotification newToClientServiceOrderUpdateNotification = new ToClientServiceOrderUpdateNotification() { OrderNotification = OrderNotification };
                        lock (item.Lock)
                        {
                            try
                            {


                                if (item.IsConnected)
                                {
                                    if (null != item.SignalRClientSessionId)
                                    {
                                        SignalRCallback.Instance.ServiceOrderUpdateNotification(item.SignalRClientSessionId, newToClientServiceOrderUpdateNotification);
                                    }
                                    else
                                    {
                                        item.ClientCallback.ServiceOrderUpdateNotification(newToClientServiceOrderUpdateNotification);
                                    }

                                }
                                else
                                {
                                    item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(newToClientServiceOrderUpdateNotification), Type = NotificationType.OrderUpdate });
                                }

                            }
                            catch
                            {
                                item.IsConnected = false;
                                item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(newToClientServiceOrderUpdateNotification), Type = NotificationType.OrderUpdate });
                            }

                        }
                    });
                }
            });
        }






        /// <summary>
        /// 客户端给服务端发送新增明细通知(验证模式)
        /// </summary>
        /// <param name="RoomId"></param>
        /// <param name="Order"></param>
        private void NotificateToServicAddOrderDetails(string Order, long RoomId, string OrderSessionId, string sessionId)
        {
            CallbackTask.QueueJob(() =>
            {
                foreach (var item in Resources.GetRes().Services.Where(x => x.SessionId != sessionId))
                {
                    item.TheTask.QueueJob(() =>
                    {
                        ToClientServiceOrderDetailsAddNotification newToClientServiceOrderDetailsAddNotification = new ToClientServiceOrderDetailsAddNotification { Order = Order, RoomId = RoomId, OrderSessionId = OrderSessionId };
                        lock (item.Lock)
                        {
                            try
                            {
                                if (item.IsConnected)
                                {
                                    if (null != item.SignalRClientSessionId)
                                    {
                                        SignalRCallback.Instance.ServiceOrderDetailsAddNotification(item.SignalRClientSessionId, newToClientServiceOrderDetailsAddNotification);
                                    }
                                    else
                                    {
                                        item.ClientCallback.ServiceOrderDetailsAddNotification(newToClientServiceOrderDetailsAddNotification);
                                    }
                                }
                                else
                                {
                                    item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(newToClientServiceOrderDetailsAddNotification), Type = NotificationType.OrderDetailsAdd });
                                }

                            }
                            catch
                            {

                                item.IsConnected = false;
                                item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(newToClientServiceOrderDetailsAddNotification), Type = NotificationType.OrderDetailsAdd });
                            }
                        }
                    });
                }
            });
        }





        /// <summary>
        /// 服务为更新外卖通知所有设备(获取除了当前ServiceClient之外的所有其他的. 并发送订单更新信息.)
        /// </summary>
        /// <param name="TakeoutsWithSession"></param>
        /// <param name="sessionId"></param>
        private void NotificateToServiceUpdateTakeout(string TakeoutNotification, string sessionId)
        {
            CallbackTask.QueueJob(() =>
            {
                foreach (var item in Resources.GetRes().Services.Where(x => x.SessionId != sessionId))
                {
                    item.TheTask.QueueJob(() =>
                    {
                        ToClientServiceTakeoutUpdateNotification newToClientServiceTakeoutUpdateNotification = new ToClientServiceTakeoutUpdateNotification() { TakeoutNotification = TakeoutNotification };
                        lock (item.Lock)
                        {
                            try
                            {

                                if (item.IsConnected)
                                {
                                    if (null != item.SignalRClientSessionId)
                                    {
                                        SignalRCallback.Instance.ServiceTakeoutUpdateNotification(item.SignalRClientSessionId, newToClientServiceTakeoutUpdateNotification);
                                    }
                                    else
                                    {
                                        item.ClientCallback.ServiceTakeoutUpdateNotification(newToClientServiceTakeoutUpdateNotification);
                                    }
                                }
                                else
                                {
                                    item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(newToClientServiceTakeoutUpdateNotification), Type = NotificationType.TakeoutUpdate });
                                }
                            }
                            catch
                            {

                                item.IsConnected = false;
                                item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(newToClientServiceTakeoutUpdateNotification), Type = NotificationType.TakeoutUpdate });
                            }
                        }
                    });
                }
            });
        }








        /// <summary>
        /// 服务为更新模型发送通知(获取除了当前ServiceClient之外的所有其他的.)
        /// </summary>
        /// <param name="RoomId"></param>
        /// <param name="Order"></param>
        private void NotificateToServiceModelUpdate(ToClientServiceModelUpdateNotification notification, string sessionId)
        {
            CallbackTask.QueueJob(() => {
                foreach (var item in Resources.GetRes().Services.Where(x => x.SessionId != sessionId))
                {
                    item.TheTask.QueueJob(() =>
                    {
                        lock (item.Lock)
                        {
                            try
                            {
                                if (item.IsConnected)
                                {
                                    if (null != item.SignalRClientSessionId)
                                    {
                                        SignalRCallback.Instance.ServiceModelUpdateNotification(item.SignalRClientSessionId, notification);
                                    }
                                    else
                                    {
                                        item.ClientCallback.ServiceModelUpdateNotification(notification);
                                    }
                                }
                                else
                                {
                                    item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(notification), Type = NotificationType.ModelUpdate });
                                }

                            }
                            catch
                            {

                                item.IsConnected = false;
                                item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(notification), Type = NotificationType.ModelUpdate });

                            }
                        }
                    });

                }
            });
        }



        /// <summary>
        /// 服务为更新产品数量通知(获取除了当前ServiceClient之外的所有其他的.)
        /// </summary>
        /// <param name="RoomId"></param>
        /// <param name="Order"></param>
        private void NotificateToServiceProductCountUpdate(ToClientServiceProductCountUpdateNotification notification, string sessionId)
        {
            CallbackTask.QueueJob(() =>
            {
                foreach (var item in Resources.GetRes().Services.Where(x => x.SessionId != sessionId))
                {
                    item.TheTask.QueueJob(() =>
                    {
                        lock (item.Lock)
                        {
                            try
                            {
                                if (item.IsConnected)
                                {
                                    if (null != item.SignalRClientSessionId)
                                    {
                                        SignalRCallback.Instance.ServiceProductCountUpdateNotification(item.SignalRClientSessionId, notification);
                                    }
                                    else
                                    {
                                        item.ClientCallback.ServiceProductCountUpdateNotification(notification);
                                    }
                                }
                                else
                                {
                                    item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(notification), Type = NotificationType.ProductCountUpdate });
                                }
                            }
                            catch
                            {

                                item.IsConnected = false;
                                item.NotificationCaches.Add(new NotificationCache() { Notification = JsonConvert.SerializeObject(notification), Type = NotificationType.ProductCountUpdate });
                            }
                        }

                    });
                }
            });
            }


        #endregion Notification




        #region Tool

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        private string GetIp(string IpAddress)
        {
            if (!string.IsNullOrWhiteSpace(IpAddress))
                return IpAddress;

            OperationContext context = OperationContext.Current;
            System.ServiceModel.Channels.MessageProperties prop = context.IncomingMessageProperties;
            System.ServiceModel.Channels.RemoteEndpointMessageProperty endpoint =
                prop[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name] as System.ServiceModel.Channels.RemoteEndpointMessageProperty;
            return endpoint.Address;
        }


        private string version = null;
        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        private string GetVersion()
        {
            if (null == version)
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                version = assembly.GetName().Version.ToString();//获取主版本号  
            }
            return version;
        }

        #endregion Tool



    }
}

