using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.AdminLog
{

    public class ToClientServiceEditAdminLog : ToClientService
    {

        public bool Result { get; set; }

        public string AdminLog { get; set; }
    }
}
