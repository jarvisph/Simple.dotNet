using System;

namespace Simple.Core.Logger
{
    public interface ILogger
    {
        bool Log(string message);
        bool Error(Guid guid, Exception exception);
    }
}
