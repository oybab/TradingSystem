using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Order
{

    public class ToServerServiceEditOrder : ToServerService
    {

        public string Order { get; set; }

        public string OrderPays { get; set; }

        //[Newtonsoft.Json.JsonIgnore]
        public string RoomStateSession { get; set; }


        public bool Rechecked { get; set; }
    }
}
