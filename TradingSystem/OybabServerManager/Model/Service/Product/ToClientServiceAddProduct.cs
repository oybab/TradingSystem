using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Product
{

    public class ToClientServiceAddProduct : ToClientService
    {

        public string Product { get; set; }

        public string Pprs { get; set; }

        public bool Result { get; set; }
    }
}
