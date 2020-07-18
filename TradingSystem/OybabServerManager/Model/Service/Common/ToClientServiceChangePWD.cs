using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToClientServiceChangePWD : ToClientService
    {

        public bool Result { get; set; }

        public bool ValidResult { get; set; }

        public string Admin { get; set; }
    }
}
