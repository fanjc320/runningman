using System;
using TapjoyUnity;
using UnityEngine;

namespace Wenee
{
	public class AdManager : MonoBehaviour
	{
		private static AdManager _instance;

		private bool isTapjoyReadyContents;

		private bool isTapjoyRequesting;

		private TJPlacement createdPlacement;

		public static AdManager Instance
		{
			get
			{
				if (!_instance)
				{
					GameObject gameObject = new GameObject(typeof(AdManager).Name);
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					_instance = gameObject.AddComponent<AdManager>();
				}
				return _instance;
			}
		}

		public void Init()
		{
		}

		private void requestTapjoyContents(Action<bool> resultHandler = null)
		{
			TJPlacement.OnRequestSuccessHandler requestSuccessHandler = null;
			TJPlacement.OnRequestFailureHandler requestFailureHandler = null;
			requestSuccessHandler = delegate
			{
				TJPlacement.OnRequestSuccess -= requestSuccessHandler;
				TJPlacement.OnRequestFailure -= requestFailureHandler;
				if (resultHandler != null)
				{
					resultHandler(obj: true);
				}
				isTapjoyRequesting = false;
			};
			TJPlacement.OnRequestSuccess += requestSuccessHandler;
			requestFailureHandler = delegate
			{
				TJPlacement.OnRequestSuccess -= requestSuccessHandler;
				TJPlacement.OnRequestFailure -= requestFailureHandler;
				if (resultHandler != null)
				{
					resultHandler(obj: false);
				}
				isTapjoyRequesting = false;
			};
			TJPlacement.OnRequestFailure += requestFailureHandler;
			if (createdPlacement == null)
			{
				createdPlacement = TJPlacement.CreatePlacement("FreeGemsReward");
			}
			createdPlacement.RequestContent();
			isTapjoyRequesting = true;
		}

		private void initTapjoy(Action<bool> resultHandler = null)
		{
			if (Tapjoy.IsConnected)
			{
				requestTapjoyContents(delegate(bool isRequestSuccess)
				{
					if (isRequestSuccess)
					{
						isTapjoyReadyContents = true;
						if (resultHandler != null)
						{
							resultHandler(obj: true);
						}
					}
					else
					{
						isTapjoyReadyContents = false;
						if (resultHandler != null)
						{
							resultHandler(obj: false);
						}
					}
				});
				return;
			}
			Tapjoy.OnConnectFailureHandler connectFailureHandler = null;
			Tapjoy.OnConnectSuccessHandler connectSuccessHandler = null;
			connectSuccessHandler = delegate
			{
				Tapjoy.OnConnectSuccess -= connectSuccessHandler;
				Tapjoy.OnConnectFailure -= connectFailureHandler;
				requestTapjoyContents(delegate(bool isRequestSuccess)
				{
					if (isRequestSuccess)
					{
						isTapjoyReadyContents = true;
						if (resultHandler != null)
						{
							resultHandler(obj: true);
						}
					}
					else
					{
						isTapjoyReadyContents = false;
						if (resultHandler != null)
						{
							resultHandler(obj: false);
						}
					}
				});
			};
			Tapjoy.OnConnectSuccess += connectSuccessHandler;
			connectFailureHandler = delegate
			{
				Tapjoy.OnConnectSuccess -= connectSuccessHandler;
				Tapjoy.OnConnectFailure -= connectFailureHandler;
				isTapjoyReadyContents = false;
				if (resultHandler != null)
				{
					resultHandler(obj: false);
				}
			};
			Tapjoy.OnConnectFailure += connectFailureHandler;
			Tapjoy.SetGcmSender("942041070039");
			Tapjoy.Connect("ZMaFt9W5Q0u-v9soW43XpQECu7L85nNvbSdRyPipVkEKWW78nREWeVZakk_E");
		}

