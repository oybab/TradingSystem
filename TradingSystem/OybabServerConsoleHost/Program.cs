#define W_TRANS
using Oybab.ServerManager;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Operate;
using Oybab.Trans;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace Oybab.Server.ConsoleHost
{
    /// <summary>
    /// This project just for test and fast run server part of service as a console Host.
    /// </summary>
    class Program
    {
        /// <summary>
        /// 从GAC加载没能加载的程序集, 比如System.Data.SQLite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static System.Reflection.Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {

                if (args.Name.Contains("System.Data.SQLite"))
                {
                    return System.Reflection.Assembly.Load("System.Data.SQLite.EF6.SQLiteProviderFactory, System.Data.SQLite.EF6, Version=1.0.98.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139"); //System.Data.SQLite, Version=1.0.98.0, Culture=neutral, PublicKeyToken=db937bc2d44ff139
                }
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
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionPro.ExpLog(e.ExceptionObject, null, false, "Find a BUG.");
        }


        private static ServiceHost host;//服务
        private static string address = "net.tcp://{0}:19998/OybabService";
        private static string address2 = "http://*:19988";




        static void Main(string[] args)
        {
            if (null == host)
            {

                Logger.Create();




                AppDomain.CurrentDomain.AssemblyResolve -= HandleAssemblyResolve;
                AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


                //Resources.GetRes().InitialServer();



                //加密
                Uri adrbase = new Uri(string.Format(address, Resources.GetRes().IPAddress));
                host = new ServiceHost(typeof(Service), adrbase);

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

                tcpb.SendTimeout = TimeSpan.FromSeconds(180);

                //tcpb.MaxConnections = 30;
                host.Description.Behaviors.Add(new ServiceThrottlingBehavior()
                {
                    MaxConcurrentCalls = 30,
                    MaxConcurrentInstances = 30,
                    MaxConcurrentSessions = 100 // Init32.MaxValue(2147483647)
                });


#if !W_TRANS
                host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, CertName);

                host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
                host.Credentials.ClientCertificate.Authentication.TrustedStoreLocation = StoreLocation.LocalMachine;
                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
                //host.Credentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, "OybabClient");

                host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new MyX509CertificateValidator("Oybab");
#endif

                Binding binding = CreateCustomBinding(tcpb, TimeSpan.FromMinutes(10));
                host.AddServiceEndpoint(typeof(IService), binding, string.Format(address, Resources.GetRes().IPAddress));



                //上线时去掉, 免得暴露元数据
                ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(mBehave);
                host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

                // For DDNS
                ServiceBehaviorAttribute attribute = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                attribute.AddressFilterMode = AddressFilterMode.Any;


                host.Open();
                SignalRSession.Instance.StartSignalR(address2);


                try
                {

                    // 创建检查线程
                    Resources.GetRes().CreateThread();
                    Console.WriteLine();
                    Console.WriteLine("Service start Successfully (This console host just for test and fast run server part of service).");
                    Console.WriteLine();
                    Console.WriteLine("We recommend you open this with another instance of Visual Studio, and run it as a server service, debug it with another Visual Studio projects!");
                    Console.WriteLine();
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.Read();
        }






        /// <summary>
        /// 设置验证时间间隔
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="clockSkew"></param>
        /// <returns></returns>
        private static Binding CreateCustomBinding(Binding binding, TimeSpan clockSkew)
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
