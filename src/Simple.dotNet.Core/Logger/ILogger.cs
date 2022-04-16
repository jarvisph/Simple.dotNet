using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Logger
{
    public interface ILogger
    {
        public bool Log(string message);
    }
}
