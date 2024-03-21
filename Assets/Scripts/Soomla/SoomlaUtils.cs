using System;

namespace Soomla
{
	public static class SoomlaUtils
	{
		private static bool isDebugBuild;

		private static bool isDebugBuildSet;

		public static void LogDebug(string tag, string message)
		{
			if (!isDebugBuildSet)
			{
				try
				{
					isDebugBuild = UnityEngine.Debug.isDebugBuild;
				}
				catch (Exception)
				{
					isDebugBuild = true;
				}
				isDebugBuildSet = true;
			}
			if (isDebugBuild && !CoreSettings.DebugUnityMessages)
			{
			}
		}

		public static void LogError(string tag, string message)
		{
		}

		public static void LogWarning(string tag, string message)
		{
		}

		public static string GetClassName(object target)
		{
			return target.GetType().Name;
		}
	}
}
