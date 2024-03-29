﻿using Microsoft.Extensions.Configuration;
using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Simple.Core.Localization
{
    /// <summary>
    /// 本地appsettings.json配置
    /// </summary>
    public static class AppsettingConfig
    {

        private static readonly IConfigurationRoot config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true) //增加环境配置文件
                                                                                                                                                   //.AddEnvironmentVariables()
            .Build();
        static AppsettingConfig()
        {
            // 获取当前项目的服务名
            var entryAssembly = Assembly.GetEntryAssembly();
            ServiceName = entryAssembly.EntryPoint.Module.Name.Substring(0, entryAssembly.EntryPoint.Module.Name.LastIndexOf("."));
        }
        public static string ServiceName { get; }

        public static IConfigurationRoot GetConfig() => config;

        public static string GetConfig(string application, string name)
        {
            return config[$"{application}:{name}"];
        }
        public static T GetConfig<T>(string application) where T : class, new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            T config = new T();
            foreach (PropertyInfo property in properties)
            {
                object value = GetConfig(application, property.Name);
                if (value == null) continue;
                property.SetValue(config, value.GetValue(property.PropertyType));
            }
            return config;
        }

        public static string GetConfig(params string[] args)
        {
            return config[string.Join(":", args)];
        }

        public static string GetConnectionString(string name)
        {
            return config.GetConnectionString(name);
        }
    }
}
