using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Order
{

    public class ToServerServiceGetOrders: ToServerService
    {

        public long StartTime { get; set; }

        public long EndTime { get; set; }

        public long AddTimeStart { get; set; }

        public long AddTimeEnd { get; set; }

        public long FinishTime { get; set; }

        public long RoomId { get; set; }

        public long AdminId { get; set; }

        public long FinishAdminId { get; set; }

        public long State { get; set; }

        public string MemberNo { get; set; }

        public string CardNo { get; set; }

        public bool IsIncludeRef { get; set; }


    }
}
