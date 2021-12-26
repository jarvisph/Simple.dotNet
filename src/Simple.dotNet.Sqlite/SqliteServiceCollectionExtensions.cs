using Microsoft.Extensions.DependencyInjection;
using Simple.dotNet.Core.Dapper;
using Simple.dotNet.Core.Data.Provider;
using Simple.dotNet.Core.Data.Repository;
using Simple.dotNet.Core.Localization;

namespace Simple.dotNet.Sqlite
{

    public static class SqliteServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlite(this IServiceCollection services)
        {
            return services.AddSqlite(AppsettingConfig.GetConnectionString("DbConnection"));
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
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="writeConnectionString">写库</param>
        /// <param name="readConnectionString">读库</param>
        /// <returns></returns>
        public static IServiceCollection AddSqlite(this IServiceCollection services, string writeConnectionString, string readConnectionString)
        {
            services.AddScoped<IWriteRepository>(opt => new SqliteRepository(writeConnectionString));
            services.AddScoped<IReadRepository>(opt => new SqliteRepository(readConnectionString));
            services.AddScoped<IDapperDatabase>(opt => new SqliteRepository(writeConnectionString));
            services.AddScoped<IProcedureRepository>(opt => new SqliteRepository(writeConnectionString));
            services.AddSingleton<ISqliteConnectionProvider, SqliteConnectionProvider>();
            return services;
        }
    }
}
