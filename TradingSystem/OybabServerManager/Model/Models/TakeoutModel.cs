using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oybab.DAL;

namespace Oybab.ServerManager.Model.Models
{
    internal sealed class TakeoutModel
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "as2")]
        public Takeout Takeout { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "$ga")]
        public string TakeoutSession { get; set; }
    }
}
