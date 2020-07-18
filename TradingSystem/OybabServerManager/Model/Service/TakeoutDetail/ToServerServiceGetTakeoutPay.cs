using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.TakeoutDetail
{

    public class ToServerServiceGetTakeoutPay : ToServerService
    {

        public long AddTimeStart { get; set; }
        public long AddTimeEnd { get; set; }

    }
}
