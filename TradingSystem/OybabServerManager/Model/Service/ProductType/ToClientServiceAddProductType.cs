using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.ProductType
{

    public class ToClientServiceAddProductType : ToClientService
    {

        public string ProductType { get; set; }

        public bool Result { get; set; }
    }
}
