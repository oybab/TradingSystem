using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.MemberPay
{

    public class ToClientServiceGetMemberPay : ToClientService
    {

        public bool Result { get; set; }

        public string MemberPays { get; set; }

        public string OrderPays { get; set; }
        public string TakeoutPays { get; set; }

    }
}
