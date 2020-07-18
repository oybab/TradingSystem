using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Oybab.ServerManager.Model;
using Oybab.ServerManager.Model.Service;

namespace Oybab.ServerManager.Model.Service.Import
{

    public class ToClientServiceNewImport : ToClientService
    {

        public bool Result { get; set; }

        public string Import { get; set; }

        public string ImportDetails { get; set; }

        public string ImportPays { get; set; }

        public long UpdateTime { get; set; }
    }
}
