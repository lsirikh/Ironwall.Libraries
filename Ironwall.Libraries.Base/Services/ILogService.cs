using System;

namespace Ironwall.Libraries.Base.Services
{
    public interface ILogService
    {
        void Error(string msg, Type type = default, bool debug = true);
        void Info(string msg, Type type = default, bool debug = true);
        void Warning(string msg, Type type=default, bool debug = true);

        event EventHandler<LogEventArgs> LogEvent;
    }
}