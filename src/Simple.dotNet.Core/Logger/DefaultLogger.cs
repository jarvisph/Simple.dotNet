using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Core.Logger
{
    internal class DefaultLogger
    {
        public bool Log(string message)
        {
            Console.WriteLine(message);
            return true;
        }

        public bool Error(Guid guid, Exception exception)
        {
            Console.WriteLine(exception);
            return true;
        }
    }
}
