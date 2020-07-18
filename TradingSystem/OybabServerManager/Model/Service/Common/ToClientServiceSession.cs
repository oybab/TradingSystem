using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToClientServiceSession : ToClientService
    {

        public bool Result { get; set; }

        public string NewSessionId { get; set; }

        public bool IsNeedConfirm { get; set; }

        public string ConfirmToken { get; set; }

        public string RoomsModel { get; set; }

        public string Not { get; set; }


        public string ServiceModel { get; set; }
    }
}
