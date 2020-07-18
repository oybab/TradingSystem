using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Oybab.ServerManager.Model;
using Oybab.ServerManager.Model.Service;

namespace Oybab.ServerManager.Model.Service.Import
{

    public class ToClientServiceEditImport : ToClientService
    {

        public bool Result { get; set; }

        public string Import { get; set; }

        public string ImportSessionId { get; set; }

        public long UpdateTime { get; set; }
    }
}
