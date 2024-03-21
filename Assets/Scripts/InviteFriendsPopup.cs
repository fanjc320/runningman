using EnhancedUI.EnhancedScroller;
using Facebook.Unity;
using Lean;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InviteFriendsPopup : MonoBehaviour, IEnhancedScrollerDelegate
{
	public EnhancedScroller Scroller;

	public EnhancedScrollerCellView CVContentPref;

	private Button closeBtn;

	private Button sendBtn;

	public Toggle selectAllToggle;

	public bool passOnSelectAllChange;

	public static InviteFriendsPopup instance;

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
			FB.AppRequest(LeanLocalization.GetTranslationText("187"), (from s in tempList
				where s.UISelected[0]
				select s.Id).ToList(), null, null, 0, null, LeanLocalization.GetTranslationText("186"), SendHelp());
		});
	}

	private FacebookDelegate<IAppRequestResult> SendHelp()
	{
		return delegate(IAppRequestResult result)
		{
			if (!result.Cancelled && string.IsNullOrEmpty(result.Error))
			{
				foreach (FBUserProfile temp in tempList)
				{
					if (temp.UISelected[0])
					{
						FBManager.Instance.AddReqLog(temp.UserName + temp.ImageURL, 1, DateTime.Now);
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
		tempList.Clear();
		foreach (FBUserProfile s in FBManager.Instance.InviteNonGameFriendsProfile)
		{
			if (FBManager.Instance.RequestLog.FindIndex((FBLogData f) => f.Id == s.UserName + s.ImageURL && f.fbType == 1) == -1)
			{
				tempList.Add(s);
			}
		}
		selectAllToggle.isOn = true;
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
		int count = tempList.Count;
		FBManager.Instance.UISelectedCount[0] = 0;
		for (int i = 0; count > i; i++)
		{
			if (50 > i)
			{
				tempList[i].UISelected[0] = isSelect;
				if (isSelect)
				{
					FBManager.Instance.UISelectedCount[0]++;
				}
			}
			else
			{
				tempList[i].UISelected[0] = false;
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
		FBManager.Instance.UpdateReqLog();
		tempList.Clear();
		int num = 0;
		foreach (FBUserProfile s in FBManager.Instance.InviteNonGameFriendsProfile)
		{
			if (FBManager.Instance.RequestLog.FindIndex((FBLogData f) => f.Id == s.UserName + s.ImageURL && f.fbType == 1) == -1)
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
		FBUserProfile data = tempList[num];
		FBUserProfile data2 = (tempList.Count > num + 1) ? tempList[num + 1] : null;
		cVInviteFriends.transform.localPosition = Vector3.zero;
		cVInviteFriends.SetData(data, data2, FBUserProfile.UISelectType.InviteNonGameFriend, base.gameObject);
		return cVInviteFriends;
	}
}
