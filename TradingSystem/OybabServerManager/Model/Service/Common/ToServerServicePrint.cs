using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Oybab.ServerManager.Model.Models;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToServerServicePrint : ToServerService
    {

        public ModelType ModelType { get; set; }

        public StatisticType StatisticType { get; set; }

        public string Model { get; set; }

        public string Printer { get; set; }

        public string ModelRef { get; set; }


        public long AdminId { get; set; }

        public long Lang { get; set; }

        public bool IsLocalPrint { get; set; }
    }
}
