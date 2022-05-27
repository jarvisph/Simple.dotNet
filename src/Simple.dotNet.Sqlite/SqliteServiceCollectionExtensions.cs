using Microsoft.Extensions.DependencyInjection;
using Simple.Core.Dapper;
using Simple.Core.Data.Provider;
using Simple.Core.Data.Repository;

namespace Simple.Sqlite
{

    public static class SqliteServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlite(this IServiceCollection services)
        {
            services.AddSingleton<ISqliteConnectionProvider, SqliteConnectionProvider>();
            return services;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">数据库链接字符串</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlite(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IWriteRepository>(opt => new SqliteRepository(connectionString));
            services.AddScoped<IReadRepository>(opt => new SqliteRepository(connectionString));
            services.AddScoped<IDapperDatabase>(opt => new SqliteRepository(connectionString));
            services.AddScoped<IProcedureRepository>(opt => new SqliteRepository(connectionString));
            services.AddSingleton<ISqliteConnectionProvider, SqliteConnectionProvider>();
            return services;
        }
    }
}
