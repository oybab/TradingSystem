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
using Oybab.ServerManager.Model.Service.Admin;
using Oybab.ServerManager.Model.Service.Member;
using Oybab.ServerManager.Model.Service.Printer;
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

namespace Oybab.Trans
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract(CallbackContract = typeof(IServiceCallback))]
    public interface IService
    {
        #region Service

        #region Common

        /// <summary>
        /// 服务新情求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceNewRequest ServiceNewRequest(ToServerServiceNewRequest toServer);


         /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceChangePWD ServiceChangePWD(ToServerServiceChangePWD toServer);

        /// <summary>
        /// 获取UID
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceUID ServiceUID(ToServerServiceUID toServer);


         /// <summary>
        /// 写入设置
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceSetCon ServiceSetCon(ToServerServiceSetCon toServer);


        /// <summary>
        /// 发送操作
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceSend ServiceSend(ToServerServiceSend toServer);


        /// <summary>
        /// 打印操作
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServicePrint ServicePrint(ToServerServicePrint toServer);



        /// <summary>
        /// 锁住
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceLock ServiceLock(ToServerServiceLock toServer);

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceClose ServiceClose(ToServerServiceClose toServer);


        /// <summary>
        /// 获取备份
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetbak ServiceGetbak(ToServerServiceGetbak toServer);


        /// <summary>
        /// 设置备份
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceSetbak ServiceSetbak(ToServerServiceSetbak toServer);


        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceSetContent ServiceSetContent(ToServerServiceSetContent toServer);


        /// <summary>
        /// 获取时间请求码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceRequestTimeCode ServiceTimeRequestCode(ToServerServiceRequestTimeCode toServer);



        /// <summary>
        /// 注册时间
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceRegTime ServiceRegTime(ToServerServiceRegTime toServer);



        /// <summary>
        /// 获取数量请求码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceRequestCountCode ServiceCountRequestCode(ToServerServiceRequestCountCode toServer);



        /// <summary>
        /// 注册数量
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceRegCount ServiceRegCount(ToServerServiceRegCount toServer);



         /// <summary>
        /// 获取模型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetModel ServiceGetModel(ToServerServiceGetModel toServer);

        /// <summary>
        /// 刷新会话
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceSession ServiceSession(ToServerServiceSession toServer);

        #endregion Common


        #region Room

        /// <summary>
        /// 新增包厢
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddRoom ServiceAddRoom(ToServerServiceAddRoom toServer);


        /// <summary>
        /// 修改包厢
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditRoom ServiceEditRoom(ToServerServiceEditRoom toServer);



        /// <summary>
        /// 删除包厢
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelRoom ServiceDelRoom(ToServerServiceDelRoom toServer);


        #endregion Room


        #region Product

        /// <summary>
        /// 获取产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetAllProduct ServiceGetAllProduct(ToServerServiceGetAllProduct toServer);

        /// <summary>
        /// 新增产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddProduct ServiceAddProduct(ToServerServiceAddProduct toServer);


        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditProduct ServiceEditProduct(ToServerServiceEditProduct toServer);



        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelProduct ServiceDelProduct(ToServerServiceDelProduct toServer);


        #endregion Product


        #region ProductType

        /// <summary>
        /// 新增产品类型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddProductType ServiceAddProductType(ToServerServiceAddProductType toServer);


        /// <summary>
        /// 修改产品类型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditProductType ServiceEditProductType(ToServerServiceEditProductType toServer);



        /// <summary>
        /// 删除产品类型
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelProductType ServiceDelProductType(ToServerServiceDelProductType toServer);


        #endregion ProductType


        #region Order

        /// <summary>
        /// 新建订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceNewOrder ServiceAddOrder(ToServerServiceNewOrder toServer);


        /// <summary>
        /// 编辑订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditOrder ServiceEditOrder(ToServerServiceEditOrder toServer);



        /// <summary>
        /// 替换订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceReplaceOrder ServiceReplaceOrder(ToServerServiceReplaceOrder toServer);


        /// <summary>
        /// 查找订单
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetOrders ServiceGetOrders(ToServerServiceGetOrders toServer);



        #endregion Order


        #region OrderDetail


        /// <summary>
        /// 增加订单明细(顾客模式)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddOrderDetail ServiceAddOrderDetail(ToServerServiceAddOrderDetail toServer);

        /// <summary>
        /// 修改订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceSaveOrderDetail ServiceSaveOrderDetail(ToServerServiceSaveOrderDetail toServer);


        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelOrderDetail ServiceDelOrderDetail(ToServerServiceDelOrderDetail toServer);




        /// <summary>
        /// 查找订单明细(按订单)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetOrderDetail ServiceGetOrderDetail(ToServerServiceGetOrderDetail toServer);





        /// <summary>
        /// 查找订单支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetOrderPay ServiceGetOrderPay(ToServerServiceGetOrderPay toServer);



        #endregion Order


        #region Takeout

        /// <summary>
        /// 新建外卖
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceNewTakeout ServiceAddTakeout(ToServerServiceNewTakeout toServer);


        /// <summary>
        /// 编辑外卖
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditTakeout ServiceEditTakeout(ToServerServiceEditTakeout toServer);

        /// <summary>
        /// 查找外卖
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetTakeouts ServiceGetTakeout(ToServerServiceGetTakeouts toServer);


        #endregion Takeout


        #region TakeoutDetail

        /// <summary>
        /// 修改外卖明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceSaveTakeoutDetail ServiceSaveTakeoutDetail(ToServerServiceSaveTakeoutDetail toServer);


        /// <summary>
        /// 删除订单明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelTakeoutDetail ServiceDelTakeoutDetail(ToServerServiceDelTakeoutDetail toServer);




        /// <summary>
        /// 查找外卖明细(按外卖)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetTakeoutDetail ServiceGetTakeoutDetail(ToServerServiceGetTakeoutDetail toServer);






        /// <summary>
        /// 查找外卖支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetTakeoutPay ServiceGetTakeoutPay(ToServerServiceGetTakeoutPay toServer);

        #endregion TakeoutDetail


        #region ImportWithDetails

        /// <summary>
        /// 增加进货及明细
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceNewImport ServiceAddImportWithDetail(ToServerServiceNewImport toServer);


        /// <summary>
        /// 编辑进货
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditImport ServiceEditImport(ToServerServiceEditImport toServer);



        /// <summary>
        /// 查找进货
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetImports ServiceGetImports(ToServerServiceGetImports toServer);



        /// <summary>
        /// 查找进货明细(按订单)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetImportDetail ServiceGeImportDetail(ToServerServiceGetImportDetail toServer);





        /// <summary>
        /// 查找进货支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetImportPay ServiceGetImportPay(ToServerServiceGetImportPay toServer);


        #endregion ImportWithDetails


        #region Admin

        /// <summary>
        /// 新增管理员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddAdmin ServiceAddAdmin(ToServerServiceAddAdmin toServer);


        /// <summary>
        /// 修改管理员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditAdmin ServiceEditAdmin(ToServerServiceEditAdmin toServer);


        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceResetAdmin ServiceResetAdmin(ToServerServiceResetAdmin toServer);


        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelAdmin ServiceDelAdmin(ToServerServiceDelAdmin toServer);


        #endregion Admin


        #region AdminPay


        /// <summary>
        /// 增加管理员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddAdminPay ServiceAddAdminPay(ToServerServiceAddAdminPay toServer);

        /// <summary>
        /// 删除管理员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelAdminPay ServiceDelAdminPay(ToServerServiceDelAdminPay toServer);




        /// <summary>
        /// 查找管理员支付(按管理员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetAdminPay ServiceGetAdminPay(ToServerServiceGetAdminPay toServer);

        #endregion AdminPay


        #region Balance

        /// <summary>
        /// 新增余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddBalance ServiceAddBalance(ToServerServiceAddBalance toServer);


        /// <summary>
        /// 修改余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditBalance ServiceEditBalance(ToServerServiceEditBalance toServer);



        /// <summary>
        /// 删除余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelBalance ServiceDelBalance(ToServerServiceDelBalance toServer);


        /// <summary>
        /// 查找余额
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetBalance ServiceGetBalances(ToServerServiceGetBalance toServer);


        #endregion Balance


        #region BalancePay


        /// <summary>
        /// 增加余额支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddBalancePay ServiceAddBalancePay(ToServerServiceAddBalancePay toServer);


        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceTransferBalancePay ServiceTransferBalancePay(ToServerServiceTransferBalancePay toServer);

        /// <summary>
        /// 删除余额支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelBalancePay ServiceDelBalancePay(ToServerServiceDelBalancePay toServer);




        /// <summary>
        /// 查找余额支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetBalancePay ServiceGetBalancePay(ToServerServiceGetBalancePay toServer);

        #endregion BalancePay


        #region AdminLog

        /// <summary>
        /// 管理员日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddAdminLog ServiceAddAdminLog(ToServerServiceAddAdminLog toServer);


        /// <summary>
        /// 修改管理员日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditAdminLog ServiceEditAdminLog(ToServerServiceEditAdminLog toServer);



        /// <summary>
        /// 删除管理员日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelAdminLog ServiceDelAdminLog(ToServerServiceDelAdminLog toServer);


        /// <summary>
        /// 查找管理员日志(按管理员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetAdminLog ServiceGetAdminLog(ToServerServiceGetAdminLog toServer);


        #endregion AdminLog


        #region Member

        /// <summary>
        /// 新增会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddMember ServiceAddMember(ToServerServiceAddMember toServer);


        /// <summary>
        /// 修改会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditMember ServiceEditMember(ToServerServiceEditMember toServer);



        /// <summary>
        /// 删除会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelMember ServiceDelMember(ToServerServiceDelMember toServer);


        /// <summary>
        /// 查找会员
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetMember ServiceGetMembers(ToServerServiceGetMember toServer);


        #endregion Member


        #region MemberPay


        /// <summary>
        /// 增加会员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddMemberPay ServiceAddMemberPay(ToServerServiceAddMemberPay toServer);

        /// <summary>
        /// 删除会员支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelMemberPay ServiceDelMemberPay(ToServerServiceDelMemberPay toServer);




        /// <summary>
        /// 查找会员支付(按会员)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetMemberPay ServiceGetMemberPay(ToServerServiceGetMemberPay toServer);

        #endregion MemberPay


        #region Supplier

        /// <summary>
        /// 新增供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddSupplier ServiceAddSupplier(ToServerServiceAddSupplier toServer);


        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditSupplier ServiceEditSupplier(ToServerServiceEditSupplier toServer);



        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelSupplier ServiceDelSupplier(ToServerServiceDelSupplier toServer);




        /// <summary>
        /// 查找供应商
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetSupplier ServiceGetSupplier(ToServerServiceGetSupplier toServer);


        #endregion Supplier


        #region SupplierPay


        /// <summary>
        /// 增加供应商支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddSupplierPay ServiceAddSupplierPay(ToServerServiceAddSupplierPay toServer);

        /// <summary>
        /// 删除供应商支付
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelSupplierPay ServiceDelSupplierPay(ToServerServiceDelSupplierPay toServer);




        /// <summary>
        /// 查找供应商支付(按供应商)
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetSupplierPay ServiceGetSupplierPay(ToServerServiceGetSupplierPay toServer);


        #endregion MemberPay


        #region Printer

        /// <summary>
        /// 新增打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddPrinter ServiceAddPrinter(ToServerServiceAddPrinter toServer);


        /// <summary>
        /// 修改打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditPrinter ServiceEditPrinter(ToServerServiceEditPrinter toServer);



        /// <summary>
        /// 删除打印机
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelPrinter ServiceDelPrinter(ToServerServiceDelPrinter toServer);


        #endregion Printer


        #region Request

        /// <summary>
        /// 新增请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddRequest ServiceAddRequest(ToServerServiceAddRequest toServer);


        /// <summary>
        /// 修改请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditRequest ServiceEditRequest(ToServerServiceEditRequest toServer);



        /// <summary>
        /// 删除请求
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelRequest ServiceDelRequest(ToServerServiceDelRequest toServer);


        #endregion Request


        #region Device

        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceAddDevice ServiceAddDevice(ToServerServiceAddDevice toServer);


        /// <summary>
        /// 修改设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceEditDevice ServiceEditDevice(ToServerServiceEditDevice toServer);



        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceDelDevice ServiceDelDevice(ToServerServiceDelDevice toServer);


        #endregion Device


        #region Log

        /// <summary>
        /// 查找日志
        /// </summary>
        /// <param name="toServer"></param>
        /// <returns></returns>
        [OperationContract]
        ToClientServiceGetLog ServiceGetLog(ToServerServiceGetLog toServer);

        #endregion Log

        #endregion Service
    }
}
