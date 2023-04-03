using System;

namespace Simple.Core.Logger
{
    public interface ILogger
    {
        bool Log(string message);
        bool Log(string message, int userId);
        bool Error(Guid guid, Exception exception);
    }
}
