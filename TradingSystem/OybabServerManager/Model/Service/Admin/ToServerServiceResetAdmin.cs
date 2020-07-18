using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Admin
{

    public class ToServerServiceResetAdmin : ToServerService
    {

        public string Admin { get; set; }

        public string Password { get; set; }

    }
}
