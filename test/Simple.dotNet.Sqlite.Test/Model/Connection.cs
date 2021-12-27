using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.dotNet.Core.Data;
using Simple.dotNet.Core.Data.Schema;

namespace Simple.dotNet.Sqlite.Test.Model
{
    public class Connection : IEntity
    {
        /// <summary>
        /// 链接ID
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ConnectionID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string ConnectionName { get; set; }
        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 链接类型
        /// </summary>
        public DatabaseType DatabaseType { get; set; }
    }
}
