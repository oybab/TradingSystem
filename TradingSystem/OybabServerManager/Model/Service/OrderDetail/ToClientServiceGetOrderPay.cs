using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.OrderDetail
{

    public class ToClientServiceGetOrderPay : ToClientService
    {

        public bool Result { get; set; }

        public string OrderPays { get; set; }
    }
}
