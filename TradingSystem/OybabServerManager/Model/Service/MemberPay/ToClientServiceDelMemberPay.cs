using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.MemberPay
{

    public class ToClientServiceDelMemberPay : ToClientService
    {

        public bool Result { get; set; }

        public string Member { get; set; }
    }
}
