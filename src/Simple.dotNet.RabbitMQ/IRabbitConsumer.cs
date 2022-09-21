using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.Core.Dependency;

namespace Simple.RabbitMQ
{
    public interface IRabbitConsumer 
    {
        void Start();
    }
}
