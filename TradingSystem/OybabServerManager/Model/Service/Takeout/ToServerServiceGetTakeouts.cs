using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Takeout
{

    public class ToServerServiceGetTakeouts : ToServerService
    {

        public bool IsFromCacheOnly { get; set; }

        public long AddTimeStart { get; set; }

        public long AddTimeEnd { get; set; }

        public long SendAdminId { get; set; }

        public long FinishTime { get; set; }

        public long State { get; set; }

        public long AdminId { get; set; }

        public long FinishAdminId { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string MemberNo { get; set; }

        public string CardNo { get; set; }

        public bool IsIncludeRef { get; set; }


    }
}
