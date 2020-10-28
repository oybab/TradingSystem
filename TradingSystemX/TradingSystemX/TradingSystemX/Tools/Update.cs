using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Oybab.Res.Exceptions;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX;
using System.Threading.Tasks;
using Oybab.TradingSystemX.Tools;
using Newtonsoft.Json;
using System.Net.Http;
using Xamarin.Forms;
using System.Runtime.Serialization.Json;
using Xamarin.Essentials;

namespace Oybab.Res.Tools
{
    public static class Update
    {
        /// <summary>
        /// 查找新版本
        /// </summary>
        public static void SearchUpdate(string more, Action<UpdateModel> FindNewVersion = null)
        {
            Task.Run(async () =>
            {
                try
                {
                    //1分钟后执行
                    await ExtX.Sleep(1000 * 60 * 1);

                    string version = OperatesService.Instance.AllVersion;

                    //获取硬件号
                    string uid = await OperatesService.Instance.ServiceUID();

                    CookieContainer CookieContainer = new CookieContainer();
                    //
                    //定义webClient对象
                    var cookieContainer = new CookieContainer();
                    using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer, UseCookies = true })
                    {
                        //定义通信地址 和 JSON数据
                        string URL = "https://www.oybab.net/software/update/";
                        //string URL = "http://localhost:3282/software/update/";

                        using (var client = new HttpClient(handler) { BaseAddress = new Uri(URL) })
                        {
                            var values = new Dictionary<string, string>();
                            values.Add("version", version);
                            values.Add("uid", uid);
                            values.Add("lang", TradingSystemX.Res.Instance.GetLangByLangIndex(TradingSystemX.Res.Instance.CurrentLangIndex).Culture.Name);
                            values.Add("app", Resources.Instance.SOFT_SERVICE_NAME);
                            values.Add("more", more);
                            values.Add("name", Resources.Instance.KEY_NAME_0);
                            values.Add("os", OperatesService.Instance.GetOS());
                            values.Add("ostype", DeviceInfo.VersionString);

                            var content = new FormUrlEncodedContent(values);
                         


                            //向服务器发送POST数据
                            var response = await client.PostAsync(URL, content);
                            string data = Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());

                            //如果空
                            if (string.IsNullOrWhiteSpace(data))
                            {
#if DEBUG
                                ExceptionPro.ExpInfoLog("Response empty!");
#endif
                                return;
                            }

                            UpdateModel model = data.DeserializeObject<UpdateModel>();
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
                                    if (await OperatesService.Instance.ServiceLock())
                                    {

                                        values.Add("ConfirmLock", "1");
                                        values.Add("TId", model.TId);

                                        var content2 = new FormUrlEncodedContent(values);


                                        await client.PostAsync(URL, content2);

                                        ExceptionPro.ExpErrorLog("LS");

                                    }
                                    else
                                    {
                                        values.Add("ConfirmLock", "0");
                                        values.Add("TId", model.TId);

                                        var content2 = new FormUrlEncodedContent(values);

                                        await client.PostAsync(URL, content2);

                                        ExceptionPro.ExpErrorLog("LF");

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
                                        throw new OybabException(string.Format(TradingSystemX.Res.Instance.GetString("UpdateUnableError"), model.Code), true);
#endif
                                }
                            }
                            //数据返回空或解析失败
                            else
                            {
#if DEBUG
                                throw new OybabException(TradingSystemX.Res.Instance.GetString("UpdateNonReturnOrCantRead"), true);
#endif
                            }
                        }
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
            
        }

        
    }

    /// <summary>
    /// 检查更新类
    /// </summary>
    public sealed class UpdateModel
    {
        [JsonProperty(PropertyName = "Code")]
        public string Code { get; set; }
        [JsonProperty(PropertyName = "ErrorMsg")]
        public string ErrorMsg { get; set; }
        [JsonProperty(PropertyName = "Url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "DisplayMsg")]
        public string DisplayMsg { get; set; }
        [JsonProperty(PropertyName = "NewVersion")]
        public string NewVersion { get; set; }
        [JsonProperty(PropertyName = "TId")]
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
