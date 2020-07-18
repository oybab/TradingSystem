using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Oybab.ServerManager.Model.Models;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToClientServiceSendNotification : ToClientService
    {

        public SendType SendType { get; set; }

        public long? RoomId { get; set; }

        public long? DeviceId { get; set; }

        public string Identity { get; set; }

        public string Message { get; set; }

        public string Model { get; set; }
        public string ModelExt { get; set; }

        public long AdminId { get; set; }

    }
}
