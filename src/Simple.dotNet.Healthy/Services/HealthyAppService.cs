﻿using Simple.dotNet.Core.Dapper;
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

        public bool Cancel(string clientId)
        {
            return WriteRepository.Delete<HealthExamination>(c => c.ID == clientId);
        }

        public bool Check(string clientId)
        {
            var health = WriteRepository.FirstOrDefault<HealthExamination>(c => c.ID == clientId);
            if (health == null) return false;
            health.CheckAt = DateTime.Now;
            health.IsOnline = true;
            return WriteRepository.Update(health, c => c.ID == health.ID, c => c.CheckAt, c => c.IsOnline);
        }

        public void Check(Action<HealthExamination> action)
        {
            while (true)
            {
                var list = WriteRepository.GetAll<HealthExamination>().ToList();
                int interval = list.FirstOrDefault()?.Interval ?? 5000;
                foreach (var item in list.Where(c => c.Port == 0))
                {
                    if (DateTime.Now.AddMilliseconds((item.Interval * 5) * -1) > item.CheckAt)
                    {
                        using (IDapperDatabase db = CreateDatabase())
                        {
                            db.Update<HealthExamination, bool>(c => c.ID == item.ID, c => c.IsOnline, false);
                        }
                        action(item);
                    }
                }
                foreach (var item in list.Where(c => c.Port > 0))
                {
                    try
                    {
                        string result = NetHelper.Get($"http://{item.Host}:{item.Port}{item.HealthCheck}");
                        using (IDapperDatabase db = CreateDatabase())
                        {
                            item.CheckAt = DateTime.Now;
                            item.IsOnline = true;
                            db.Update(item, c => c.ID == item.ID, c => c.CheckAt, c => c.IsOnline);
                        }
                        Thread.Sleep(1000);
                    }
                    catch
                    {
                        using (IDapperDatabase db = CreateDatabase())
                        {
                            db.Update<HealthExamination, bool>(c => c.ID == item.ID, c => c.IsOnline, false);
                        }
                        action(item);
                    }
                }
                Thread.Sleep(interval);

            }
        }

        public bool Delete(string clientId)
        {
            return WriteRepository.Delete<HealthExamination>(c => c.ID == clientId);
        }

        public List<HealthExamination> List()
        {
            return WriteRepository.GetAll<HealthExamination>().ToList();
        }

        public bool Register(string clientId, HealthyOptions options)
        {
            if (string.IsNullOrWhiteSpace(clientId)) return false;
            if (string.IsNullOrWhiteSpace(options.ServiceName)) return false;
            if (string.IsNullOrWhiteSpace(options.Host)) return false;
            if (string.IsNullOrWhiteSpace(options.HostName)) return false;
            if (WriteRepository.Any<HealthExamination>(c => c.HostName == options.HostName && c.Host == options.Host && c.Port == options.Port && c.ServiceName == options.ServiceName))
            {
                WriteRepository.Delete<HealthExamination>(c => c.HostName == options.HostName && c.Host == options.Host && c.Port == options.Port && c.ServiceName == options.ServiceName);
            }
            HealthExamination health = new HealthExamination
            {
                CheckAt = DateTime.Now,
                ID = clientId,
                CreateAt = DateTime.Now,
                HealthCheck = options.HealthCheck ?? string.Empty,
                Host = options.Host ?? string.Empty,
                Port = options.Port ?? 0,
                ServiceName = options.ServiceName ?? string.Empty,
                IsOnline = true,
                HostName = options.HostName ?? string.Empty,
                Tags = options.Tags ?? string.Empty,
                Interval = options.Interval.HasValue ? options.Interval.Value.Milliseconds : 1000 * 5,
            };
            return WriteRepository.Insert(health);
        }
    }
}
