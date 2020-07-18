using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.AdminLog
{

    public class ToServerServiceGetAdminLog : ToServerService
    {

        public long AdminId { get; set; }

        public long AddTimeStart { get; set; }

        public long AddTimeEnd{ get; set; }

        public long LogTimeStart { get; set; }

        public long LogTimeEnd { get; set; }

    }
}
