using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToServerServiceSetContent : ToServerService
    {


        public long AdminId { get; set; }


        public string Key { get; set; }

        public string Value { get; set; }

        public string Token { get; set; }
    }
}
