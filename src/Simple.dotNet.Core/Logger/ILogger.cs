using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Core.Logger
{
    public interface ILogger
    {
        public bool Log(string message);
    }
}
