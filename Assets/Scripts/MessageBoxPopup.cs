using EnhancedUI.EnhancedScroller;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxPopup : MonoBehaviour, IEnhancedScrollerDelegate
{
	public EnhancedScroller Scroller;

	public CVMessageBoxSend CVMsgBoxSendPref;

	public CVMessageBoxRecv CVMsgBoxRecvPref;

	public Text InviteMsgText;

	public Button InviteBtn;

	private Button closeBtn;

	private void Awake()
	{
		Scroller.Delegate = this;
		closeBtn = base.transform.Find("CloseBtn").GetComponent<Button>();
		closeBtn.onClick.AddListener(delegate
		{
			MenuUIManager.Instance.PlayClickAud();
			Close();
		});
		InviteBtn.onClick.AddListener(delegate
		{
			MenuUIManager.Instance.PlayClickAud();
			Close();
			LeanTween.delayedCall(0f, (Action)delegate
			{
				MenuUIManager.Instance.OpenInviteFriendPopup();
			});
		});
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
	}

	public void CheckNoItems()
	{
		if (0 < FBManager.Instance.MessageBoxItems.Count)
		{
			RectTransform component = GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(1.9f, -10.1f);
			component.sizeDelta = new Vector2(537.2f, 735.9f);
			InviteMsgText.gameObject.SetActive(value: false);
			InviteBtn.gameObject.SetActive(value: false);
		}
		else
		{
			RectTransform component2 = GetComponent<RectTransform>();
			component2.anchoredPosition = new Vector2(1.9f, -48.1f);
			component2.sizeDelta = new Vector2(483.5f, 416.9f);
			InviteMsgText.gameObject.SetActive(value: true);
			InviteBtn.gameObject.SetActive(value: true);
		}
	}

	public void Open()
	{
		MenuUIManager.Instance.backBtnStackDepth = 1;
		MenuUIManager.Instance.SetActivateFilter(activate: true);
		base.gameObject.SetActive(value: true);
		CheckNoItems();
		Scroller.ReloadData();
		Scroller.ScrollPosition = 1f;
		Scroller.ScrollPosition = 0f;
	}

	public void Close()
	{
		MenuUIManager.Instance.SetActivateFilter(activate: false);
		base.gameObject.SetActive(value: false);
	}

	public int GetNumberOfCells(EnhancedScroller scroller)
	{
		return Mathf.CeilToInt(FBManager.Instance.MessageBoxItems.Count);
	}

	public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
	{
		return 117.5f;
	}

	public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
	{
		EnhancedScrollerCellView enhancedScrollerCellView = null;
		if (FBManager.Instance.MessageBoxItems[dataIndex].IsRequest)
		{
			CVMessageBoxSend cVMessageBoxSend = scroller.GetCellView(CVMsgBoxSendPref) as CVMessageBoxSend;
			cVMessageBoxSend.name = dataIndex.ToString();
			cVMessageBoxSend.transform.localPosition = Vector3.zero;
			cVMessageBoxSend.SetData(Scroller, FBManager.Instance.MessageBoxItems[dataIndex]);
			return cVMessageBoxSend;
		}
		CVMessageBoxRecv cVMessageBoxRecv = scroller.GetCellView(CVMsgBoxRecvPref) as CVMessageBoxRecv;
		cVMessageBoxRecv.name = dataIndex.ToString();
		cVMessageBoxRecv.transform.localPosition = Vector3.zero;
		cVMessageBoxRecv.SetData(Scroller, FBManager.Instance.MessageBoxItems[dataIndex]);
		return cVMessageBoxRecv;
	}
}
