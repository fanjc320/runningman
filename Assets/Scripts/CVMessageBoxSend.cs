using EnhancedUI.EnhancedScroller;
using Facebook.Unity;
using Lean;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CVMessageBoxSend : EnhancedScrollerCellView
{
	public Text MessageText;

	public Image[] Items;

	public Button SendBtn;

	public void SetData(EnhancedScroller scroller, MessageBoxRequests msgBoxData)
	{
		if (msgBoxData.Requests.Length == 0)
		{
			return;
		}
		string empty = string.Empty;
		int num = (from s in msgBoxData.Requests
			where null != s
			select s).Count();
		empty = ((num != 1) ? string.Format(LeanLocalization.GetTranslationText("188"), msgBoxData.Requests[0].FromUser.UserName, num - 1) : string.Format(LeanLocalization.GetTranslationText("189"), msgBoxData.Requests[0].FromUser.UserName));
		MessageText.text = empty;
		for (int i = 0; 5 > i; i++)
		{
			if (msgBoxData.Requests[i] == null)
			{
				Items[i].transform.Find("Deco").gameObject.SetActive(value: false);
				continue;
			}
			Items[i].transform.Find("Deco").gameObject.SetActive(value: true);
			Image renderer = Items[i].transform.Find("Deco/Photo").GetComponent<Image>();
			renderer.sprite = WebImageCache.GetSprite(msgBoxData.Requests[i].FromUser.ImageURL, ref renderer);
		}
		SendBtn.onClick.RemoveAllListeners();
		SendBtn.onClick.AddListener(delegate
		{
			MenuUIManager.Instance.PlayClickAud();
			MenuUIManager.Instance.ActivateLoadingScreenFilter(isActivate: true);
			FB.AppRequest("Message: Send help testing", (from s in msgBoxData.Requests
				where null != s
				select s.FromUser.Id).ToList(), null, null, 0, "send", "Title: Send help testing", SendHelp(msgBoxData));
			FBManager.Instance.MessageBoxItems.Remove(msgBoxData);
			MenuUIManager.Instance.NeedCheckMessageBoxNew();
			LateUpdater.Instance.AddAction(delegate
			{
				scroller.transform.parent.SendMessage("CheckNoItems");
				scroller.ReloadData();
				scroller.ScrollPosition = 1f;
				scroller.ScrollPosition = 0f;
			});
		});
	}

	private static FacebookDelegate<IAppRequestResult> SendHelp(MessageBoxRequests msgBoxData)
	{
		return delegate(IAppRequestResult result)
		{
			MenuUIManager.Instance.ActivateLoadingScreenFilter(isActivate: false);
			if (!result.Cancelled && string.IsNullOrEmpty(result.Error))
			{
				for (int i = 0; 50 > i; i++)
				{
					if (msgBoxData.Requests[i] != null)
					{
						string key = "PPrefKey_LastSendRequestTime_" + msgBoxData.Requests[i].FromUser.Id;
						PlayerPrefs.SetString(key, DateTime.Today.Ticks.ToString());
						FB.API("/" + msgBoxData.Requests[i].RequestID, HttpMethod.DELETE, delegate
						{
						});
					}
				}
			}
		};
	}
}
