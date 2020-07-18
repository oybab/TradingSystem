using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Oybab.ServerManager.Model.Models;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToServerServiceSend : ToServerService
    {

        public string RoomsId { get; set; }

        public SendType SendType { get; set; }

        public string Message { get; set; }

        public string MessageExt { get; set; }

        public string Model { get; set; }

        public string ModelExt { get; set; }

        public long AdminId { get; set; }
    }
}
