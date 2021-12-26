﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Simple.dotNet.Redis
{
    /// <summary>
    /// 注册Redis
    /// </summary>
    public static class RedisServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            return services;
        }
    }
}
