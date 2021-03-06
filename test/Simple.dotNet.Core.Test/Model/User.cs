using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Data.Schema;
using Simple.Core.Domain.Enums;

namespace Simple.Core.Test.Model
{
    [Table("Users")]
    public class User : UserBase, IEntity
    {

        #region ======== 构造函数 ===========
        public User() { }




        #endregion

        #region ======== 数据库字段 ========

        ///<summary>
        ///[ID]会员ID
        ///</summary>
        [Column("UserID"), DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public int ID { get; set; }


        [Column("UserName")]
        public override string UserName { get; set; } = string.Empty;


        ///<summary>
        ///会员状态
        ///</summary>
        [Column("Status")]
        public UserStatus Status { get; set; }


        ///<summary>
        ///余额
        ///</summary>
        [Column("Money")]
        public decimal Money { get; set; }


        ///<summary>
        ///创建时间
        ///</summary>
        [Column("CreateAt")]
        public DateTime CreateAt { get; set; }

        #endregion


        #region ======== 扩展方法 =========

        [NotMapped]
        public List<UserDetail> Details { get; set; }

        #endregion

    }

}
