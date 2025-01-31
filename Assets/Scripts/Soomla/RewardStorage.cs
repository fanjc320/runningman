using System;

namespace Soomla
{
	public class RewardStorage
	{
		protected const string TAG = "SOOMLA RewardStorage";

		private static RewardStorage _instance;

		private static RewardStorage instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RewardStorageAndroid();
				}
				return _instance;
			}
		}

		public static void SetRewardStatus(Reward reward, bool give)
		{
			SetRewardStatus(reward, give, notify: true);
		}

		public static void SetRewardStatus(Reward reward, bool give, bool notify)
		{
			instance._setTimesGiven(reward, give, notify);
		}

		public static bool IsRewardGiven(Reward reward)
		{
			return GetTimesGiven(reward) > 0;
		}

		public static int GetTimesGiven(Reward reward)
		{
			return instance._getTimesGiven(reward);
		}

		public static DateTime GetLastGivenTime(Reward reward)
		{
			return instance._getLastGivenTime(reward);
		}

		public static int GetLastSeqIdxGiven(SequenceReward reward)
		{
			return instance._getLastSeqIdxGiven(reward);
		}

		public static void SetLastSeqIdxGiven(SequenceReward reward, int idx)
		{
			instance._setLastSeqIdxGiven(reward, idx);
		}

		protected virtual int _getLastSeqIdxGiven(SequenceReward seqReward)
		{
			return 0;
		}

		protected virtual void _setLastSeqIdxGiven(SequenceReward seqReward, int idx)
		{
		}

		protected virtual void _setTimesGiven(Reward reward, bool up, bool notify)
		{
		}

		protected virtual int _getTimesGiven(Reward reward)
		{
			return 0;
		}

		protected virtual DateTime _getLastGivenTime(Reward reward)
		{
			return default(DateTime);
		}
	}
}
