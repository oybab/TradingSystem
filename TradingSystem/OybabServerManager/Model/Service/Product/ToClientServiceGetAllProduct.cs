using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Product
{

    public class ToClientServiceGetAllProduct : ToClientService
    {

        public string Products { get; set; }

        public string Product { get; set; }

        public string ProductTypes { get; set; }

        public bool Result { get; set; }
    }
}
