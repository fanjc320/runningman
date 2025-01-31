using System.Collections.Generic;

namespace Soomla
{
	public class KeyValueStorage
	{
		protected const string TAG = "SOOMLA KeyValueStorage";

		private static KeyValueStorage _instance;

		private static KeyValueStorage instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new KeyValueStorageAndroid();
				}
				return _instance;
			}
		}

		public static string GetValue(string key)
		{
			return instance._getValue(key);
		}

		public static void SetValue(string key, string val)
		{
			instance._setValue(key, val);
		}

		public static void DeleteKeyValue(string key)
		{
			instance._deleteKeyValue(key);
		}

		public static List<string> GetEncryptedKeys()
		{
			return instance._getEncryptedKeys();
		}

		public static void Purge()
		{
			instance._purge();
		}

		protected virtual string _getValue(string key)
		{
			return null;
		}

		protected virtual void _setValue(string key, string val)
		{
		}

		protected virtual void _deleteKeyValue(string key)
		{
		}

		protected virtual List<string> _getEncryptedKeys()
		{
			return null;
		}

		protected virtual void _purge()
		{
		}
	}
}
