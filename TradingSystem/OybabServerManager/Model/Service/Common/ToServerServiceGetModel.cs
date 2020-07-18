using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToServerServiceGetbak : ToServerService
    {

        public long AdminId { get; set; }

        public string ModelType { get; set; }
    }
}
