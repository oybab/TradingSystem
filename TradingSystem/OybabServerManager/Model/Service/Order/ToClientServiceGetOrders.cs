using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Order
{

    public class ToClientServiceGetOrders : ToClientService
    {

        public bool Result { get; set; }

        public string Orders { get; set; }
    }
}
