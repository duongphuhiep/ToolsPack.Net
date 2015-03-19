using System;
using System.Configuration;
using log4net;

namespace ToolsPack.Log4net
{
	public static class ConfigReader
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(ConfigReader));

		public static T Read<T>(string key, T defaultValue)
		{
			try
			{
				string value = ConfigurationManager.AppSettings.Get(key);
				if (value == null)
				{
					return defaultValue;
				}
				return (T)Convert.ChangeType(value, typeof(T));
			}
			catch (Exception ex)
			{
				Log.ErrorFormat("Failed reading config '{0}', use default value = '{1}' - {2}",
										key, defaultValue, ex.Message);
				return defaultValue;
			}
		}
	}
}
