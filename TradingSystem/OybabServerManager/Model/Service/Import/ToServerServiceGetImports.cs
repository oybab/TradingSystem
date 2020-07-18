using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Import
{

    public class ToServerServiceGetImports : ToServerService
    {

        public long ImportTimeStart { get; set; }

        public long ImportTimeEnd { get; set; }

        public long AddTimeStart { get; set; }

        public long AddTimeEnd { get; set; }

        public long FinishAdminId { get; set; }

        public string SupplierNo { get; set; }

        public string CardNo { get; set; }

        public bool IsIncludeRef { get; set; }


    }
}
