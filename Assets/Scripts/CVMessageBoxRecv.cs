using EnhancedUI.EnhancedScroller;
using Facebook.Unity;
using Lean;
using System.Linq;
using UnityEngine.UI;

public class CVMessageBoxRecv : EnhancedScrollerCellView
{
	public Text MessageText;

	public Image[] Items;

	public Button RecvBtn;

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
		empty = ((num != 1) ? string.Format(LeanLocalization.GetTranslationText("190"), msgBoxData.Requests[0].FromUser.UserName, num - 1) : string.Format(LeanLocalization.GetTranslationText("191"), msgBoxData.Requests[0].FromUser.UserName));
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
		if (10 <= PlayerInfo.Instance.NameTagCount)
		{
			RecvBtn.enabled = false;
			RecvBtn.transform.parent.Find("RecvBtn_Disable").gameObject.SetActive(value: true);
			return;
		}
		RecvBtn.enabled = true;
		RecvBtn.transform.parent.Find("RecvBtn_Disable").gameObject.SetActive(value: false);
		RecvBtn.onClick.RemoveAllListeners();
		RecvBtn.onClick.AddListener(delegate
		{
			MenuUIManager.Instance.PlayClickAud();
			int num2 = 0;
			for (int j = 0; 5 > j; j++)
			{
				if (msgBoxData.Requests[j] != null)
				{
					num2++;
					FB.API("/" + msgBoxData.Requests[j].RequestID, HttpMethod.DELETE, delegate
					{
					});
				}
			}
			FBManager.Instance.MessageBoxItems.Remove(msgBoxData);
			MenuUIManager.Instance.NeedCheckMessageBoxNew();
			PlayerInfo.Instance.NameTagCount += num2;
			LateUpdater.Instance.AddAction(delegate
			{
				scroller.transform.parent.SendMessage("CheckNoItems");
				scroller.ReloadData();
				scroller.ScrollPosition = 1f;
				scroller.ScrollPosition = 0f;
			});
		});
	}
}