		public void ShowTapjoyOfferwall(Action<bool> resultHandler)
		{
			Func<bool> showContentClosure = delegate
			{
				TJPlacement.OnContentDismissHandler contentDismissHandler = null;
				contentDismissHandler = delegate
				{
					TJPlacement.OnContentDismiss -= contentDismissHandler;
					isTapjoyReadyContents = false;
					requestTapjoyContents();
					resultHandler(obj: true);
				};
				TJPlacement.OnContentDismiss += contentDismissHandler;
				createdPlacement.ShowContent();
				return true;
			};
			if (isTapjoyRequesting)
			{
				resultHandler(obj: false);
			}
			else if (!isTapjoyReadyContents)
			{
				initTapjoy(delegate(bool isSuccess)
				{
					if (isSuccess)
					{
						showContentClosure();
					}
					else
					{
						resultHandler(obj: false);
					}
				});
			}
			else
			{
				showContentClosure();
			}
		}

		public void GetTapjoyCurrencyBalance(Action<bool, int> resultHandler)
		{
			Func<bool> getCurrencyBalanceClosure = delegate
			{
				Tapjoy.OnGetCurrencyBalanceResponseHandler getCurrencyBalanceResponseHandler = null;
				Tapjoy.OnGetCurrencyBalanceResponseFailureHandler getCurrencyBalanceResponseFailureHandler = null;
				getCurrencyBalanceResponseHandler = delegate(string currencyName, int balance)
				{
					Tapjoy.OnGetCurrencyBalanceResponse -= getCurrencyBalanceResponseHandler;
					Tapjoy.OnGetCurrencyBalanceResponseFailure -= getCurrencyBalanceResponseFailureHandler;
					if (currencyName.Equals("Gems"))
					{
						resultHandler(arg1: true, balance);
					}
					else
					{
						resultHandler(arg1: false, -1);
					}
				};
				Tapjoy.OnGetCurrencyBalanceResponse += getCurrencyBalanceResponseHandler;
				getCurrencyBalanceResponseFailureHandler = delegate
				{
					Tapjoy.OnGetCurrencyBalanceResponse -= getCurrencyBalanceResponseHandler;
					Tapjoy.OnGetCurrencyBalanceResponseFailure -= getCurrencyBalanceResponseFailureHandler;
					resultHandler(arg1: false, -1);
				};
				Tapjoy.OnGetCurrencyBalanceResponseFailure += getCurrencyBalanceResponseFailureHandler;
				Tapjoy.GetCurrencyBalance();
				return true;
			};
			if (!Tapjoy.IsConnected)
			{
				initTapjoy(delegate(bool isSuccess)
				{
					if (isSuccess)
					{
						getCurrencyBalanceClosure();
					}
					else
					{
						resultHandler(arg1: false, -1);
					}
				});
			}
			else
			{
				getCurrencyBalanceClosure();
			}
		}

		public void SpendTapjoyCurrency(int quantity, Action<bool> resultHandler)
		{
			Func<bool> spendCurrencyClosure = delegate
			{
				Tapjoy.OnSpendCurrencyResponseHandler spendCurrencyResponseHandler = null;
				Tapjoy.OnSpendCurrencyResponseFailureHandler spendCurrencyResponseFailureHandler = null;
				spendCurrencyResponseHandler = delegate(string currencyName, int balance)
				{
					Tapjoy.OnSpendCurrencyResponse -= spendCurrencyResponseHandler;
					Tapjoy.OnSpendCurrencyResponseFailure -= spendCurrencyResponseFailureHandler;
					if (currencyName.Equals("Gems"))
					{
						resultHandler(obj: true);
					}
					else
					{
						resultHandler(obj: false);
					}
				};
				Tapjoy.OnSpendCurrencyResponse += spendCurrencyResponseHandler;
				spendCurrencyResponseFailureHandler = delegate
				{
					Tapjoy.OnSpendCurrencyResponse -= spendCurrencyResponseHandler;
					Tapjoy.OnSpendCurrencyResponseFailure -= spendCurrencyResponseFailureHandler;
					resultHandler(obj: false);
				};
				Tapjoy.OnSpendCurrencyResponseFailure += spendCurrencyResponseFailureHandler;
				Tapjoy.SpendCurrency(quantity);
				return true;
			};
			if (!Tapjoy.IsConnected)
			{
				initTapjoy(delegate(bool isSuccess)
				{
					if (isSuccess)
					{
						spendCurrencyClosure();
					}
					else
					{
						resultHandler(obj: false);
					}
				});
			}
			else
			{
				spendCurrencyClosure();
			}
		}

		private void Start()
		{
			initTapjoy();
		}
	}
}
