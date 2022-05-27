using Microsoft.Extensions.DependencyInjection;
using Simple.Core.Dapper;
using Simple.Core.Data.Provider;
using Simple.Core.Data.Repository;

namespace Simple.Core.Data
{
    public static class SqlServerServiceCollectionExtensions
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <returns>默认sqlserver</returns>
        public static IServiceCollection AddSqlServer(this IServiceCollection services)
        {
            services.AddSingleton<ISqlServerConnectionProvider, SqlServerConnectionProvider>();
            return services;
        }
        public static IServiceCollection AddSqlServer(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IWriteRepository>(opt => new SqlServerRepository(connectionString));
            services.AddScoped<IReadRepository>(opt => new SqlServerRepository(connectionString));
            services.AddScoped<IDapperDatabase>(opt => new SqlServerRepository(connectionString));
            services.AddScoped<IProcedureRepository>(opt => new SqlServerRepository(connectionString));
            services.AddSingleton(c => new DbConnection(connectionString, DatabaseType.SqlServer));
            services.AddSingleton<ISqlServerConnectionProvider, SqlServerConnectionProvider>();
            return services;
        }
    }
}
