using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service
{
    public enum ServiceExceptionType
    {
        /// <summary>
        /// 正常
        /// </summary>
        None,
        /// <summary>
        /// 服务器错误
        /// </summary>
        ServerFaild,
        /// <summary>
        /// 找不到KEY
        /// </summary>
        KeyCheckFaild,
        /// <summary>
        /// 应用程序验证失败
        /// </summary>
        ApplicationValidFaild,
        /// <summary>
        /// 数据未准备好
        /// </summary>
        DataNotReady,
        /// <summary>
        /// KEY内部错误
        /// </summary>
        KeyFaild,
        /// <summary>
        /// 数据获取错误
        /// </summary>
        DataFaild,
        /// <summary>
        /// 超出连接限制
        /// </summary>
        CountOutOfLimit,
        /// <summary>
        /// 超出2次同一个IP连接限制
        /// </summary>
        CountOutOfIPRequestLimit,
        /// <summary>
        /// 数据有关系无法删除
        /// </summary>
        DataHasRefrence,
        /// <summary>
        /// 刷新会话模型模型
        /// </summary>
        RefreshSessionModel,
        /// <summary>
        /// 刷新会话模型模型(因为同一时间操作)
        /// </summary>
        RefreshSessionModelForSameTimeOperate,
        /// <summary>
        /// 更新模型
        /// </summary>
        UpdateModel,
        /// <summary>
        /// 更新辅助模型
        /// </summary>
        UpdateRefModel,
        /// <summary>
        /// 产品数量不够
        /// </summary>
        ProductCountLimit,
        /// <summary>
        /// IP无效
        /// </summary>
        IPConflict,
        /// <summary>
        /// 刷新模型(数据有改动)
        /// </summary>
        IPInvalid,
        /// <summary>
        /// 会话过期
        /// </summary>
        SessionExpired,
        /// <summary>
        /// 会话更新
        /// </summary>
        SessionUpdate,
        /// <summary>
        /// 无效会话, IP跟当初不一样
        /// </summary>
        SessionInvalid,
        /// <summary>
        /// 重新登录
        /// </summary>
        Relogin,
        /// <summary>
        /// 未找到设备(设备被禁用, 设备类型不匹配)
        /// </summary>
        UnknownDevice,
        /// <summary>
        /// 雅座超出最大限制
        /// </summary>
        RoomCountOutOfLimit,
        /// <summary>
        /// 设备超出最大限制
        /// </summary>
        DeviceCountOutOfLimit,
        /// <summary>
        /// 未找到用户(用户不存在或被禁用)
        /// </summary>
        UnknownAdmin,
        /// <summary>
        /// 管理员已存在
        /// </summary>
        AdminExists,
        /// <summary>
        /// 设备已存在
        /// </summary>
        DeviceExists,
        /// <summary>
        /// 数据库不存在或不准确配置
        /// </summary>
        DatabaseNotFound,
        /// <summary>
        /// 数据库加载失败
        /// </summary>
        DatabaseLoadFailed,
        /// <summary>
        /// 管理员使用中
        /// </summary>
        AdminUsing,
        /// <summary>
        /// 请求超载
        /// </summary>
        RequestOverload,
        /// <summary>
        /// 服务器和客户端时间不对齐
        /// </summary>
        ServerClientTimeMisalignment,
        /// <summary>
        /// 服务端和客户端版本不一致(一本用于提醒低版本客户端连接新版服务端. 因为新版数据库更新会导致服务端无法正常和客户端正常交互)
        /// </summary>
        ServerClientVersionMisalignment,
        /// <summary>
        /// 密码错误次数过多
        /// </summary>
        PasswordErrorCountLimit,
        /// <summary>
        /// 自定义1
        /// </summary>
        Custom1,
        /// <summary>
        /// 自定义2
        /// </summary>
        Custom2,
        /// <summary>
        /// 自定义3
        /// </summary>
        Custom3
    }
}
