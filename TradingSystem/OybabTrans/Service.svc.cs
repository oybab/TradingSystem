using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Oybab.ServerManager.Model.Models;
using Oybab.ServerManager.Model.Service.Common;
using Oybab.ServerManager.Model.Service.Import;
using Oybab.ServerManager.Model.Service.ImportDetail;
using Oybab.ServerManager.Model.Service.Order;
using Oybab.ServerManager.Model.Service.OrderDetail;
using Oybab.ServerManager.Model.Service.Product;
using Oybab.ServerManager.Model.Service.ProductType;
using Oybab.ServerManager.Model.Service.Room;
using Oybab.ServerManager.Operate;
using Oybab.ServerManager.Model.Service.Admin;
using Oybab.ServerManager.Model.Service.Printer;
using Oybab.ServerManager.Model.Service.Member;
using Oybab.ServerManager.Model.Service.Device;
using Oybab.ServerManager.Model.Service.Takeout;
using Oybab.ServerManager.Model.Service.TakeoutDetail;
using Oybab.ServerManager.Model.Service.MemberPay;
using Oybab.ServerManager.Model.Service.AdminPay;
using Oybab.ServerManager.Model.Service.AdminLog;
using Oybab.ServerManager.Model.Service.Supplier;
using Oybab.ServerManager.Model.Service.SupplierPay;
using Oybab.ServerManager.Model.Service.Log;
using Oybab.ServerManager.Model.Service.BalancePay;
using Oybab.ServerManager.Model.Service.Request;
using Oybab.ServerManager.Model.Service.Balance;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Oybab.Trans
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple), CallbackBehavior]//(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [HubName("Service")]
    public class Service : ServiceHub, IService
    {
        #region Service

      
        #region Common

        /// <summary>
        /// 服务新情求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceNewRequest ServiceNewRequest(ToServerServiceNewRequest toServer)
        {
            string IpAddress = Context?.Request?.Environment["server.RemoteIpAddress"]?.ToString();

            if (!string.IsNullOrWhiteSpace(IpAddress))
            {
                // 实属无奈, SignalR设计原因只能这么做了....
                if (null == ServiceHubContext.Instance.ServiceHub)
                {
                    ServiceHubContext.Instance.Initial(this);
                }
            }

            return ServiceOperate.GetServiceOperate().ServiceNewRequest(toServer, Context?.ConnectionId, IpAddress);
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceChangePWD ServiceChangePWD(ToServerServiceChangePWD toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceChangePWD(toServer);
        }


        /// <summary>
        /// 获取UID
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceUID ServiceUID(ToServerServiceUID toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceUID(toServer);
        }

        /// <summary>
        /// 写入设置
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSetCon ServiceSetCon(ToServerServiceSetCon toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceSetCon(toServer);
        }

        /// <summary>
        /// 发送操作
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSend ServiceSend(ToServerServiceSend toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceSend(toServer);
        }


        /// <summary>
        /// 打印操作
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServicePrint ServicePrint(ToServerServicePrint toServer)
        {
            return ServiceOperate.GetServiceOperate().ServicePrint(toServer);
        }


        /// <summary>
        /// 锁住
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceLock ServiceLock(ToServerServiceLock toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceLock(toServer);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceClose ServiceClose(ToServerServiceClose toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceClose(toServer);
        }


        /// <summary>
        /// 获取备份
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetbak ServiceGetbak(ToServerServiceGetbak toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetbak(toServer);
        }


        /// <summary>
        /// 设置备份
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSetbak ServiceSetbak(ToServerServiceSetbak toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceSetbak(toServer);
        }


        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSetContent ServiceSetContent(ToServerServiceSetContent toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceSetContent(toServer);
        }


        /// <summary>
        /// 获取请求码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceRequestTimeCode ServiceTimeRequestCode(ToServerServiceRequestTimeCode toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceTimeRequestCode(toServer);
        }



        /// <summary>
        /// 注册时间
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceRegTime ServiceRegTime(ToServerServiceRegTime toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceRegTime(toServer);
        }



        /// <summary>
        /// 获取数量请求码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceRequestCountCode ServiceCountRequestCode(ToServerServiceRequestCountCode toServer)
         {
            return ServiceOperate.GetServiceOperate().ServiceCountRequestCode(toServer);
        }


        /// <summary>
        /// 注册数量
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceRegCount ServiceRegCount(ToServerServiceRegCount toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceRegCount(toServer);
        }



        /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetModel ServiceGetModel(ToServerServiceGetModel toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetModel(toServer);
        }

        /// <summary>
        /// 刷新会话
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSession ServiceSession(ToServerServiceSession toServer)
        {
            string IpAddress = Context?.Request?.Environment["server.RemoteIpAddress"]?.ToString();
            return ServiceOperate.GetServiceOperate().ServiceSession(toServer, Context?.ConnectionId, IpAddress);
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
            return ServiceOperate.GetServiceOperate().ServiceAddRoom(toServer);
        }


        /// <summary>
        /// 修改包厢
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditRoom ServiceEditRoom(ToServerServiceEditRoom toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditRoom(toServer);
        }



        /// <summary>
        /// 删除包厢
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelRoom ServiceDelRoom(ToServerServiceDelRoom toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelRoom(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceGetAllProduct(toServer);
        }


        /// <summary>
        /// 新增产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddProduct ServiceAddProduct(ToServerServiceAddProduct toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceAddProduct(toServer);
        }


        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditProduct ServiceEditProduct(ToServerServiceEditProduct toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditProduct(toServer);
        }



        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelProduct ServiceDelProduct(ToServerServiceDelProduct toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelProduct(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddProductType(toServer);
        }


        /// <summary>
        /// 修改产品类型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditProductType ServiceEditProductType(ToServerServiceEditProductType toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditProductType(toServer);
        }



        /// <summary>
        /// 删除产品类型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelProductType ServiceDelProductType(ToServerServiceDelProductType toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelProductType(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddOrder(toServer);
        }


        /// <summary>
        /// 编辑订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditOrder ServiceEditOrder(ToServerServiceEditOrder toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditOrder(toServer);
        }



        /// <summary>
        /// 替换订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceReplaceOrder ServiceReplaceOrder(ToServerServiceReplaceOrder toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceReplaceOrder(toServer);
        }


        /// <summary>
        /// 查找订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetOrders ServiceGetOrders(ToServerServiceGetOrders toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetOrders(toServer);
        }


        #endregion Order


        #region OrderDetail


        /// <summary>
        /// 增加订单明细(顾客模式)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddOrderDetail ServiceAddOrderDetail(ToServerServiceAddOrderDetail toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceAddOrderDetail(toServer);
        }


        /// <summary>
        /// 修改订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceSaveOrderDetail ServiceSaveOrderDetail(ToServerServiceSaveOrderDetail toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceSaveOrderDetail(toServer);
        }


        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelOrderDetail ServiceDelOrderDetail(ToServerServiceDelOrderDetail toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelOrderDetail(toServer);
        }




        /// <summary>
        /// 查找订单明细(按订单)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetOrderDetail ServiceGetOrderDetail(ToServerServiceGetOrderDetail toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetOrderDetail(toServer);
        }




        /// <summary>
        /// 查找订单支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetOrderPay ServiceGetOrderPay(ToServerServiceGetOrderPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetOrderPay(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddTakeout(toServer);
        }


        /// <summary>
        /// 编辑外卖
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditTakeout ServiceEditTakeout(ToServerServiceEditTakeout toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditTakeout(toServer);
        }

        /// <summary>
        /// 查找外卖
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetTakeouts ServiceGetTakeout(ToServerServiceGetTakeouts toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetTakeout(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceSaveTakeoutDetail(toServer);
        }


        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelTakeoutDetail ServiceDelTakeoutDetail(ToServerServiceDelTakeoutDetail toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelTakeoutDetail(toServer);
        }


        /// <summary>
        /// 查找外卖明细(按外卖)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetTakeoutDetail ServiceGetTakeoutDetail(ToServerServiceGetTakeoutDetail toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetTakeoutDetail(toServer);
        }



        /// <summary>
        /// 查找外卖支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetTakeoutPay ServiceGetTakeoutPay(ToServerServiceGetTakeoutPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetTakeoutPay(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddImportWithDetail(toServer);
        }



        /// <summary>
        /// 编辑进货
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditImport ServiceEditImport(ToServerServiceEditImport toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditImport(toServer);
        }

        /// <summary>
        /// 查找进货
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetImports ServiceGetImports(ToServerServiceGetImports toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetImports(toServer);
        }



        /// <summary>
        /// 查找进货明细(按订单)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetImportDetail ServiceGeImportDetail(ToServerServiceGetImportDetail toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGeImportDetail(toServer);
        }



        /// <summary>
        /// 查找进货支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetImportPay ServiceGetImportPay(ToServerServiceGetImportPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetImportPay(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddAdmin(toServer);
        }


        /// <summary>
        /// 修改管理员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditAdmin ServiceEditAdmin(ToServerServiceEditAdmin toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditAdmin(toServer);
        }





        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceResetAdmin ServiceResetAdmin(ToServerServiceResetAdmin toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceResetAdmin(toServer);
        }



        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelAdmin ServiceDelAdmin(ToServerServiceDelAdmin toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelAdmin(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddAdminPay(toServer);
        }

        /// <summary>
        /// 删除管理员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelAdminPay ServiceDelAdminPay(ToServerServiceDelAdminPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelAdminPay(toServer);
        }




        /// <summary>
        /// 查找管理员支付(按管理员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetAdminPay ServiceGetAdminPay(ToServerServiceGetAdminPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetAdminPay(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddBalance(toServer);
        }


        /// <summary>
        /// 修改余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditBalance ServiceEditBalance(ToServerServiceEditBalance toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditBalance(toServer);
        }



        /// <summary>
        /// 删除余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelBalance ServiceDelBalance(ToServerServiceDelBalance toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelBalance(toServer);
        }


        /// <summary>
        /// 查找余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetBalance ServiceGetBalances(ToServerServiceGetBalance toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetBalances(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddBalancePay(toServer);
        }

        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceTransferBalancePay ServiceTransferBalancePay(ToServerServiceTransferBalancePay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceTransferBalancePay(toServer);
        }

        /// <summary>
        /// 删除余额支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelBalancePay ServiceDelBalancePay(ToServerServiceDelBalancePay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelBalancePay(toServer);
        }




        /// <summary>
        /// 查找余额支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetBalancePay ServiceGetBalancePay(ToServerServiceGetBalancePay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetBalancePay(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddAdminLog(toServer);
        }


        /// <summary>
        /// 修改管理员日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditAdminLog ServiceEditAdminLog(ToServerServiceEditAdminLog toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditAdminLog(toServer);
        }



        /// <summary>
        /// 删除管理员日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelAdminLog ServiceDelAdminLog(ToServerServiceDelAdminLog toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelAdminLog(toServer);
        }



        /// <summary>
        /// 查找管理员日志(按管理员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetAdminLog ServiceGetAdminLog(ToServerServiceGetAdminLog toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetAdminLog(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddMember(toServer);
        }


        /// <summary>
        /// 修改会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditMember ServiceEditMember(ToServerServiceEditMember toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditMember(toServer);
        }



        /// <summary>
        /// 删除会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelMember ServiceDelMember(ToServerServiceDelMember toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelMember(toServer);
        }


        /// <summary>
        /// 查找会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetMember ServiceGetMembers(ToServerServiceGetMember toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetMembers(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddMemberPay(toServer);
        }

        /// <summary>
        /// 删除会员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelMemberPay ServiceDelMemberPay(ToServerServiceDelMemberPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelMemberPay(toServer);
        }




        /// <summary>
        /// 查找会员支付(按会员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetMemberPay ServiceGetMemberPay(ToServerServiceGetMemberPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetMemberPay(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddSupplier(toServer);
        }


        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditSupplier ServiceEditSupplier(ToServerServiceEditSupplier toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditSupplier(toServer);
        }



        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelSupplier ServiceDelSupplier(ToServerServiceDelSupplier toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelSupplier(toServer);
        }




        /// <summary>
        /// 查找供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetSupplier ServiceGetSupplier(ToServerServiceGetSupplier toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetSupplier(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceAddSupplierPay(toServer);
        }

        /// <summary>
        /// 删除供应商支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelSupplierPay ServiceDelSupplierPay(ToServerServiceDelSupplierPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelSupplierPay(toServer);
        }




        /// <summary>
        /// 查找供应商支付(按供应商)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceGetSupplierPay ServiceGetSupplierPay(ToServerServiceGetSupplierPay toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceGetSupplierPay(toServer);
        }


        #endregion MemberPay


        #region Printer

        /// <summary>
        /// 新增打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddPrinter ServiceAddPrinter(ToServerServiceAddPrinter toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceAddPrinter(toServer);
        }


        /// <summary>
        /// 修改打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditPrinter ServiceEditPrinter(ToServerServiceEditPrinter toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditPrinter(toServer);
        }



        /// <summary>
        /// 删除打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelPrinter ServiceDelPrinter(ToServerServiceDelPrinter toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelPrinter(toServer);
        }


        #endregion Printer


        #region Request

        /// <summary>
        /// 新增请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddRequest ServiceAddRequest(ToServerServiceAddRequest toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceAddRequest(toServer);
        }


        /// <summary>
        /// 修改请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditRequest ServiceEditRequest(ToServerServiceEditRequest toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditRequest(toServer);
        }



        /// <summary>
        /// 删除请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelRequest ServiceDelRequest(ToServerServiceDelRequest toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelRequest(toServer);
        }


        #endregion Request


        #region Device

        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceAddDevice ServiceAddDevice(ToServerServiceAddDevice toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceAddDevice(toServer);
        }


        /// <summary>
        /// 修改设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceEditDevice ServiceEditDevice(ToServerServiceEditDevice toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceEditDevice(toServer);
        }



        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        public ToClientServiceDelDevice ServiceDelDevice(ToServerServiceDelDevice toServer)
        {
            return ServiceOperate.GetServiceOperate().ServiceDelDevice(toServer);
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
            return ServiceOperate.GetServiceOperate().ServiceGetLog(toServer);
        }

        #endregion Log


        #endregion Service

    }

}
