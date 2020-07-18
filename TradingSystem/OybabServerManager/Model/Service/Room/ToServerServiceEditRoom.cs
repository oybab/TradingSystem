using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Room
{

    public class ToServerServiceEditRoom : ToServerService
    {

        public string Room { get; set; }
        public string RoomStateSession { get; set; }

    }
}
