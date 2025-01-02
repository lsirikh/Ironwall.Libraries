using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using log4net;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System;

namespace Ironwall.Libraries.Base.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/26/2023 2:47:25 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class LogService : ILogService
    {

        #region - Ctors -
        public LogService()
        {
            Log4NetSettings();
            _iLog = LogManager.GetLogger(typeof(LogService));
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void Log4NetSettings()
        {
            //Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            //PatternLayout patternLayout = new PatternLayout();
            //patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            //patternLayout.ActivateOptions();

            //RollingFileAppender roller = new RollingFileAppender();
            //roller.AppendToFile = true;
            //roller.File = @"Logs\log.txt";
            //roller.Layout = patternLayout;
            //roller.MaxSizeRollBackups = 5;
            //roller.MaximumFileSize = "1GB";
            //roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            //roller.StaticLogFileName = true;
            //roller.ActivateOptions();
            //hierarchy.Root.AddAppender(roller);

            //hierarchy.Root.Level = Level.All;
            //hierarchy.Configured = true;

            Hierarchy hierarchy = (Hierarchy)log4net.LogManager.GetRepository();
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            // 파일명 형식을 날짜 형식으로 설정합니다. 예: log-2022-11-03.txt
            roller.File = @"Logs\log-";
            roller.DatePattern = "yyyy-MM-dd'.txt'";
            roller.StaticLogFileName = false;

            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5; // 최대 백업 파일 수
            roller.MaximumFileSize = "50MB"; // 최대 파일 크기
            roller.RollingStyle = RollingFileAppender.RollingMode.Composite; // 날짜와 크기 기반 롤링
            roller.ActivateOptions();

            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;

#if DEBUG
            log4net.Util.LogLog.InternalDebugging = true; // 테스트 환경에서는 true로 설정
#else
             log4net.Util.LogLog.InternalDebugging = false; // 실제 운영 환경에서는 false로 설정
#endif
        }

        public void Info(string msg, Type type = default, bool debug = false)
        {
            if(type == null)
                type = new StackFrame(1).GetMethod().DeclaringType;
            Log(msg, type, Level.Info, debug);
        }

        public void Warning(string msg, Type type = default, bool debug = false)
        {
            if (type == null)
                type = new StackFrame(1).GetMethod().DeclaringType;
            Log(msg, type, Level.Warn, debug);
        }

        public void Error(string msg, Type type = default, bool debug = false)
        {
            if (type == null)
                type = new StackFrame(1).GetMethod().DeclaringType;
            Log(msg, type, Level.Error, debug);
        }

        private void Log(string msg, Type type = default, Level level = default, bool debug = false)
        {
            if (debug)
                Debug.WriteLine(msg);

            // 호출자의 Logger를 동적으로 생성
            var dynamicLogger = LogManager.GetLogger(type);
            dynamicLogger.Logger.Log(type, level, msg, null);

            OnLogEvent(new LogEventArgs(msg, level));
        }

        private void OnLogEvent(LogEventArgs e)
        {
            if (LogEvent != null)
            {
                foreach (var handler in LogEvent.GetInvocationList())
                {
                    var eventHandler = (EventHandler<LogEventArgs>)handler;
                    Task.Run(() => eventHandler.Invoke(this, e));
                }
            }
        }

        private void eventData(IAsyncResult ar)
        {
            try
            {
                var del = (EventHandler<LogEventArgs>)((AsyncResult)ar).AsyncDelegate;
                del.EndInvoke(ar); // 비동기 호출 완료 대기 및 결과 처리
            }
            catch (TaskCanceledException)
            {
                // 작업이 정상적으로 취소됨
                // 로그 또는 추가 처리가 필요하지 않을 수 있음
            }
            catch (Exception ex)
            {
                // 다른 예외 처리
                _iLog.Error($"Raised {nameof(Exception)} in {nameof(eventData)} of {nameof(LogService)} : {ex.Message}");
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        #endregion
        #region - Attributes -
        public event EventHandler<LogEventArgs> LogEvent;
        private ILog _iLog;
        #endregion
    }

    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(string message, Level level)
        {
            Message = message;
            LogLevel = level;
        }

        public string Message { get; }
        public Level LogLevel { get; }
    }
}
