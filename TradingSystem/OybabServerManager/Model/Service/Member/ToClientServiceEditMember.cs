using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Member
{

    public class ToClientServiceEditMember : ToClientService
    {

        public bool Result { get; set; }

        public bool IsMemberExists { get; set; }

        public bool IsCardExists { get; set; }

        public string Member { get; set; }
    }
}
