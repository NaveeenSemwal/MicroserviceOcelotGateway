using System;
using System.Collections.Generic;
using System.Text;

namespace TweetBook.Utilities
{
    public interface ILog
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string message);
        void LogError(string message);
    }
}
