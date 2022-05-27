using Microsoft.Extensions.DependencyInjection;
using Simple.Core.Dapper;
using Simple.Core.Data.Provider;
using Simple.Core.Data.Repository;
using Simple.Core.Localization;

namespace Simple.MySql
{
    public static class MySqlServiceCollectionExtensions
    {
        public static IServiceCollection AddMySql(this IServiceCollection services)
        {
            services.AddSingleton<IMySqlConnectionProvider, MySqlConnectionProvider>();
            return services;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <returns></returns>
        public static IServiceCollection AddMySql(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IWriteRepository>(opt => new MySqlRepository(connectionString));
            services.AddScoped<IReadRepository>(opt => new MySqlRepository(connectionString));
            services.AddScoped<IDapperDatabase>(opt => new MySqlRepository(connectionString));
            services.AddScoped<IProcedureRepository>(opt => new MySqlRepository(connectionString));
            services.AddSingleton<IMySqlConnectionProvider, MySqlConnectionProvider>();
            return services;
        }
    }
}
