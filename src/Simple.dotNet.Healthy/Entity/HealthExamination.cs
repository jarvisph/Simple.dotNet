using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Data.Schema;

namespace Simple.dotNet.Healthy.Entity
{
    [Table("HealthExamination")]
    public class HealthExamination : IEntity
    {

        #region ======== 构造函数 ===========
        public HealthExamination() { }




        #endregion

        #region ======== 数据库字段 ========

        /// <summary>
        /// 注册ID
        /// </summary>
        [Column("ClientID"), Key]
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 服务名
        /// </summary>
        [Column("ServiceName")]
        public string ServiceName { get; set; } = string.Empty;


        /// <summary>
        /// Host
        /// </summary>
        [Column("Host")]
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// 主机名称
        /// </summary>
        public string HostName { get; set; } = string.Empty;
        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; } = string.Empty;
        /// <summary>
        /// 服务端口
        /// </summary>
        [Column("Port")]
        public int Port { get; set; }


        /// <summary>
        /// 检查地址
        /// </summary>
        [Column("HealthCheck")]
        public string HealthCheck { get; set; } = string.Empty;


        /// <summary>
        /// 是否在线
        /// </summary>
        [Column("IsOnline")]
        public bool IsOnline { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        [Column("CreateAt")]
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// 检查时间
        /// </summary>
        [Column("CheckAt")]
        public DateTime CheckAt { get; set; }

        /// <summary>
        /// 检查间隔时间
        /// </summary>
        [Column("Interval")]
        public int Interval { get; set; }
        #endregion


        #region ======== 扩展方法 =========


        #endregion

    }

}
