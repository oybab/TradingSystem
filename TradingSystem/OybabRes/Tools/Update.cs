using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Oybab.Res.Exceptions;

namespace Oybab.Res.Tools
{
    public static class Update
    {
        /// <summary>
        /// 查找新版本
        /// </summary>
        public static void SearchUpdate(string more, Action<UpdateModel> FindNewVersion = null)
        {
            Action action = new Action(() =>
            {
                try
                {
                    //1分钟后执行
                    System.Threading.Thread.Sleep(1000 * 60 * 1);
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    string version = assembly.GetName().Version.ToString();//获取主版本号  

                    //获取硬件号
                    string uid = Res.Server.OperatesService.GetOperates().ServiceUID();
                    //
                    //定义webClient对象
                    CookieAwareWebClient webClient = new CookieAwareWebClient();
                    //定义通信地址 和 JSON数据
                    string URL = "https://www.oybab.net/software/update/";
                    //string URL = "http://192.168.1.100/OyBabNet/software/update/";

                    //组装数据
                    NameValueCollection postValues = new NameValueCollection();
                    //postValues.Add("json_data", jsonString);
                    postValues.Add("version", version);
                    postValues.Add("uid", uid);
                    postValues.Add("lang", Resources.GetRes().GetLangByLangIndex(Resources.GetRes().CurrentLangIndex).Culture.Name);
                    postValues.Add("app", Resources.GetRes().SOFT_SERVICE_NAME);
                    postValues.Add("more", more);
                    postValues.Add("name", Resources.GetRes().KEY_NAME_0);
                    postValues.Add("os", OSCheck.GetOS());
                    postValues.Add("ostype", OSCheck.GetOSType());

                    //向服务器发送POST数据
                    byte[] responseArray = webClient.UploadValues(URL, postValues);
                    string data = Encoding.UTF8.GetString(responseArray);

                    //如果空
                    if (string.IsNullOrWhiteSpace(data))
                    {
                         #if DEBUG
                            ExceptionPro.ExpInfoLog("Response empty!");
                        #endif
                        return;
                    }

                    UpdateModel model = UpdateModel.FromJsonTo<UpdateModel>(data);
                    if (null != model)
                    {
                        //如果成功
                        if (model.Code == "1")
                        {
                            if (null != FindNewVersion)
                                FindNewVersion(model);
                        }
                        //如果需要锁住
                        else if (model.Code == "-4")
                        {
                            if (Res.Server.OperatesService.GetOperates().ServiceLock())
                            {
                                postValues.Add("ConfirmLock", "1");
                                postValues.Add("TId", model.TId);
                                webClient.UploadValues(URL, postValues);
                                //ExceptionPro.ExpInfoLog("Lock Key Success!");
                                //#if DEBUG
                                    ExceptionPro.ExpErrorLog("LS");
                                //#endif
                            }
                            else
                            {
                                postValues.Add("ConfirmLock", "0");
                                postValues.Add("TId", model.TId);
                                webClient.UploadValues(URL, postValues);
                                //ExceptionPro.ExpErrorLog("Lock Key Faild!");
                                //#if DEBUG
                                    ExceptionPro.ExpErrorLog("LF");
                                //#endif
                            }
                            return;
                        }
                        //如果其他或失败
                        else
                        {
                            #if DEBUG
                                if (!string.IsNullOrWhiteSpace(model.ErrorMsg))
                                    throw new OybabException(model.ErrorMsg, true);
                                else
                                    throw new OybabException(string.Format(Res.Resources.GetRes().GetString("UpdateUnableError"), model.Code), true);
                            #endif
                        }
                    }
                    //数据返回空或解析失败
                    else
                    {
                        #if DEBUG
                            throw new OybabException(Res.Resources.GetRes().GetString("UpdateNonReturnOrCantRead"), true);
                        #endif
                    }
                }
                catch
#if DEBUG
                       (Exception ex)
#endif
                {
#if DEBUG
                    ExceptionPro.ExpLog(ex, null, true);
#endif
                }
            });
            action.BeginInvoke(null, null);
        }

        private class CookieAwareWebClient : WebClient
        {
            public CookieAwareWebClient()
                : this(new CookieContainer())
            { }
            public CookieAwareWebClient(CookieContainer c)
            {
                this.CookieContainer = c;
            }
            public CookieContainer CookieContainer { get; set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);

                var castRequest = request as HttpWebRequest;
                if (castRequest != null)
                {
                    castRequest.CookieContainer = this.CookieContainer;
                }

                return request;
            }
        }
    }

    /// <summary>
    /// 检查更新类
    /// </summary>
    [DataContract]
    public sealed class UpdateModel
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string ErrorMsg { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string DisplayMsg { get; set; }
        [DataMember]
        public string NewVersion { get; set; }
        [DataMember]
        public string TId { get; set; }

        /// <summary>
        /// 解析JSON
        /// </summary>
        /// <typeparam name="UpdateModel"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T FromJsonTo<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T jsonObject = (T)ser.ReadObject(ms);
                return jsonObject;
            }
        }
    }
}
