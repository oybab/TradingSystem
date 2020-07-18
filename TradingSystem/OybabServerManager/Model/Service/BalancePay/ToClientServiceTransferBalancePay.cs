using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.BalancePay
{

    public class ToClientServiceTransferBalancePay : ToClientService
    {

        public bool Result { get; set; }

        public string BalancePay1 { get; set; }

        public string Balance1 { get; set; }

        public string BalancePay2 { get; set; }

        public string Balance2 { get; set; }
    }
}
