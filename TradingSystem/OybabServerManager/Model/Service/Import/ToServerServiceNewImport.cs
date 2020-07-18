using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Import
{

    public class ToServerServiceNewImport : ToServerService
    {

        public string Import { get; set; }

        public string ImportDetails { get; set; }

        public string ImportPays { get; set; }


        public bool Rechecked { get; set; }
    }
}
