using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Device
{

    public class ToClientServiceDeviceModeUpdateNotification : ToClientService
    {

        public string Devices { get; set; }

        public ServiceType ServiceType { get; set; }
    }
}
