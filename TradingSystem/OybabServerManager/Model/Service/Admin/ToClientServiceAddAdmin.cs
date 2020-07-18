using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Admin
{

    public class ToClientServiceAddAdmin : ToClientService
    {

        public string Admin { get; set; }

        public bool Result { get; set; }
    }
}
