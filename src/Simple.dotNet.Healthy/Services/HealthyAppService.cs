using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Domain;
using Simple.dotNet.Healthy.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Simple.dotNet.Core.Extensions;
using System.Linq;
using Simple.dotNet.Core.Helper;
using System.Threading;
using Simple.dotNet.Core.Localization;
using Simple.dotNet.Core.Healthy;
using Simple.dotNet.Core.Data;
using System.Threading.Tasks;

namespace Simple.dotNet.Healthy.Services
{

    internal class HealthyAppService : AppServiceBase, IHealthyAppService
    {
        public HealthyAppService() : base(AppsettingConfig.GetConnectionString("DbConnection"), DatabaseType.Sqlite)
        {

        }

        public bool Cancel(string id)
        {
            return WriteRepository.Delete<HealthExamination>(c => c.ID == id);
        }

        public bool Check(string id)
        {
            var health = WriteRepository.FirstOrDefault<HealthExamination>(c => c.ID == id);
            if (health == null) return false;
            health.CheckAt = DateTime.Now;
            return WriteRepository.Update(health, c => c.CheckAt, health.CheckAt);
        }

        public void Check(Action<HealthExamination> action)
        {
            while (true)
            {
                var list = WriteRepository.GetAll<HealthExamination>().ToList();
                int interval = list.FirstOrDefault()?.Interval ?? 5000;
                foreach (var item in list.Where(c => c.Port == 0))
                {
                    if (DateTime.Now.AddMilliseconds((item.Interval * 10) * -1) > item.CheckAt)
                    {
                        using (IDapperDatabase db = CreateDatabase())
                        {
                            db.Delete<HealthExamination>(c => c.ID == item.ID);
                        }
                        action(item);
                    }
                }
                foreach (var item in list.Where(c => c.Port > 0))
                {
                    try
                    {
                        string result = NetHelper.Get($"http://{item.Host}/{item.HealthCheck}");
                    }
                    catch
                    {
                        using (IDapperDatabase db = CreateDatabase())
                        {
                            db.Delete<HealthExamination>(c => c.ID == item.ID);
                        }
                        action(item);
                    }
                }
                Thread.Sleep(interval);
            }
        }

        public List<HealthExamination> List()
        {
            return WriteRepository.GetAll<HealthExamination>().ToList();
        }

        public bool Register(string id, HealthyOptions options)
        {
            HealthExamination health = new HealthExamination
            {
                CheckAt = DateTime.Now,
                ID = id,
                CreateAt = DateTime.Now,
                HealthCheck = options.HealthCheck ?? string.Empty,
                Host = options.Host ?? string.Empty,
                Port = options.Port ?? 0,
                ServiceName = options.ServiceName ?? string.Empty,
                IsOnline = true,
                Interval = options.Interval.HasValue ? options.Interval.Value.Milliseconds : 1000 * 5,
            };
            return WriteRepository.Insert(health);
        }
    }
}
