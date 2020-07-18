using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Common
{

    public class ToServerServiceSession : ToServerService
    {

        public long[] RoomsId { get; set; }


        public string RoomsInfoModel { get; set; }

        public bool RefreshService { get; set; }

        /// <summary>
        /// 新连接(手机客户端Abort掉了连接继续操作时用)
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// (SignalR模式, 这个模式下重新连接时把所有数据重新推送一遍)
        /// </summary>
        public bool IsSignalRMode { get; set; }

        public bool IsNeedConfirm { get; set; }

        public string ConfirmToken { get; set; }
    }
}
