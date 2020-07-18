using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Product
{

    public class ToClientServiceEditProduct : ToClientService
    {

        public bool Result { get; set; }

        public string Product { get; set; }

        public string Pprs { get; set; }
    }
}
