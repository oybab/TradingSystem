#define W_TRANS
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oybab.Res.Tools;
using Oybab.DAL;
using Newtonsoft.Json;
using Oybab.Res.Server;
using System.Net;
using System.Reflection;
using System.Net.NetworkInformation;
using Oybab.TradingSystemX.Server;
using System.Security.Cryptography.X509Certificates;
using Oybab.TradingSystemX.Tools;
using System.IO;
using Microsoft.AspNet.SignalR.Client;
using Oybab.ServerManager.Model.Service;
using Oybab.ServerManager.Model.Service.Common;
using Oybab.ServerManager.Model.Models;
using Oybab.ServerManager.Model.Service.Order;
using Oybab.ServerManager.Model.Service.Takeout;
using Oybab.ServerManager.Model.Service.Product;
using Oybab.ServerManager.Model.Service.Device;
using Oybab.ServerManager.Model.Service.OrderDetail;
using Oybab.ServerManager.Model.Service.TakeoutDetail;
using Oybab.ServerManager.Model.Service.MemberPay;
using Oybab.ServerManager.Model.Service.Log;
using Oybab.ServerManager.Model.Service.BalancePay;
using Oybab.ServerManager.Model.Service.Member;
using Xamarin.Essentials;
using Oybab.ServerManager.Model.Service.SupplierPay;
using Oybab.ServerManager.Model.Service.Balance;
using Oybab.ServerManager.Model.Service.Import;
using Oybab.ServerManager.Model.Service.Supplier;

namespace Oybab.TradingSystemX.Server
{
    internal sealed class OperatesService
    {
        #region Instance
        private OperatesService() { }

        private static readonly Lazy<OperatesService> _instance = new Lazy<OperatesService>(() => new OperatesService());
        internal static OperatesService Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance


        private HubConnection hubConnection = null;
        IHubProxy hubProxy = null;

        internal bool IsExpired = false;
        internal bool IsAdminUsing = false;


        /// <summary>
        /// 检查并创建
        /// </summary>
        private async Task CheckConnectionAndConnection()
        {
            if (null != hubConnection)
            {
                if (hubConnection.State == ConnectionState.Connecting || hubConnection.State == ConnectionState.Disconnected || hubConnection.State == ConnectionState.Reconnecting)
                {
                    
                    //打开通道并发送获取数据
                    await ConnectionServer();
                }
            }
            else
            {
                //打开通道并发送获取数据
                await ConnectionServer();
            }

        }

        private bool IsTry = false;

        /// <summary>
        /// 出现无法连接服务器时只能关掉并重连
        /// </summary>
        /// <param name="ex"></param>
        private async Task CommunicationExceptionHandle(Exception ex)
        {
            ExceptionPro.ExpLog(ex);





            if (Resources.Instance.IsSessionExists() && !IsTry)
            {
                IsTry = true;

                try
                {
                   
                    if (await ServiceSession(true))
                        IsTry = false;
                  
                }
                catch (Exception ex2)
                {
                    IsTry = false;

                    ExceptionPro.ExpLog(ex2);

                    if (ex2 is OybabException && ex2.Message != Resources.Instance.GetString("Exception_SessionRequestFaild"))
                        throw ex2;
                }
            }
            else
            {
                IsTry = false;
            }
        }

        /// <summary>
        /// 创建服务器连接会话
        /// </summary>
        /// <returns></returns>
        private async Task ConnectionServer()
        {
            try
            {
                if (null == hubConnection)
                {
                    string address = "http://" + Resources.Instance.SERVER_ADDRESS + ":19988";

                    // For DDNS
                    if (Resources.Instance.SERVER_ADDRESS.Contains(":"))
                        address = "http://" + Resources.Instance.SERVER_ADDRESS + "";

                    hubConnection = new HubConnection(address);

                    hubConnection.JsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    });

                    hubConnection.TraceLevel = TraceLevels.All;
                    hubConnection.TraceWriter = Console.Out;

                   
                    hubProxy = hubConnection.CreateHubProxy("Service");

                    RegisterCallback(hubProxy);



                }



