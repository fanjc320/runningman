namespace GooglePlayGames
{
	public static class GameInfo
	{
		private const string UnescapedApplicationId = "APP_ID";

		private const string UnescapedIosClientId = "IOS_CLIENTID";

		private const string UnescapedWebClientId = "WEB_CLIENTID";

		private const string UnescapedNearbyServiceId = "NEARBY_SERVICE_ID";

		public const string ApplicationId = "319515497078";

		public const string IosClientId = "";

		public const string WebClientId = "319515497078-m2v1trgpgc28nd16hurl1q387jddef9g.apps.googleusercontent.com";

		public const string NearbyConnectionServiceId = "";

		public static bool ApplicationIdInitialized()
		{
			return !string.IsNullOrEmpty("319515497078") && !"319515497078".Equals(ToEscapedToken("APP_ID"));
		}

		public static bool IosClientIdInitialized()
		{
			return !string.IsNullOrEmpty(string.Empty) && !string.Empty.Equals(ToEscapedToken("IOS_CLIENTID"));
		}

		public static bool WebClientIdInitialized()
		{
			return !string.IsNullOrEmpty("319515497078-m2v1trgpgc28nd16hurl1q387jddef9g.apps.googleusercontent.com") && !"319515497078-m2v1trgpgc28nd16hurl1q387jddef9g.apps.googleusercontent.com".Equals(ToEscapedToken("WEB_CLIENTID"));
		}

		public static bool NearbyConnectionsInitialized()
		{
			return !string.IsNullOrEmpty(string.Empty) && !string.Empty.Equals(ToEscapedToken("NEARBY_SERVICE_ID"));
		}

		private static string ToEscapedToken(string token)
		{
			return $"__{token}__";
		}
	}
}
