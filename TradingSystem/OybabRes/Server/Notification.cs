using Oybab.DAL;
using Oybab.Res.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Server
{
    public sealed class Notification
    {
        #region Instance
        private Notification() {
            ActionSend = (obj, value, args) => { if (null != NotificateSend) NotificateSend(obj, value, args); };
            ActionSendFromService = (obj, value, args) => { if (null != NotificateSendFromServer)NotificateSendFromServer(obj, value, args); };
            ActionSendsFromService = (obj, value, args) => { if (null != NotificateSendsFromServer)NotificateSendsFromServer(obj, value, args); };
            ActionGetsFromService = (obj, value, args) => { if (null != NotificateGetsFromServer)NotificateGetsFromServer(obj, value, args); };
            ActionConfig = (obj, value, args) => { if (null != NotificationConfig)NotificationConfig(obj, value, args); };
            //ActionProductCounts = (obj, value, args) => { if (null != NotificationProductCounts)NotificationProductCounts(obj, value, args); };
            ActionProduct = (obj, value, args) => { if (null != NotificationProduct)NotificationProduct(obj, value, args); };
            ActionAdminLog = (obj, value, args) => { if (null != NotificationAdminLog)NotificationAdminLog(obj, value, args); };
            ActionOrder = (obj, value, args) => { if (null != NotificationOrder)NotificationOrder(obj, value, args); };
            ActionRoomModel = (obj, value, args) => { if (null != NotificationRoomModel)NotificationRoomModel(obj, value, args); };
            ActionImport = (obj, value, args) => { if (null != NotificationImport)NotificationImport(obj, value, args); };
            ActionTakeout = (obj, value, args) => { if (null != NotificationTakeout)NotificationTakeout(obj, value, args); };
            ActionAdmin = (obj, value, args) => { if (null != NotificationAdmin)NotificationAdmin(obj, value, args); };
            ActionDevice = (obj, value, args) => { if (null != NotificationDevice)NotificationDevice(obj, value, args); };
            ActionPrinter = (obj, value, args) => { if (null != NotificationPrinter)NotificationPrinter(obj, value, args); };
            ActionRequest = (obj, value, args) => { if (null != NotificationRequest)NotificationRequest(obj, value, args); };
            ActionPprs = (obj, values, value2, args) => { if (null != NotificationPprs)NotificationPprs(obj, values, value2, args); };
            ActionMember = (obj, value, args) => { if (null != NotificationMember)NotificationMember(obj, value, args); };
            ActionBalance = (obj, value, args) => { if (null != NotificationBalance) NotificationBalance(obj, value, args); };
            ActionSupplier = (obj, value, args) => { if (null != NotificationSupplier)NotificationSupplier(obj, value, args); };
            ActionProductType = (obj, value, args) => { if (null != NotificationProductType)NotificationProductType(obj, value, args); };
            ActionRoom = (obj, value, args) => { if (null != NotificationRoom)NotificationRoom(obj, value, args); };
            ActionLanguage = (obj, value, args) => { if (null != NotificationLanguage)NotificationLanguage(obj, value, args); };
            ActionLogin = (obj, value, args) => { if (null != NotificationLogin) NotificationLogin(obj, value, args); };
            ActionBarcodeReader = (obj, value, args) => { if (null != NotificationBarcodeReader) NotificationBarcodeReader(obj, value, args); };
            ActionCardReader = (obj, value, args) => { if (null != NotificationCardReader) NotificationCardReader(obj, value, args); };


        }

        private static readonly Lazy<Notification> _instance = new Lazy<Notification>(() => new Notification());
        public static Notification Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance




        // 服务器通知
        public event EventSend NotificateSend;
        public event EventLong NotificateSendFromServer;
        public event EventLongs NotificateSendsFromServer;
        public event EventLongs NotificateGetsFromServer;

        public event EventString NotificationConfig;
        public event EventProduct NotificationProduct;
        public event EventOrder NotificationOrder;
        public event EventRoomModel NotificationRoomModel;
        public event EventImport NotificationImport;
        public event EventTakeout NotificationTakeout;
        public event EventAdmin NotificationAdmin;
        public event EventDevice NotificationDevice;
        public event EventPrinter NotificationPrinter;
        public event EventRequest NotificationRequest;
        public event EventAdminLog NotificationAdminLog;
        public event EventPprs NotificationPprs;
        public event EventMember NotificationMember;
        public event EventBalance NotificationBalance;
        public event EventSupplier NotificationSupplier;
        public event EventProductType NotificationProductType;
        public event EventRoom NotificationRoom;
        public event EventLanguage NotificationLanguage;
        public event EventLogin NotificationLogin;
        public event EventBarcodeReader NotificationBarcodeReader;
        public event EventCardReader NotificationCardReader;






        // Action
        public Action<object, int, object> ActionSend;
        public Action<object, long, object> ActionSendFromService;
        public Action<object, List<long>, object> ActionSendsFromService;
        public Action<object, List<long>, object> ActionGetsFromService;
        public Action<object, string, object> ActionConfig;
        public Action<object, Product, object> ActionProduct;
        public Action<object, Order, object> ActionOrder;
        public Action<object, RoomModel, object> ActionRoomModel;
        public Action<object, Import, object> ActionImport;
        public Action<object, Takeout, object> ActionTakeout;
        public Action<object, Admin, object> ActionAdmin;
        public Action<object, Device, object> ActionDevice;
        public Action<object, Printer, object> ActionPrinter;
        public Action<object, Request, object> ActionRequest;
        public Action<object, AdminLog, object> ActionAdminLog;
        public Action<object, List<Ppr>, Product, object> ActionPprs;
        public Action<object, Member, object> ActionMember;
        public Action<object, Balance, object> ActionBalance;
        public Action<object, Supplier, object> ActionSupplier;
        public Action<object, ProductType, object> ActionProductType;
        public Action<object, Room, object> ActionRoom;
        public Action<object, int, object> ActionLanguage;
        public Action<object, string, object> ActionLogin;
        public Action<object, string, object> ActionBarcodeReader;
        public Action<object, string, object> ActionCardReader;














    }


    // delegate
    public delegate void EventSend(object sender, int value, object args);
    public delegate void EventInt(object sender, int value, object args);
    public delegate void EventLong(object sender, long value, object args);
    public delegate void EventLongs(object sender, List<long> value, object args);
    public delegate void EventString(object sender, string value, object args);
    public delegate void EventProduct(object sender, Product value, object args);
    public delegate void EventOrder(object sender, Order value, object args);
    public delegate void EventRoomModel(object sender, RoomModel value, object args);
    public delegate void EventImport(object sender, Import value, object args);
    public delegate void EventTakeout(object sender, Takeout value, object args);
    public delegate void EventAdmin(object sender, Admin value, object args);
    public delegate void EventDevice(object sender, Device value, object args);
    public delegate void EventPrinter(object sender, Printer value, object args);
    public delegate void EventRequest(object sender, Request value, object args);
    public delegate void EventAdminLog(object sender, AdminLog value, object args);
    public delegate void EventPprs(object sender, List<Ppr> values, Product value2, object args);
    public delegate void EventMember(object sender, Member value, object args);
    public delegate void EventBalance(object sender, Balance value, object args);
    public delegate void EventSupplier(object sender, Supplier value, object args);
    public delegate void EventProductType(object sender, ProductType value, object args);
    public delegate void EventRoom(object sender, Room value, object args);
    public delegate void EventLanguage(object sender, int value, object args);
    public delegate void EventLogin(object sender, string value, object args);
    public delegate void EventBarcodeReader(object sender, string value, object args);
    public delegate void EventCardReader(object sender, string value, object args);


}
