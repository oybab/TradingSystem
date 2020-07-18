using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Device
{

    public class ToClientServiceAddDevice : ToClientService
    {

        public string Device { get; set; }

        public bool Result { get; set; }
    }
}
