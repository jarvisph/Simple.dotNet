using Simple.Core.Data.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Test.Model
{
    public class User : IEntity
    {
        [Column("userid")]
        public int Id { get; set; }
        [Column("username")]
        public string Name { get; set; } = string.Empty;
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("isadmin")]
        public bool IsAdmin { get; set; }
        [Column("age")]
        public int Age { get; set; }
        [Column("created")]
        public DateTime Created { get; set; }
    }
}
