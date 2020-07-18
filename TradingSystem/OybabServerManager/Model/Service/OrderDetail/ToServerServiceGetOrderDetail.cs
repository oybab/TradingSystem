using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.OrderDetail
{

    public class ToServerServiceGetOrderDetail : ToServerService
    {

        public long OrderId { get; set; }

    }
}
