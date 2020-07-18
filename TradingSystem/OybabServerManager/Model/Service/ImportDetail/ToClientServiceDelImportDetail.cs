using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.ImportDetail
{

    public class ToClientServiceDelImportDetail : ToClientService
    {

        public bool Result { get; set; }

        public string Import { get; set; }

        public string ImportDetail { get; set; }
    }
}
