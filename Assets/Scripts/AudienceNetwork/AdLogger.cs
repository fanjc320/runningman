namespace AudienceNetwork
{
	internal class AdLogger
	{
		private enum AdLogLevel
		{
			None,
			Notification,
			Error,
			Warning,
			Log,
			Debug,
			Verbose
		}

		private static AdLogLevel logLevel = AdLogLevel.Log;

		private static readonly string logPrefix = "Audience Network Unity ";

		internal static void Log(string message)
		{
			AdLogLevel adLogLevel = AdLogLevel.Log;
			if (logLevel < adLogLevel)
			{
			}
		}

		internal static void LogWarning(string message)
		{
			AdLogLevel adLogLevel = AdLogLevel.Warning;
			if (logLevel < adLogLevel)
			{
			}
		}

		internal static void LogError(string message)
		{
			AdLogLevel adLogLevel = AdLogLevel.Error;
			if (logLevel < adLogLevel)
			{
			}
		}

		private static string levelAsString(AdLogLevel logLevel)
		{
			switch (logLevel)
			{
			case AdLogLevel.Notification:
				return string.Empty;
			case AdLogLevel.Error:
				return "<error>: ";
			case AdLogLevel.Warning:
				return "<warn>: ";
			case AdLogLevel.Log:
				return "<log>: ";
			case AdLogLevel.Debug:
				return "<debug>: ";
			case AdLogLevel.Verbose:
				return "<verbose>: ";
			default:
				return string.Empty;
			}
		}
	}
}
