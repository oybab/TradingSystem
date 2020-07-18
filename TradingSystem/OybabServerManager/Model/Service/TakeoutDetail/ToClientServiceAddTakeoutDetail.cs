using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.TakeoutDetail
{

    public class ToClientServiceAddTakeoutDetail : ToClientService
    {

        public string Takeout { get; set; }

        public bool Result { get; set; }

        public string TakeoutDetails { get; set; }

        public string TakeoutSessionId { get; set; }
    }
}
