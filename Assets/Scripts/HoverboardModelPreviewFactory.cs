using System;
using System.Collections.Generic;
using UnityEngine;

public class HoverboardModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	public class HoverboardModelSetup
	{
		public string name;

		public Vector3 eulerAngles;

		public float menuYPosition;

		public AnimationClip clipHangtime;

		public AnimationClip clipRun;

		public GameObject hoverboardPrefab;
	}

	[SerializeField]
	private HoverboardModelSetup[] hoverboards;

	private Dictionary<string, HoverboardModelSetup> name2character = new Dictionary<string, HoverboardModelSetup>();

	public static HoverboardModelPreviewFactory instance;

	public static HoverboardModelPreviewFactory Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(HoverboardModelPreviewFactory)) as HoverboardModelPreviewFactory));

	public void Awake()
	{
		HoverboardModelSetup[] array = hoverboards;
		foreach (HoverboardModelSetup hoverboardModelSetup in array)
		{
			name2character.Add(hoverboardModelSetup.name, hoverboardModelSetup);
		}
	}

	public void SelectHoverboard(string name, ref GameObject hoverboardGO, Animation characterAnimation)
	{
		if (name2character.TryGetValue(name, out HoverboardModelSetup value))
		{
			string name2 = hoverboardGO.name;
			Transform parent = hoverboardGO.transform.parent;
			UnityEngine.Object.Destroy(hoverboardGO);
			hoverboardGO = UnityEngine.Object.Instantiate(value.hoverboardPrefab);
			hoverboardGO.transform.parent = parent;
			hoverboardGO.transform.localPosition = Vector3.zero;
			hoverboardGO.transform.localRotation = Quaternion.identity;
			hoverboardGO.transform.localScale = Vector3.one;
			hoverboardGO.name = name2;
			if (characterAnimation[value.clipHangtime.name] == null)
			{
				characterAnimation.AddClip(value.clipHangtime, value.clipHangtime.name);
			}
			characterAnimation[value.clipHangtime.name].wrapMode = WrapMode.Once;
			characterAnimation.CrossFade(value.clipHangtime.name, 0.15f);
			if (characterAnimation[value.clipRun.name] == null)
			{
				characterAnimation.AddClip(value.clipRun, value.clipRun.name);
			}
			characterAnimation.CrossFadeQueued(value.clipRun.name, 0.5f);
		}
	}

	public Quaternion GetHoverboardDefaultRotation(string name)
	{
		if (name2character.TryGetValue(name, out HoverboardModelSetup value))
		{
			return Quaternion.Euler(value.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
		}
		return Quaternion.Euler(194f, 110.5f, 180f);
	}

	public GameObject GetHoverboardModelPreview(string name)
	{
		if (name2character.TryGetValue(name, out HoverboardModelSetup value))
		{
			GameObject gameObject = new GameObject("Hoverboard: " + name);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(value.hoverboardPrefab);
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localRotation = Quaternion.Euler(value.eulerAngles) * Quaternion.Euler(0f, 180f, 0f);
			Vector3 localPosition = gameObject2.transform.localPosition;
			gameObject2.transform.localPosition = localPosition + new Vector3(0f, value.menuYPosition, 0f);
			return gameObject;
		}
		return null;
	}
}
