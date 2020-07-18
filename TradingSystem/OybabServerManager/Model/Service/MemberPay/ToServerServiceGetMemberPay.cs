using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.MemberPay
{

    public class ToServerServiceGetMemberPay : ToServerService
    {

        public long BalanceType { get; set; }

        public long MemberId { get; set; }

        public long AddAdminId { get; set; }

        public long AddTimeStart { get; set; }

        public long AddTimeEnd{ get; set; }

    }
}
