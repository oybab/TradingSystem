#define W_TRANS
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server.Model;
using Oybab.Res.Service;
using Oybab.Res.Tools;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.InteropServices;
using Oybab.ServerManager.Model.Models;

namespace Oybab.Res.Server
{
    public sealed class OperatesService
    {
        private static OperatesService operates = null;
        private OperatesService() { }

        public static OperatesService GetOperates()
        {
            if (null == operates)
                operates = new OperatesService();
            return operates;
        }



        private DuplexChannelFactory<IService> channelFactory = null;
        private InstanceContext instanceContext;
        private IService service = null;

        internal bool IsExpired = false;
        internal bool IsAdminUsing = false;


        /// <summary>
        /// 检查并创建
        /// </summary>
        private void CheckConnectionAndConnection()
        {
            if (null != channelFactory)
            {
                if (channelFactory.State == CommunicationState.Faulted)
                {
                    channelFactory.Abort();
                    //打开通道并发送获取数据
                    ConnectionServer();
                }
            }
            else
            {
                //打开通道并发送获取数据
                ConnectionServer();
            }

        }

        private bool IsTry = false;
        /// <summary>
        /// 出现无法连接服务器时只能关掉并重连
        /// </summary>
        /// <param name="ex"></param>
        private void CommunicationExceptionHandle(Exception ex)
        {
            ExceptionPro.ExpLog(ex);

            if (ex is System.ServiceModel.CommunicationException)
            {
                if (null != channelFactory)
                {
                    channelFactory.Abort();
                }
                channelFactory = null;
                service = null;

                if (Resources.GetRes().IsSessionExists() && !IsTry)
                {
                    IsTry = true;
                    try
                    {
                        if (ServiceSession(true))
                            IsTry = false;
                    }
                    catch (Exception ex2)
                    {
                        IsTry = false;
                        ExceptionPro.ExpLog(ex2);

                        if (ex2 is OybabException && ex2.Message != Resources.GetRes().GetString("Exception_SessionRequestFaild"))
                            throw ex2;
                    }
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
        private void ConnectionServer()
        {
            try
            {
                //加密
                // Create Binding
#if W_TRANS
                NetTcpBinding tcpb = new NetTcpBinding(SecurityMode.None);
#else
                NetTcpBinding tcpb = new NetTcpBinding(SecurityMode.Message);
                tcpb.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
#endif

                //Updated: to enable file transefer of 64 MB(67108864) 655360000(Before);
                tcpb.MaxBufferSize = 67108864;
                tcpb.MaxBufferPoolSize = 67108864;
                tcpb.MaxReceivedMessageSize = 67108864;

                tcpb.ReaderQuotas.MaxArrayLength = 67108864;
                tcpb.ReaderQuotas.MaxBytesPerRead = 67108864;
                tcpb.ReaderQuotas.MaxStringContentLength = 67108864;


                string address = "net.tcp://" + Resources.GetRes().SERVER_ADDRESS + ":" + Resources.GetRes().SERVER_PORT + "/OybabService";

                // For DDNS
                if (Resources.GetRes().SERVER_ADDRESS.Contains(":"))
                    address = "net.tcp://" + Resources.GetRes().SERVER_ADDRESS + "/OybabService";

                // Create End Point
                EndpointAddress ep = new EndpointAddress(new Uri(address), EndpointIdentity.CreateDnsIdentity("OybabServer"));

                channelFactory = new DuplexChannelFactory<IService>(instanceContext = new InstanceContext(new ServiceCallback()), tcpb, ep);

#if !W_TRANS

                channelFactory.Credentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, CertName);
                
                channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;

#endif
                // Create Channel
                service = channelFactory.CreateChannel();


              

            }
            catch (Exception ex)
            {
                channelFactory = null;
                service = null;
                throw new OybabException(Resources.GetRes().GetString("Exception_RemoteServerConn"), ex);
            }
        }


        /// <summary>
        /// 终止服务
        /// </summary>
        internal void AbortService()
        {
            try
            {
                IsTry = true;
                if (null != channelFactory)
                {
                    channelFactory.Abort();
                   
                }
                channelFactory = null;
                service = null;
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
                if (null != channelFactory)
                {
                    try
                    {
                        channelFactory.Close();
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                        channelFactory.Abort();
                    }
                }
                channelFactory = null;
                service = null;
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
        private void HandleException(ServiceExceptionType type, bool IsAuto = false)
        {
            switch (type)
            {
                case ServiceExceptionType.ServerFaild:
                    throw new OybabException(Resources.GetRes().GetString("Exception_ServerFaild"));
                case ServiceExceptionType.DataNotReady:
                    throw new OybabException(Resources.GetRes().GetString("Exception_DataNotReady"));
                case ServiceExceptionType.DatabaseNotFound:
                    throw new OybabException(Resources.GetRes().GetString("Exception_DatabaseNotFound"));
                case ServiceExceptionType.DatabaseLoadFailed:
                    throw new OybabException(Resources.GetRes().GetString("Exception_DatabaseLoadFailed"));
                case ServiceExceptionType.DataFaild:
                    throw new OybabException(Resources.GetRes().GetString("Exception_CheckDataFaild"));
                case ServiceExceptionType.KeyCheckFaild:
                    throw new OybabException(Resources.GetRes().GetString("Exception_ServerCheckFaild"));
                case ServiceExceptionType.KeyFaild:
                    throw new OybabException(Resources.GetRes().GetString("Exception_ServerKeyFaild"));
                case ServiceExceptionType.CountOutOfLimit:
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_OutClientLimitFaild"), Common.GetCommon().GetFormat()));
                case ServiceExceptionType.RoomCountOutOfLimit:
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_RoomCountOutOfLimit"), Common.GetCommon().GetFormat()));
                case ServiceExceptionType.DeviceCountOutOfLimit:
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_DeviceCountOutOfLimit"), Common.GetCommon().GetFormat()));
                case ServiceExceptionType.CountOutOfIPRequestLimit:
                     throw new OybabException(Resources.GetRes().GetString("Exception_CountOutOfIPRequestLimit"));
                case ServiceExceptionType.ApplicationValidFaild:
                    throw new OybabException(Resources.GetRes().GetString("Exception_ApplicationCheckFaild"));
                case ServiceExceptionType.ServerClientTimeMisalignment:
                    throw new OybabException(Resources.GetRes().GetString("Exception_ServerClientTimeMisalignment"));
                case ServiceExceptionType.ServerClientVersionMisalignment:
                    throw new OybabException(Resources.GetRes().GetString("Exception_ServerClientVersionMisalignment"));
                case ServiceExceptionType.PasswordErrorCountLimit:
                    throw new OybabException(Resources.GetRes().GetString("Exception_PasswordErrorCountLimit"));
                case ServiceExceptionType.IPConflict:
                    throw new OybabException(Resources.GetRes().GetString("Exception_IPConflict")); 
                case ServiceExceptionType.IPInvalid:
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_UnknownDevice"), Common.GetCommon().GetFormat())); //Exception_IPInvalid
                case ServiceExceptionType.UnknownDevice:
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_UnknownDevice"), Common.GetCommon().GetFormat()));
                case ServiceExceptionType.UnknownAdmin:
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_UnknownAdmin"), Common.GetCommon().GetFormat()));
                case ServiceExceptionType.SessionInvalid:
                    throw new OybabException(Resources.GetRes().GetString("Exception_SessionInvalid"));
                case ServiceExceptionType.AdminExists:
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_AdminExists"), Common.GetCommon().GetFormat()));
                case ServiceExceptionType.DeviceExists:
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_DeviceExists"), Common.GetCommon().GetFormat()));
                case ServiceExceptionType.Relogin:
                    IsExpired = true;

                    if (IsAdminUsing)
                        throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_AdminExists"), Common.GetCommon().GetFormat()));
                    else
                        throw new OybabException(Resources.GetRes().GetString("Exception_Relogin"));
                case ServiceExceptionType.AdminUsing:
                    IsAdminUsing = true;
                    throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_AdminExists"), Common.GetCommon().GetFormat()));
                case ServiceExceptionType.ProductCountLimit:
                    ExceptionPro.ExpErrorLog(Resources.GetRes().GetString("Exception_ProductCountRefreshRetry"));
                    if (ServiceGetAllProduct())
                        ExceptionPro.ExpInfoLog(Resources.GetRes().GetString("ProductCountRefreshSuccess"));
                    else
                        ExceptionPro.ExpErrorLog(Resources.GetRes().GetString("Exception_ProductCountRefreshFailed"));

                    if (!IsAuto)
                        throw new OybabException(Resources.GetRes().GetString("Exception_ProductCountRefreshExpiredRetry"));
                    break;
                case ServiceExceptionType.SessionExpired:
                    ExceptionPro.ExpErrorLog(Resources.GetRes().GetString("Exception_SessionFaildAndTry"));
                    if (ServiceSession())
                        ExceptionPro.ExpInfoLog(Resources.GetRes().GetString("SessionUpdateSuccess"));
                    else
                        ExceptionPro.ExpErrorLog(Resources.GetRes().GetString("Exception_SessionUpdateFaild"));

                    if (!IsAuto)
                        throw new OybabException(Resources.GetRes().GetString("Exception_SessionExpiredRetry"));
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
        internal ResultModel NewRequest(string adminNo, string Password)
        {
            ResultModel result = new ResultModel();
            // 已获取过就别重复获取了
            if (Resources.GetRes().IsSessionExists())
            {
                result.Result = true;
            }


            // 检查
            if (!Check())
            {
                result.Result = true;
                return result;
            }
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceNewRequest client = null;
            try
            {
                client = service.ServiceNewRequest(new ToServerServiceNewRequest() { Soft_Service_PC_Name = Resources.GetRes().SOFT_SERVICE_PC_NAME, Soft_Service_Tablet_Name = Resources.GetRes().SOFT_SERVICE_TABLET_NAME, Soft_Service_Mobile_Name = Resources.GetRes().SOFT_SERVICE_MOBILE_NAME, SessionId = Resources.GetRes().SERVER_SESSION, AdminNo = adminNo, PWD = Password, IsLocalPrintCustomOrder = Resources.GetRes().IsLocalPrintCustomOrder, DeviceType = Resources.GetRes().DevicesType, CI = GetIP(), CM = GetMac(), CurrentVersion = GetVersion(), Time = DateTime.Now });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(string.Format(Resources.GetRes().GetString("Exception_NewRequestFailt"), Common.GetCommon().GetFormat()), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);
            //设置值

            if (client.ValidResult && client.Result && client.ExceptionType == ServiceExceptionType.None && !client.IsExpired)
            {
                Resources.GetRes().KEY_NAME_0 = client.Name_0;
                Resources.GetRes().KEY_NAME_1 = client.Name_1;
                Resources.GetRes().KEY_NAME_2 = client.Name_2;
                Resources.GetRes().SERVER_SESSION = client.SessionId;
                Resources.GetRes().Rooms = client.Rooms.DeserializeObject<List<Room>>();
                Resources.GetRes().RoomsModel = client.RoomsModel.DeserializeObject<List<RoomModel>>();
                Resources.GetRes().Products = client.Products.DeserializeObject<List<Product>>();
                Resources.GetRes().ProductTypes = client.ProductTypes.DeserializeObject<List<ProductType>>();
                Resources.GetRes().Admins = client.Admins.DeserializeObject<List<Admin>>();
                Resources.GetRes().Devices = client.Devices.DeserializeObject<List<Device>>();
                Resources.GetRes().Printers = client.Printers.DeserializeObject<List<Printer>>();
                Resources.GetRes().Requests = client.Requests.DeserializeObject<List<Request>>();
                Resources.GetRes().AdminModel = client.Admin.DeserializeObject<Admin>();
                Resources.GetRes().DeviceModel = client.Device.DeserializeObject<Device>();
                Resources.GetRes().Pprs = client.Pprs.DeserializeObject<List<Ppr>>();
                Resources.GetRes().IsExpired = client.IsExpired;
                Resources.GetRes().IsFireAlarmEnable = client.IsFireAlarmEnable;
                Resources.GetRes().ExpiredRemainingDays = client.ExpiredRemaningDays;
                Resources.GetRes().RegTimeRequestCode = client.RegTimeRequestCode;
                Resources.GetRes().DeviceCount = client.DeviceCount;
                Resources.GetRes().RoomCount = client.RoomCount;
                Resources.GetRes().MinutesIntervalTime = client.MinutesIntervalTime;
                Resources.GetRes().HoursIntervalTime = client.HoursIntervalTime;
                Resources.GetRes().PrintInfo = client.PrintInfo.DeserializeObject<PrintInfo>();
                Resources.GetRes().ExtendInfo = client.ExtendInfo.DeserializeObject<ExtendInfo>();
                Resources.GetRes().Balances = client.Balances.DeserializeObject<List<Balance>>();

                // 设置会话是否过期为:否
                OperatesService.GetOperates().IsExpired = false;
                OperatesService.GetOperates().IsAdminUsing = false;
            }
            else
            {
                Resources.GetRes().IsExpired = client.IsExpired;
                Resources.GetRes().RegTimeRequestCode = client.RegTimeRequestCode;
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
        public ResultModel ChangePWD(string oldPWD, string newPWD)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceChangePWD client = null;
            try
            {
                client = service.ServiceChangePWD(new ToServerServiceChangePWD() { OldPWD = oldPWD, NewPWD = newPWD, SessionId = Resources.GetRes().SERVER_SESSION, Admin = JsonConvert.SerializeObject(Resources.GetRes().AdminModel) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_ChangePWDRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.ValidResult = client.ValidResult;
            result.Result = client.Result;

            //返回值
            return result;
        }


        private string uid;
        /// <summary>
        /// UID
        /// </summary>
        /// <returns></returns>
        public string ServiceUID()
        {
            //已获取就无需重新获取
            if (!string.IsNullOrEmpty(OperatesService.GetOperates().uid))
                return uid;

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceUID client = null;
            try
            {
                client = service.ServiceUID(new ToServerServiceUID() { SessionId = Resources.GetRes().SERVER_SESSION });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_UIDRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            OperatesService.GetOperates().uid = client.UID;

            //返回值
            return client.UID;
        }


        /// <summary>
        /// 写入设置
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        public bool ServiceSetCon(PrintInfo PrintInfo)
        {
            List<string> Config = new List<string>();
            Config.Add("PrintInfo=" + JsonConvert.SerializeObject(PrintInfo));

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceSetCon client = null;
            try
            {
                client = service.ServiceSetCon(new ToServerServiceSetCon() { Config = JsonConvert.SerializeObject(Config), SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_SetConRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            //成功了直接写入
            if (client.Result)
            {
                Resources.GetRes().PrintInfo= PrintInfo;
            }

            //返回值
            return client.Result;
        }

        /// <summary>
        /// 发送操作
        /// </summary>
        /// <param name="Rooms"></param>
        /// <param name="SendType"></param>
        /// <returns></returns>
        public bool ServiceSend(List<long> Rooms, int SendType, string Model = null)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceSend client = null;
            try
            {
                client = service.ServiceSend(new ToServerServiceSend() { RoomsId = JsonConvert.SerializeObject(Rooms), SendType = (SendType)SendType, SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId, Model = Model });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            if (client.Result)
            {
                SendType type = (SendType)SendType;

                if (type == Service.SendType.FireOn)
                {
                    Resources.GetRes().IsFireAlarmEnable = true;
                }
                else if (type == Service.SendType.FireOff)
                {
                    Resources.GetRes().IsFireAlarmEnable = false;
                }
                else if (type == Service.SendType.ExtendInfo)
                {
                    Resources.GetRes().ExtendInfo = Model.DeserializeObject<ExtendInfo>();
                }
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="ModelType"></param>
        /// <param name="Lang"></param>
        /// <returns></returns>
        internal bool ServicePrint(string Model, int ModelType, long Lang, int StatisticType = 0)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServicePrint client = null;
            try
            {
                client = service.ServicePrint(new ToServerServicePrint() { ModelType = (ModelType)ModelType, StatisticType = (StatisticType)StatisticType, SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId, Model = Model, Lang = Lang });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);


            //返回值
            return client.Result;
        }



        /// <summary>
        /// 锁住
        /// </summary>
        /// <returns></returns>
        public bool ServiceLock()
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceLock client = null;
            try
            {
                client = service.ServiceLock(new ToServerServiceLock() { SessionId = Resources.GetRes().SERVER_SESSION });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            //返回值
            return client.IsSuccessLock;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        internal bool ServiceClose()
        {
            try
            {
                //如果没获取过会话,则无需发送关闭请求
                if (string.IsNullOrEmpty(Resources.GetRes().SERVER_SESSION))
                {
                    CloseConnection();
                    return true;
                }

                //打开通道并发送获取数据
                CheckConnectionAndConnection();
                ToClientServiceClose client = null;
                try
                {
                    client = service.ServiceClose(new ToServerServiceClose() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId });
                }
                catch (Exception ex)
                {
                    CommunicationExceptionHandle(ex);
                    throw new OybabException(Resources.GetRes().GetString("Exception_CloseRequestFaild"), ex);
                }
                //处理错误
                HandleException(client.ExceptionType);

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
        /// 获取备份
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public bool ServiceGetbak(string ModelType, out string Model)
        {
            Model = null;
            if (null == Resources.GetRes().AdminModel || Resources.GetRes().AdminModel.AdminNo != "1000")
                return false;

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetbak client = null;
            try
            {
                client = service.ServiceGetbak(new ToServerServiceGetbak() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId, ModelType = ModelType});
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            if (client.Result)
                Model = client.Model;

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 设置备份
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public bool ServiceSetbak(string models)
        {
            if (null == Resources.GetRes().AdminModel || Resources.GetRes().AdminModel.AdminNo != "1000")
                return false;

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceSetbak client = null;
            try
            {
                client = service.ServiceSetbak(new ToServerServiceSetbak() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId, Model = JsonConvert.SerializeObject(models) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public bool ServiceSetContent(string value, out string Content, bool IsRemove = false, bool IsRestart = false)
        {
            if (null == Resources.GetRes().AdminModel || Resources.GetRes().AdminModel.AdminNo != "1000")
            {
                Content = null;
                return false;
            }
             

            string token = "Vod1";

            if (IsRemove)
            {
                token = "Vod0";
            }

            if (IsRestart)
                token += "1";
            else
                token += "0";


            if (value == "-1")
                token = "Vod0";


            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceSetContent client = null;
            try
            {
                client = service.ServiceSetContent(new ToServerServiceSetContent() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId, Token = token, Key = "1", Value = value });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            Content = client.Content;

            //返回值
            return client.Result;
        }

        /// <summary>
        /// 获取时间请求码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns
        public bool ServiceRequestTimeCode()
        {
            // 已获取过就别重复获取了
            if (!string.IsNullOrWhiteSpace(Resources.GetRes().RegTimeRequestCode))
            {
                return true;
            }


            // 检查
            if (!Check())
            {
                return false;
            }

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceRequestTimeCode client = null;
            try
            {
                Admin admin = Resources.GetRes().AdminModel;
                long adminId = 0;
                if (null != admin)
                    adminId = admin.AdminId;



                client = service.ServiceTimeRequestCode(new ToServerServiceRequestTimeCode() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = adminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            Resources.GetRes().RegTimeRequestCode = client.RequestCode;

            //返回值
            return true;
        }



        /// <summary>
        /// 注册时间
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public bool ServiceRegTime(string regNo)
        {

            // 检查
            if (!Check())
            {
                return false;
            }

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceRegTime client = null;
            try
            {
                Admin admin = Resources.GetRes().AdminModel;
                long adminId = 0;
                if (null != admin)
                    adminId = admin.AdminId;

                client = service.ServiceRegTime(new ToServerServiceRegTime() { SessionId = Resources.GetRes().SERVER_SESSION, RegCode = regNo, AdminId = adminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            //返回值
            return client.Result;
        }




        /// <summary>
        /// 获取数量请求码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns
        public bool ServiceRequestCountCode()
        {
            // 已获取过就别重复获取了
            if (!string.IsNullOrWhiteSpace(Resources.GetRes().RegCountRequestCode))
            {
                return true;
            }


            // 检查
            if (!Check())
            {
                return false;
            }

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceRequestCountCode client = null;
            try
            {
                client = service.ServiceCountRequestCode(new ToServerServiceRequestCountCode() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            Resources.GetRes().RegCountRequestCode = client.RequestCode;

            //返回值
            return true;
        }



        /// <summary>
        /// 注册数量
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public bool ServiceRegCount(string regNo)
        {

            // 检查
            if (!Check())
            {
                return false;
            }

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceRegCount client = null;
            try
            {
                client = service.ServiceRegCount(new ToServerServiceRegCount() { SessionId = Resources.GetRes().SERVER_SESSION, RegCode = regNo, AdminId = Resources.GetRes().AdminModel.AdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            //返回值
            return client.Result;
        }



        /// <summary>
        ///  获取模型
        /// </summary>
        /// <returns></returns>
        private bool ServiceGetModel()
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetModel client = null;
            try
            {
                client = service.ServiceGetModel(new ToServerServiceGetModel() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = Resources.GetRes().AdminModel.AdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            //返回值
            return client.Result;
        }

        /// <summary>
        /// 刷新会话
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public bool ServiceSession(bool IsAuto = false, long[] RoomsId = null, bool IsNew = false)
        {

            if (IsNew && IsTry)
                IsTry = false;

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceSession client = null;
            try
            {
                string RoomsInfoModel = null;

                client = service.ServiceSession(new ToServerServiceSession() { RoomsId = RoomsId, RoomsInfoModel = RoomsInfoModel, SessionId = Resources.GetRes().SERVER_SESSION });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_SessionRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType, IsAuto);

            //如果会话成功更新了
            if (client.ExceptionType == ServiceExceptionType.SessionUpdate)
                Resources.GetRes().SERVER_SESSION = client.NewSessionId;



            List<RoomModel> rooms = client.RoomsModel.DeserializeObject<List<RoomModel>>();




            if (null != rooms && rooms.Count > 0)
            {
                foreach (var item in rooms)
                {
                    // 成功获取了则更新

                    RoomModel oldModel = Resources.GetRes().RoomsModel.Where(x => x.RoomId == item.RoomId).FirstOrDefault();
                    Resources.GetRes().RoomsModel.Remove(oldModel);

                    Resources.GetRes().RoomsModel.Add(item);

                    Notification.Instance.ActionSendFromService(null, item.RoomId, null);

                }
            }




            //返回值
            return client.Result;
        }

        #endregion Common


        #region Room

        /// <summary>
        /// 新增包厢
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool ServiceAddRoom(Room room, out string RoomSession)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddRoom client = null;
            try
            {
                client = service.ServiceAddRoom(new ToServerServiceAddRoom() { SessionId = Resources.GetRes().SERVER_SESSION, Room = JsonConvert.SerializeObject(room) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            RoomSession = client.RoomStateSession;
            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Room model = client.Room.DeserializeObject<Room>();
                room.RoomId = model.RoomId;
                room.AddTime = model.AddTime;
            }
            
            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改包厢
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public ResultModel ServiceEditRoom(Room room, out string OrderSession)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditRoom client = null;
            try
            {
                client = service.ServiceEditRoom(new ToServerServiceEditRoom() { SessionId = Resources.GetRes().SERVER_SESSION, Room = JsonConvert.SerializeObject(room) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;
            OrderSession = client.RoomStateSession;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Room model = client.Room.DeserializeObject<Room>();
                room.UpdateTime = model.UpdateTime;
            }

            //返回值
            return result;
        }



        /// <summary>
        /// 删除包厢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelRoom(Room room)
        {
            ResultModel result = new ResultModel();
            
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelRoom client = null;
            try
            {
                client = service.ServiceDelRoom(new ToServerServiceDelRoom() { SessionId = Resources.GetRes().SERVER_SESSION, Room = JsonConvert.SerializeObject(room) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }


        #endregion Room


        #region Product

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <returns></returns>
        private bool ServiceGetAllProduct()
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetAllProduct client = null;
            try
            {
                client = service.ServiceGetAllProduct(new ToServerServiceGetAllProduct() { SessionId = Resources.GetRes().SERVER_SESSION});
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            if (client.Result)
            {
                // 有可能是刷新某个产品. 有可能刷新整个. 目前只刷新一个
                Resources.GetRes().Products = client.Products.DeserializeObject<List<Product>>();
            }

            return client.Result;
        }

        /// <summary>
        /// 新增产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool ServiceAddProduct(Product product, List<Ppr> pprs, out List<Ppr> pprsList)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddProduct client = null;
            try
            {
                client = service.ServiceAddProduct(new ToServerServiceAddProduct() { SessionId = Resources.GetRes().SERVER_SESSION, Product = JsonConvert.SerializeObject(product), Pprs = JsonConvert.SerializeObject(pprs) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Product model = client.Product.DeserializeObject<Product>();
                product.ProductId = model.ProductId;
                product.AddTime = model.AddTime;
                pprsList = client.Pprs.DeserializeObject<List<Ppr>>();
            }
            else
            {
                pprsList = new List<Ppr>();
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public ResultModel ServiceEditProduct(Product product,List<Ppr> pprs, out List<Ppr> pprsList)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditProduct client = null;
            try
            {
                client = service.ServiceEditProduct(new ToServerServiceEditProduct() { SessionId = Resources.GetRes().SERVER_SESSION, Product = JsonConvert.SerializeObject(product), Pprs = JsonConvert.SerializeObject(pprs) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }

            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Product model = client.Product.DeserializeObject<Product>();
                product.UpdateTime = model.UpdateTime;
                pprsList = client.Pprs.DeserializeObject<List<Ppr>>();
            }
            else
            {
                pprsList = new List<Ppr>();
            }

            //返回值
            return result;
        }



        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelProduct(Product product)
        {
            ResultModel result = new ResultModel();
            
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelProduct client = null;
            try
            {
                client = service.ServiceDelProduct(new ToServerServiceDelProduct() { SessionId = Resources.GetRes().SERVER_SESSION, Product = JsonConvert.SerializeObject(product)});
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }


        #endregion Product


        #region ProductType

        /// <summary>
        /// 新增产品类型
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public bool ServiceAddProductType(ProductType productType)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddProductType client = null;
            try
            {
                client = service.ServiceAddProductType(new ToServerServiceAddProductType() { SessionId = Resources.GetRes().SERVER_SESSION, ProductType = JsonConvert.SerializeObject(productType) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                ProductType model = client.ProductType.DeserializeObject<ProductType>();
                productType.ProductTypeId = model.ProductTypeId;
                productType.AddTime = model.AddTime;
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改产品类型
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        public ResultModel ServiceEditProductType(ProductType productType)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditProductType client = null;
            try
            {
                client = service.ServiceEditProductType(new ToServerServiceEditProductType() { SessionId = Resources.GetRes().SERVER_SESSION, ProductType = JsonConvert.SerializeObject(productType) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                ProductType model = client.ProductType.DeserializeObject<ProductType>();
                productType.UpdateTime = model.UpdateTime;
            }

            result.Result = client.Result;

            //返回值
            return result;
        }



        /// <summary>
        /// 删除产品类型
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelProductType(ProductType productType)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelProductType client = null;
            try
            {
                client = service.ServiceDelProductType(new ToServerServiceDelProductType() { SessionId = Resources.GetRes().SERVER_SESSION, ProductType = JsonConvert.SerializeObject(productType) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }


        #endregion ProductType


        #region Order

        /// <summary>
        /// 新建订单
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetails"></param>
        /// <param name="roomStateSession"></param>
        /// <param name="orderDetailsResult"></param>
        /// <returns></returns>
        public ResultModel ServiceAddOrder(Order order, List<OrderDetail> orderDetails, List<OrderPay> orderPays, string roomStateSession, out List<OrderDetail> orderDetailsResult, out List<OrderPay> orderPaysResult, out string newRoomStateSession, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceNewOrder client = null;
            try
            {
                client = service.ServiceAddOrder(new ToServerServiceNewOrder() { SessionId = Resources.GetRes().SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderDetails = JsonConvert.SerializeObject(orderDetails), OrderPays = JsonConvert.SerializeObject(orderPays), RoomStateSession = roomStateSession });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
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
            orderDetailsResult = client.OrderDetails.DeserializeObject<List<OrderDetail>>();
            orderPaysResult = client.OrderPays.DeserializeObject<List<OrderPay>>();
            newRoomStateSession = client.RoomSessionId;
            UpdateTime = client.UpdateTime;

            //返回值
            return result;
        }


        /// <summary>
        /// 编辑订单(只有订单, 如结账, 取消)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetails"></param>
        /// <param name="roomStateSession"></param>
        /// <returns></returns>
        public ResultModel ServiceEditOrder(Order order, List<OrderDetail> orderDetails, List<OrderPay> orderPays, string roomStateSession, bool IsRechecked, out string newRoomStateSession, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditOrder client = null;
            try
            {
                client = service.ServiceEditOrder(new ToServerServiceEditOrder() { SessionId = Resources.GetRes().SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderPays = JsonConvert.SerializeObject(orderPays), RoomStateSession = roomStateSession, Rechecked = IsRechecked });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

      
            result.Result = client.Result;
            newRoomStateSession = client.RoomStateSession;
            UpdateTime = client.UpdateTime;

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
            return result;
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
        public ResultModel ServiceReplaceOrder(long oldRoomId, long newRoomId, Order oldOrder, Order newOrder, string oldRoomSession, string newRoomSession, out string oldRoomSessionResult, out string newRoomSessionResult)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceReplaceOrder client = null;
            try
            {
                client = service.ServiceReplaceOrder(new ToServerServiceReplaceOrder() { SessionId = Resources.GetRes().SERVER_SESSION, NewRoomId = newRoomId, OldRoomId = oldRoomId, NewRoomSession = newRoomSession, OldRoomSession = oldRoomSession, OldOrder = JsonConvert.SerializeObject(oldOrder), NewOrder = JsonConvert.SerializeObject(newOrder)});
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;
            oldRoomSessionResult = client.OldRoomSession;
            newRoomSessionResult = client.NewRoomSession;


            //返回值
            return result;
        }


        /// <summary>
        /// 查找订单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="addTime"></param>
        /// <param name="finishTime"></param>
        /// <param name="roomId"></param>
        /// <param name="state"></param>
        /// <param name="IsIncludeRef"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public bool ServiceGetOrders(long startTime, long endTime, long addTimeStart, long addTimeEnd, long roomId, long state, bool IsIncludeRef, long AdminId, long FinishAdminId, out List<Order> orders)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetOrders client = null;
            try
            {
                client = service.ServiceGetOrders(new ToServerServiceGetOrders() { SessionId = Resources.GetRes().SERVER_SESSION, StartTime = startTime, EndTime = endTime, RoomId = roomId, State = state, AddTimeStart = addTimeStart, AddTimeEnd = addTimeEnd, IsIncludeRef = IsIncludeRef, AdminId = AdminId, FinishAdminId = FinishAdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            orders = client.Orders.DeserializeObject<List<Order>>();

            //返回值
            return client.Result;
        }


        #endregion Order


        #region OrderDetail

        /// <summary>
        /// 增加订单明细(顾客模式)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetailsAdd"></param>
        /// <param name="isNewOrder"></param>
        /// <param name="roomStateSession"></param>
        /// <param name="newRoomStateSession"></param>
        /// <param name="orderDetailAddResult"></param>
        /// <returns></returns>
        public ResultModel ServiceAddOrderDetail(Order order, OrderDetail orderDetailsAdd, bool isNewOrder, string roomStateSession, string newRoomStateSession, out List<OrderDetail> orderDetailAddResult)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddOrderDetail client = null;
            try
            {
                client = service.ServiceAddOrderDetail(new ToServerServiceAddOrderDetail() { SessionId = Resources.GetRes().SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderDetails = JsonConvert.SerializeObject(orderDetailsAdd), IsNewOrder = isNewOrder, RoomStateSession = roomStateSession });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newRoomStateSession = client.OrderSessionId;

            // 如果操作成功, 则更新信息
            if (client.Result && isNewOrder)
            {
                Order model = client.Order.DeserializeObject<Order>();
                order.OrderId = model.OrderId;
                order.AddTime = model.AddTime;
                order.AdminId = model.AdminId;
                order.DeviceId = model.DeviceId;
                order.Mode = model.Mode;
                order.RoomPriceCalcTime = model.RoomPriceCalcTime;
            }

            orderDetailAddResult = client.OrderDetails.DeserializeObject<List<OrderDetail>>();

            //返回值
            return result;
        }


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
        public ResultModel ServiceSaveOrderDetail(Order order, List<OrderDetail> orderDetailsAdd, List<OrderPay> orderPaysAdd, List<OrderDetail> orderDetailsEdit, List<OrderDetail> orderDetailsConfirm, string roomStateSession, out List<OrderDetail> orderDetailsAddResult, out List<OrderPay> orderPaysAddResult, out List<OrderDetail> orderDetailsEditResult, out List<OrderDetail> orderDetailsConfirmResult, out string newRoomStateSession, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceSaveOrderDetail client = null;
            try
            {
                client = service.ServiceSaveOrderDetail(new ToServerServiceSaveOrderDetail() { SessionId = Resources.GetRes().SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderDetailsAdd = JsonConvert.SerializeObject(orderDetailsAdd), OrderPaysAdd = JsonConvert.SerializeObject(orderPaysAdd), OrderDetailsEdit = JsonConvert.SerializeObject(orderDetailsEdit), OrderDetailsConfirm = JsonConvert.SerializeObject(orderDetailsConfirm), RoomStateSession = roomStateSession });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newRoomStateSession = client.OrderSessionId;
            UpdateTime = client.UpdateTime;
            orderDetailsAddResult = client.OrderDetailAdd.DeserializeObject<List<OrderDetail>>();
            orderPaysAddResult = client.OrderPayAdd.DeserializeObject<List<OrderPay>>();
            orderDetailsEditResult = client.OrderDetailEdit.DeserializeObject<List<OrderDetail>>();
            orderDetailsConfirmResult = client.OrderDetailConfirm.DeserializeObject<List<OrderDetail>>();


            // 时间不同, 说明到期时间不同(一般多见于按时间收费上的改动)
            if (null != order.EndTime)
                order.RoomPriceCalcTime = order.EndTime.Value;
            
            //返回值
            return result;
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
        public ResultModel ServiceDelOrderDetail(Order order, List<OrderDetail> orderDetails, string roomStateSession, out string newRoomStateSession, out List<OrderDetail> newOrderDetails, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelOrderDetail client = null;
            try
            {
                client = service.ServiceDelOrderDetail(new ToServerServiceDelOrderDetail() { SessionId = Resources.GetRes().SERVER_SESSION, Order = JsonConvert.SerializeObject(order), OrderDetails = JsonConvert.SerializeObject(orderDetails), RoomStateSession = roomStateSession });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newRoomStateSession = client.OrderSessionId;
            UpdateTime = client.UpdateTime;
            newOrderDetails = client.OrderDetails.DeserializeObject<List<OrderDetail>>();

            //返回值
            return result;
        }




        /// <summary>
        /// 查找订单明细(按订单)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderDetails"></param>
        /// <returns></returns>
        public bool ServiceGetOrderDetail(long orderId, out List<OrderDetail> orderDetails, out List<OrderPay> orderPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetOrderDetail client = null;
            try
            {
                client = service.ServiceGetOrderDetail(new ToServerServiceGetOrderDetail() { SessionId = Resources.GetRes().SERVER_SESSION, OrderId = orderId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            orderDetails = client.OrderDetails.DeserializeObject<List<OrderDetail>>();
            orderPays = client.OrderPays.DeserializeObject<List<OrderPay>>();

            //返回值
            return client.Result;
        }





        /// <summary>
        /// 查找订单支付
        /// </summary>
        /// <param name="addTimeStart"></param>
        /// <param name="addTimeEnd"></param>
        /// <param name="orderPays"></param>
        /// <returns></returns>
        public bool ServiceGetOrderPay(long addTimeStart, long addTimeEnd, out List<OrderPay> orderPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetOrderPay client = null;
            try
            {
                client = service.ServiceGetOrderPay(new ToServerServiceGetOrderPay() { SessionId = Resources.GetRes().SERVER_SESSION, AddTimeStart = addTimeStart, AddTimeEnd = addTimeEnd });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            orderPays = client.OrderPays.DeserializeObject<List<OrderPay>>();

            //返回值
            return client.Result;
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
        public ResultModel ServiceAddTakeout(Takeout takeout, List<TakeoutDetail> takeoutDetails, List<TakeoutPay> takeoutPays, string takeoutStateSession, out List<TakeoutDetail> takeoutDetailsResult, out List<TakeoutPay> takeoutPaysResult, out string newTakeoutStateSession, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceNewTakeout client = null;
            try
            {
                client = service.ServiceAddTakeout(new ToServerServiceNewTakeout() { SessionId = Resources.GetRes().SERVER_SESSION, Takeout = JsonConvert.SerializeObject(takeout), TakeoutDetails = JsonConvert.SerializeObject(takeoutDetails), TakeoutPays = JsonConvert.SerializeObject(takeoutPays), TakeoutStateSession = takeoutStateSession });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
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
            takeoutDetailsResult = client.TakeoutDetails.DeserializeObject<List<TakeoutDetail>>();
            takeoutPaysResult = client.TakeoutPays.DeserializeObject<List<TakeoutPay>>();
            newTakeoutStateSession = client.TakeoutSessionId;
            UpdateTime = client.UpdateTime;

            //返回值
            return result;
        }


        /// <summary>
        /// 编辑外卖(只有外卖, 如结账, 取消)
        /// </summary>
        /// <param name="takeout"></param>
        /// <param name="takeoutDetails"></param>
        /// <param name="takeoutStateSession"></param>
        /// <param name="newTakeoutStateSession"></param>
        /// <returns></returns>
        public ResultModel ServiceEditTakeout(Takeout takeout, List<TakeoutDetail> takeoutDetails, string takeoutStateSession, bool IsRechecked, out string newTakeoutStateSession, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditTakeout client = null;
            try
            {
                client = service.ServiceEditTakeout(new ToServerServiceEditTakeout() { SessionId = Resources.GetRes().SERVER_SESSION, Takeout = JsonConvert.SerializeObject(takeout), TakeoutStateSession = takeoutStateSession, Rechecked = IsRechecked });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;
            newTakeoutStateSession = client.TakeoutStateSession;
            UpdateTime = client.UpdateTime;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Takeout model = client.Takeout.DeserializeObject<Takeout>();
                takeout.FinishTime = model.FinishTime;
                takeout.UpdateTime = model.UpdateTime;
                takeout.ReCheckedCount = model.ReCheckedCount;
            }

            //返回值
            return result;
        }
        


        /// <summary>
        /// 查找外卖
        /// </summary>
        /// <param name="sendAdminId"></param>
        /// <param name="addTime"></param>
        /// <param name="finishTime"></param>
        /// <param name="state"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="IsFromCacheOnly"></param>
        /// <param name="takeouts"></param>
        /// <returns></returns>
        public bool ServiceGetTakeouts(long sendAdminId, long addTimeStart, long addTimeEnd, long state, string name, string phone, bool IsFromCacheOnly, bool IsIncludeRef, long AdminId, long FinishAdminId, out List<Takeout> takeouts)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetTakeouts client = null;
            try
            {
                client = service.ServiceGetTakeout(new ToServerServiceGetTakeouts() { SessionId = Resources.GetRes().SERVER_SESSION, State = state, AddTimeStart = addTimeStart, AddTimeEnd = addTimeEnd, IsFromCacheOnly = IsFromCacheOnly, IsIncludeRef = IsIncludeRef, Name = name, Phone = phone, SendAdminId = sendAdminId, AdminId = AdminId, FinishAdminId = FinishAdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            takeouts = client.Takeouts.DeserializeObject<List<Takeout>>();

            //返回值
            return client.Result;
        }


        #endregion Takeout


        #region TakeoutDetail




        /// <summary>
        /// 修改外卖明细
        /// </summary>
        /// <param name="takeout"></param>
        /// <param name="takeoutDetailsAdd"></param>
        /// <param name="takeoutDetailsEdit"></param>
        /// <param name="takeoutDetailsConfirm"></param>
        /// <param name="takeoutStateSession"></param>
        /// <param name="takeoutDetailsAddResult"></param>
        /// <param name="takeoutDetailsEditResult"></param>
        /// <param name="takeoutDetailsConfirmResult"></param>
        /// <param name="newTakeoutStateSession"></param>
        /// <returns></returns>
        public ResultModel ServiceSaveTakeoutDetail(Takeout takeout, List<TakeoutDetail> takeoutDetailsAdd, List<TakeoutDetail> takeoutDetailsEdit, List<TakeoutDetail> takeoutDetailsConfirm, string takeoutStateSession, out List<TakeoutDetail> takeoutDetailsAddResult, out List<TakeoutDetail> takeoutDetailsEditResult, out List<TakeoutDetail> takeoutDetailsConfirmResult, out string newTakeoutStateSession, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceSaveTakeoutDetail client = null;
            try
            {
                client = service.ServiceSaveTakeoutDetail(new ToServerServiceSaveTakeoutDetail() { SessionId = Resources.GetRes().SERVER_SESSION, Takeout = JsonConvert.SerializeObject(takeout), TakeoutDetailsAdd = JsonConvert.SerializeObject(takeoutDetailsAdd), TakeoutDetailsEdit = JsonConvert.SerializeObject(takeoutDetailsEdit), TakeoutDetailsConfirm = JsonConvert.SerializeObject(takeoutDetailsConfirm), TakeoutStateSession = takeoutStateSession });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newTakeoutStateSession = client.TakeoutSessionId;
            UpdateTime = client.UpdateTime;
            takeoutDetailsAddResult = client.TakeoutDetailAdd.DeserializeObject<List<TakeoutDetail>>();
            takeoutDetailsEditResult = client.TakeoutDetailEdit.DeserializeObject<List<TakeoutDetail>>();
            takeoutDetailsConfirmResult = client.TakeoutDetailConfirm.DeserializeObject<List<TakeoutDetail>>();

            

            //返回值
            return result;
        }


        /// <summary>
        /// 删除外卖明细
        /// </summary>
        /// <param name="order"></param>
        /// <param name="takeoutDetails"></param>
        /// <param name="takeoutStateSession"></param>
        /// <param name="newTakeoutStateSession"></param>
        /// <returns></returns>
        public ResultModel ServiceDelTakeoutDetail(Takeout order, List<TakeoutDetail> takeoutDetails, string takeoutStateSession, out string newTakeoutStateSession, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelTakeoutDetail client = null;
            try
            {
                client = service.ServiceDelTakeoutDetail(new ToServerServiceDelTakeoutDetail() { SessionId = Resources.GetRes().SERVER_SESSION, Takeout = JsonConvert.SerializeObject(order), TakeoutDetails = JsonConvert.SerializeObject(takeoutDetails), TakeoutStateSession = takeoutStateSession });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newTakeoutStateSession = client.TakeoutSessionId;
            UpdateTime = client.UpdateTime;

            //返回值
            return result;
        }




        /// <summary>
        /// 查找外卖明细(按外卖)
        /// </summary>
        /// <param name="takeoutId"></param>
        /// <param name="takeoutDetails"></param>
        /// <returns></returns>
        public bool ServiceGetTakeoutDetail(long takeoutId, out List<TakeoutDetail> takeoutDetails, out List<TakeoutPay> takeoutPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetTakeoutDetail client = null;
            try
            {
                client = service.ServiceGetTakeoutDetail(new ToServerServiceGetTakeoutDetail() { SessionId = Resources.GetRes().SERVER_SESSION, TakeoutId = takeoutId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            takeoutDetails = client.TakeoutDetails.DeserializeObject<List<TakeoutDetail>>();
            takeoutPays = client.TakeoutPays.DeserializeObject<List<TakeoutPay>>();

            //返回值
            return client.Result;
        }




        /// <summary>
        /// 查找外卖支付
        /// </summary>
        /// <param name="addTimeStart"></param>
        /// <param name="addTimeEnd"></param>
        /// <param name="takeoutPays"></param>
        /// <returns></returns>
        public bool ServiceGetTakeoutPay(long addTimeStart, long addTimeEnd, out List<TakeoutPay> takeoutPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetTakeoutPay client = null;
            try
            {
                client = service.ServiceGetTakeoutPay(new ToServerServiceGetTakeoutPay() { SessionId = Resources.GetRes().SERVER_SESSION, AddTimeStart = addTimeStart, AddTimeEnd = addTimeEnd });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            takeoutPays = client.TakeoutPays.DeserializeObject<List<TakeoutPay>>();

            //返回值
            return client.Result;
        }



        #endregion TakeoutDetail


        #region ImportWithDetails

        /// <summary>
        /// 增加进货及明细
        /// </summary>
        /// <param name="import"></param>
        /// <param name="importDetails"></param>
        /// <param name="importDetailsAddResult"></param>
        /// <returns></returns>
        public ResultModel ServiceAddImportWithDetail(Import import, List<ImportDetail> importDetails, List<ImportPay> importPays, out List<ImportDetail> importDetailsAddResult, out List<ImportPay> importPaysAddResult, out long UpdateTime)
        {
            ResultModel result = new ResultModel();
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceNewImport client = null;
            try
            {
                client = service.ServiceAddImportWithDetail(new ToServerServiceNewImport() { SessionId = Resources.GetRes().SERVER_SESSION, Import = JsonConvert.SerializeObject(import), ImportDetails = JsonConvert.SerializeObject(importDetails), ImportPays = JsonConvert.SerializeObject(importPays) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            importDetailsAddResult = client.ImportDetails.DeserializeObject<List<ImportDetail>>();
            importPaysAddResult = client.ImportPays.DeserializeObject<List<ImportPay>>();

            UpdateTime = client.UpdateTime;

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
            return result;
        }




        /// <summary>
        /// 编辑进货(只有进货, 如结账, 取消)
        /// </summary>
        /// <param name="import"></param>
        /// <param name="IsRechecked"></param>
        /// <param name="UpdateTime"></param>
        /// <returns></returns>
        public ResultModel ServiceEditImport(Import import, bool IsRechecked, out long UpdateTime)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditImport client = null;
            try
            {
                client = service.ServiceEditImport(new ToServerServiceEditImport() { SessionId = Resources.GetRes().SERVER_SESSION, Import = JsonConvert.SerializeObject(import), Rechecked = IsRechecked });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;
            UpdateTime = client.UpdateTime;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Import model = client.Import.DeserializeObject<Import>();
                model.UpdateTime = model.UpdateTime;
                model.ReCheckedCount = model.ReCheckedCount;

            }

            //返回值
            return result;
        }



        /// <summary>
        /// 查找进货
        /// </summary>
        /// <param name="importTimeStart"></param>
        /// <param name="importTimeEnd"></param>
        /// <param name="IsIncludeRef"></param>
        /// <param name="imports"></param>
        /// <returns></returns>
        public bool ServiceGetImports(long addTimeStart, long addTimeEnd, bool IsIncludeRef, out List<Import> imports)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetImports client = null;
            try
            {
                client = service.ServiceGetImports(new ToServerServiceGetImports() { SessionId = Resources.GetRes().SERVER_SESSION, AddTimeStart = addTimeStart, AddTimeEnd = addTimeEnd, IsIncludeRef = IsIncludeRef });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            imports = client.Imports.DeserializeObject<List<Import>>();

            //返回值
            return client.Result;
        }



        /// <summary>
        /// 查找进货明细(按订单)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public bool ServiceGetImportDetail(long importId, out List<ImportDetail> importDetails, out List<ImportPay> importPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetImportDetail client = null;
            try
            {
                client = service.ServiceGeImportDetail(new ToServerServiceGetImportDetail() { SessionId = Resources.GetRes().SERVER_SESSION, ImportId = importId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            importDetails = client.ImportDetails.DeserializeObject<List<ImportDetail>>();
            importPays = client.ImportPays.DeserializeObject<List<ImportPay>>();

            //返回值
            return client.Result;
        }




        /// <summary>
        /// 查找进货支付
        /// </summary>
        /// <param name="addTimeStart"></param>
        /// <param name="addTimeEnd"></param>
        /// <param name="importPays"></param>
        /// <returns></returns>
        public bool ServiceGetImportPay(long addTimeStart, long addTimeEnd, out List<ImportPay> importPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetImportPay client = null;
            try
            {
                client = service.ServiceGetImportPay(new ToServerServiceGetImportPay() { SessionId = Resources.GetRes().SERVER_SESSION, AddTimeStart = addTimeStart, AddTimeEnd = addTimeEnd });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            importPays = client.ImportPays.DeserializeObject<List<ImportPay>>();

            //返回值
            return client.Result;
        }


        #endregion ImportWithDetails


        #region Admin

        /// <summary>
        /// 新增管理员
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public bool ServiceAddAdmin(Admin admin)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddAdmin client = null;
            try
            {
                client = service.ServiceAddAdmin(new ToServerServiceAddAdmin() { SessionId = Resources.GetRes().SERVER_SESSION, Admin = JsonConvert.SerializeObject(admin) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Admin model = client.Admin.DeserializeObject<Admin>();
                admin.AdminId = model.AdminId;
                admin.AddTime = model.AddTime;
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改管理员
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public ResultModel ServiceEditAdmin(Admin admin)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditAdmin client = null;
            try
            {
                client = service.ServiceEditAdmin(new ToServerServiceEditAdmin() { SessionId = Resources.GetRes().SERVER_SESSION, Admin = JsonConvert.SerializeObject(admin) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }

            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Admin model = client.Admin.DeserializeObject<Admin>();
                admin.UpdateTime = model.UpdateTime;
            }


            //返回值
            return result;
        }





        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public ResultModel ServiceResetAdmin(Admin admin, string Password)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceResetAdmin client = null;
            try
            {
                client = service.ServiceResetAdmin(new ToServerServiceResetAdmin() { SessionId = Resources.GetRes().SERVER_SESSION, Admin = JsonConvert.SerializeObject(admin), Password = Password });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }


            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Admin model = client.Admin.DeserializeObject<Admin>();
                admin.Password = model.Password;
                admin.UpdateTime = model.UpdateTime;
            }


            //返回值
            return result;
        }



        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelAdmin(Admin admin)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelAdmin client = null;
            try
            {
                client = service.ServiceDelAdmin(new ToServerServiceDelAdmin() { SessionId = Resources.GetRes().SERVER_SESSION, Admin = JsonConvert.SerializeObject(admin) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }


        #endregion Admin


        #region AdminPay


        /// <summary>
        /// 增加管理员支付
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="adminPay"></param>
        /// <param name="newAdmin"></param>
        /// <param name="newAdminPay"></param>
        /// <returns></returns>
        public ResultModel ServiceAddAdminPay(Admin admin, AdminPay adminPay, out Admin newAdmin, out AdminPay newAdminPay)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddAdminPay client = null;
            try
            {
                client = service.ServiceAddAdminPay(new ToServerServiceAddAdminPay() { SessionId = Resources.GetRes().SERVER_SESSION, Admin = JsonConvert.SerializeObject(admin), AdminPay = JsonConvert.SerializeObject(adminPay) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newAdmin = client.Admin.DeserializeObject<Admin>();
            newAdminPay = client.AdminPay.DeserializeObject<AdminPay>();


            //返回值
            return result;
        }

        /// <summary>
        /// 删除管理员支付
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="adminPay"></param>
        /// <param name="newAdmin"></param>
        /// <returns></returns>
        public ResultModel ServiceDelAdminPay(Admin admin, AdminPay adminPay, out Admin newAdmin)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelAdminPay client = null;
            try
            {
                client = service.ServiceDelAdminPay(new ToServerServiceDelAdminPay() { SessionId = Resources.GetRes().SERVER_SESSION, Admin = JsonConvert.SerializeObject(admin), AdminPay = JsonConvert.SerializeObject(adminPay) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newAdmin = client.Admin.DeserializeObject<Admin>();

            //返回值
            return result;
        }




        /// <summary>
        /// 查找管理员支付(按管理员)
        /// </summary>
        /// <param name="AdminId"></param>
        /// <param name="NotAdminId"></param>
        /// <param name="AddTimeStart"></param>
        /// <param name="AddTimeEnd"></param>
        /// <param name="adminPays"></param>
        /// <returns></returns>
        public bool ServiceGetAdminPay(long AdminId, long NotAdminId, long AddTimeStart, long AddTimeEnd, long AddAdminId, out List<AdminPay> adminPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetAdminPay client = null;
            try
            {
                client = service.ServiceGetAdminPay(new ToServerServiceGetAdminPay() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = AdminId, AddTimeStart = AddTimeStart, AddTimeEnd = AddTimeEnd, NotAdminId = NotAdminId, AddAdminId = AddAdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            adminPays = client.AdminPays.DeserializeObject<List<AdminPay>>();

            //返回值
            return client.Result;
        }

        #endregion AdminPay


        #region Balance

        /// <summary>
        /// 新增余额
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool ServiceAddBalance(Balance balance, out bool IsBalanceExists)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddBalance client = null;
            try
            {
                client = service.ServiceAddBalance(new ToServerServiceAddBalance() { SessionId = Resources.GetRes().SERVER_SESSION, Balance = JsonConvert.SerializeObject(balance) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Balance model = client.Balance.DeserializeObject<Balance>();
                balance.BalanceId = model.BalanceId;
                balance.AddTime = model.AddTime;
            }
            IsBalanceExists = client.IsBalanceExists;

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改余额
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public ResultModel ServiceEditBalance(Balance balance, out bool IsBalanceExists)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditBalance client = null;
            try
            {
                client = service.ServiceEditBalance(new ToServerServiceEditBalance() { SessionId = Resources.GetRes().SERVER_SESSION, Balance = JsonConvert.SerializeObject(balance) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Balance model = client.Balance.DeserializeObject<Balance>();
                balance.UpdateTime = model.UpdateTime;
                balance.BalancePrice = model.BalancePrice;
            }

            IsBalanceExists = client.IsBalanceExists;

            //返回值
            return result;
        }



        /// <summary>
        /// 删除余额
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelBalance(Balance balance)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelBalance client = null;
            try
            {
                client = service.ServiceDelBalance(new ToServerServiceDelBalance() { SessionId = Resources.GetRes().SERVER_SESSION, Balance = JsonConvert.SerializeObject(balance) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }



        /// <summary>
        /// 查找余额
        /// </summary>
        /// <param name="balanceId"></param>
        /// <param name="balances"></param>
        /// <returns></returns>
        public bool ServiceGetBalances(long balanceId, out List<Balance> balances)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetBalance client = null;
            try
            {
                client = service.ServiceGetBalances(new ToServerServiceGetBalance() { SessionId = Resources.GetRes().SERVER_SESSION, BalanceId = balanceId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            balances = client.Balances.DeserializeObject<List<Balance>>();

            //返回值
            return client.Result;
        }


        #endregion Balance


        #region BalancePay


        /// <summary>
        /// 增加余额支付
        /// </summary>
        /// <param name="balancePay"></param>
        /// <param name="newBalancePay"></param>
        /// <returns></returns>
        public ResultModel ServiceAddBalancePay(BalancePay balancePay, out BalancePay newBalancePay, out Balance newBalance)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddBalancePay client = null;
            try
            {
                client = service.ServiceAddBalancePay(new ToServerServiceAddBalancePay() { SessionId = Resources.GetRes().SERVER_SESSION, BalancePay = JsonConvert.SerializeObject(balancePay) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newBalancePay = client.BalancePay.DeserializeObject<BalancePay>();
            newBalance = client.Balance.DeserializeObject<Balance>();


            //返回值
            return result;
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
        public ResultModel ServiceTransferBalancePay(BalancePay balancePay1, BalancePay balancePay2, out BalancePay newBalancePay1, out Balance newBalance1, out BalancePay newBalancePay2, out Balance newBalance2)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceTransferBalancePay client = null;
            try
            {
                client = service.ServiceTransferBalancePay(new ToServerServiceTransferBalancePay() { SessionId = Resources.GetRes().SERVER_SESSION, BalancePay1 = JsonConvert.SerializeObject(balancePay1), BalancePay2 = JsonConvert.SerializeObject(balancePay2) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newBalancePay1 = client.BalancePay1.DeserializeObject<BalancePay>();
            newBalance1 = client.Balance1.DeserializeObject<Balance>();
            newBalancePay2 = client.BalancePay2.DeserializeObject<BalancePay>();
            newBalance2 = client.Balance2.DeserializeObject<Balance>();


            //返回值
            return result;
        }

        /// <summary>
        /// 删除余额支付
        /// </summary>
        /// <param name="balancePay"></param>
        /// <returns></returns>
        public ResultModel ServiceDelBalancePay(BalancePay balancePay)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelBalancePay client = null;
            try
            {
                client = service.ServiceDelBalancePay(new ToServerServiceDelBalancePay() { SessionId = Resources.GetRes().SERVER_SESSION, BalancePay = JsonConvert.SerializeObject(balancePay) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);


            result.Result = client.Result;

            //返回值
            return result;
        }




        /// <summary>
        /// 查找余额支付
        /// </summary>
        /// <param name="BalanceId"></param>
        /// <param name="AdminId"></param>
        /// <param name="NotAdminId"></param>
        /// <param name="AddTimeStart"></param>
        /// <param name="AddTimeEnd"></param>
        /// <param name="balancePays"></param>
        /// <returns></returns>
        public bool ServiceGetBalancePay(long BalanceType, long BalanceId, long AdminId, long NotAdminId, long AddTimeStart, long AddTimeEnd, out List<BalancePay> balancePays, out List<OrderPay> orderPays, out List<TakeoutPay> takeoutPays, out List<MemberPay> memberPays, out List<SupplierPay> supplierPays, out List<AdminPay> adminPays, out List<ImportPay> importPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetBalancePay client = null;
            try
            {
                client = service.ServiceGetBalancePay(new ToServerServiceGetBalancePay() { SessionId = Resources.GetRes().SERVER_SESSION, BalanceType = BalanceType, AdminId = AdminId, AddTimeStart = AddTimeStart, AddTimeEnd = AddTimeEnd, NotAdminId = NotAdminId, BalanceId = BalanceId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            balancePays = client.BalancePays.DeserializeObject<List<BalancePay>>();
            orderPays = client.OrderPays.DeserializeObject<List<OrderPay>>();
            takeoutPays = client.TakeoutPays.DeserializeObject<List<TakeoutPay>>();
            memberPays = client.MemberPays.DeserializeObject<List<MemberPay>>();
            adminPays = client.AdminPays.DeserializeObject<List<AdminPay>>();
            supplierPays = client.SupplierPays.DeserializeObject<List<SupplierPay>>();
            importPays = client.ImportPays.DeserializeObject<List<ImportPay>>();

            //返回值
            return client.Result;
        }

        #endregion BalancePay



        #region AdminLog

        /// <summary>
        /// 管理员日志
        /// </summary>
        /// <param name="adminLog"></param>
        /// <returns></returns>
        public bool ServiceAddAdminLog(AdminLog adminLog)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddAdminLog client = null;
            try
            {
                client = service.ServiceAddAdminLog(new ToServerServiceAddAdminLog() { SessionId = Resources.GetRes().SERVER_SESSION, AdminLog = JsonConvert.SerializeObject(adminLog) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                AdminLog model = client.AdminLog.DeserializeObject<AdminLog>();
                adminLog.AdminLogId = model.AdminLogId;
                adminLog.AddTime = model.AddTime;

                adminLog.AdminId = model.AdminId;
                adminLog.DeviceId = model.DeviceId;
                adminLog.Mode = model.Mode;
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改管理员日志
        /// </summary>
        /// <param name="adminLog"></param>
        /// <returns></returns>
        public ResultModel ServiceEditAdminLog(AdminLog adminLog)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditAdminLog client = null;
            try
            {
                client = service.ServiceEditAdminLog(new ToServerServiceEditAdminLog() { SessionId = Resources.GetRes().SERVER_SESSION, AdminLog = JsonConvert.SerializeObject(adminLog) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                AdminLog model = client.AdminLog.DeserializeObject<AdminLog>();
                adminLog.UpdateTime = model.UpdateTime;
            }

            //返回值
            return result;
        }



        /// <summary>
        /// 删除管理员日志
        /// </summary>
        /// <param name="adminLog"></param>
        /// <returns></returns>
        public ResultModel ServiceDelAdminLog(AdminLog adminLog)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelAdminLog client = null;
            try
            {
                client = service.ServiceDelAdminLog(new ToServerServiceDelAdminLog() { SessionId = Resources.GetRes().SERVER_SESSION, AdminLog = JsonConvert.SerializeObject(adminLog) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }



        /// <summary>
        /// 查找管理员日志(按管理员)
        /// </summary>
        /// <param name="AdminId"></param>
        /// <param name="AddTimeStart"></param>
        /// <param name="AddTimeEnd"></param>
        /// <param name="adminLogs"></param>
        /// <returns></returns>
        public bool ServiceGetAdminLog(long AdminId, long AddTimeStart, long AddTimeEnd, out List<AdminLog> adminLogs)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetAdminLog client = null;
            try
            {
                client = service.ServiceGetAdminLog(new ToServerServiceGetAdminLog() { SessionId = Resources.GetRes().SERVER_SESSION, AdminId = AdminId, AddTimeStart = AddTimeStart, AddTimeEnd = AddTimeEnd });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            adminLogs = client.AdminLogs.DeserializeObject<List<AdminLog>>();

            //返回值
            return client.Result;
        }


        #endregion AdminLog


        #region Member

        /// <summary>
        /// 新增会员
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool ServiceAddMember(Member member, out bool IsMemberExists, out bool IsCardExists)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddMember client = null;
            try
            {
                client = service.ServiceAddMember(new ToServerServiceAddMember() { SessionId = Resources.GetRes().SERVER_SESSION, Member = JsonConvert.SerializeObject(member) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Member model = client.Member.DeserializeObject<Member>();
                member.AdminId = model.AdminId;
                member.MemberId = model.MemberId;
                member.MemberNo = model.MemberNo;
                member.AddTime = model.AddTime;
            }
            IsMemberExists = client.IsMemberExists;
            IsCardExists = client.IsCardExists;

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改会员
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public ResultModel ServiceEditMember(Member member, out bool IsMemberExists, out bool IsCardExists)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditMember client = null;
            try
            {
                client = service.ServiceEditMember(new ToServerServiceEditMember() { SessionId = Resources.GetRes().SERVER_SESSION, Member = JsonConvert.SerializeObject(member) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Member model = client.Member.DeserializeObject<Member>();
                member.UpdateTime = model.UpdateTime;
            }

            IsMemberExists = client.IsMemberExists;
            IsCardExists = client.IsCardExists;

            //返回值
            return result;
        }



        /// <summary>
        /// 删除会员
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelMember(Member member)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelMember client = null;
            try
            {
                client = service.ServiceDelMember(new ToServerServiceDelMember() { SessionId = Resources.GetRes().SERVER_SESSION, Member = JsonConvert.SerializeObject(member) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }



        /// <summary>
        /// 查找会员
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberNo"></param>
        /// <param name="cardNo"></param>
        /// <param name="Name"></param>
        /// <param name="Phone"></param>
        /// <param name="SingleMemberNo"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public bool ServiceGetMembers(long memberId, string memberNo, string cardNo, string Name, string Phone, bool SingleMemberNo, out List<Member> members)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetMember client = null;
            try
            {
                client = service.ServiceGetMembers(new ToServerServiceGetMember() { SessionId = Resources.GetRes().SERVER_SESSION, MemberId = memberId, MemberNo = memberNo, Name = Name, Phone = Phone, SingleMemberNo = SingleMemberNo, CardNo = cardNo });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            members = client.Members.DeserializeObject<List<Member>>();

            //返回值
            return client.Result;
        }


        #endregion Member


        #region MemberPay


        /// <summary>
        /// 增加会员支付
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberPay"></param>
        /// <param name="newMember"></param>
        /// <param name="newMemberPay"></param>
        /// <returns></returns>
        public ResultModel ServiceAddMemberPay(Member member, MemberPay memberPay, out Member newMember, out MemberPay newMemberPay)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddMemberPay client = null;
            try
            {
                client = service.ServiceAddMemberPay(new ToServerServiceAddMemberPay() { SessionId = Resources.GetRes().SERVER_SESSION, Member = JsonConvert.SerializeObject(member), MemberPay = JsonConvert.SerializeObject(memberPay) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newMember = client.Member.DeserializeObject<Member>();
            newMemberPay = client.MemberPay.DeserializeObject<MemberPay>();


            //返回值
            return result;
        }

        /// <summary>
        /// 删除会员支付
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberPay"></param>
        /// <param name="newMember"></param>
        /// <returns></returns>
        public ResultModel ServiceDelMemberPay(Member member, MemberPay memberPay, out Member newMember)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelMemberPay client = null;
            try
            {
                client = service.ServiceDelMemberPay(new ToServerServiceDelMemberPay() { SessionId = Resources.GetRes().SERVER_SESSION, Member = JsonConvert.SerializeObject(member), MemberPay = JsonConvert.SerializeObject(memberPay) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newMember = client.Member.DeserializeObject<Member>();

            //返回值
            return result;
        }




        /// <summary>
        /// 查找会员支付(按会员)
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="AddTimeStart"></param>
        /// <param name="AddTimeEnd"></param>
        /// <param name="memberPays"></param>
        /// <returns></returns>
        public bool ServiceGetMemberPay(long BalanceType, long MemberId, long AddTimeStart, long AddTimeEnd, long AddAdminId, out List<MemberPay> memberPays, out List<OrderPay> orderPays, out List<TakeoutPay> takeoutPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetMemberPay client = null;
            try
            {
                client = service.ServiceGetMemberPay(new ToServerServiceGetMemberPay() { SessionId = Resources.GetRes().SERVER_SESSION, BalanceType = BalanceType, MemberId = MemberId, AddTimeStart = AddTimeStart, AddTimeEnd = AddTimeEnd, AddAdminId = AddAdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            memberPays = client.MemberPays.DeserializeObject<List<MemberPay>>();
            orderPays = client.OrderPays.DeserializeObject<List<OrderPay>>();
            takeoutPays = client.TakeoutPays.DeserializeObject<List<TakeoutPay>>();

            //返回值
            return client.Result;
        }

        #endregion MemberPay


        #region Supplier

        /// <summary>
        /// 新增供应商
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public bool ServiceAddSupplier(Supplier supplier, out bool IsSupplierExists, out bool IsCardExists)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddSupplier client = null;
            try
            {
                client = service.ServiceAddSupplier(new ToServerServiceAddSupplier() { SessionId = Resources.GetRes().SERVER_SESSION, Supplier = JsonConvert.SerializeObject(supplier) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Supplier model = client.Supplier.DeserializeObject<Supplier>();
                supplier.AdminId = model.AdminId;
                supplier.SupplierId = model.SupplierId;
                supplier.SupplierNo = model.SupplierNo;
                supplier.AddTime = model.AddTime;
            }

            IsSupplierExists = client.IsSupplierExists;
            IsCardExists = client.IsCardExists;

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public ResultModel ServiceEditSupplier(Supplier supplier, out bool IsSupplierExists, out bool IsCardExists)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditSupplier client = null;
            try
            {
                client = service.ServiceEditSupplier(new ToServerServiceEditSupplier() { SessionId = Resources.GetRes().SERVER_SESSION, Supplier = JsonConvert.SerializeObject(supplier) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Supplier model = client.Supplier.DeserializeObject<Supplier>();
                supplier.UpdateTime = model.UpdateTime;
            }

            IsSupplierExists = client.IsSupplierExists;
            IsCardExists = client.IsCardExists;

            //返回值
            return result;
        }



        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public ResultModel ServiceDelSupplier(Supplier supplier)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelSupplier client = null;
            try
            {
                client = service.ServiceDelSupplier(new ToServerServiceDelSupplier() { SessionId = Resources.GetRes().SERVER_SESSION, Supplier = JsonConvert.SerializeObject(supplier) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }



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
        public bool ServiceGetSupplier(long supplierId, string supplierNo, string cardNo, string Name, string Phone, bool SingleSupplierNo, out List<Supplier> suppliers)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetSupplier client = null;
            try
            {
                client = service.ServiceGetSupplier(new ToServerServiceGetSupplier() { SessionId = Resources.GetRes().SERVER_SESSION, SupplierId = supplierId, SupplierNo = supplierNo, Name = Name, Phone = Phone, SingleSupplierNo = SingleSupplierNo, CardNo = cardNo });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            suppliers = client.Suppliers.DeserializeObject<List<Supplier>>();

            //返回值
            return client.Result;
        }


        #endregion Supplier


        #region SupplierPay


        /// <summary>
        /// 增加供应商支付
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="supplierPay"></param>
        /// <param name="newSupplier"></param>
        /// <param name="newSupplierPay"></param>
        /// <returns></returns>
        public ResultModel ServiceAddSupplierPay(Supplier supplier, SupplierPay supplierPay, out Supplier newSupplier, out SupplierPay newSupplierPay)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddSupplierPay client = null;
            try
            {
                client = service.ServiceAddSupplierPay(new ToServerServiceAddSupplierPay() { SessionId = Resources.GetRes().SERVER_SESSION, Supplier = JsonConvert.SerializeObject(supplier), SupplierPay = JsonConvert.SerializeObject(supplierPay) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newSupplier = client.Supplier.DeserializeObject<Supplier>();
            newSupplierPay = client.SupplierPay.DeserializeObject<SupplierPay>();


            //返回值
            return result;
        }

        /// <summary>
        /// 删除供应商支付
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="supplierPay"></param>
        /// <param name="newSupplier"></param>
        /// <returns></returns>
        public ResultModel ServiceDelSupplierPay(Supplier supplier, SupplierPay supplierPay, out Supplier newSupplier)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelSupplierPay client = null;
            try
            {
                client = service.ServiceDelSupplierPay(new ToServerServiceDelSupplierPay() { SessionId = Resources.GetRes().SERVER_SESSION, Supplier = JsonConvert.SerializeObject(supplier), SupplierPay = JsonConvert.SerializeObject(supplierPay) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            newSupplier = client.Supplier.DeserializeObject<Supplier>();

            //返回值
            return result;
        }




        /// <summary>
        /// 查找供应商支付(按供应商)
        /// </summary>
        /// <param name="SupplierId"></param>
        /// <param name="AddTimeStart"></param>
        /// <param name="AddTimeEnd"></param>
        /// <param name="supplierPays"></param>
        /// <returns></returns>
        public bool ServiceGetSupplierPay(long BalanceType, long SupplierId, long AddTimeStart, long AddTimeEnd, long AddAdminId, out List<SupplierPay> supplierPays, out List<ImportPay> importPays)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetSupplierPay client = null;
            try
            {
                client = service.ServiceGetSupplierPay(new ToServerServiceGetSupplierPay() { SessionId = Resources.GetRes().SERVER_SESSION, BalanceType = BalanceType, SupplierId = SupplierId, AddTimeStart = AddTimeStart, AddTimeEnd = AddTimeEnd, AddAdminId = AddAdminId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            supplierPays = client.SupplierPays.DeserializeObject<List<SupplierPay>>();
            importPays = client.ImportPays.DeserializeObject<List<ImportPay>>();

            //返回值
            return client.Result;
        }

        #endregion SupplierPay


        #region Printer

        /// <summary>
        /// 新增打印机
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        public bool ServiceAddPrinter(Printer printer)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddPrinter client = null;
            try
            {
                client = service.ServiceAddPrinter(new ToServerServiceAddPrinter() { SessionId = Resources.GetRes().SERVER_SESSION, Printer = JsonConvert.SerializeObject(printer) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Printer model = client.Printer.DeserializeObject<Printer>();
                printer.PrinterId = model.PrinterId;
                printer.AddTime = model.AddTime;
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改打印机
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        public ResultModel ServiceEditPrinter(Printer printer)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditPrinter client = null;
            try
            {
                client = service.ServiceEditPrinter(new ToServerServiceEditPrinter() { SessionId = Resources.GetRes().SERVER_SESSION, Printer = JsonConvert.SerializeObject(printer) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Printer model = client.Printer.DeserializeObject<Printer>();
                printer.UpdateTime = model.UpdateTime;
            }

            //返回值
            return result;
        }



        /// <summary>
        /// 删除打印机
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelPrinter(Printer printer)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelPrinter client = null;
            try
            {
                client = service.ServiceDelPrinter(new ToServerServiceDelPrinter() { SessionId = Resources.GetRes().SERVER_SESSION, Printer = JsonConvert.SerializeObject(printer) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }


        #endregion Printer


        #region Device

        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool ServiceAddDevice(Device device)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddDevice client = null;
            try
            {
                client = service.ServiceAddDevice(new ToServerServiceAddDevice() { SessionId = Resources.GetRes().SERVER_SESSION, Device = JsonConvert.SerializeObject(device) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Device model = client.Device.DeserializeObject<Device>();
                device.DeviceId = model.DeviceId;
                device.AddTime = model.AddTime;
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改设备
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public ResultModel ServiceEditDevice(Device device)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditDevice client = null;
            try
            {
                client = service.ServiceEditDevice(new ToServerServiceEditDevice() { SessionId = Resources.GetRes().SERVER_SESSION, Device = JsonConvert.SerializeObject(device) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Device model = client.Device.DeserializeObject<Device>();
                device.UpdateTime = model.UpdateTime;
            }

            //返回值
            return result;
        }



        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelDevice(Device device)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelDevice client = null;
            try
            {
                client = service.ServiceDelDevice(new ToServerServiceDelDevice() { SessionId = Resources.GetRes().SERVER_SESSION, Device = JsonConvert.SerializeObject(device) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }


        #endregion Device



        #region Request

        /// <summary>
        /// 新增请求
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        public bool ServiceAddRequest(Request request)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceAddRequest client = null;
            try
            {
                client = service.ServiceAddRequest(new ToServerServiceAddRequest() { SessionId = Resources.GetRes().SERVER_SESSION, Request = JsonConvert.SerializeObject(request) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Request model = client.Request.DeserializeObject<Request>();
                request.RequestId = model.RequestId;
                request.AddTime = model.AddTime;
            }

            //返回值
            return client.Result;
        }


        /// <summary>
        /// 修改请求
        /// </summary>
        /// <param name="printer"></param>
        /// <returns></returns>
        public ResultModel ServiceEditRequest(Request request)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceEditRequest client = null;
            try
            {
                client = service.ServiceEditRequest(new ToServerServiceEditRequest() { SessionId = Resources.GetRes().SERVER_SESSION, Request = JsonConvert.SerializeObject(request) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            // 如果操作成功, 则更新信息
            if (client.Result)
            {
                Request model = client.Request.DeserializeObject<Request>();
                request.UpdateTime = model.UpdateTime;
            }

            //返回值
            return result;
        }



        /// <summary>
        /// 删除请求
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResultModel ServiceDelRequest(Request request)
        {
            ResultModel result = new ResultModel();

            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceDelRequest client = null;
            try
            {
                client = service.ServiceDelRequest(new ToServerServiceDelRequest() { SessionId = Resources.GetRes().SERVER_SESSION, Request = JsonConvert.SerializeObject(request) });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);
            HandleResultException(client.ExceptionType, result);

            result.Result = client.Result;

            //返回值
            return result;
        }


        #endregion Request



        #region Log

        /// <summary>
        /// 查找日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public bool ServiceGetLog(long IsBalancePrice, long IsBalanceChange, long AddTimeStart, long AddTimeEnd, out List<Balance> balances, out List<Log> logs, long OperateId = 0)
        {
            //打开通道并发送获取数据
            CheckConnectionAndConnection();
            ToClientServiceGetLog client = null;
            try
            {
                client = service.ServiceGetLog(new ToServerServiceGetLog() { SessionId = Resources.GetRes().SERVER_SESSION,  AddTimeStart = AddTimeStart, AddTimeEnd = AddTimeEnd, IsBalanceChange = IsBalanceChange, IsBalancePrice = IsBalancePrice, OperateId = OperateId });
            }
            catch (Exception ex)
            {
                CommunicationExceptionHandle(ex);
                throw new OybabException(Resources.GetRes().GetString("Exception_OperateRequestFaild"), ex);
            }
            //处理错误
            HandleException(client.ExceptionType);

            balances = client.Balance.DeserializeObject<List<Balance>>();
            logs = client.Logs.DeserializeObject<List<Log>>();

            //返回值
            return client.Result;
        }

        #endregion Log



        #endregion Service



        #region Tools

        /// <summary>
        /// 获取MAC列表
        /// </summary>
        /// <returns></returns>
        private string GetMac()
        {
            try
            {
                List<string> macs = NetworkInterface.GetAllNetworkInterfaces().Where(nic => nic.OperationalStatus == OperationalStatus.Up).Select(nic => nic.GetPhysicalAddress().ToString()).ToList();
                if (null != macs && macs.Count > 0)
                {
                    return string.Join("|", macs);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return "error";
            }
        }

        /// <summary>
        /// 获取版本
        /// </summary>
        /// <returns></returns>
        private string GetVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetName().Version.ToString();//获取主版本号  
        }


        /// <summary>
        /// 获取IP列表
        /// </summary>
        /// <returns></returns>
        private string GetIP()
        {
            try
            {

                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                if (null != localIPs && localIPs.Length > 0)
                {
                    return string.Join("|", localIPs.Select(x => x.ToString()));
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return "error";
            }
        }

        #endregion



        #region Check

        /// <summary>
        /// 检查
        /// </summary>
        /// <returns></returns>
        private bool Check()
        {
          
                return true;
           
        }


        #endregion Check
    }

    #region CallBack

    /// <summary>
    /// 回调
    /// </summary>
    internal class ServiceCallback : IServiceCallback 
    {

        /// <summary>
        /// 服务获取通知
        /// </summary>
        /// <param name="toClient"></param>
        public void ServiceSendNotification(ToClientServiceSendNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>{
                lock(NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceSendNotification(toClient);
            });
        }

        /// <summary>
        /// 订单更新
        /// </summary>
        /// <param name="toClient"></param>
        public void ServiceOrderUpdateNotification(ToClientServiceOrderUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>{
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceOrderUpdateNotification(toClient);
            });
        }

        /// <summary>
        /// 外卖更新
        /// </summary>
        /// <param name="toClient"></param>
        public void ServiceTakeoutUpdateNotification(ToClientServiceTakeoutUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>{
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceTakeoutUpdateNotification(toClient);
            });
        }

       

        /// <summary>
        /// 产品数量更新
        /// </summary>
        /// <param name="toClient"></param>
        public void ServiceProductCountUpdateNotification(ToClientServiceProductCountUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>{
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceProductCountUpdateNotification(toClient);
            });
        }

        /// <summary>
        /// 设备登录或退出等状态有变
        /// </summary>
        /// <param name="toClient"></param>
        public void ServiceDeviceModeUpdateNotification(ToClientServiceDeviceModeUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>{
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceDeviceModeUpdateNotification(toClient);
            });
        }




        /// <summary>
        /// 服务订单新增明细通知(客户端顾客先验证模式)
        /// </summary>
        /// <param name="toClient"></param>
        public void ServiceOrderDetailsAddNotification(ToClientServiceOrderDetailsAddNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>{
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceOrderDetailsAddNotification(toClient);
            });
        }



        /// <summary>
        /// 服务外卖新增外卖通知(客户端顾客先验证模式)
        /// </summary>
        /// <param name="toClient"></param>
        public void ServiceTakeoutAddNotification(ToClientServiceTakeoutAddNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>{
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceTakeoutAddNotification(toClient);
            });
        }


        /// <summary>
        /// 服务发送模型更改通知
        /// </summary>
        /// <param name="toClient"></param>
        public void ServiceModelUpdateNotification(ToClientServiceModelUpdateNotification toClient)
        {
            System.Threading.Tasks.Task.Factory.StartNew(()=>{
                lock (NotificationService.Instance.NotificationLock)
                    NotificationService.Instance.ServiceModelUpdateNotification(toClient);
            });
        }

        
    }





    #endregion CallBack

}
