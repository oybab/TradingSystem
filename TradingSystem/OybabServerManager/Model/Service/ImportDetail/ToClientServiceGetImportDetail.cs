using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.ImportDetail
{

    public class ToClientServiceGetImportDetail : ToClientService
    {

        public bool Result { get; set; }

        public string ImportDetails { get; set; }

        public string ImportPays { get; set; }
    }
}
