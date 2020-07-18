using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.ProductType
{

    public class ToClientServiceEditProductType : ToClientService
    {

        public bool Result { get; set; }

        public string ProductType { get; set; }
    }
}
