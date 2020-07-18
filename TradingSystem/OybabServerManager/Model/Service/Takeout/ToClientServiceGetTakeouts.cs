using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Takeout
{

    public class ToClientServiceGetTakeouts : ToClientService
    {

        public bool Result { get; set; }

        public string Takeouts { get; set; }
    }
}
