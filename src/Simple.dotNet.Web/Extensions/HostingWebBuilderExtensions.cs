﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Web.Extensions
{
    /// <summary>
    /// Host主机扩展方法
    /// </summary>
    public static class HostingWebBuilderExtensions
    {
        /// <summary>
        /// 支持CLI
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseCli(this IWebHostBuilder hostBuilder, string[] args)
        {
            //支持cli
            var config = new ConfigurationBuilder()
                        .Build();
            return hostBuilder.UseConfiguration(config);
        }
    }
}
