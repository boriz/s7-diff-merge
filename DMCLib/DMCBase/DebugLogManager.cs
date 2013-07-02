using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace DMCBase
{
    public static class DebugLogManager
    {
        public static void Initialize()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            DebuggerTarget targetDebugger = new DebuggerTarget();
            NLogViewerTarget targetViewer = new NLogViewerTarget()
            {
                Name = "nlogviewer",
                Address = "tcp://127.0.0.1:8888",
                IncludeCallSite = true,
                IncludeSourceInfo = true,
            };

            config.AddTarget("debugger", targetDebugger);
            config.AddTarget("nlogviewer", targetViewer);

            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, targetDebugger));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, targetViewer));

            LogManager.Configuration = config;
        }
    }
}
