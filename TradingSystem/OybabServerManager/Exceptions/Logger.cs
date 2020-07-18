using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.ServerManager.Exceptions
{
    /// <summary>
    /// 日志
    /// </summary>
    public sealed class Logger
    {
        private PatternLayout _layout = new PatternLayout();
        private static string LOG_PATTERN = "%date [%thread] %-5level - %message%newline";//%date [%thread] %-5level %logger [%property{NDC}] - %message%newline

        public string DefaultPattern
        {
            get { return LOG_PATTERN; }
        }

        public Logger()
        {
            _layout.ConversionPattern = DefaultPattern;
            _layout.ActivateOptions();
        }

        public PatternLayout DefaultLayout
        {
            get { return _layout; }
        }

        public void AddAppender(IAppender appender)
        {
            Hierarchy hierarchy =
                (Hierarchy)LogManager.GetRepository();

            hierarchy.Root.AddAppender(appender);
        }


        static Logger()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            TraceAppender tracer = new TraceAppender();
            PatternLayout patternLayout = new PatternLayout();

            patternLayout.ConversionPattern = LOG_PATTERN;
            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);

            RollingFileAppender roller = new RollingFileAppender();
            roller.Layout = patternLayout;
            roller.AppendToFile = true;
            //roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            //roller.MaxSizeRollBackups = 4;
            roller.Encoding = Encoding.UTF8;
            //roller.MaximumFileSize = "1000KB";
            //roller.StaticLogFileName = true;
            roller.File = "Log\\LogFile.log";
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;


            //roller.MaximumFileSize = "100MB";
        }

        public static void Create()
        {
            LogManager.GetLogger("TradingSystem");
        }

    }
}
