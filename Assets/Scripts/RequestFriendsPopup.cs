using EnhancedUI.EnhancedScroller;
using Facebook.Unity;
using Lean;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RequestFriendsPopup : MonoBehaviour, IEnhancedScrollerDelegate
{
	public EnhancedScroller Scroller;

	public EnhancedScrollerCellView CVContentPref;

	private Toggle[] tabToggle;

	private Button closeBtn;

	private Button sendBtn;

	private bool[] toggleState = new bool[2];

	public Toggle selectAllToggle;

	public bool passOnSelectAllChange;

	public static RequestFriendsPopup instance;

	private List<FBUserProfile> tempList = new List<FBUserProfile>();

	private void OnDestroy()
	{
		instance = null;
	}

	private void Awake()
	{
		instance = this;
		Scroller.Delegate = this;
		closeBtn = base.transform.Find("CloseBtn").GetComponent<Button>();
		closeBtn.onClick.AddListener(delegate
		{
			MenuUIManager.Instance.PlayClickAud();
			Close();
		});
		sendBtn = base.transform.Find("SendBtn").GetComponent<Button>();
		sendBtn.onClick.AddListener(delegate
		{
			MenuUIManager.Instance.PlayClickAud();
			FB.AppRequest(LeanLocalization.GetTranslationText("185"), (!tabToggle[0].isOn) ? (from s in tempList
				where s.UISelected[2]
				select s.Id).ToList() : (from s in tempList
				where s.UISelected[1]
				select s.Id).ToList(), null, null, 0, "request", LeanLocalization.GetTranslationText("184"), SendCallback());
		});
		tabToggle = base.transform.GetComponentsInChildren<Toggle>(includeInactive: true);
		Enumerable.Range(0, 2).All(delegate(int index)
		{
			RequestFriendsPopup requestFriendsPopup = this;
			tabToggle[index].onValueChanged.AddListener(delegate(bool isOn)
			{
				EventSystem current = EventSystem.current;
				bool flag = requestFriendsPopup.tabToggle[index].gameObject == current.currentSelectedGameObject;
				if (isOn)
				{
					requestFriendsPopup.selectAllToggle.isOn = requestFriendsPopup.toggleState[index];
					if (flag)
					{
						MenuUIManager.Instance.PlayClickAud();
					}
					requestFriendsPopup.Scroller.ReloadData();
					requestFriendsPopup.Scroller.ScrollPosition = 1f;
					requestFriendsPopup.Scroller.ScrollPosition = 0f;
					requestFriendsPopup.tabToggle[index].transform.Find("Selected").gameObject.SetActive(value: true);
				}
				else
				{
					requestFriendsPopup.tabToggle[index].transform.Find("Selected").gameObject.SetActive(value: false);
				}
			});
			return true;
		});
	}

	private FacebookDelegate<IAppRequestResult> SendCallback()
	{
		return delegate(IAppRequestResult result)
		{
			if (!result.Cancelled && string.IsNullOrEmpty(result.Error))
			{
				foreach (FBUserProfile temp in tempList)
				{
					int num = tabToggle[0].isOn ? 1 : 2;
					if (temp.UISelected[num])
					{
						FBManager.Instance.AddReqLog(temp.Id, 0, DateTime.Now);
					}
				}
				FBManager.Instance.SaveReqLog();
			}
			Close();
		};
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
	}

	public void Open()
	{
		MenuUIManager.Instance.backBtnStackDepth = 1;
		MenuUIManager.Instance.SetActivateFilter(activate: true);
		base.gameObject.SetActive(value: true);
		toggleState[0] = true;
		toggleState[1] = true;
		FBManager.Instance.UpdateReqLog();
		tabToggle[1].isOn = true;
		tempList.Clear();
		foreach (FBUserProfile s2 in FBManager.Instance.RequestGameFriendsProfile)
		{
			if (FBManager.Instance.RequestLog.FindIndex((FBLogData f) => f.Id == s2.Id && f.fbType == 0) == -1)
			{
				tempList.Add(s2);
			}
		}
		SelectAll(isSelect: true);
		tabToggle[0].isOn = true;
		tempList.Clear();
		foreach (FBUserProfile s in FBManager.Instance.RequestAllFriendsProfile)
		{
			if (FBManager.Instance.RequestLog.FindIndex((FBLogData f) => f.Id == s.Id || f.Id == s.UserName + s.ImageURL) == -1)
			{
				tempList.Add(s);
			}
		}
		SelectAll(isSelect: true);
		Scroller.ReloadData();
		Scroller.ScrollPosition = 1f;
		Scroller.ScrollPosition = 0f;
	}

	public void Close()
	{
		MenuUIManager.Instance.SetActivateFilter(activate: false);
		base.gameObject.SetActive(value: false);
	}

	public void SelectAll(bool isSelect)
	{
		if (tabToggle[0].isOn)
		{
			int count = tempList.Count;
			FBManager.Instance.UISelectedCount[1] = 0;
			for (int i = 0; count > i; i++)
			{
				if (50 > i)
				{
					tempList[i].UISelected[1] = isSelect;
					if (isSelect)
					{
						FBManager.Instance.UISelectedCount[1]++;
					}
				}
				else
				{
					tempList[i].UISelected[1] = false;
				}
			}
		}
		else
		{
			int count2 = tempList.Count;
			FBManager.Instance.UISelectedCount[2] = 0;
			for (int j = 0; count2 > j; j++)
			{
				if (50 > j)
				{
					tempList[j].UISelected[2] = isSelect;
					if (isSelect)
					{
						FBManager.Instance.UISelectedCount[2]++;
					}
				}
				else
				{
					tempList[j].UISelected[2] = false;
				}
			}
		}
		Scroller.RefreshActiveCellViews();
	}

	public void OnSelectAllChange(bool isOn)
	{
		EventSystem current = EventSystem.current;
		if (base.transform.Find("Check").gameObject == current.currentSelectedGameObject)
		{
			MenuUIManager.Instance.PlayClickAud();
		}
		if (tabToggle[0].isOn)
		{
			toggleState[0] = isOn;
		}
		else
		{
			toggleState[1] = isOn;
		}
		if (passOnSelectAllChange)
		{
			passOnSelectAllChange = false;
		}
		else if (isOn)
		{
			SelectAll(isSelect: true);
		}
		else
		{
			SelectAll(isSelect: false);
		}
	}

	public int GetNumberOfCells(EnhancedScroller scroller)
	{
		List<FBUserProfile> list = (!tabToggle[0].isOn) ? FBManager.Instance.RequestGameFriendsProfile : FBManager.Instance.RequestAllFriendsProfile;
		int num = 0;
		FBManager.Instance.UpdateReqLog();
		tempList.Clear();
		foreach (FBUserProfile s in list)
		{
			if (tabToggle[0].isOn)
			{
				if (FBManager.Instance.RequestLog.FindIndex((FBLogData f) => f.Id == s.Id || f.Id == s.UserName + s.ImageURL) == -1)
				{
					tempList.Add(s);
					num++;
				}
			}
			else if (FBManager.Instance.RequestLog.FindIndex((FBLogData f) => f.Id == s.Id && f.fbType == 0) == -1)
			{
				tempList.Add(s);
				num++;
			}
		}
		return Mathf.CeilToInt((float)num / 2f);
	}

	public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
	{
		return 117.5f;
	}

	public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
	{
		CVInviteFriends cVInviteFriends = scroller.GetCellView(CVContentPref) as CVInviteFriends;
		cVInviteFriends.name = dataIndex.ToString();
		int num = dataIndex * 2;
		bool isOn = tabToggle[0].isOn;
		List<FBUserProfile> list = tempList;
		FBUserProfile data = list[num];
		FBUserProfile data2 = (list.Count > num + 1) ? list[num + 1] : null;
		cVInviteFriends.transform.localPosition = Vector3.zero;
		cVInviteFriends.SetData(data, data2, isOn ? FBUserProfile.UISelectType.RequestAllFriend : FBUserProfile.UISelectType.RequestGameFriend, base.gameObject);
		return cVInviteFriends;
	}
}
