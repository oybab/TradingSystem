using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Model.Models;
using Microsoft.Win32;

namespace Oybab.ServerManager.Operate
{
    internal static class Update
    {

        internal static void UpdateIt(string more)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                //1分钟后执行
                System.Threading.Thread.Sleep(1000 * 60 * 1);//1分钟后执行
                while (true)
                {
                    // 更新
                    SearchUpdate(more);
                    System.Threading.Thread.Sleep(1000 * 60 * 60 * 48);// 每隔48小时执行一次
                }

            }, System.Threading.Tasks.TaskCreationOptions.LongRunning);

        }

        /// <summary>
        /// 查找新版本
        /// </summary>
        internal static void SearchUpdate(string more, Action<UpdateModel> FindNewVersion = null, bool IsReconnect = false)
        {
            Action action = new Action(() =>
            {

                try
                {
                    
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    string version = assembly.GetName().Version.ToString();//获取主版本号  

                    //获取硬件号
                    string uid = Resources.GetRes().UID;

                    if (string.IsNullOrWhiteSpace(uid))
                        return;

                    //定义webClient对象
                    CookieAwareWebClient webClient = new CookieAwareWebClient();
                    //定义通信地址 和 JSON数据
                    string URL = "https://www.oybab.net/software/update/";
                    //string URL = "http://192.168.1.100/OyBabNet/software/update/";

                    //组装数据
                    NameValueCollection postValues = new NameValueCollection();
                    postValues.Add("version", version);
                    postValues.Add("uid", uid);
                    postValues.Add("lang", Resources.GetRes().AllLangList.Where(x => x.Value.LangIndex == Resources.GetRes().CurrentLangIndex).FirstOrDefault().Value.Culture.Name);
                    postValues.Add("app", Resources.GetRes().SOFT_SERVICE_NAME);
                    postValues.Add("more", more);
                    postValues.Add("name", Resources.GetRes().KEY_NAME_0);
                    postValues.Add("os", OSCheck.GetOS());
                    postValues.Add("ostype", OSCheck.GetOSType());

                    // 去锁(还有下面的锁住逻辑)
                    postValues.Add("LeftDay", Resources.GetRes().ExpiredRemainingDays.ToString());
                    if (Resources.GetRes().ExpiredRemainingDays < 7 && null != Resources.GetRes().RegTimeRequestCode)
                    {
                        postValues.Add("machineNo", Resources.GetRes().RegTimeRequestCode);
                    }

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
                        }// 如果需要延长时间
                        else if (model.Code == "8")
                        {
                            if (Res.Key.GetKeys().SetRegCode(model.DisplayMsg))
                            {
                                Res.Key.GetKeys().Clear(true);
                                if (DBOperate.GetDBOperate().IsDataReady)
                                {
                                    Res.Key.GetKeys().Check(false, true);
                                }
                                else
                                {
                                    Res.Key.GetKeys().Check(false);
                                }
#if DEBUG
                                ExceptionPro.ExpInfoLog("Extend Success!");
#endif

                                postValues.Add("RequestId", model.NewVersion);

                                webClient.UploadValues(URL, postValues);

                            }
                            else
                            {
#if DEBUG
                                ExceptionPro.ExpInfoLog("Extend Failed!");
#endif
                            }
                        }
                        //如果需要锁住
                        else if (model.Code == "-4")
                        {
                            if (Res.Key.GetKeys().LockKey())
                            {
                                postValues.Add("ConfirmLock", "1");
                                postValues.Add("TId", model.TId);
                                webClient.UploadValues(URL, postValues);
                                OperateLog.Instance.AddRecord(0, null, "LS#" + OperateType.None);
                            }
                            else
                            {
                                postValues.Add("ConfirmLock", "0");
                                postValues.Add("TId", model.TId);
                                webClient.UploadValues(URL, postValues);
                                OperateLog.Instance.AddRecord(0, null, "LS#" + OperateType.None);
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
                                    throw new OybabException(string.Format("Update Unable Error. code: {0}", model.Code), true);
                            #endif
                        }
                    }
                    //数据返回空或解析失败
                    else
                    {
                        #if DEBUG
                            throw new OybabException("Update Non Return Or Cant Read", true);
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
}
