using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Logger
{
    internal class DefaultLogger : ILogger
    {
        public bool Log(string message)
        {
            Console.WriteLine(message);
            return true;
        }

        public bool Errror(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
