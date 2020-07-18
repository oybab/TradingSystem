using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Server.Model
{
    public sealed class ResultModel
    {
        public bool Result { get; set; }
        public bool ValidResult { get; set; }
        public bool IsDataHasRefrence { get; set; }// 删除ROOM, 编辑ROOM. 删除产品, 删除产品类型
        public bool IsRefreshSessionModel { get; set; }// 会话不一致(用于订单)

        public bool IsSessionModelSameTimeOperate { get; set; } // 同一时间不同操作访问
        public bool UpdateModel { get; set; } // 模型在服务器已更新, 需要先刷新当前模型

        public bool UpdateRefModel { get; set; } // 与该模型相关的模型在服务器已更新, 需要先刷新当前模型
        public bool IsExpired { get; set; } // 时间过期

        public bool IsAdminUsing { get; set; } // 当前管理员在使用中
    }
}
