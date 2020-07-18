using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Printer
{

    public class ToClientServiceAddPrinter : ToClientService
    {

        public string Printer { get; set; }

        public bool Result { get; set; }
    }
}
