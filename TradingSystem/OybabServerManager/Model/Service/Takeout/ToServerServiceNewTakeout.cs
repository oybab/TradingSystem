using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Takeout
{

    public class ToServerServiceNewTakeout : ToServerService
    {

        public string Takeout { get; set; }

        public string TakeoutDetails { get; set; }

        public string TakeoutPays { get; set; }


        public string TakeoutStateSession { get; set; }

    }
}
