using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Data.Schema;
using Simple.Core.Domain.Enums;

namespace Simple.Test.Model
{
    [Table("Users")]
    public class User : IEntity
    {
        [Column("UserID")]
        public int ID { get; set; }
        public string UserName { get; set; }

        public UserStatus Status { get; set; }
        public decimal Money { get; set; }
        public DateTime CreateAt { get; set; }

        [NotMapped]
        public List<UserDetail> Details { get; set; }
    }
    public class UserDetail
    {
        public int DetailId { get; set; }
    }
}
