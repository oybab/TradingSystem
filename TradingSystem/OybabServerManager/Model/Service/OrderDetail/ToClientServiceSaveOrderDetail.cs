using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.OrderDetail
{

    public class ToClientServiceSaveOrderDetail : ToClientService
    {

        public bool Result { get; set; }

        public string OrderDetailAdd { get; set; }

        public string OrderPayAdd { get; set; }

        public string OrderDetailEdit { get; set; }

        public string OrderDetailConfirm { get; set; }

        public string Order { get; set; }

        public string OrderSessionId { get; set; }

        public long UpdateTime { get; set; }

    }
}
