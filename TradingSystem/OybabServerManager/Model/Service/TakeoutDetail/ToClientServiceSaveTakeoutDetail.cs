using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.TakeoutDetail
{

    public class ToClientServiceSaveTakeoutDetail : ToClientService
    {

        public bool Result { get; set; }

        public string TakeoutDetailAdd { get; set; }

        public string TakeoutDetailEdit { get; set; }

        public string TakeoutDetailConfirm { get; set; }

        public string TakeoutSessionId { get; set; }

        public long UpdateTime { get; set; }

        public string Takeout { get; set; }

    }
}
