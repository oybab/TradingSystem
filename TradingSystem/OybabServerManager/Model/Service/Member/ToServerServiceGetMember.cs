using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Member
{

    public class ToServerServiceGetMember : ToServerService
    {

        public long MemberId { get; set; }

        public bool SingleMemberNo { get; set; }

        public string MemberNo { get; set; }

        public string CardNo { get; set; }

        public string PWD { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

    }
}
