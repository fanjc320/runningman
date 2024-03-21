using System;
using UnityEngine;

namespace Soomla.Store
{
	public class StoreEventPusherAndroid : StoreEvents.StoreEventPusher
	{
		protected override void _pushEventSoomlaStoreInitialized(string message)
		{
			pushEvent("SoomlaStoreInitialized", message);
		}

		protected override void _pushEventUnexpectedStoreError(string message)
		{
			pushEvent("UnexpectedStoreError", message);
		}

		protected override void _pushEventCurrencyBalanceChanged(string message)
		{
			pushEvent("SoomlaStoreInitialized", message);
		}

		protected override void _pushEventGoodBalanceChanged(string message)
		{
			pushEvent("CurrencyBalanceChanged", message);
		}

		protected override void _pushEventGoodEquipped(string message)
		{
			pushEvent("GoodEquipped", message);
		}

		protected override void _pushEventGoodUnequipped(string message)
		{
			pushEvent("GoodUnequipped", message);
		}

		protected override void _pushEventGoodUpgrade(string message)
		{
			pushEvent("GoodUpgrade", message);
		}

		protected override void _pushEventItemPurchased(string message)
		{
			pushEvent("ItemPurchased", message);
		}

		protected override void _pushEventItemPurchaseStarted(string message)
		{
			pushEvent("ItemPurchaseStarted", message);
		}

		private void pushEvent(string name, string message)
		{
			AndroidJNI.PushLocalFrame(100);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.unity.StoreEventHandler"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]))
				{
					androidJavaObject.Call("pushEvent" + name, message);
				}
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
	}
}
