using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.AdminPay
{

    public class ToClientServiceDelAdminPay : ToClientService
    {

        public bool Result { get; set; }

        public string Admin { get; set; }
    }
}
