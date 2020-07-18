using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
    /// <summary>
    /// 附加信息
    /// </summary>
    public sealed class ExtendInfo
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "SliderMode")]
        public string SliderMode { get; set; } = "2";

        
    }
}
