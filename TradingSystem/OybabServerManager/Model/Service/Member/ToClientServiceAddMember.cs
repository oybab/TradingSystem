using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Member
{

    public class ToClientServiceAddMember : ToClientService
    {

        public string Member { get; set; }

        public bool IsMemberExists { get; set; }

        public bool IsCardExists { get; set; }

        public bool Result { get; set; }
    }
}
