using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
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
