using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.BalancePay
{

    public class ToServerServiceGetBalancePay : ToServerService
    {
        public long BalanceType { get; set; }

        public long AdminId { get; set; }

        public long NotAdminId { get; set; }

        public long BalanceId { get; set; }

        public long AddTimeStart { get; set; }

        public long AddTimeEnd{ get; set; }

    }
}
