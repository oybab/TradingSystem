using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.BalancePay
{

    public class ToClientServiceGetBalancePay : ToClientService
    {

        public bool Result { get; set; }

        public string BalancePays { get; set; }

        public string OrderPays { get; set; }
        public string TakeoutPays { get; set; }
        public string MemberPays { get; set; }
        public string SupplierPays { get; set; }
        public string AdminPays { get; set; }

        public string ImportPays { get; set; }
    }
}
