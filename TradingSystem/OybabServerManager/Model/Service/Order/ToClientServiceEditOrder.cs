using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Oybab.ServerManager.Model;
using Oybab.ServerManager.Model.Service;

namespace Oybab.ServerManager.Model.Service.Order
{

    public class ToClientServiceEditOrder : ToClientService
    {

        public bool Result { get; set; }

        public string RoomStateSession { get; set; }

        public string Order { get; set; }

        public long UpdateTime { get; set; }
    }
}
