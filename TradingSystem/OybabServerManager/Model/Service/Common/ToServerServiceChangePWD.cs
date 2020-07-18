using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{
    public class ToServerServiceChangePWD : ToServerService
    {



        public string OldPWD { get; set; }

        public string NewPWD { get; set; }

        public string Admin { get; set; }
    }
}
