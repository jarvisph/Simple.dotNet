using Microsoft.IdentityModel.Tokens;
using Simple.Core.Dependency;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Simple.Redis
{
    public abstract class RedisDatabase
    {
        private static ConnectionMultiplexer _connectionMultiplexer;
        private static RedisConnection _connectionString;

        private static readonly object _lock = new object();
        /// <summary>
        /// 获取Database
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IDatabase Redis => Connection().GetDatabase(this.Db);

        /// <summary>
        /// 库 
        /// </summary>
        protected virtual int Db => -1;

        public ConnectionMultiplexer Connection()
        {
            return _connectionMultiplexer;
        }

        public RedisDatabase()
        {
            _connectionString = IocCollection.Resolve<RedisConnection>();
            lock (_lock)
            {
                if (_connectionMultiplexer == null)
                {
                    ConfigurationOptions opt = ConfigurationOptions.Parse(_connectionString.ConnectionString);
                    opt.SyncTimeout = int.MaxValue;
                    opt.AllowAdmin = true;
                    opt.AbortOnConnectFail = true;
                    _connectionMultiplexer = ConnectionMultiplexer.Connect(opt);
                }
            }
        }

        public void SetHash(string key, string hashKey, object value) => Redis.HashSet(key, hashKey.GetRedisValue(), value.GetRedisValue());

        public T GetHash<T>(string key, string hashKey) => this.Redis.HashGet(key, hashKey).GetRedisValue<T>();


        protected const string LOGIN_TOKEN = "LOGIN_TOKEN";

        public string GenerateToken(string userId, string userName, TimeSpan expires)
        {
            // 1. 创建 Claims（用户信息）
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(ClaimTypes.Role, "User"),
                //new Claim("tenant_id", "12345")
            };

            // 2. 从配置获取密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("134c5503debc47b0a258370fb9839b09"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. 创建 Token
            var token = new JwtSecurityToken(
                issuer: string.Empty,
                audience: string.Empty,
                claims: claims,
                expires: DateTime.UtcNow.Add(expires),
                signingCredentials: creds
            );
            // 4. 生成 Token 字符串
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected string Login(int userId, TimeSpan expires)
        {
            string login_token = $"{LOGIN_TOKEN}:{userId}";
            string token = GenerateToken(userId.ToString(), userId.ToString(), expires);
            this.Redis.StringSet(login_token, token, expires);
            return token;
        }


        protected void Logout(int userId)
        {
            string login_token = $"{LOGIN_TOKEN}:{userId}";
            this.Redis.KeyDelete(login_token);
        }

        protected bool CheckToken(int userId, string token)
        {
            string login_token = $"{LOGIN_TOKEN}:{userId}";
            RedisValue value = this.Redis.StringGet(login_token);
            if (value.IsNullOrEmpty) return false;
            return value.GetRedisValue<string>() == token;
        }
    }
}
