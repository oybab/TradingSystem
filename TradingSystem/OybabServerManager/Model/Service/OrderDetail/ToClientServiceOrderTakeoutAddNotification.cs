using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.OrderDetail
{

    public class ToClientServiceOrderDetailsAddNotification
    {

        public string Order { get; set; }

        public string OrderSessionId { get; set; }

        public long RoomId { get; set; }
    }
}
