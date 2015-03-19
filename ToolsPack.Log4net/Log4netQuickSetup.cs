using log4net.Appender;
using Log4Net.Async;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace ToolsPack.Log4net
{
	/// <summary>
	/// No need to create a log4net.config file. We can quickly setup log4net in one line of code. 
	/// This helper is often use in a console program or a unit test.
	/// </summary>
	public static class Log4NetQuickSetup
	{
		private const string DefaultPattern = "%date{HH:mm:ss,fff} [%-5level] %message    [%logger{1}:%line]%newline";
		private const string DefaultLogFile = @".\Log\program.log";

		/// <summary>
		/// Create a pattern which log a thread property value in all log message
		/// Example
		///     ThreadContext.Properties["SessionId"] = "FXA_36985";
		///     var patternWithSessionId = CreatePattern("SessionId");
		/// if the patternWithSessionId is use in the config log4net, so the "FXA_36985" will be displayed
		/// on every message comming from the Thread that set the property "SessionId"
		/// </summary>
		/// <param name="threadContextPropertyName"></param>
		/// <returns></returns>
		public static string CreatePattern(string threadContextPropertyName)
		{
			return "%date{HH:mm:ss,fff} [%-5level] [%mdc{" + threadContextPropertyName +
			       "}] %message    [%logger{1}:%line]%newline";
		}

		public static void SetUpConsole(string pattern = DefaultPattern, bool colored = false)
		{
			var layout = new PatternLayout(pattern);

			//create appender
			AppenderSkeleton appender;
			if (colored)
			{
				appender = new ColoredConsoleAppender();
			}
			else
			{
				appender = new ConsoleAppender();
			}

			//setup appender
			appender.Layout = layout;
			appender.Threshold = Level.Debug;

			Setup(layout, appender);
		}

		public static void SetUpFileRolling(string filePath = DefaultLogFile, string pattern = DefaultPattern, bool async = true)
		{
			var layout = new PatternLayout(pattern);
			var appender = async ? new AsyncRollingFileAppender() : new RollingFileAppender();

			//setup appender
			appender.File = filePath;
			appender.Layout = layout;
			appender.AppendToFile = true;
			appender.MaximumFileSize = "100MB";
			appender.DatePattern = "yyyyMMdd";
			appender.MaxSizeRollBackups = 50;
			appender.StaticLogFileName = true;

			Setup(layout, appender);
		}

		private static void Setup(PatternLayout layout, AppenderSkeleton appender)
		{
			layout.ActivateOptions();
			appender.ActivateOptions();
			BasicConfigurator.Configure(appender);
		}
	}
}