//using Newtonsoft.Json;
//using RabbitMQ.Client;
//using Simple.Core.Extensions;
//using System.Text;

//namespace Simple.RabbitMQ
//{
//    public class RabbitProducer : RabbitConnection, IRabbitProducer
//    {

//        public void Send<TMessageQueue>(TMessageQueue message) where TMessageQueue : IMessageQueue
//        {
//            Send(message, string.Empty);
//        }

//        public void Send<TMessageQueue>(TMessageQueue message, string routingKey) where TMessageQueue : IMessageQueue
//        {
//            using (var channel = _connection.CreateModel())
//            {
//                ProducerAttribute exchange = typeof(TMessageQueue).GetAttribute<ProducerAttribute>();
//                if (exchange == null) throw new RabbitException(nameof(ProducerAttribute));
//                string msg = JsonConvert.SerializeObject(message);
//                channel.ExchangeDeclare(exchange.Name, exchange.Type, true, false, null);
//                // 消息持久化
//                var properties = channel.CreateBasicProperties();
//                properties.Persistent = true;
//                properties.DeliveryMode = 2;
//                var body = Encoding.UTF8.GetBytes(msg);
//                //绑定交换机
//                channel.BasicPublish(exchange.Name, routingKey, properties, body);
//            }
//        }
//    }
//}
