using System;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace ToolsPack.Log4net
{
	public static class Log4NetQuickSetup
	{
		private const string DefaultPattern = "%date{HH:mm:ss,fff} [%-5level] %message [%logger{1}:%line]%newline";

		/// <summary>
		/// Create a pattern which log a thread property value in all log message
		/// Example
		/// ThreadContext.Properties["SessionId"] = "FXA_36985";
		/// var patternWithSessionId = CreatePattern("SessionId");
		/// if the patternWithSessionId is use in the config log4net, so the "FXA_36985" will be displayed
		/// on every message comming from the Thread that set the property "SessionId"
		/// </summary>
		/// <param name="threadContextPropertyName"></param>
		/// <returns></returns>
		public static string CreatePattern(string threadContextPropertyName)
		{
			return "%date{HH:mm:ss,fff} [%-5level] [%property{" + threadContextPropertyName +
			"}] %message [%logger{1}]%newline";
		}

		/// <summary>
		/// No date time, no logger name (fit to display in a console)
		/// </summary>
		public static string GetSimplePattern(string threadContextPropertyName = null)
		{
			if (string.IsNullOrEmpty(threadContextPropertyName))
			{
				return "[%-5level] %message%newline";
			}
			return "[%-5level] [%property{" + threadContextPropertyName +
			"}] %message%newline";
		}

		public static void SetUpConsole(string pattern = DefaultPattern)
		{
			var layout = new PatternLayout(pattern);
			var appender = new ConsoleAppender
			{
				Layout = layout,
				Threshold = Level.Debug
			};
			layout.ActivateOptions();
			appender.ActivateOptions();
			BasicConfigurator.Configure(appender);
		}

		public static void SetUpFile(string filePath, string pattern = DefaultPattern)
		{
			var layout = new PatternLayout(pattern);
			var appender = new RollingFileAppender()
			{
				Layout = layout,
				Threshold = Level.Debug,
				File = filePath,
			};
			layout.ActivateOptions();
			appender.ActivateOptions();
			BasicConfigurator.Configure(appender);
		}
	}
}
