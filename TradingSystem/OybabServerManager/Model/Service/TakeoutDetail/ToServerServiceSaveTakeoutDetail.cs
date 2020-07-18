using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.TakeoutDetail
{

    public class ToServerServiceSaveTakeoutDetail : ToServerService
    {

        public string Takeout { get; set; }

        public string TakeoutDetailsAdd { get; set; }

        public string TakeoutDetailsEdit { get; set; }

        public string TakeoutDetailsConfirm { get; set; }

        public string TakeoutStateSession { get; set; }
    }
}

