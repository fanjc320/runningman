using Soomla.Singletons;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Soomla
{
	public class CoreEvents : CodeGeneratedSingleton
	{
		public delegate void Action();

		private const string TAG = "SOOMLA CoreEvents";

		public static CoreEvents Instance = null;

		public static Action<Reward> OnRewardGiven = delegate
		{
		};

		public static Action<Reward> OnRewardTaken = delegate
		{
		};

		public static Action<string, Dictionary<string, string>> OnCustomEvent = delegate
		{
		};

		protected override bool DontDestroySingleton => true;

		public static void Initialize()
		{
			if (Instance == null)
			{
				Instance = UnitySingleton.GetSynchronousCodeGeneratedInstance<CoreEvents>();
				SoomlaUtils.LogDebug("SOOMLA CoreEvents", "Initializing CoreEvents and Soomla Core ...");
				AndroidJNI.PushLocalFrame(100);
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.SoomlaConfig"))
				{
					androidJavaClass.SetStatic("logDebug", CoreSettings.DebugMessages);
				}
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.soomla.core.unity.SoomlaEventHandler"))
				{
					androidJavaClass2.CallStatic("initialize");
				}
				using (AndroidJavaClass androidJavaClass4 = new AndroidJavaClass("com.soomla.Soomla"))
				{
					AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject @static = androidJavaClass3.GetStatic<AndroidJavaObject>("currentActivity");
					androidJavaClass4.CallStatic("initialize", @static, CoreSettings.SoomlaSecret);
				}
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
		}

		public void onRewardGiven(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA CoreEvents", "SOOMLA/UNITY onRewardGiven:" + message);
			JSONObject jSONObject = new JSONObject(message);
			string str = jSONObject["rewardId"].str;
			OnRewardGiven(Reward.GetReward(str));
		}

		public void onRewardTaken(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA CoreEvents", "SOOMLA/UNITY onRewardTaken:" + message);
			JSONObject jSONObject = new JSONObject(message);
			string str = jSONObject["rewardId"].str;
			OnRewardTaken(Reward.GetReward(str));
		}

		public void onCustomEvent(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA CoreEvents", "SOOMLA/UNITY onCustomEvent:" + message);
			JSONObject jSONObject = new JSONObject(message);
			string str = jSONObject["name"].str;
			Dictionary<string, string> arg = jSONObject["extra"].ToDictionary();
			OnCustomEvent(str, arg);
		}
	}
}
