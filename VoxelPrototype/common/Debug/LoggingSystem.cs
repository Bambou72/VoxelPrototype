 using NLog;
using NLog.Conditions;
using NLog.Layouts;
using NLog.Targets;
namespace VoxelPrototype.common.Debug
{
    public static class LoggingSystem
    {
        public static void Init()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new FileTarget("logfile")
            {
                FileName = "logs/log.txt",
                ArchiveFileName = "logs/logfile.{#}.txt",
                ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                ArchiveEvery = FileArchivePeriod.None,
                ArchiveOldFileOnStartup = true,
                MaxArchiveFiles = 10,
                ConcurrentWrites = true,
                KeepFileOpen = false,
            };
            logfile.Layout = new SimpleLayout("${longdate}|${level:uppercase=true}|${callsite:className=true:includeSourcePath=false:methodName=false:includeNamespace=false}|${message}${exception}");
            config.AddTarget(logfile);
            var logconsole = new ColoredConsoleTarget("logconsole");
            //Info
            var InfohighlightRule = new ConsoleRowHighlightingRule();
            InfohighlightRule.Condition = ConditionParser.ParseExpression("level == LogLevel.Info");
            InfohighlightRule.ForegroundColor = ConsoleOutputColor.White;
            logconsole.RowHighlightingRules.Add(InfohighlightRule);
            //Warn
            var WarnhighlightRule = new ConsoleRowHighlightingRule();
            WarnhighlightRule.Condition = ConditionParser.ParseExpression("level == LogLevel.Warn");
            WarnhighlightRule.ForegroundColor = ConsoleOutputColor.Yellow;
            logconsole.RowHighlightingRules.Add(WarnhighlightRule);
            //Error
            var ErrorhighlightRule = new ConsoleRowHighlightingRule();
            ErrorhighlightRule.Condition = ConditionParser.ParseExpression("level == LogLevel.Error");
            ErrorhighlightRule.ForegroundColor = ConsoleOutputColor.Red;
            logconsole.RowHighlightingRules.Add(ErrorhighlightRule);
            //Fatal
            var FatalhighlightRule = new ConsoleRowHighlightingRule();
            FatalhighlightRule.Condition = ConditionParser.ParseExpression("level == LogLevel.Fatal");
            FatalhighlightRule.ForegroundColor = ConsoleOutputColor.Magenta;
            logconsole.RowHighlightingRules.Add(FatalhighlightRule);
            //
            logconsole.Layout = new SimpleLayout("${level:uppercase=true}|${callsite:className=true:includeSourcePath=false:methodName=false:includeNamespace=false}|${message}${exception}");
            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            // Apply config           
            LogManager.Configuration = config;
        }
    }
}
