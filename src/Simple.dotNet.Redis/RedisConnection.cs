namespace Simple.Redis
{
    public class RedisConnection
    {
        public RedisConnection(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public string ConnectionString { get; set; }
    }
}
