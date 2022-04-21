using Simple.Core.Data.Schema;
using Simple.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Test.Model
{
    [Table("Users")]
    public class User : IEntity
    {
        [Column("UserID")]
        public int ID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public decimal Money { get; set; }
        public DateTime CreateAt { get; set; }
        public UserStatus Status { get; set; }

    }
}
