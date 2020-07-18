using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToServerServiceNewRequest : ToServerService
    {

        public string AdminNo { get; set; }

        
        public string PWD { get; set; }

        public string Soft_Service_PC_Name { get; set; }

        public string Soft_Service_Tablet_Name { get; set; }

        public string Soft_Service_Mobile_Name { get; set; }

        public long DeviceType { get; set; }

        public string CM { get; set; }

        public string CI { get; set; }

        public DateTime Time { get; set; }

        public string CurrentVersion { get; set; }

        public bool IsLocalPrintCustomOrder { get; set; }

        public string DeviceId { get; set; }

    }
}
