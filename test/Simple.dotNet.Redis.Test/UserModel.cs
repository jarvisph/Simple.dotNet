using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Domain.Enums;

namespace Simple.Redis.Test
{
    public struct UserModel
    {
        public int Id;
        public string Name;
        public string Password;
        public string Email;
        public UserStatus Status;
        public static implicit operator HashEntry[](UserModel user)
        {
            return new HashEntry[]
            {
                //new HashEntry("Id",user.Id.ToRedisValue()),
                //new HashEntry("Name",user.Name.ToRedisValue()),
                //new HashEntry("Password",user.Password.ToRedisValue()),
                //new HashEntry("Email",user.Email.ToRedisValue()),
                //new HashEntry ("Status",user.Status.ToRedisValue())
            };
        }
    }
}
