using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Import
{

    public class ToClientServiceGetImports : ToClientService
    {

        public bool Result { get; set; }

        public string Imports { get; set; }
    }
}
