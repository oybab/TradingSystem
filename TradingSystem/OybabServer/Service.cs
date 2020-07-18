#define W_TRANS
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Oybab.ServerManager;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Operate;
using Oybab.ServerManager.Res;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.IO;
using System.ServiceModel.Security.Tokens;

namespace Oybab.Server
{
    /// <summary>
    /// 宿主服务启动WCF
    /// </summary>
    public sealed partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
            Logger.Create();
            AppDomain.CurrentDomain.AssemblyResolve -= HandleAssemblyResolve;
            AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// 从GAC加载没能加载的程序集, 比如System.Data.SQLite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private System.Reflection.Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                if (args.Name.Contains("System.Data.SQLite"))
                {
                    return System.Reflection.Assembly.Load("System.Data.SQLite.EF6.SQLiteProviderFactory, System.Data.SQLite.EF6, Version=1.0.98.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139"); //System.Data.SQLite, Version=1.0.98.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139
                }
                // SignalR
                else if (args.Name.Contains("Microsoft.Owin.Host.HttpListener"))
                {
                    return System.Reflection.Assembly.Load("Microsoft.Owin.Host.HttpListener, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                }
                else if (args.Name.Contains("Microsoft.Owin,"))
                {
                    return System.Reflection.Assembly.Load("Microsoft.Owin, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                }
                else if (args.Name.Contains("System.Web.Cors,"))
                {
                    return System.Reflection.Assembly.Load("System.Web.Cors, Version=5.2.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                }
                else if (args.Name.Contains("Newtonsoft.Json,"))
                {
                    return System.Reflection.Assembly.Load("Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed");
                }
                else if (args.Name.Contains("Microsoft.Owin.Security,"))
                {
                    return System.Reflection.Assembly.Load("Microsoft.Owin.Security, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                }

                return null;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpErrorLog("load throw exception：" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 未捕获异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionPro.ExpLog(e.ExceptionObject, null, false, "Find a BUG.");
        }


        /// <summary>
        /// 启动失败是重启
        /// </summary>
        /// <param name="serviceName"></param>
        private void SetRecoveryOptions(string serviceName)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    int exitCode;
                    using (var process = new Process())
                    {
                        var startInfo = process.StartInfo;
                        startInfo.FileName = "sc";
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                        // tell Windows that the service should restart if it fails
                        startInfo.Arguments = string.Format("failure \"{0}\" reset= 0 actions= restart/60000", serviceName);

                        process.Start();
                        process.WaitForExit();

                        exitCode = process.ExitCode;
                    }
                    if (exitCode != 0)
                        throw new InvalidOperationException("windows service failure return error ! " + exitCode);
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }

            });


        }

        private ServiceHost host;//服务

        private string address = "net.tcp://{0}:19998/OybabService";
        private static string address2 = "http://*:19988";





        protected override void OnStart(string[] args)
        {
            if (null == host)
            {
                try
                {
                    // When Debug
                    //System.Diagnostics.Debugger.Launch();

#if !DEBUG
                    SetRecoveryOptions("OybabTradingSystemService");
#endif




                    // 初始化服务器(获取绑定IP地址)
                    //Resources.GetRes().InitialServer();

                    //加密
                    Uri adrbase = new Uri(string.Format(address, Resources.GetRes().IPAddress));
                    host = new ServiceHost(typeof(Trans.Service), adrbase);
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



                    tcpb.MaxConnections = 30;
                    host.Description.Behaviors.Add(new ServiceThrottlingBehavior()
                    {
                        MaxConcurrentCalls = 30,
                        MaxConcurrentInstances = 30,
                        MaxConcurrentSessions = 30 // Init32.MaxValue(2147483647)
                    });

                   
                    AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);

#if !W_TRANS
                    host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, CertName);

                    host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                    host.Credentials.ClientCertificate.Authentication.TrustedStoreLocation = StoreLocation.LocalMachine;
                    host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;

                    host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new MyX509CertificateValidator("OU=Oybab");
#endif


                    Binding binding = CreateCustomBinding(tcpb, TimeSpan.FromMinutes(10));
                    host.AddServiceEndpoint(typeof(Trans.IService), binding, string.Format(address, Resources.GetRes().IPAddress));


                    // For DDNS
                    ServiceBehaviorAttribute attribute = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                    attribute.AddressFilterMode = AddressFilterMode.Any;

                   

                    Resources.GetRes().App = "thisisService";




                    host.Open();
                    SignalRSession.Instance.StartSignalR(address2);

                    

                    // 创建检查线程
                    Resources.GetRes().CreateThread();

                    ExceptionPro.ExpInfoLog("Service started successfully.");
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);

                    //关闭服务
                    if (null != host)
                    {
                        try
                        {
                            host.Close();
                            host = null;
                        }
                        catch (Exception ex2)
                        {
                            ExceptionPro.ExpLog(ex2);
                        }
                        
                    }

                    
                    //关闭线程
                    Resources.GetRes().CloseThread();

                    ExceptionPro.ExpLog(ex, null, false, "Service start failed.");
                }
            }
            
        }


        protected override void OnStop()
        {
            Stop();
        }


        /// <summary>
        /// 关闭
        /// </summary>
        private new void Stop()
        {

                try
                {
                // 服务关闭时应该会自动关闭吧
                ////关闭服务
                //if (null != host)
                //{
                //    try
                //    {
                //        host.Close();
                //        host = null;
                //    }
                //    catch (Exception ex)
                //    {
                //        ExceptionPro.ExpLog(ex);
                //    }
                //} 

                //关闭线程
                Resources.GetRes().CloseThread();


                    ExceptionPro.ExpInfoLog("Service closed successfully.");
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex, null, false, "Service close failed.");
                }
            
        }

