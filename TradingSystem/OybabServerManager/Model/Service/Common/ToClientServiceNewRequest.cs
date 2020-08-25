using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToClientServiceNewRequest : ToClientService
    {

        public bool Result { get; set; }

        public bool ValidResult { get; set; }

        public string SessionId { get; set; }

        public bool IsExpired { get; set; }

        public bool IsAdminUsing { get; set; }

        public int ExpiredRemaningDays { get; set; }

        public string RegTimeRequestCode { get; set; }

        public bool ChangePassword { get; set; }

        public string Name_0 { get; set; }

        public string Name_1 { get; set; }

        public string Name_2 { get; set; }

        public string Admin { get; set; }

        public string Device { get; set; }

        public string Rooms { get; set; }

        public string RoomsModel { get; set; }

        public string TakeoutModel { get; set; }

        public string ProductTypes { get; set; }

        public string Products { get; set; }

        public string Admins { get; set; }

        public string Printers { get; set; }

        public string Pprs { get; set; }

        public string Devices { get; set; }

        public string Requests { get; set; }

        public string Members { get; set; }

        public string Services { get; set; }

        public string Balances { get; set; }

        public string Config { get; set; }

        public int DeviceCount { get; set; }

        public int RoomCount { get; set; }

        public int MinutesIntervalTime { get; set; }

        public int HoursIntervalTime { get; set; }

        public string PrintInfo { get; set; }

        public bool IsFireAlarmEnable { get; set; }
        
        public string ExtendInfo { get; set; }

        public string ExtendMessage { get; set; }

    }
}
