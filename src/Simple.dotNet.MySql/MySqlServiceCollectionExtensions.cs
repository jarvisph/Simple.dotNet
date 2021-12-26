using Microsoft.Extensions.DependencyInjection;
using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Data.Provider;
using Simple.dotNet.Core.Data.Repository;
using Simple.dotNet.Core.Localization;

namespace Simple.dotNet.MySql
{
    public static class MySqlServiceCollectionExtensions
    {
        public static IServiceCollection AddMySql(this IServiceCollection services)
        {
            return services.AddMySql(AppsettingConfig.GetConnectionString("DbConnection"));
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
        public static IServiceCollection AddMySqlProvider(this IServiceCollection services)
        {
            services.AddSingleton<IMySqlConnectionProvider, MySqlConnectionProvider>();
            return services;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="writeConnectionString">写库</param>
        /// <param name="readConnectionString">读库</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlite(this IServiceCollection services, string writeConnectionString, string readConnectionString)
        {
            services.AddScoped<IWriteRepository>(opt => new MySqlRepository(writeConnectionString));
            services.AddScoped<IReadRepository>(opt => new MySqlRepository(readConnectionString));
            services.AddScoped<IDapperDatabase>(opt => new MySqlRepository(writeConnectionString));
            services.AddScoped<IProcedureRepository>(opt => new MySqlRepository(writeConnectionString));
            return services;
        }
    }
}
