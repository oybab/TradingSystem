using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.AdminLog
{

    public class ToClientServiceAddAdminLog : ToClientService
    {

        public string AdminLog { get; set; }

        public bool Result { get; set; }
    }
}
