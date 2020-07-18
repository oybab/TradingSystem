using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Printer
{

    public class ToClientServiceEditPrinter : ToClientService
    {

        public bool Result { get; set; }

        public string Printer { get; set; }
    }
}
