using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Import
{

    public class ToServerServiceEditImport : ToServerService
    {

        public string Import { get; set; }

        public string ImportSessionId { get; set; }



        public bool Rechecked { get; set; }
    }
}
