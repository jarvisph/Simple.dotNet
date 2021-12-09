using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Simple.Core.Mapper;
using Simple.Core.Extensions;
using Simple.Test.Model;
using Simple.Core.Domain.Enums;

namespace Simple.Test
{
    [TestClass]
    public class Program
    {
        [TestMethod]
        public void Main()
        {
            //映射初始化写法1
            Stopwatch stopwatch = Stopwatch.StartNew();
            User user = new User()
            {
                CreateAt = DateTime.Now,
                ID = 1,
                Money = 10,
                Status = UserStatus.Deleted,
                UserName = "张三",
                Details = new List<UserDetail> {
                        new UserDetail{DetailId=1 },
                        new UserDetail{DetailId=2 }
                }
            };
            long time1 = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            var model = user.Map<UserModel>();
            model.Details = user.Details.Map<UserModelDetail>();
            long time2 = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            model = user.Map<UserModel>();
            model.Details = user.Details.Map<UserModelDetail>();
            user = model.Map<User>();
            user.Details = model.Details.Map<UserDetail>();

            long time3 = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            model = new UserModel
            {
                CreateAt = new DateTime(),
                Id = 1,
                Money = 10,
                Status = UserStatus.Deleted,
                UserName = "张三"
            };

            long time4 = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            List<User> users1 = new List<User>();
            List<UserModel> users = new List<UserModel>();
            for (int i = 0; i < 10000; i++)
            {
                users.Add(new UserModel
                {
                    CreateAt = new DateTime(),
                    Id = 1,
                    Money = 10,
                    Status = UserStatus.Deleted,
                    UserName = "张三"
                });
            }

            long time5 = stopwatch.ElapsedMilliseconds;
            for (int i = 0; i < 10000; i++)
            {
                users1.Add(user);
            }

            stopwatch.Restart();

            var users2 = users1.Map<UserModel>();
            users1 = users2.Map<User>();
            long time6 = stopwatch.ElapsedMilliseconds;


            Console.WriteLine("结束");

        }
    }
}
