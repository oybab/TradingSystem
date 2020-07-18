using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Balance
{

    public class ToClientServiceGetBalance : ToClientService
    {

        public bool Result { get; set; }

        public string Balances { get; set; }
    }
}
