using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.AspNet.SignalR.Hubs;
using Oybab.ServerManager.Model.Service.Common;
using System.Threading.Tasks;
using System.Threading;
using Oybab.ServerManager.Model.Models;
using Oybab.ServerManager.Model.Service.Device;
using Oybab.ServerManager.Model.Service.OrderDetail;
using Oybab.ServerManager.Model.Service.Order;
using Oybab.ServerManager.Model.Service.Product;
using Oybab.ServerManager.Model.Service.TakeoutDetail;
using Oybab.ServerManager.Model.Service.Takeout;
using System.Diagnostics;
using Oybab.ServerManager.Exceptions;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;

namespace Oybab.ServerManager.Operate
{

    public class SignalRSession
    {

        #region Instance
        /// <summary>
        /// For Instance
        /// </summary>
        private static SignalRSession _instance;
        public static SignalRSession Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SignalRSession();
                return _instance;
            }
        }

        internal SignalRSession()
        {
            _instance = this;
        }


        #endregion

        
        IDisposable SignalR { get; set; }
        public void StartSignalR(string address)
        {
            // For fix assembly missing problems...
            // load assembly
            AppDomain.CurrentDomain.Load(typeof(Microsoft.Owin.Host.HttpListener.OwinHttpListener).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(Microsoft.Owin.Security.AppBuilderSecurityExtensions).Assembly.GetName());
            AppDomain.CurrentDomain.Load(typeof(System.Web.Cors.CorsConstants).Assembly.GetName());



            SignalR = WebApp.Start<Startup>(address);

        }


        public void CloseSignalR()
        {
            if (SignalR != null)
            {
                SignalR.Dispose();
                GlobalHost.DependencyResolver.Dispose();
            }
        }

    }

    public class Startup
    {

        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);


            var hubConfiguration = new HubConfiguration
            {
                EnableDetailedErrors = true,
                EnableJSONP = true,
                EnableJavaScriptProxies = false
            };

            

            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            }));
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());


            app.MapSignalR(hubConfiguration);


        }

        /// <summary>
        /// 处理服务器错误
        /// </summary>
        internal class ErrorHandlingPipelineModule : HubPipelineModule
        {
            protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
            {
                ExceptionPro.ExpErrorLog("=> Exception " + exceptionContext.Error.Message);
                if (exceptionContext.Error.InnerException != null)
                {
                    ExceptionPro.ExpErrorLog("=> Inner Exception " + exceptionContext.Error.InnerException.Message);
                }
                base.OnIncomingError(exceptionContext, invokerContext);

            }
        }

    }




    /// <summary>
    /// 自定义Hub, 万一以后有需要功能可以增加
    /// </summary>
    public class ServiceHub : Hub
    {
        
       
    }

    /// <summary>
    /// SignalR不提供对应办法, 只好出此下策
    /// </summary>
    public class ServiceHubContext
    {
        // Singleton instance
        #region Instance
        /// <summary>
        /// For Instance
        /// </summary>
        private static ServiceHubContext _instance;
        public static ServiceHubContext Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ServiceHubContext();
                return _instance;
            }
        }

        internal ServiceHubContext()
        {
            _instance = this;
        }


        #endregion

        public IHub ServiceHub { get; private set; }


        /// <summary>
        /// 初始化
        /// </summary>
        public void Initial(IHub _context)
        {
            if (null == this.ServiceHub)
                this.ServiceHub = _context;
        }

    }


    /// <summary>
    /// 实现WCF接口
    /// </summary>
    public class SignalRCallback //: IServiceCallback
    {
        #region Instance
        /// <summary>
        /// For Instance
        /// </summary>
        private static SignalRCallback _instance;
        public static SignalRCallback Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SignalRCallback();
                return _instance;
            }
        }

        internal SignalRCallback()
        {
            _instance = this;
        }


        #endregion 

        public void ServiceDeviceModeUpdateNotification(string ConnectionId, ToClientServiceDeviceModeUpdateNotification toClient)
        {
            Task.Run(() => ServiceHubContext.Instance.ServiceHub.Clients.Client(ConnectionId).ServiceDeviceModeUpdateNotification(toClient)).Wait();
        }

        public void ServiceModelUpdateNotification(string ConnectionId, ToClientServiceModelUpdateNotification toClient)
        {
            Task.Run(() => ServiceHubContext.Instance.ServiceHub.Clients.Client(ConnectionId).ServiceModelUpdateNotification(toClient)).Wait();
        }

        public void ServiceOrderDetailsAddNotification(string ConnectionId, ToClientServiceOrderDetailsAddNotification toClient)
        {
            Task.Run(()=> ServiceHubContext.Instance.ServiceHub.Clients.Client(ConnectionId).ServiceOrderDetailsAddNotification(toClient)).Wait();
        }

        public void ServiceOrderUpdateNotification(string ConnectionId, ToClientServiceOrderUpdateNotification toClient)
        {
            Task.Run(() => ServiceHubContext.Instance.ServiceHub.Clients.Client(ConnectionId).ServiceOrderUpdateNotification(toClient)).Wait();
        }

        public void ServiceProductCountUpdateNotification(string ConnectionId, ToClientServiceProductCountUpdateNotification toClient)
        {
            Task.Run(() => ServiceHubContext.Instance.ServiceHub.Clients.Client(ConnectionId).ServiceProductCountUpdateNotification(toClient)).Wait();
        }

        public void ServiceSendNotification(string ConnectionId, ToClientServiceSendNotification toClient)
        {
            Task.Run(() => ServiceHubContext.Instance.ServiceHub.Clients.Client(ConnectionId).ServiceSendNotification(toClient)).Wait();
        }

        public void ServiceTakeoutAddNotification(string ConnectionId, ToClientServiceTakeoutAddNotification toClient)
        {
            Task.Run(() => ServiceHubContext.Instance.ServiceHub.Clients.Client(ConnectionId).ServiceTakeoutAddNotification(toClient)).Wait();
        }

        public void ServiceTakeoutUpdateNotification(string ConnectionId, ToClientServiceTakeoutUpdateNotification toClient)
        {
            Task.Run(() => ServiceHubContext.Instance.ServiceHub.Clients.Client(ConnectionId).ServiceTakeoutUpdateNotification(toClient)).Wait();
        }
    }



}
