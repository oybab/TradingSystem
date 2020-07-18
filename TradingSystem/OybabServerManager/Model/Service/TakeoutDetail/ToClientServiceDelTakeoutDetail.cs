using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.TakeoutDetail
{

    public class ToClientServiceDelTakeoutDetail : ToClientService
    {

        public bool Result { get; set; }

        public string TakeoutSessionId { get; set; }

        public long UpdateTime { get; set; }
    }
}
