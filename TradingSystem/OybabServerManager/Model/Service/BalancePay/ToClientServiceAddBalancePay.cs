using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.BalancePay
{

    public class ToClientServiceAddBalancePay : ToClientService
    {

        public bool Result { get; set; }

        public string BalancePay { get; set; }

        public string Balance { get; set; }
    }
}
