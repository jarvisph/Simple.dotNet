using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Data.Schema;

namespace Simple.Sqlite.Test.Model
{
    [Table("user")]
    public class User : IEntity
    {
        public int Id { get; set; }
    }
}
