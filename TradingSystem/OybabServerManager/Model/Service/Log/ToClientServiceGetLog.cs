using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Log
{

    public class ToClientServiceGetLog : ToClientService
    {

        public bool Result { get; set; }

        public string Logs { get; set; }

        public string Balance { get; set; }
    }
}