        protected override void OnShutdown()
        {
            Stop();
            base.OnShutdown();
        }


        /// <summary>
        /// 设置验证时间间隔
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="clockSkew"></param>
        /// <returns></returns>
        private Binding CreateCustomBinding(Binding binding, TimeSpan clockSkew)
        {

            CustomBinding myCustomBinding = new CustomBinding(binding);

            SymmetricSecurityBindingElement security = myCustomBinding.Elements.Find<SymmetricSecurityBindingElement>();

            if (null != security)
            {
                security.LocalClientSettings.MaxClockSkew = clockSkew;

                security.LocalServiceSettings.MaxClockSkew = clockSkew;
                SecureConversationSecurityTokenParameters securityProtectionTokenParameters = security.ProtectionTokenParameters as SecureConversationSecurityTokenParameters;

                if (null != securityProtectionTokenParameters)
                {

                    SecurityBindingElement bootstrap = securityProtectionTokenParameters.BootstrapSecurityBindingElement;

                    if (null != bootstrap)
                    {
                        
                    bootstrap.LocalClientSettings.MaxClockSkew = clockSkew;

                    bootstrap.LocalServiceSettings.MaxClockSkew = clockSkew;
                    }

                }     
            }
            return myCustomBinding;

        }

        /// <summary>
        /// 设置证书验证
        /// </summary>
        private class MyX509CertificateValidator : X509CertificateValidator
        {
            string allowedIssuerName;

            public MyX509CertificateValidator(string allowedIssuerName)
            {
                if (allowedIssuerName == null)
                {
                    throw new ArgumentNullException("allowedIssuerName");
                }

                this.allowedIssuerName = allowedIssuerName;
            }

            public override void Validate(X509Certificate2 certificate)
            {
                // Check that there is a certificate.
                if (certificate == null)
                {
                    throw new ArgumentNullException("certificate");
                }

                // Check that the certificate issuer matches the configured issuer.
                if (!certificate.SubjectName.Name.Contains(allowedIssuerName))
                {
                    throw new SecurityTokenValidationException
                      ("Certification not correct!");
                }
            }
        }


    }
}
