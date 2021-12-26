using Microsoft.Extensions.DependencyInjection;
using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Data.Provider;
using Simple.dotNet.Core.Data.Repository;
using Simple.dotNet.Core.Localization;

namespace Simple.dotNet.Core.Data
{
    public static class SqlServerServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServer(this IServiceCollection services)
        {
            return services.AddSqlServer(AppsettingConfig.GetConnectionString("DbConnection"));
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlServer(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IWriteRepository>(opt => new SqlServerRepository(connectionString));
            services.AddScoped<IReadRepository>(opt => new SqlServerRepository(connectionString));
            services.AddScoped<IDapperDatabase>(opt => new SqlServerRepository(connectionString));
            services.AddScoped<IProcedureRepository>(opt => new SqlServerRepository(connectionString));
            services.AddSingleton<ISqlServerConnectionProvider, SqlServerConnectionProvider>();
            return services;
        }
        public static IServiceCollection AddSqlServerProvider(this IServiceCollection services)
        {
            services.AddSingleton<ISqlServerConnectionProvider, SqlServerConnectionProvider>();
            return services;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="writeConnectionString">写库</param>
        /// <param name="readConnectionString">读库</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlServer(this IServiceCollection services, string writeConnectionString, string readConnectionString)
        {
            services.AddScoped<IWriteRepository>(opt => new SqlServerRepository(writeConnectionString));
            services.AddScoped<IReadRepository>(opt => new SqlServerRepository(readConnectionString));
            services.AddScoped<IDapperDatabase>(opt => new SqlServerRepository(writeConnectionString));
            services.AddScoped<IProcedureRepository>(opt => new SqlServerRepository(writeConnectionString));
            services.AddSingleton<ISqlServerConnectionProvider, SqlServerConnectionProvider>();
            return services;
        }
    }
}
