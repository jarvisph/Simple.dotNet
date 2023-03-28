using Simple.Core.Dependency;
using StackExchange.Redis;
using System;

namespace Simple.Redis
{
    public abstract class RedisDatabase
    {
        private readonly IConnectionMultiplexer _connection;
        /// <summary>
        /// 获取Database
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public IDatabase Redis { get; }

        private const string LOGIN = "LOGIN:";
        private const string TOKEN = "TOKEN:";
        /// <summary>
        /// 库 
        /// </summary>
        protected virtual int Db => -1;

        private RedisConnection ConnectionString
        {
            get
            {
                return IocCollection.Resolve<RedisConnection>();
            }
        }
        public RedisDatabase()
        {
            if (_connection == null)
                _connection = RedisConnectionFactory.GetConnection(ConnectionString.ConnectionString);
            this.Redis = _connection.GetDatabase(Db);
        }

        public void SetHash(string key, string hashKey, object value) => Redis.HashSet(key, hashKey.ToRedisValue(), value.ToRedisValue());

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
        protected  string Login(int userId)
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
        protected  int GetTokenID(string token)
        {
            string key = this.GetTokenKey(token);
            return this.Redis.HashGet(key, token).GetRedisValue<int>();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="userId"></param>
        protected  void Logout(int userId)
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
        }
    }
}
