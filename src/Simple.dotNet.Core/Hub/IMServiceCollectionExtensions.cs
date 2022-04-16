using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Hub
{
    public static class IMServiceCollectionExtensions
    {
        public static IServiceCollection UseIM(this IServiceCollection services)
        {
            return services;
        }
    }
}
