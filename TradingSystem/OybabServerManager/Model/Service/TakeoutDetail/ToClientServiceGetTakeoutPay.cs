using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.TakeoutDetail
{

    public class ToClientServiceGetTakeoutPay : ToClientService
    {

        public bool Result { get; set; }

        public string TakeoutPays { get; set; }
    }
}
