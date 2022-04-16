using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Domain.Enums;

namespace Simple.Test.Model
{
    public class UserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public UserStatus Status { get; set; }
        public decimal Money { get; set; }
        public DateTime CreateAt { get; set; }

        public List<UserModelDetail> Details { get; set; }
    }

    public class UserModelDetail
    {
        public int DetailId { get; set; }
    }
}
