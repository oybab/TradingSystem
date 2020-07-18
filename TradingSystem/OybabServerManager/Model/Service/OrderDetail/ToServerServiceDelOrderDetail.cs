using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.OrderDetail
{

    public class ToServerServiceDelOrderDetail : ToServerService
    {

        public string Order { get; set; }

        public string OrderDetails { get; set; }


        public string RoomStateSession { get; set; }


    }
}
