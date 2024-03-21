using Lean;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{
	[SerializeField]
	private bool displayText;

	[SerializeField]
	private string text;

	[SerializeField]
	private bool displayMesh;

	[SerializeField]
	private float direction;

	[SerializeField]
	private float time = 1f;

	[SerializeField]
	private bool endTutorial;

	[SerializeField]
	private bool allowHoverboard;

	[SerializeField]
	private int type;

	private Character character;

	private Track track;

	private bool Initialiseret;

	private GameObject _mesh;

	private GameObject mesh
	{
		get
		{
			if (_mesh == null)
			{
				_mesh = Camera.main.gameObject.transform.Find("arrow").gameObject;
			}
			return _mesh;
		}
	}

	private void Awake()
	{
	}

	private void Update()
	{
		if (!(Game.Instance == null) && !Initialiseret)
		{
			character = Game.Instance.character;
			track = Track.Instance;
			Initialiseret = true;
		}
	}

	private IEnumerator ShowArrow()
	{
		mesh.transform.rotation = Quaternion.AngleAxis(direction, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		mesh.SetActive(value: true);
		Vector3 pos = new Vector3(0f, 0f, 20f);
		yield return StartCoroutine(pTween.To(time, delegate(float t)
		{
			mesh.transform.localPosition = Vector3.Lerp(pos - mesh.transform.up * 5f, pos + mesh.transform.up * 5f, t);
			mesh.GetComponent<Renderer>().material.mainTextureOffset = Vector2.Lerp(Vector2.zero, new Vector2(0f, -0.035f), t);
			if (!Game.Instance.IsInGame.Value)
			{
				mesh.transform.localPosition = new Vector3(2000f, 2000f, 0f);
			}
		}));
		mesh.SetActive(value: false);
	}

	private void OnTriggerEnter(Collider collider)
	{
		MainUIManager.Instance.DoTutorialEvent(this.text);
		string text = this.text;
		if (text == null)
		{
			return;
		}
		if (!(text == "DBLJump"))
		{
			if (!(text == "Shield"))
			{
				if (text == "End")
				{
					GoogleAnalyticsV4.getInstance().LogEvent("Tutorial", "End", "Tutorial Complete", -1L);
					GoogleAnalyticsV4.getInstance().LogScreen("Tutorial Complete");
					StartCoroutine(pTween.To(0.5f, delegate(float norm)
					{
						Time.timeScale = 1f - norm;
					}, delegate
					{
						Time.timeScale = 0f;
					}));
					Action value = delegate
					{
						Time.timeScale = 1f;
						LateUpdater.Instance.AddAction(delegate
						{
							Time.timeScale = 1f;
						});
						LeanTween.delayedCall(0f, (Action)delegate
						{
							Time.timeScale = 1f;
						});
						MainUIManager.Instance.OnBtnClick_GotoMenu();
						PlayerInfo.Instance.TutorialCompleted = true;
					};
					MainUIManager.Instance.ShowPopupCommon(new Dictionary<string, object>
					{
						{
							"type",
							"Notify"
						},
						{
							"msg",
							LeanLocalization.GetTranslationText("169")
						},
						{
							"okHandler",
							value
						}
					});
				}
			}
			else
			{
				Hoverboard.Instance.isAllowed = true;
				PlayerInfo.Instance.StartItems[2] = true;
			}
		}
		else
		{
			PlayerInfo.Instance.StartItems[3] = true;
			PlayerInfo.Instance.StartItemCounts[3] = 1;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (!character.stopColliding && collider.gameObject.name.Equals("Character"))
		{
			if (allowHoverboard)
			{
				Hoverboard.Instance.isAllowed = true;
			}
			if (endTutorial)
			{
				PlayerInfo instance = PlayerInfo.Instance;
				track.IsRunningOnTutorialTrack = false;
				instance.TutorialCompleted = true;
				track.tutorial = false;
			}
		}
	}
}
