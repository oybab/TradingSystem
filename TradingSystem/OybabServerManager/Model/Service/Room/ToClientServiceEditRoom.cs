using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Room
{

    public class ToClientServiceEditRoom : ToClientService
    {

        public bool Result { get; set; }
        public string RoomStateSession { get; set; }

        public string Room { get; set; }
    }
}
