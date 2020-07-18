using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToClientServiceGetModel : ToClientService
    {

         public bool Result { get; set; }

         public string Model { get; set; }

         public string ModelRef { get; set; }

         public string ModelRef2 { get; set; }
    }
}
