using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Web.Jwt
{
    public static class JwtSerivceCollectionExtensions
    {
        public static IServiceCollection AddJwt(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(c => new JWTOption(connectionString));
            return services;
        }
    }
}
