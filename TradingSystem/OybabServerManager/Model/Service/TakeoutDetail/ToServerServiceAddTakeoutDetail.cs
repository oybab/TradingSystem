using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.TakeoutDetail
{
    public class ToServerServiceAddTakeoutDetail : ToServerService
    {

        public bool IsNewTakeout { get; set; }

        public string Takeout { get; set; }

        public string TakeoutDetails { get; set; }


        public string TakeoutStateSession { get; set; }

    }
}
