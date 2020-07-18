using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.MemberPay
{

    public class ToServerServiceDelMemberPay : ToServerService
    {

        public string MemberPay { get; set; }

        public string Member { get; set; }

    }
}