                await hubConnection.Start();


            }
            catch (Exception ex)
            {
                hubConnection = null;
                hubProxy = null;
                throw new OybabException(Resources.Instance.GetString("Exception_RemoteServerConn"), ex);
            }
        }


        private void RegisterCallback(IHubProxy _hubProxy)
        {
            hubProxy.On<ToClientServiceDeviceModeUpdateNotification>("ServiceDeviceModeUpdateNotification", x =>
            {
                ServiceCallback.Instance.ServiceDeviceModeUpdateNotification(x);
            });

            hubProxy.On<ToClientServiceModelUpdateNotification>("ServiceModelUpdateNotification", x =>
            {
                ServiceCallback.Instance.ServiceModelUpdateNotification(x);
            });

            hubProxy.On<ToClientServiceOrderDetailsAddNotification>("ServiceOrderDetailsAddNotification", x =>
            {
                ServiceCallback.Instance.ServiceOrderDetailsAddNotification(x);
            });

            hubProxy.On<ToClientServiceOrderUpdateNotification>("ServiceOrderUpdateNotification", x =>
            {
                ServiceCallback.Instance.ServiceOrderUpdateNotification(x);
            });

            hubProxy.On<ToClientServiceProductCountUpdateNotification>("ServiceProductCountUpdateNotification", x =>
            {
                ServiceCallback.Instance.ServiceProductCountUpdateNotification(x);
            });

            hubProxy.On<ToClientServiceSendNotification>("ServiceSendNotification", x =>
            {
                ServiceCallback.Instance.ServiceSendNotification(x);
            });

            hubProxy.On<ToClientServiceTakeoutAddNotification>("ServiceTakeoutAddNotification", x =>
            {
                ServiceCallback.Instance.ServiceTakeoutAddNotification(x);
            });

            hubProxy.On<ToClientServiceTakeoutUpdateNotification>("ServiceTakeoutUpdateNotification", x =>
            {
                ServiceCallback.Instance.ServiceTakeoutUpdateNotification(x);
            });
        }


        /// <summary>
        /// 终止服务(强制)
        /// </summary>
        internal void AbortService()
        {
            try
            {
                IsTry = true;
                

                hubConnection = null;
                hubProxy = null;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }

        /// <summary>
        /// 关闭服务器会话
        /// </summary>
        private void CloseConnection()
        {
            try
            {
                if (null != hubConnection && hubConnection.State != ConnectionState.Disconnected)
                {
                    try
                    {
                        hubConnection.Stop();
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);

                    }
                }
                hubConnection = null;
                hubProxy = null;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }

        /// <summary>
        /// 处理错误
        /// </summary>
        /// <param name="type"></param>
        private async Task HandleException(ServiceExceptionType type, bool IsAuto = false)
        {
            ////先关闭通道
            //CloseConnection();
            switch (type)
            {
                case ServiceExceptionType.ServerFaild:
                    throw new OybabException(Resources.Instance.GetString("Exception_ServerFaild"));
                case ServiceExceptionType.DataNotReady:
                    throw new OybabException(Resources.Instance.GetString("Exception_DataNotReady"));
                case ServiceExceptionType.DatabaseNotFound:
                    throw new OybabException(Resources.Instance.GetString("Exception_DatabaseNotFound"));
                case ServiceExceptionType.DatabaseLoadFailed:
                    throw new OybabException(Resources.Instance.GetString("Exception_DatabaseLoadFailed"));
                case ServiceExceptionType.DataFaild:
                    throw new OybabException(Resources.Instance.GetString("Exception_CheckDataFaild"));
                case ServiceExceptionType.KeyCheckFaild:
                    throw new OybabException(Resources.Instance.GetString("Exception_ServerCheckFaild"));
                case ServiceExceptionType.KeyFaild:
                    throw new OybabException(Resources.Instance.GetString("Exception_ServerKeyFaild"));
                case ServiceExceptionType.CountOutOfLimit:
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_OutClientLimitFaild"), Common.Instance.GetFormat()));
                case ServiceExceptionType.RoomCountOutOfLimit:
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_RoomCountOutOfLimit"), Common.Instance.GetFormat()));
                case ServiceExceptionType.DeviceCountOutOfLimit:
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_DeviceCountOutOfLimit"), Common.Instance.GetFormat()));
                case ServiceExceptionType.CountOutOfIPRequestLimit:
                    throw new OybabException(Resources.Instance.GetString("Exception_CountOutOfIPRequestLimit"));
                case ServiceExceptionType.ApplicationValidFaild:
                    throw new OybabException(Resources.Instance.GetString("Exception_ApplicationCheckFaild"));
                case ServiceExceptionType.ServerClientTimeMisalignment:
                    throw new OybabException(Resources.Instance.GetString("Exception_ServerClientTimeMisalignment"));
                case ServiceExceptionType.ServerClientVersionMisalignment:
                    throw new OybabException(Resources.Instance.GetString("Exception_ServerClientVersionMisalignment"));
                case ServiceExceptionType.PasswordErrorCountLimit:
                    throw new OybabException(Resources.Instance.GetString("Exception_PasswordErrorCountLimit"));
                case ServiceExceptionType.IPConflict:
                    throw new OybabException(Resources.Instance.GetString("Exception_IPConflict"));
                case ServiceExceptionType.IPInvalid:
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_UnknownDevice"), Common.Instance.GetFormat())); //Exception_IPInvalid
                case ServiceExceptionType.UnknownDevice:
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_UnknownDevice"), Common.Instance.GetFormat()));
                case ServiceExceptionType.UnknownAdmin:
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_UnknownAdmin"), Common.Instance.GetFormat()));
                case ServiceExceptionType.SessionInvalid:
                    throw new OybabException(Resources.Instance.GetString("Exception_SessionInvalid"));
                case ServiceExceptionType.AdminExists:
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_AdminExists"), Common.Instance.GetFormat()));
                case ServiceExceptionType.DeviceExists:
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_DeviceExists"), Common.Instance.GetFormat()));
                case ServiceExceptionType.Relogin:
                    IsExpired = true;

                    if (IsAdminUsing)
                        throw new OybabException(string.Format(Resources.Instance.GetString("Exception_AdminExists"), Common.Instance.GetFormat()));
                    else
                        throw new OybabException(Resources.Instance.GetString("Exception_Relogin"));
                case ServiceExceptionType.AdminUsing:
                    IsAdminUsing = true;
                    throw new OybabException(string.Format(Resources.Instance.GetString("Exception_AdminExists"), Common.Instance.GetFormat()));

                case ServiceExceptionType.SessionExpired:
                    ExceptionPro.ExpErrorLog(Resources.Instance.GetString("Exception_SessionFaildAndTry"));
                    if (await ServiceSession())
                        ExceptionPro.ExpInfoLog(Resources.Instance.GetString("SessionUpdateSuccess"));
                    else
                        ExceptionPro.ExpErrorLog(Resources.Instance.GetString("Exception_SessionUpdateFaild"));

                    if (!IsAuto)
                        throw new OybabException(Resources.Instance.GetString("Exception_SessionExpiredRetry"));
                    break;
                default:
                    break;

            }
        }

        /// <summary>
        /// 返回到UI那边的错误结果
        /// </summary>
        /// <param name="type"></param>
        private void HandleResultException(ServiceExceptionType type, ResultModel result)
        {
            switch (type)
            {
                case ServiceExceptionType.DataHasRefrence: // 数据有引用, 指的是无法删除, 因为当前数据已有引用, 得提前删除引用
                    result.IsDataHasRefrence = true;
                    break;
                case ServiceExceptionType.RefreshSessionModel: // 订单会话已不一致, 得刷新
                    result.IsRefreshSessionModel = true;
                    break;
                case ServiceExceptionType.RefreshSessionModelForSameTimeOperate: // 这个订单不能被同时操作, 操作晚的另一方需要刷新或者停止当前操作
                    result.IsSessionModelSameTimeOperate = true;
                    break;
                case ServiceExceptionType.UpdateModel: // 刷新模型, 因为两方模型不一致
                    result.UpdateModel = true;
                    break;
                case ServiceExceptionType.UpdateRefModel: // 暂时把它当作是更新模型, 其实是模型中的某个tb_引用使用时检查发现update time不一致, 要求更新那个引用. 比如结账时会员信息不一致(提前增加了会员信息导致可能会覆盖改动后的新会员信息)
                    result.UpdateModel = true;
                    break;

            }
        }










        #region Service

        #region Common

        /// <summary>
        /// 服务新请求
        /// </summary>
        /// <param name="adminNo"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        internal async Task<ResultModel> NewRequest(string adminNo, string Password)
        {
            ResultModel result = new ResultModel();
            // 已获取过就别重复获取了
            if (Resources.Instance.IsSessionExists())
            {
                result.Result = true;
            }



            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceNewRequest client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceNewRequest>("ServiceNewRequest", new ToServerServiceNewRequest() { Soft_Service_PC_Name = Resources.Instance.SOFT_SERVICE_PC_NAME, Soft_Service_Tablet_Name = Resources.Instance.SOFT_SERVICE_TABLET_NAME, Soft_Service_Mobile_Name = Resources.Instance.SOFT_SERVICE_MOBILE_NAME, SessionId = Resources.Instance.SERVER_SESSION, AdminNo = adminNo, PWD = Password, IsLocalPrintCustomOrder = Resources.Instance.IsLocalPrintCustomOrder, DeviceType = Resources.Instance.DeviceType, CI = GetIP(), CM = GetOS(), CurrentVersion = Version, Time = DateTime.Now });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(string.Format(Resources.Instance.GetString("Exception_NewRequestFailt"), Common.Instance.GetFormat()), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);
            //设置值

            if (client.ValidResult && client.Result && client.ExceptionType == ServiceExceptionType.None && !client.IsExpired)
            {
                Resources.Instance.KEY_NAME_0 = client.Name_0;
                Resources.Instance.KEY_NAME_1 = client.Name_1;
                Resources.Instance.KEY_NAME_2 = client.Name_2;
                Resources.Instance.SERVER_SESSION = client.SessionId;
                Resources.Instance.Rooms = client.Rooms.DeserializeObject<List<Room>>();
                Resources.Instance.RoomsModel = client.RoomsModel.DeserializeObject<List<RoomModel>>();
                Resources.Instance.Products = client.Products.DeserializeObject<List<Product>>();
                Resources.Instance.ProductTypes = client.ProductTypes.DeserializeObject<List<ProductType>>();
                Resources.Instance.Admins = client.Admins.DeserializeObject<List<Admin>>();
                Resources.Instance.Devices = client.Devices.DeserializeObject<List<Device>>();
                Resources.Instance.Printers = client.Printers.DeserializeObject<List<Printer>>();
                Resources.Instance.Requests = client.Requests.DeserializeObject<List<Request>>();
                Resources.Instance.AdminModel = client.Admin.DeserializeObject<Admin>();
                Resources.Instance.DeviceModel = client.Device.DeserializeObject<Device>();
                Resources.Instance.Pprs = client.Pprs.DeserializeObject<List<Ppr>>();
                Resources.Instance.IsExpired = client.IsExpired;
                Resources.Instance.IsFireAlarmEnable = client.IsFireAlarmEnable;
                Resources.Instance.ExpiredRemainingDays = client.ExpiredRemaningDays;
                Resources.Instance.RegTimeRequestCode = client.RegTimeRequestCode;
                Resources.Instance.DeviceCount = client.DeviceCount;
                Resources.Instance.RoomCount = client.RoomCount;
                Resources.Instance.MinutesIntervalTime = client.MinutesIntervalTime;
                Resources.Instance.HoursIntervalTime = client.HoursIntervalTime;
                Resources.Instance.PrintInfo = client.PrintInfo.DeserializeObject<PrintInfo>();
                Resources.Instance.ExtendInfo = client.ExtendInfo.DeserializeObject<ExtendInfo>();
                Resources.Instance.Balances = client.Balances.DeserializeObject<List<Balance>>();

                // 设置会话是否过期为:否
                OperatesService.Instance.IsExpired = false;
                OperatesService.Instance.IsAdminUsing = false;
            }
            else
            {
                Resources.Instance.IsExpired = client.IsExpired;
                Resources.Instance.RegTimeRequestCode = client.RegTimeRequestCode;
            }

            result.IsExpired = client.IsExpired;
            result.ValidResult = client.ValidResult;
            result.Result = client.Result;
            result.IsAdminUsing = client.IsAdminUsing;


            return result;
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPWD"></param>
        /// <param name="newPWD"></param>
        /// <returns></returns>
        internal async Task<ResultModel> ChangePWD(string oldPWD, string newPWD)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceChangePWD client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceChangePWD>("ServiceChangePWD", new ToServerServiceChangePWD() { OldPWD = oldPWD, NewPWD = newPWD, SessionId = Resources.Instance.SERVER_SESSION, Admin = JsonConvert.SerializeObject(Resources.Instance.AdminModel) });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_ChangePWDRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.ValidResult = client.ValidResult;
            result.Result = client.Result;

            //返回值
            return result;
        }

        private string uid;

        /// <summary>
        /// 获取ID
        /// </summary>
        /// <returns></returns>
        internal async Task<string> ServiceUID()
        {
            //已获取就无需重新获取
            if (!string.IsNullOrEmpty(OperatesService.Instance.uid))
                return uid;


            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceUID client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceUID>("ServiceUID", new ToServerServiceUID() { SessionId = Resources.Instance.SERVER_SESSION });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);

            OperatesService.Instance.uid = client.UID;

            //返回值
            return client.UID;
        }


        /// <summary>
        /// 发送操作
        /// </summary>
        /// <param name="Rooms"></param>
        /// <param name="SendType"></param>
        /// <returns></returns>
        internal async Task<bool> ServiceSend(List<long> Rooms, int SendType, string Model = null, string Message = null)
        {
            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceSend client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceSend>("ServiceSend", new ToServerServiceSend() { RoomsId = JsonConvert.SerializeObject(Rooms), SendType = (SendType)SendType, SessionId = Resources.Instance.SERVER_SESSION, AdminId = Resources.Instance.AdminModel.AdminId, Model = Model, Message = Message });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);

            if (client.Result)
            {
                SendType type = (SendType)SendType;

                if (type == Oybab.ServerManager.Model.Models.SendType.FireOn)
                {
                    Resources.Instance.IsFireAlarmEnable = true;
                }
                else if (type == Oybab.ServerManager.Model.Models.SendType.FireOff)
                {
                    Resources.Instance.IsFireAlarmEnable = false;
                }
                //else 
                if (type == Oybab.ServerManager.Model.Models.SendType.ExtendInfo)
                {
                    Resources.Instance.ExtendInfo = Model.DeserializeObject<ExtendInfo>();
                }
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 锁住
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> ServiceLock()
        {
            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceLock client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceLock>("ServiceLock", new ToServerServiceLock() { SessionId = Resources.Instance.SERVER_SESSION });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            //返回值
            return client.IsSuccessLock;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> ServiceClose()
        {
            try
            {
                //如果没获取过会话,则无需发送关闭请求
                if (string.IsNullOrEmpty(Resources.Instance.SERVER_SESSION))
                {
                    CloseConnection();
                    return true;
                }

                //打开通道并发送获取数据
                await CheckConnectionAndConnection();
                ToClientServiceClose client = null;
                try
                {
                    client = await hubProxy.Invoke<ToClientServiceClose>("ServiceClose", new ToServerServiceClose() { SessionId = Resources.Instance.SERVER_SESSION, AdminId = Resources.Instance.AdminModel.AdminId });
                }
                catch (Exception ex)
                {
                    await CommunicationExceptionHandle(ex);
                    throw new OybabException(Resources.Instance.GetString("Exception_CloseRequestFaild"), ex);
                }
                //处理错误
                await HandleException(client.ExceptionType);

                CloseConnection();

                //返回值
                return client.Result;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                return false;
            }
        }



        /// <summary>
        /// 刷新会话
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        internal async Task<bool> ServiceSession(bool IsAuto = false, long[] RoomsId = null, bool IsNew = false)
        {
            if (IsNew && IsTry)
                IsTry = false;

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceSession client = null;
            try
            {
                string RoomsInfoModel = null;

                client = await hubProxy.Invoke<ToClientServiceSession>("ServiceSession", new ToServerServiceSession() { RoomsId = RoomsId, RoomsInfoModel = RoomsInfoModel, SessionId = Resources.Instance.SERVER_SESSION, IsNew = IsNew, IsSignalRMode = true });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_SessionRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType, IsAuto);

            //如果会话成功更新了
            if (client.ExceptionType == ServiceExceptionType.SessionUpdate)
                Resources.Instance.SERVER_SESSION = client.NewSessionId;


            List<RoomModel> rooms = client.RoomsModel.DeserializeObject<List<RoomModel>>();




            if (null != rooms && rooms.Count > 0)
            {
                foreach (var item in rooms)
                {
                    // 成功获取了则更新

                    RoomModel oldModel = Resources.Instance.RoomsModel.Where(x => x.RoomId == item.RoomId).FirstOrDefault();
                    Resources.Instance.RoomsModel.Remove(oldModel);

                    Resources.Instance.RoomsModel.Add(item);

                    Notification.Instance.ActionSendFromService(null, item.RoomId, null);

                }
            }


            //返回值
            return client.Result;
        }

        #endregion Common


        #region Order

        /// <summary>
        /// 新建订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetails"></param>
        /// <param name="roomStateSession"></param>
        /// <param name="orderDetailsResult"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, List<OrderDetail> orderDetailsResult, List<OrderPay> orderPaysResult, string newRoomStateSession, long UpdateTime)> ServiceAddOrder(Order order, List<OrderDetail> orderDetails, List<OrderPay> orderPays, string roomStateSession)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceNewOrder client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceNewOrder>("ServiceAddOrder", new ToServerServiceNewOrder() { SessionId = Resources.Instance.SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderDetails = JsonConvert.SerializeObject(orderDetails), OrderPays = JsonConvert.SerializeObject(orderPays), RoomStateSession = roomStateSession });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 返回结果
            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Order model = client.Order.DeserializeObject<Order>();
                order.OrderId = model.OrderId;
                order.AddTime = model.AddTime;
                order.AdminId = model.AdminId;
                order.DeviceId = model.DeviceId;
                order.Mode = model.Mode;
                order.RoomPriceCalcTime = model.RoomPriceCalcTime;
            }

            //返回值
            return (result, client.OrderDetails.DeserializeObject<List<OrderDetail>>(), client.OrderPays.DeserializeObject<List<OrderPay>>(), client.RoomSessionId, client.UpdateTime);
        }


        /// <summary>
        /// 编辑订单(只有订单, 如结账, 取消)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetails"></param>
        /// <param name="roomStateSession"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, string newRoomStateSession, long UpdateTime)> ServiceEditOrder(Order order, List<OrderDetail> orderDetails, List<OrderPay> orderPays, string roomStateSession, bool IsRechecked)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceEditOrder client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceEditOrder>("ServiceEditOrder", new ToServerServiceEditOrder() { SessionId = Resources.Instance.SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderPays = JsonConvert.SerializeObject(orderPays), RoomStateSession = roomStateSession, Rechecked = IsRechecked });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);


            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Order model = client.Order.DeserializeObject<Order>();
                order.FinishTime = model.FinishTime;
                order.FinishAdminId = model.FinishAdminId;
                order.FinishDeviceId = model.FinishDeviceId;
                order.UpdateTime = model.UpdateTime;
                order.ReCheckedCount = model.ReCheckedCount;
            }

            //返回值
            return (result, client.RoomStateSession, client.UpdateTime);
        }



        /// <summary>
        /// 替换订单(替换包厢)
        /// </summary>
        /// <param name="oldRoomId"></param>
        /// <param name="newRoomId"></param>
        /// <param name="oldOrder"></param>
        /// <param name="newOrder"></param>
        /// <param name="oldRoomSession"></param>
        /// <param name="newRoomSession"></param>
        /// <param name="oldRoomSessionResult"></param>
        /// <param name="newRoomSessionResult"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, string oldRoomSessionResult, string newRoomSessionResult)> ServiceReplaceOrder(long oldRoomId, long newRoomId, Order oldOrder, Order newOrder, string oldRoomSession, string newRoomSession)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceReplaceOrder client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceReplaceOrder>("ServiceReplaceOrder", new ToServerServiceReplaceOrder() { SessionId = Resources.Instance.SERVER_SESSION, NewRoomId = newRoomId, OldRoomId = oldRoomId, NewRoomSession = newRoomSession, OldRoomSession = oldRoomSession, OldOrder = JsonConvert.SerializeObject(oldOrder), NewOrder = JsonConvert.SerializeObject(newOrder) });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;


            //返回值
            return (result, client.OldRoomSession, client.NewRoomSession);
        }




        #endregion Order


        #region OrderDetail



        /// <summary>
        /// 修改订单明细
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetailsAdd"></param>
        /// <param name="orderDetailsEdit"></param>
        /// <param name="orderDetailsConfirm"></param>
        /// <param name="roomStateSession"></param>
        /// <param name="orderDetailsAddResult"></param>
        /// <param name="orderDetailsEditResult"></param>
        /// <param name="orderDetailsConfirmResult"></param>
        /// <param name="newRoomStateSession"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, List<OrderDetail> orderDetailsAddResult, List<OrderPay> orderPaysAddResult, List<OrderDetail> orderDetailsEditResult, List<OrderDetail> orderDetailsConfirmResult, string newRoomStateSession, long UpdateTime)> ServiceSaveOrderDetail(Order order, List<OrderDetail> orderDetailsAdd, List<OrderPay> orderPaysAdd, List<OrderDetail> orderDetailsEdit, List<OrderDetail> orderDetailsConfirm, string roomStateSession)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceSaveOrderDetail client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceSaveOrderDetail>("ServiceSaveOrderDetail", new ToServerServiceSaveOrderDetail() { SessionId = Resources.Instance.SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderDetailsAdd = JsonConvert.SerializeObject(orderDetailsAdd), OrderPaysAdd = JsonConvert.SerializeObject(orderPaysAdd), OrderDetailsEdit = JsonConvert.SerializeObject(orderDetailsEdit), OrderDetailsConfirm = JsonConvert.SerializeObject(orderDetailsConfirm), RoomStateSession = roomStateSession });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 时间不同, 说明到期时间不同(一般多见于按时间收费上的改动)
            if (null != order.EndTime)
                order.RoomPriceCalcTime = order.EndTime.Value;

            //返回值
            return (result, client.OrderDetailAdd.DeserializeObject<List<OrderDetail>>(), client.OrderPayAdd.DeserializeObject<List<OrderPay>>(), client.OrderDetailEdit.DeserializeObject<List<OrderDetail>>(), client.OrderDetailConfirm.DeserializeObject<List<OrderDetail>>(), client.OrderSessionId, client.UpdateTime);
        }


        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetails"></param>
        /// <param name="roomStateSession"></param>
        /// <param name="newRoomStateSession"></param>
        /// <param name="newOrderDetails"></param>
        /// <param name="UpdateTime"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel,  string newRoomStateSession, List<OrderDetail> newOrderDetails, long UpdateTime)> ServiceDelOrderDetail(Order order, List<OrderDetail> orderDetails, string roomStateSession)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceDelOrderDetail client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceDelOrderDetail>("ServiceDelOrderDetail", new ToServerServiceDelOrderDetail() { SessionId = Resources.Instance.SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderDetails = JsonConvert.SerializeObject(orderDetails), RoomStateSession = roomStateSession });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return (result, client.OrderSessionId, client.OrderDetails.DeserializeObject<List<OrderDetail>>(), client.UpdateTime);
        }





        #endregion OrderDetail


        #region Takeout

        /// <summary>
        /// 新建外卖
        /// </summary>
        /// <param name="takeout"></param>
        /// <param name="takeoutDetails"></param>
        /// <param name="takeoutStateSession"></param>
        /// <param name="takeoutDetailsResult"></param>
        /// <param name="newTakeoutStateSession"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, List<TakeoutDetail> takeoutDetailsResult, List<TakeoutPay> takeoutPaysResult, string newTakeoutStateSession, long UpdateTime)> ServiceAddTakeout(Takeout takeout, List<TakeoutDetail> takeoutDetails, List<TakeoutPay> takeoutPays, string takeoutStateSession)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceNewTakeout client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceNewTakeout>("ServiceAddTakeout", new ToServerServiceNewTakeout() { SessionId = Resources.Instance.SERVER_SESSION, Takeout = JsonConvert.SerializeObject(takeout), TakeoutDetails = JsonConvert.SerializeObject(takeoutDetails), TakeoutPays = JsonConvert.SerializeObject(takeoutPays), TakeoutStateSession = takeoutStateSession });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 返回结果
            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Takeout model = client.Takeout.DeserializeObject<Takeout>();
                takeout.TakeoutId = model.TakeoutId;
                takeout.AddTime = model.AddTime;

                takeout.AdminId = model.AdminId;
                takeout.DeviceId = model.DeviceId;
                takeout.Mode = model.Mode;

                takeout.FinishAdminId = model.FinishAdminId;
                takeout.FinishDeviceId = model.FinishDeviceId;
                takeout.FinishTime = model.FinishTime;
            }

            //返回值
            return (result, client.TakeoutDetails.DeserializeObject<List<TakeoutDetail>>(), client.TakeoutPays.DeserializeObject<List<TakeoutPay>>(), client.TakeoutSessionId, client.UpdateTime);
        }





        #endregion Takeout





        #region ImportWithDetails

        /// <summary>
        /// 增加进货及明细
        /// </summary>
        /// <param name="import"></param>
        /// <param name="importDetails"></param>
        /// <param name="importDetailsAddResult"></param>
        /// <returns></returns>
        public async Task<(ResultModel resultModel, List<ImportDetail> importDetailsAddResult, List<ImportPay> importPaysAddResult, long UpdateTime )> ServiceAddImportWithDetail(Import import, List<ImportDetail> importDetails, List<ImportPay> importPays)
        {
            ResultModel result = new ResultModel();
            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceNewImport client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceNewImport>("ServiceAddImportWithDetail", new ToServerServiceNewImport() { SessionId = Resources.Instance.SERVER_SESSION, Import = JsonConvert.SerializeObject(import), ImportDetails = JsonConvert.SerializeObject(importDetails), ImportPays = JsonConvert.SerializeObject(importPays) });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;


            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Import model = client.Import.DeserializeObject<Import>();
                import.ImportId = model.ImportId;
                import.AddTime = model.AddTime;
                import.UpdateTime = model.UpdateTime;
                import.DeviceId = model.DeviceId;
                import.Mode = model.Mode;
                import.AdminId = model.AdminId;
                import.ReCheckedCount = model.ReCheckedCount;
                import.tb_supplier = model.tb_supplier;

            }


            //返回值
            return (result, client.ImportDetails.DeserializeObject<List<ImportDetail>>(), client.ImportPays.DeserializeObject<List<ImportPay>>(), client.UpdateTime);
        }


        #endregion ImportWithDetails


        #region Member


        /// <summary>
        /// 修改会员
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, bool IsMemberExists, bool IsCardExists)> ServiceEditMember(Member member)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceEditMember client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceEditMember>("ServiceEditMember", new ToServerServiceEditMember() { SessionId = Resources.Instance.SERVER_SESSION, Member = JsonConvert.SerializeObject(member) });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Member model = client.Member.DeserializeObject<Member>();
                member.UpdateTime = model.UpdateTime;
            }


            //返回值
            return (result, client.IsMemberExists, client.IsCardExists);
        }


        /// <summary>
        /// 查找会员
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="cardNo"></param>
        /// <param name="memberNo"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="SingleMemberNo"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        internal async Task<(bool result,  List<Member> members)> ServiceGetMembers(long memberId, string memberNo, string cardNo, string Name, string Phone, bool SingleMemberNo)
        {
            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceGetMember client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceGetMember>("ServiceGetMembers", new ToServerServiceGetMember() { SessionId = Resources.Instance.SERVER_SESSION, MemberId = memberId, MemberNo = memberNo, Name = Name, Phone = Phone, SingleMemberNo = SingleMemberNo, CardNo = cardNo });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);


            //返回值
            return (client.Result, client.Members.DeserializeObject<List<Member>>());
        }


        #endregion Member



        #region Supplier


        /// <summary>
        /// 查找供应商
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="supplierNo"></param>
        /// <param name="cardNo"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="SingleSupplierNo"></param>
        /// <param name="suppliers"></param>
        /// <returns></returns>
        public async Task<(bool result, List<Supplier> suppliers)> ServiceGetSupplier(long supplierId, string supplierNo, string cardNo, string Name, string Phone, bool SingleSupplierNo)
        {
            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceGetSupplier client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceGetSupplier>("ServiceGetSupplier", new ToServerServiceGetSupplier() { SessionId = Resources.Instance.SERVER_SESSION, SupplierId = supplierId, SupplierNo = supplierNo, Name = Name, Phone = Phone, SingleSupplierNo = SingleSupplierNo, CardNo = cardNo });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);

            //返回值
            return (client.Result, client.Suppliers.DeserializeObject<List<Supplier>>());
        }


        #endregion Supplier


        #region Balance

        /// <summary>
        /// 查找余额
        /// </summary>
        /// <param name="balanceId"></param>
        /// <param name="balances"></param>
        /// <returns></returns>
        internal async Task<(bool result, List<Balance> balances)> ServiceGetBalances(long balanceId)
        {
            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceGetBalance client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceGetBalance>("ServiceGetBalances", new ToServerServiceGetBalance() { SessionId = Resources.Instance.SERVER_SESSION, BalanceId = balanceId });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);


            //返回值
            return (client.Result, client.Balances.DeserializeObject<List<Balance>>());
        }


        #endregion Balance


        #region BalancePay


        /// <summary>
        /// 增加余额支付
        /// </summary>
        /// <param name="balancePay"></param>
        /// <param name="newBalancePay"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, BalancePay newBalancePay, Balance newBalance)> ServiceAddBalancePay(BalancePay balancePay)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceAddBalancePay client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceAddBalancePay>("ServiceAddBalancePay", new ToServerServiceAddBalancePay() { SessionId = Resources.Instance.SERVER_SESSION, BalancePay = JsonConvert.SerializeObject(balancePay) });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;


            //返回值
            return (result, client.BalancePay.DeserializeObject<BalancePay>(), client.Balance.DeserializeObject<Balance>());
        }


        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="balancePay1"></param>
        /// <param name="balancePay2"></param>
        /// <param name="newBalancePay1"></param>
        /// <param name="newBalance1"></param>
        /// <param name="newBalancePay2"></param>
        /// <param name="newBalance2"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, BalancePay newBalancePay1, Balance newBalance1, BalancePay newBalancePay2, Balance newBalance2)> ServiceTransferBalancePay(BalancePay balancePay1, BalancePay balancePay2)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceTransferBalancePay client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceTransferBalancePay>("ServiceTransferBalancePay", new ToServerServiceTransferBalancePay() { SessionId = Resources.Instance.SERVER_SESSION, BalancePay1 = JsonConvert.SerializeObject(balancePay1), BalancePay2 = JsonConvert.SerializeObject(balancePay2) });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;


            //返回值
            return (result, client.BalancePay1.DeserializeObject<BalancePay>(), client.Balance1.DeserializeObject<Balance>(), client.BalancePay2.DeserializeObject<BalancePay>(), client.Balance2.DeserializeObject<Balance>());
        }


        #endregion BalancePay


        #region Log

        /// <summary>
        /// 查找日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        internal async Task<(bool result, List<Balance> balances, List<Log> logs)> ServiceGetLog(long IsBalancePrice, long IsBalanceChange, long AddTimeStart, long AddTimeEnd, long OperateId = 0)
        {
            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceGetLog client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceGetLog>("ServiceGetLog", new ToServerServiceGetLog() { SessionId = Resources.Instance.SERVER_SESSION, AddTimeStart = AddTimeStart, AddTimeEnd = AddTimeEnd, IsBalanceChange = IsBalanceChange, IsBalancePrice = IsBalancePrice, OperateId = OperateId });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);


            //返回值
            return (client.Result, client.Balance.DeserializeObject<List<Balance>>(), client.Logs.DeserializeObject<List<Log>>());
        }

        #endregion Log




        #endregion Service


        #region Add MemberPay

        /// <summary>
        /// 增加会员支付
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberPay"></param>
        /// <param name="newMember"></param>
        /// <param name="newMemberPay"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, Member newMember, MemberPay newMemberPay)> ServiceAddMemberPay(Member member, MemberPay memberPay)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceAddMemberPay client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceAddMemberPay>("ServiceAddMemberPay", new ToServerServiceAddMemberPay() { SessionId = Resources.Instance.SERVER_SESSION, Member = JsonConvert.SerializeObject(member), MemberPay = JsonConvert.SerializeObject(memberPay) });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;


            //返回值
            return (result, client.Member.DeserializeObject<Member>(), client.MemberPay.DeserializeObject<MemberPay>());
        }

        #endregion Add MemberPay




        #region Add SupplierPay

        
        /// <summary>
        /// 增加供应商支付
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="supplierPay"></param>
        /// <returns></returns>
        internal async Task<(ResultModel resultModel, Supplier newSupplier, SupplierPay newSupplierPay)> ServiceAddSupplierPay(Supplier supplier, SupplierPay supplierPay)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            await CheckConnectionAndConnection();
            ToClientServiceAddSupplierPay client = null;
            try
            {
                client = await hubProxy.Invoke<ToClientServiceAddSupplierPay>("ServiceAddSupplierPay", new ToServerServiceAddSupplierPay() { SessionId = Resources.Instance.SERVER_SESSION, Supplier = JsonConvert.SerializeObject(supplier), SupplierPay = JsonConvert.SerializeObject(supplierPay) });
            }
            catch (Exception ex)
            {
                await CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.Instance.GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            await HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;


            //返回值
            return (result, client.Supplier.DeserializeObject<Supplier>(), client.SupplierPay.DeserializeObject<SupplierPay>());
        }

        #endregion Add SupplierPay


        #region Get Info



        /// <summary>
        /// 获取MAC列表
        /// </summary>
        /// <returns></returns>
        internal string GetOS()
        {
            try
            {
                return DeviceInfo.Model + ":" + DeviceInfo.Version + "/" + Xamarin.Forms.Device.RuntimePlatform.ToString() + "/" + Xamarin.Forms.Device.Idiom.ToString();
            }
            catch
            {
                return "error";
            }
        }



        /// <summary>
        /// 获取版本号
        /// </summary>
        internal string Version
        {
            get
            {
                var assembly = typeof(OperatesService).GetTypeInfo().Assembly;
                // In some PCL profiles the above line is: var assembly = typeof(MyType).Assembly;
                var assemblyName = new AssemblyName(assembly.FullName);
                return  assemblyName.Version.ToString();
            }
        }


        /// <summary>
        /// 获取所有版本号
        /// </summary>
        internal string AllVersion
        {
            get
            {
                var assembly = typeof(OperatesService).GetTypeInfo().Assembly;
                // In some PCL profiles the above line is: var assembly = typeof(MyType).Assembly;
                var assemblyName = new AssemblyName(assembly.FullName);
                return VersionTracking.CurrentVersion + " (" + assemblyName.Version.ToString() + ")";
            }
        }

        /// <summary>
        /// 获取IP列表
        /// </summary>
        /// <returns></returns>
        private string GetIP()
        {
            List<string> names = new List<string>();
            try
            {
                foreach (IPAddress localHostName in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    names.Add(localHostName.ToString());
                }
              
            }
            catch
            {
                try
                {
                    foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                            netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
                            foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                            {
                                if (addrInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    var ipAddress = addrInfo.Address;

                                    names.Add(ipAddress.ToString());
                                }
                            }
                        }
                    }
                }
                catch
                {
                    return "error";
                }

            }

            if (names.Count == 0)
                return null;
            else
                return string.Join("|", names);
        }




    }

    #endregion Get Info


    #region CallBack

    /// <summary>
    /// 回调
    /// </summary>
    internal sealed class ServiceCallback
    {

        #region Instance
        private ServiceCallback(){}

        private static readonly Lazy<ServiceCallback> _instance = new Lazy<ServiceCallback>(() => new ServiceCallback());
        internal static ServiceCallback Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance

        /// <summary>
        /// 服务获取通知
        /// </summary>
        /// <param name="toClient"></param>
        internal void ServiceSendNotification(ToClientServiceSendNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceSendNotification(toClient);
            });
        }

        /// <summary>
        /// 订单更新
        /// </summary>
        /// <param name="toClient"></param>
        internal void ServiceOrderUpdateNotification(ToClientServiceOrderUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceOrderUpdateNotification(toClient);
            });
        }

        /// <summary>
        /// 外卖更新
        /// </summary>
        /// <param name="toClient"></param>
        internal void ServiceTakeoutUpdateNotification(ToClientServiceTakeoutUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceTakeoutUpdateNotification(toClient);
            });
        }



        /// <summary>
        /// 产品数量更新
        /// </summary>
        /// <param name="toClient"></param>
        internal void ServiceProductCountUpdateNotification(ToClientServiceProductCountUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceProductCountUpdateNotification(toClient);
            });
        }

        /// <summary>
        /// 设备登录或退出等状态有变
        /// </summary>
        /// <param name="toClient"></param>
        internal void ServiceDeviceModeUpdateNotification(ToClientServiceDeviceModeUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceDeviceModeUpdateNotification(toClient);
            });
        }




        /// <summary>
        /// 服务订单新增明细通知(客户端顾客先验证模式)
        /// </summary>
        /// <param name="toClient"></param>
        internal void ServiceOrderDetailsAddNotification(ToClientServiceOrderDetailsAddNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceOrderDetailsAddNotification(toClient);
            });
        }



        /// <summary>
        /// 服务外卖新增外卖通知(客户端顾客先验证模式)
        /// </summary>
        /// <param name="toClient"></param>
        internal void ServiceTakeoutAddNotification(ToClientServiceTakeoutAddNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceTakeoutAddNotification(toClient);
            });
        }


        /// <summary>
        /// 服务发送模型更改通知
        /// </summary>
        /// <param name="toClient"></param>
        internal void ServiceModelUpdateNotification(ToClientServiceModelUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => {
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceModelUpdateNotification(toClient);
            });




        }

    }





    #endregion CallBack
}
