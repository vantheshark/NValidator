using System;

namespace NValidator
{
    public interface ILogger
    {
        void Debug(object message);
        void Error(Exception ex);
        void Error(object message, Exception ex);
    }
}
