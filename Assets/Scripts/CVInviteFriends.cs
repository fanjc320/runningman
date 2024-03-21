using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CVInviteFriends : EnhancedScrollerCellView
{
	public Image[] PhotoImage;

	public Text[] NameText;

	public Toggle[] CheckToggle;

	private FBUserProfile storedData1;

	private FBUserProfile storedData2;

	private FBUserProfile.UISelectType storedSelectType;

	private GameObject storedPopupGO;

	public override void RefreshCellView()
	{
		SetData(storedData1, storedData2, storedSelectType, storedPopupGO);
	}

	public void SetData(FBUserProfile data, FBUserProfile data2, FBUserProfile.UISelectType selectType, GameObject popupGO)
	{
		WebTextureCache webTextureCache = WebTextureCache.InstantiateGlobal();
		storedData1 = data;
		storedData2 = data2;
		storedSelectType = selectType;
		storedPopupGO = popupGO;
		if (data != null)
		{
			PhotoImage[0].transform.parent.gameObject.SetActive(value: true);
			PhotoImage[0].sprite = WebImageCache.Instance.LoadingSprite;
			if (!string.IsNullOrEmpty(data.ImageURL))
			{
				webTextureCache.StartCoroutine(webTextureCache.GetTexture((string)data.ImageURL.Clone(), delegate(Texture2D tex, string url)
				{
					if (storedPopupGO.activeInHierarchy && storedData1.ImageURL == url)
					{
						PhotoImage[0].sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
					}
				}));
			}
			NameText[0].text = data.UserName;
			CheckToggle[0].onValueChanged.RemoveAllListeners();
			CheckToggle[0].onValueChanged.AddListener(delegate(bool isOn)
			{
				EventSystem current2 = EventSystem.current;
				bool flag3 = CheckToggle[0].gameObject == current2.currentSelectedGameObject;
				if (flag3)
				{
					MenuUIManager.Instance.PlayClickAud();
				}
				if (isOn)
				{
					if (flag3)
					{
						bool flag4 = 50 <= FBManager.Instance.UISelectedCount[(int)selectType];
						FBManager.Instance.UISelectedCount[(int)selectType]++;
						data.UISelected[(int)selectType] = true;
						if (flag4)
						{
							CheckToggle[0].isOn = false;
						}
					}
				}
				else if (flag3)
				{
					FBManager.Instance.UISelectedCount[(int)selectType] = Mathf.Max(0, FBManager.Instance.UISelectedCount[(int)selectType] - 1);
					data.UISelected[(int)selectType] = false;
					if (selectType == FBUserProfile.UISelectType.InviteNonGameFriend)
					{
						InviteFriendsPopup.instance.passOnSelectAllChange = true;
						InviteFriendsPopup.instance.selectAllToggle.isOn = false;
					}
					else
					{
						RequestFriendsPopup.instance.passOnSelectAllChange = true;
						RequestFriendsPopup.instance.selectAllToggle.isOn = false;
					}
				}
			});
			CheckToggle[0].isOn = data.UISelected[(int)selectType];
		}
		else
		{
			PhotoImage[0].transform.parent.gameObject.SetActive(value: false);
		}
		if (data2 != null)
		{
			PhotoImage[1].transform.parent.gameObject.SetActive(value: true);
			PhotoImage[1].sprite = WebImageCache.Instance.LoadingSprite;
			if (!string.IsNullOrEmpty(data2.ImageURL))
			{
				webTextureCache.StartCoroutine(webTextureCache.GetTexture((string)data2.ImageURL.Clone(), delegate(Texture2D tex, string url)
				{
					if (storedPopupGO.activeInHierarchy && storedData2.ImageURL == url)
					{
						PhotoImage[1].sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
					}
				}));
			}
			NameText[1].text = data2.UserName;
			CheckToggle[1].onValueChanged.RemoveAllListeners();
			CheckToggle[1].onValueChanged.AddListener(delegate(bool isOn)
			{
				EventSystem current = EventSystem.current;
				bool flag = CheckToggle[1].gameObject == current.currentSelectedGameObject;
				if (flag)
				{
					MenuUIManager.Instance.PlayClickAud();
				}
				if (isOn)
				{
					if (flag)
					{
						bool flag2 = 50 <= FBManager.Instance.UISelectedCount[(int)selectType];
						FBManager.Instance.UISelectedCount[(int)selectType]++;
						data2.UISelected[(int)selectType] = true;
						if (flag2)
						{
							CheckToggle[1].isOn = false;
						}
					}
				}
				else if (flag)
				{
					FBManager.Instance.UISelectedCount[(int)selectType] = Mathf.Max(0, FBManager.Instance.UISelectedCount[(int)selectType] - 1);
					data2.UISelected[(int)selectType] = false;
					if (selectType == FBUserProfile.UISelectType.InviteNonGameFriend)
					{
						InviteFriendsPopup.instance.passOnSelectAllChange = true;
						InviteFriendsPopup.instance.selectAllToggle.isOn = false;
					}
					else
					{
						RequestFriendsPopup.instance.passOnSelectAllChange = true;
						RequestFriendsPopup.instance.selectAllToggle.isOn = false;
					}
				}
			});
			CheckToggle[1].isOn = data2.UISelected[(int)selectType];
		}
		else
		{
			PhotoImage[1].transform.parent.gameObject.SetActive(value: false);
		}
	}
}
