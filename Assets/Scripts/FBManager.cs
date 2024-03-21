using Facebook.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class FBManager : MonoBehaviour
{
	private static FBManager instance;

	public bool IsReady;

	public bool IsLoadedCompleted;

	public FBUserProfile PlayerProfile = new FBUserProfile();

	public List<FBUserProfile> FriendsProfile = new List<FBUserProfile>();

	public int[] UISelectedCount = new int[Enum.GetValues(typeof(FBUserProfile.UISelectType)).Length];

	public List<FBUserProfile> InviteNonGameFriendsProfile = new List<FBUserProfile>();

	public List<FBUserProfile> RequestAllFriendsProfile = new List<FBUserProfile>();

	public List<FBUserProfile> RequestGameFriendsProfile = new List<FBUserProfile>();

	public List<FBRequest> Requests = new List<FBRequest>();

	public List<MessageBoxRequests> MessageBoxItems = new List<MessageBoxRequests>();

	public List<FBLogData> RequestLog;

	public static FBManager Instance
	{
		get
		{
			if (null == instance)
			{
				_init();
			}
			return instance;
		}
	}

	public event Action<bool> OnReady;

	private static void _init()
	{
		instance = (UnityEngine.Object.Instantiate(Resources.Load("FBManager", typeof(GameObject))) as GameObject).GetComponent<FBManager>();
		instance.gameObject.name = instance.GetType().Name;
	}

	private void Awake()
	{
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!FB.IsInitialized)
		{
			FB.Init(InitCallback, OnHideUnity);
		}
		else
		{
			FB.ActivateApp();
		}
		LoadReqLog();
		UpdateReqLog();
		WebTextureCache.InstantiateGlobal();
	}

	public void AddReqLog(string id, int v, DateTime now)
	{
		RequestLog.Add(new FBLogData
		{
			Id = id,
			fbType = v,
			ReqDate = now
		});
	}

	public void Login()
	{
		List<string> list = new List<string>();
		list.Add("public_profile");
		list.Add("email");
		list.Add("user_friends");
		List<string> permissions = list;
		FB.LogInWithReadPermissions(permissions, OnAuth);
	}

	public void Logout()
	{
		PlayerProfile = new FBUserProfile();
		FriendsProfile = new List<FBUserProfile>();
		UISelectedCount = new int[Enum.GetValues(typeof(FBUserProfile.UISelectType)).Length];
		InviteNonGameFriendsProfile = new List<FBUserProfile>();
		RequestAllFriendsProfile = new List<FBUserProfile>();
		RequestGameFriendsProfile = new List<FBUserProfile>();
		Requests = new List<FBRequest>();
		MessageBoxItems = new List<MessageBoxRequests>();
		IsReady = false;
		FB.LogOut();
	}

	public void SaveReqLog()
	{
		if (RequestLog.Count != 0)
		{
			UpdateReqLog();
			string path = Application.persistentDataPath + "/flddata";
			try
			{
				using (Stream serializationStream = File.Open(path, FileMode.Create))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(serializationStream, RequestLog);
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public void UpdateReqLog()
	{
		if (RequestLog != null)
		{
			RequestLog.RemoveAll((FBLogData f) => (f.ReqDate.AddHours(6.0) <= DateTime.Now) ? true : false);
		}
	}

	public void LoadReqLog()
	{
		string path = Application.persistentDataPath + "/flddata";
		List<FBLogData> requestLog = new List<FBLogData>();
		try
		{
			using (Stream serializationStream = File.Open(path, FileMode.Open))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				requestLog = (List<FBLogData>)binaryFormatter.Deserialize(serializationStream);
			}
		}
		catch (Exception)
		{
			requestLog = new List<FBLogData>();
		}
		RequestLog = requestLog;
	}

	private void InitCallback()
	{
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
			if (FB.IsLoggedIn)
			{
				readyFBDataAsync(delegate
				{
					IsLoadedCompleted = true;
				});
			}
			else
			{
				IsLoadedCompleted = true;
			}
		}
	}

	private void readyFBDataAsync(Action onComplete)
	{
		if (IsReady)
		{
			onComplete();
			return;
		}
		Action<bool> onComplete2 = delegate(bool isSuccess)
		{
			IsReady = isSuccess;
			if (this.OnReady != null)
			{
				this.OnReady(IsReady);
			}
			onComplete();
		};
		getFriendInfoAsync(onComplete2);
	}

	private void getFriendInfoAsync(Action<bool> onComplete)
	{
		bool isCancel = false;
		Action<string> cancelProc = delegate
		{
			isCancel = true;
			onComplete(obj: false);
		};
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("fields", "picture,friends.limit(1000){picture,name},name");
		Dictionary<string, object> fromDict;
		FBRequest request;
		FB.API("/me", HttpMethod.GET, delegate(IGraphResult result)
		{
			if (!string.IsNullOrEmpty(result.Error) || result.Cancelled)
			{
				cancelProc(result.RawResult);
			}
			else
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)((Dictionary<string, object>)result.ResultDictionary["picture"])["data"];
				PlayerProfile = new FBUserProfile
				{
					Id = (string)result.ResultDictionary["id"],
					ImageURL = ((!(bool)dictionary2["is_silhouette"]) ? ((string)dictionary2["url"]) : null),
					UserName = (string)result.ResultDictionary["name"],
					IsAppFriend = true
				};
				Dictionary<string, object> dictionary3 = (Dictionary<string, object>)result.ResultDictionary["friends"];
				List<object> list = (List<object>)dictionary3["data"];
				int count = list.Count;
				for (int i = 0; count > i; i++)
				{
					Dictionary<string, object> dictionary4 = (Dictionary<string, object>)list[i];
					Dictionary<string, object> dictionary5 = (Dictionary<string, object>)((Dictionary<string, object>)dictionary4["picture"])["data"];
					FriendsProfile.Add(new FBUserProfile
					{
						Id = (string)dictionary4["id"],
						ImageURL = ((!(bool)dictionary5["is_silhouette"]) ? ((string)dictionary5["url"]) : null),
						UserName = (string)dictionary4["name"],
						IsAppFriend = true
					});
				}
				FB.API("me/invitable_friends?limit=1000", HttpMethod.GET, delegate(IGraphResult res2)
				{
					if (!string.IsNullOrEmpty(res2.Error) || res2.Cancelled)
					{
						cancelProc(res2.RawResult);
					}
					else
					{
						List<object> list2 = (List<object>)res2.ResultDictionary["data"];
						int count2 = list2.Count;
						for (int j = 0; count2 > j; j++)
						{
							Dictionary<string, object> dictionary6 = (Dictionary<string, object>)list2[j];
							Dictionary<string, object> dictionary7 = (Dictionary<string, object>)((Dictionary<string, object>)dictionary6["picture"])["data"];
							FriendsProfile.Add(new FBUserProfile
							{
								Id = (string)dictionary6["id"],
								ImageURL = ((!(bool)dictionary7["is_silhouette"]) ? ((string)dictionary7["url"]) : null),
								UserName = (string)dictionary6["name"],
								IsAppFriend = false
							});
						}
						InviteNonGameFriendsProfile = (from s in FriendsProfile
							where !s.IsAppFriend
							select s).ToList();
						RequestAllFriendsProfile = FriendsProfile.ToList();
						RequestGameFriendsProfile = (from s in FriendsProfile
							where s.IsAppFriend
							select s).ToList();
						FB.API("me/apprequests", HttpMethod.GET, delegate(IGraphResult res3)
						{
							if (!string.IsNullOrEmpty(res3.Error) || res3.Cancelled)
							{
								cancelProc(res3.RawResult);
							}
							else
							{
								Requests.Clear();
								List<object> list3 = (List<object>)res3.ResultDictionary["data"];
								foreach (object item in list3)
								{
									Dictionary<string, object> dictionary8 = (Dictionary<string, object>)item;
									fromDict = (Dictionary<string, object>)dictionary8["from"];
									FBUserProfile fBUserProfile = RequestGameFriendsProfile.Find((FBUserProfile f) => f.Id == (string)fromDict["id"]);
									if (fBUserProfile != null && dictionary8.ContainsKey("data"))
									{
										request = new FBRequest();
										request.RequestID = (string)dictionary8["id"];
										request.RequestType = (string)dictionary8["data"];
										request.FromUser = fBUserProfile;
										bool flag = true;
										if (request.RequestType.Equals("request"))
										{
											List<FBRequest> list4 = (from s in Requests
												where s.FromUser.Id.Equals(request.FromUser.Id) && s.RequestType.Equals("request")
												select s).ToList();
											int count3 = list4.Count;
											for (int k = 0; count3 > k; k++)
											{
												FB.API("/" + list4[k].RequestID, HttpMethod.DELETE, delegate
												{
												});
												Requests.Remove(list4[k]);
											}
											string key = "PPrefKey_LastSendRequestTime_" + request.FromUser.Id;
											if (PlayerPrefs.HasKey(key))
											{
												long num = long.Parse(PlayerPrefs.GetString(key));
												if (DateTime.Today.Ticks <= num)
												{
													flag = false;
													FB.API("/" + request.RequestID, HttpMethod.DELETE, delegate
													{
													});
												}
											}
										}
										else if (request.RequestType.Equals("send"))
										{
											List<FBRequest> list5 = (from s in Requests
												where s.FromUser.Id.Equals(request.FromUser.Id) && s.RequestType.Equals("send")
												select s).ToList();
											int count4 = list5.Count;
											for (int l = 0; count4 > l; l++)
											{
												FB.API("/" + list5[l].RequestID, HttpMethod.DELETE, delegate
												{
												});
												Requests.Remove(list5[l]);
											}
										}
										if (flag)
										{
											Requests.Add(request);
										}
									}
								}
								int num2 = -1;
								int num3 = -1;
								int count5 = Requests.Count;
								for (int m = 0; m < count5; m++)
								{
									if (Requests[m].RequestType.Equals("request"))
									{
										if (num2 == -1 || MessageBoxItems[num2].Requests[49] != null)
										{
											FBRequest[] array = new FBRequest[50];
											array[0] = Requests[m];
											MessageBoxItems.Add(new MessageBoxRequests
											{
												IsRequest = true,
												Requests = array
											});
											num2 = MessageBoxItems.Count - 1;
										}
										else
										{
											for (int n = 0; 50 > n; n++)
											{
												if (MessageBoxItems[num2].Requests[n] == null)
												{
													MessageBoxItems[num2].Requests[n] = Requests[m];
													break;
												}
											}
										}
									}
									else if (Requests[m].RequestType.Equals("send"))
									{
										if (num3 == -1 || MessageBoxItems[num3].Requests[4] != null)
										{
											FBRequest[] requests = new FBRequest[5]
											{
												Requests[m],
												null,
												null,
												null,
												null
											};
											MessageBoxItems.Add(new MessageBoxRequests
											{
												IsRequest = false,
												Requests = requests
											});
											num3 = MessageBoxItems.Count - 1;
										}
										else
										{
											for (int num4 = 0; 5 > num4; num4++)
											{
												if (MessageBoxItems[num3].Requests[num4] == null)
												{
													MessageBoxItems[num3].Requests[num4] = Requests[m];
													break;
												}
											}
										}
									}
								}
								onComplete(obj: true);
							}
						});
					}
				});
			}
		}, dictionary);
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	private void OnAuth(ILoginResult result)
	{
		if (FB.IsLoggedIn)
		{
			readyFBDataAsync(delegate
			{
			});
		}
		else
		{
			this.OnReady(obj: false);
		}
	}
}
