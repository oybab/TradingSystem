using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.OrderDetail
{

    public class ToServerServiceSaveOrderDetail : ToServerService
    {

        public string Order { get; set; }

        public string OrderDetailsAdd { get; set; }

        public string OrderPaysAdd { get; set; }

        public string OrderDetailsEdit { get; set; }

        public string OrderDetailsConfirm { get; set; }


        public string RoomStateSession { get; set; }

    }
}
