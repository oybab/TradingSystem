using Oybab.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
    /// <summary>
    /// 数据库更新模型, 更新数据库后在日志插入
    /// </summary>
    internal sealed class DatabaseUpdateModel
    {
        public Database OldDatabase { get; set; }
        public Database NewDatabase { get; set; }
    }
}
