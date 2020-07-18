using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Admin
{

    public class ToClientServiceEditAdmin : ToClientService
    {

        public bool Result { get; set; }

        public string Admin { get; set; }
    }
}
