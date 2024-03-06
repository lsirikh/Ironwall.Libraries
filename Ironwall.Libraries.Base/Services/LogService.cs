using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using log4net;
using System.Diagnostics;

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

        public void Info(string msg, bool debug = false)
        {
            if (debug)
                Debug.WriteLine(msg);
            _iLog.Info(msg);
        }

        public void Warning(string msg, bool debug = false)
        {
            if (debug)
                Debug.WriteLine(msg);
            _iLog.Warn(msg);
        }

        public void Error(string msg, bool debug = false)
        {
            if (debug)
                Debug.WriteLine(msg);
            _iLog.Error(msg);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILog _iLog;
        #endregion
    }
}
