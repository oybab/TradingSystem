using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{

    internal sealed class Service
    {

        internal string IP { get; set; }

        internal string MAC { get; set; }

        internal long AdminId { get; set; }

        internal long DeviceId { get; set; }

        internal long Mode { get; set; }

        internal DateTime FirstLogin { get; set; }

        internal DateTime LastLogin { get; set; }

        internal DateTime CheckDate { get; set; }

        internal int LostCount { get; set; }

        internal int OldRemoveCount { get; set; }
    }
}
