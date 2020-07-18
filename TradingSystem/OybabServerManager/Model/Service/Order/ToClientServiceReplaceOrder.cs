using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Order
{

    public class ToClientServiceReplaceOrder : ToClientService
    {

        public bool Result { get; set; }

        public string NewRoomSession { get; set; }

        public string OldRoomSession { get; set; }

        public string NewOrder { get; set; }

        public string OldOrder { get; set; }
    }
}
