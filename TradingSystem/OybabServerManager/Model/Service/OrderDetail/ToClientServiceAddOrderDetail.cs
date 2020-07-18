using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.OrderDetail
{

    public class ToClientServiceAddOrderDetail : ToClientService
    {

        public bool Result { get; set; }

        public string OrderDetails { get; set; }

        public string Order { get; set; }

        public string OrderSessionId { get; set; }
    }
}
