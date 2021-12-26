using System;

namespace Simple.dotNet.Core.Data.Schema
{
    /// <summary>
    /// 标记为数据库表
    /// </summary>
    public interface IEntity
    {

    }
    /// <summary>
    /// 创建时间
    /// </summary>
    public interface ICreationTime
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateTime { get; set; }
    }
    /// <summary>
    /// 最后一次修改时间
    /// </summary>
    public interface ILastModifyTime
    {
        DateTime? LastModifyTime { get; set; }
    }
    /// <summary>
    /// 软删除
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        bool IsDeleted { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        DateTime? DeleteTime { get; set; }
    }
}
