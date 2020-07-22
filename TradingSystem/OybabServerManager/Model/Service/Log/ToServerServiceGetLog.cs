using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Log
{

    public class ToServerServiceGetLog : ToServerService
    {

        public long AdminId { get; set; }

        public long AddTimeStart { get; set; }

        public long AddTimeEnd{ get; set; }

        public long IsBalanceChange { get; set; }

        public long BalanceType { get; set; }

        public long OperateId { get; set; }

        public long IsBalancePrice { get; set; }

        public long OperateTypeId { get; set; }

    }
}
