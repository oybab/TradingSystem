using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Balance
{

    public class ToClientServiceEditBalance : ToClientService
    {

        public bool Result { get; set; }

        public bool IsBalanceExists { get; set; }

        public string Balance { get; set; }
    }
}
