using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Oybab.ServerManager.Model;
using Oybab.ServerManager.Model.Service;

namespace Oybab.ServerManager.Model.Service.Takeout
{

    public class ToClientServiceEditTakeout : ToClientService
    {

        public bool Result { get; set; }

        public string TakeoutStateSession { get; set; }

        public string Takeout { get; set; }

        public long UpdateTime { get; set; }
    }
}
