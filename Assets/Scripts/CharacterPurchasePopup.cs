using Lean;
using SerializableClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterPurchasePopup : MonoBehaviour
{
	public ScrollRect Scroller;

	public GameObject CellPref;

	public Transform CharacterPreviewTrans;

	private Button closeBtn;

	private RectTransform listParentTrans;

	private Transform descriptionRoot;

	private Button purchaseBtn;

	private Text obtainText;

	private Image giftboxIcon;

	private int storedChIdx;

	public Sprite[] characterImages;

	private void Awake()
	{
		closeBtn = base.transform.Find("CloseBtn").GetComponent<Button>();
		closeBtn.onClick.AddListener(delegate
		{
			MenuUIManager.Instance.PlayClickAud();
			Close();
		});
	}

	private void Start()
	{
		base.gameObject.SetActive(value: false);
		initScroller();
		initDescription();
	}

	public void Open()
	{
		MenuUIManager.Instance.backBtnStackDepth = 1;
		MenuUIManager.Instance.SetActivateFilter(activate: true);
		base.gameObject.SetActive(value: true);
		CharacterPreviewTrans.localPosition = new Vector3(0f, -0.83f, -40f);
		CharacterPreviewTrans.localScale = Vector3.one * 1.1f;
		storedChIdx = MenuUIManager.Instance.pvIndicIdx;
		listParentTrans.GetChild(0).GetComponent<Toggle>().isOn = false;
		listParentTrans.GetChild(0).GetComponent<Toggle>().isOn = true;
		updateDescriptions(0);
		SkinnedMeshRenderer[] componentsInChildren = MenuUIManager.Instance.PreviewScrollRoot.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		componentsInChildren.All(delegate(SkinnedMeshRenderer renderer)
		{
			renderer.material.SetFloat("_Outline", -0.2f);
			return true;
		});
	}

	public void Close()
	{
		if (base.gameObject.activeInHierarchy)
		{
			MenuUIManager.Instance.SetActivateFilter(activate: false);
			base.gameObject.SetActive(value: false);
			CharacterPreviewTrans.localPosition = Vector3.zero;
			CharacterPreviewTrans.localScale = Vector3.one;
			MenuUIManager.Instance.pvIndicIdx = storedChIdx;
			MenuUIManager.Instance.ForcePreviewCharIndex(storedChIdx);
			SkinnedMeshRenderer[] componentsInChildren = MenuUIManager.Instance.PreviewScrollRoot.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
			componentsInChildren.All(delegate(SkinnedMeshRenderer renderer)
			{
				renderer.material.SetFloat("_Outline", 0.02f);
				return true;
			});
		}
	}

	private void initScroller()
	{
		listParentTrans = Scroller.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
		GridLayoutGroup component = listParentTrans.GetComponent<GridLayoutGroup>();
		ToggleGroup group = listParentTrans.GetComponent<ToggleGroup>();
		DataContainer.Instance.CharacterTableRaw.dataArray.Select(delegate(CharacterInfoData s, int i)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(CellPref);
			gameObject.transform.SetParent(listParentTrans);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			Toggle toggle = gameObject.GetComponent<Toggle>();
			toggle.group = group;
			toggle.transform.GetChild(0).Find("Photo").GetComponent<Image>()
				.sprite = FindCharacterImageFromPath(s.Iconpath);
				toggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					EventSystem current = EventSystem.current;
					bool flag = toggle.gameObject == current.currentSelectedGameObject;
					if (isOn)
					{
						if (flag)
						{
							MenuUIManager.Instance.PlayClickAud();
						}
						MenuUIManager.Instance.ForcePreviewCharIndex(i);
						updateDescriptions(i);
					}
				});
				updateChList(i);
				return i;
			}).Count();
			float num = Mathf.Ceil((float)DataContainer.Instance.CharacterTableRaw.dataArray.Length / (float)component.constraintCount);
			Vector2 sizeDelta = listParentTrans.sizeDelta;
			float num2 = num;
			Vector2 cellSize = component.cellSize;
			float num3 = num2 * cellSize.y;
			float num4 = num - 1f;
			Vector2 spacing = component.spacing;
			sizeDelta.y = num3 + num4 * spacing.y;
			listParentTrans.sizeDelta = sizeDelta;
		}

		private void initDescription()
		{
			descriptionRoot = base.transform.Find("Description");
			purchaseBtn = base.transform.Find("PurchaseBtn").GetComponent<Button>();
			purchaseBtn.onClick.AddListener(delegate
			{
				MenuUIManager.Instance.PlayClickAud();
				string iD = DataContainer.Instance.CharacterTableRaw.dataArray[MenuUIManager.Instance.pvIndicIdx].ID;
				CharacterInfoData characterInfoData = DataContainer.Instance.CharacterTableRaw[iD];
				if (PlayerInfo.Instance.CharUnlocks[iD])
				{
					storedChIdx = MenuUIManager.Instance.pvIndicIdx;
					Close();
				}
				else
				{
					int[] array = DataContainer.Instance.CharacterUnlockCounts[iD];
					int num = 0;
					for (num = 0; num < array.Length && (num == 2 || 0 >= array[num]); num++)
					{
					}
					switch (num)
					{
					default:
						return;
					case 2:
						return;
					case 0:
						if (array[0] <= PlayerInfo.Instance.Currency[CurrencyType.Gem])
						{
							CurrencyTypeMapInt currency;
							(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] - array[0];
							PlayerInfo.Instance.CharUnlocks[iD] = true;
							string value5 = string.Format(LeanLocalization.GetTranslationText("70"), LeanLocalization.GetTranslationText(characterInfoData.Name1loc));
							Action value6 = delegate
							{
							};
							MenuUIManager.Instance.ShowPopupCommon(new Dictionary<string, object>
							{
								{
									"type",
									"Notify"
								},
								{
									"okHandler",
									value6
								},
								{
									"msg",
									value5
								}
							});
							string modelname = DataContainer.Instance.CharacterTableRaw[iD].Modelname;
							modelname = Regex.Replace(modelname, "[0-9]+$", string.Empty);
							modelname = $"{modelname}01";
							MenuUIManager.Instance.pvAnims[MenuUIManager.Instance.pvIndicIdx].CrossFadeQueued(modelname + "_attacking", 0.1f, QueueMode.PlayNow);
							MenuUIManager.Instance.pvAnims[MenuUIManager.Instance.pvIndicIdx].CrossFadeQueued(modelname + "_idling01", 0.1f, QueueMode.CompleteOthers);
						}
						else
						{
							MenuUIManager.Instance.showCurrencyPop(MenuCurrencyType.Gem, MenuCurrencyType.Gem, delegate
							{
							}, delegate
							{
							});
						}
						break;
					case 1:
						if (array[1] <= PlayerInfo.Instance.Currency[CurrencyType.Gold])
						{
							CurrencyTypeMapInt currency;
							(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] - array[1];
							PlayerInfo.Instance.CharUnlocks[iD] = true;
							string value7 = string.Format(LeanLocalization.GetTranslationText("70"), LeanLocalization.GetTranslationText(characterInfoData.Name1loc));
							Action value8 = delegate
							{
							};
							MenuUIManager.Instance.ShowPopupCommon(new Dictionary<string, object>
							{
								{
									"type",
									"Notify"
								},
								{
									"okHandler",
									value8
								},
								{
									"msg",
									value7
								}
							});
						}
						else
						{
							MenuUIManager.Instance.showCurrencyPop(MenuCurrencyType.Gold, MenuCurrencyType.Gold, delegate
							{
							}, delegate
							{
							});
						}
						break;
					case 3:
						if (array[3] <= PlayerInfo.Instance.CharOwnedTokens[array[2].ToString()])
						{
							PlayerInfo.Instance.CharUnlocks[iD] = true;
							string value = string.Format(LeanLocalization.GetTranslationText("70"), LeanLocalization.GetTranslationText(characterInfoData.Name1loc));
							Action value2 = delegate
							{
							};
							MenuUIManager.Instance.ShowPopupCommon(new Dictionary<string, object>
							{
								{
									"type",
									"Notify"
								},
								{
									"okHandler",
									value2
								},
								{
									"msg",
									value
								}
							});
						}
						else
						{
							string value3 = string.Format(LeanLocalization.GetTranslationText("68"), LeanLocalization.GetTranslationText(DataContainer.Instance.TokenTableRaw[array[2].ToString()].Name1loc));
							Action value4 = delegate
							{
							};
							MenuUIManager.Instance.ShowPopupCommon(new Dictionary<string, object>
							{
								{
									"type",
									"Notify"
								},
								{
									"okHandler",
									value4
								},
								{
									"msg",
									value3
								}
							});
						}
						break;
					}
					updateDescriptions(MenuUIManager.Instance.pvIndicIdx);
					updateChList(MenuUIManager.Instance.pvIndicIdx);
				}
			});
			obtainText = base.transform.Find("ObtainText").GetComponent<Text>();
			giftboxIcon = base.transform.Find("GiftIcon").GetComponent<Image>();
		}

		private void updateDescriptions(int index)
		{
			string iD = DataContainer.Instance.CharacterTableRaw.dataArray[index].ID;
			int[] array = DataContainer.Instance.CharacterUnlockCounts[iD];
			int num = 0;
			for (num = 0; num < array.Length && (num == 2 || 0 >= array[num]); num++)
			{
			}
			switch (num)
			{
			case 0:
				obtainText.gameObject.SetActive(value: false);
				giftboxIcon.gameObject.SetActive(value: false);
				break;
			case 1:
				obtainText.gameObject.SetActive(value: false);
				giftboxIcon.gameObject.SetActive(value: false);
				break;
			case 3:
				obtainText.gameObject.SetActive(value: true);
				giftboxIcon.gameObject.SetActive(value: true);
				break;
			default:
				obtainText.gameObject.SetActive(value: false);
				giftboxIcon.gameObject.SetActive(value: false);
				break;
			}
			if (PlayerInfo.Instance.CharUnlocks[iD])
			{
				purchaseBtn.transform.Find("PurchaseRoot").gameObject.SetActive(value: false);
				purchaseBtn.transform.Find("SelectRoot").gameObject.SetActive(value: true);
			}
			else
			{
				purchaseBtn.transform.Find("PurchaseRoot").gameObject.SetActive(value: true);
				purchaseBtn.transform.Find("SelectRoot").gameObject.SetActive(value: false);
				Sprite sprite = null;
				string text = string.Empty;
				switch (num)
				{
				case 0:
					sprite = MenuUIManager.Instance.CurrencyMidIcons[0];
					text = $"{array[0]:D}";
					break;
				case 1:
					sprite = MenuUIManager.Instance.CurrencyMidIcons[1];
					text = $"{array[1]:D}";
					break;
				case 3:
				{
					string iconpath = DataContainer.Instance.TokenTableRaw[array[2].ToString()].Iconpath;
					sprite = DataContainer.Instance.GetAssetResources<Sprite>(iconpath);
					text = $"{PlayerInfo.Instance.CharOwnedTokens[array[2].ToString()]:D}/{array[3]:D}";
					break;
				}
				}
				Image component = purchaseBtn.transform.Find("PurchaseRoot/CoinIconImage").GetComponent<Image>();
				component.sprite = sprite;
				Text component2 = purchaseBtn.transform.Find("PurchaseRoot/IconCoinText").GetComponent<Text>();
				component2.text = text;
			}
			Image component3 = descriptionRoot.Find("ChPhoto/Photo").GetComponent<Image>();
			Image component4 = descriptionRoot.Find("SkillIcon").GetComponent<Image>();
			LLocImage component5 = descriptionRoot.Find("ChNameImage").GetComponent<LLocImage>();
			Text component6 = descriptionRoot.Find("DescText").GetComponent<Text>();
			Text component7 = descriptionRoot.Find("SkillDescText").GetComponent<Text>();
			CharacterInfoData characterInfoData = DataContainer.Instance.CharacterTableRaw[iD];
			component3.sprite = FindCharacterImageFromPath(characterInfoData.Iconpath);
			component4.sprite = DataContainer.Instance.GetAssetResources<Sprite>(characterInfoData.Skillimagepath);
			component5.SetPhraseName(characterInfoData.Nameimagepath);
			component6.text = LeanLocalization.GetTranslationText(characterInfoData.Chardesc1loc);
			component7.text = LeanLocalization.GetTranslationText(characterInfoData.Skilldesc1loc);
		}

		private void updateChList()
		{
			DataContainer.Instance.CharacterTableRaw.dataArray.Select(delegate(CharacterInfoData s, int i)
			{
				updateChList(i);
				return i;
			}).Count();
		}

		private void updateChList(int index)
		{
			CanvasGroup component = listParentTrans.GetChild(index).GetChild(0).GetComponent<CanvasGroup>();
			if (PlayerInfo.Instance.CharUnlocks[DataContainer.Instance.CharacterTableRaw.dataArray[index].ID])
			{
				component.alpha = 1f;
			}
			else
			{
				component.alpha = 0.3f;
			}
		}

		private Sprite FindCharacterImageFromPath(string path)
		{
			int num = characterImages.Length;
			for (int i = 0; i < num; i++)
			{
				if (characterImages[i].name == path.Remove(0, 11))
				{
					return characterImages[i];
				}
			}
			return null;
		}
	}
