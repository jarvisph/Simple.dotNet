using StackExchange.Redis;

namespace Simple.dotNet.Redis
{
    public class RedisConnectionFactory
    {
        /// <summary>
        /// 获取连接对象
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IConnectionMultiplexer GetConnection(string connectionString)
        {
            var options = ConfigurationOptions.Parse(connectionString);
            options.SyncTimeout = int.MaxValue;
            options.AllowAdmin = true;
            var connect = ConnectionMultiplexer.Connect(options);
            //注册如下事件
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.InternalError += MuxerInternalError;
            return connect;
        }
        #region 事件

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            throw new RedisException(e.Message);
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            throw new RedisException(e.EndPoint.ToString());
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            throw new RedisException(e.EndPoint + ", " + e.FailureType + (e.Exception == null ? "" : (", " + e.Exception.Message)));
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            throw new RedisException(e.Exception.Message);
        }

        #endregion
    }
}
