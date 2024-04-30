using Simple.Core.Dependency;
using StackExchange.Redis;
using System;

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

        private const string LOGIN = "LOGIN:";
        private const string TOKEN = "TOKEN:";
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
                    _connectionMultiplexer = ConnectionMultiplexer.Connect(opt);
                }
            }
        }

        public void SetHash(string key, string hashKey, object value) => Redis.HashSet(key, hashKey.GetRedisValue(), value.GetRedisValue());

        public T GetHash<T>(string key, string hashKey) => this.Redis.HashGet(key, hashKey).GetRedisValue<T>();

        /// <summary>
        /// 获取登录键
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetLoginKey(int userId)
        {
            return $"{LOGIN}{userId % 10}";
        }

        protected string GetToken(int userId)
        {
            string key = GetLoginKey(userId);
            return this.Redis.HashGet(key, userId).GetRedisValue<string>();
        }

        /// <summary>
        /// 获取token键
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GetTokenKey(string token)
        {
            return TOKEN + token.Substring(0, 2).ToUpper();
        }
        /// <summary>
        /// 登录并生成Token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected string Login(int userId)
        {
            //获取会员登录键
            string loginKey = this.GetLoginKey(userId);
            //生成token
            string token = Guid.NewGuid().ToString("N");
            //通过token生成token键
            string tokenKey = this.GetTokenKey(token);
            //获取旧token
            RedisValue value = this.Redis.HashGet(loginKey, userId);
            IBatch batch = this.Redis.CreateBatch();
            //如果之前有登陆，销毁旧token内容
            if (!value.IsNull)
            {
                string oldKey = this.GetTokenKey(value);
                batch.HashDeleteAsync(oldKey, value.GetRedisValue<string>());
            }
            batch.HashSetAsync(loginKey, userId, token);
            batch.HashSetAsync(tokenKey, token, userId);
            batch.Execute();
            return token;
        }
        /// <summary>
        /// 获取登录用户ID
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected int GetTokenID(string token)
        {
            string key = this.GetTokenKey(token);
            RedisValue value = this.Redis.HashGet(key, token);
            if (value.IsNullOrEmpty)
            {
                return 0;
            }
            return value.GetRedisValue<int>();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="userId"></param>
        protected void Logout(int userId)
        {
            //获取会员登录键
            string loginKey = this.GetLoginKey(userId);
            //获取旧token
            RedisValue value = this.Redis.HashGet(loginKey, userId);
            IBatch batch = this.Redis.CreateBatch();
            batch.HashDeleteAsync(loginKey, userId);
            //如果之前有登陆，销毁旧token内容
            if (!value.IsNull)
            {
                string oldKey = this.GetTokenKey(value);
                batch.HashDeleteAsync(oldKey, value.GetRedisValue<string>());
            }
            batch.Execute();
        }
    }
}
