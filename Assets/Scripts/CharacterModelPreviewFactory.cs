using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModelPreviewFactory : MonoBehaviour
{
	[Serializable]
	private class CharacterModelSetup
	{
		public string name;

		public Material material;

		public Mesh mesh;

		public AnimationClip animationClip;

		public AnimationClip headRotationAnimationClip;
	}

	[SerializeField]
	private GameObject characterModelPreviewPrefab;

	private CharacterCustomization characterCustomization;

	[SerializeField]
	private CharacterModelSetup[] characters;

	private Dictionary<string, CharacterModelSetup> name2character = new Dictionary<string, CharacterModelSetup>();

	public static CharacterModelPreviewFactory instance;

	public static CharacterModelPreviewFactory Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(CharacterModelPreviewFactory)) as CharacterModelPreviewFactory));

	public void Awake()
	{
	}

	public GameObject GetCharacterModelPreview(string name, int version)
	{
		if (name2character.TryGetValue(name, out CharacterModelSetup value))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(characterModelPreviewPrefab);
			SkinnedMeshRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array = componentsInChildren;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				if (skinnedMeshRenderer.gameObject.name == "renderer")
				{
					skinnedMeshRenderer.material = value.material;
					skinnedMeshRenderer.sharedMesh = value.mesh;
				}
				else if (skinnedMeshRenderer.gameObject.name == "CustomMesh")
				{
					skinnedMeshRenderer.enabled = false;
				}
			}
			Animation component = gameObject.GetComponent<Animation>();
			component.AddClip(value.animationClip, "clip");
			component.Play("clip");
			component["clip"].speed = 0f;
			component.AddClip(value.headRotationAnimationClip, "headClip");
			component["headClip"].blendMode = AnimationBlendMode.Blend;
			component["headClip"].weight = 1f;
			component["headClip"].layer = 10;
			component["headClip"].enabled = true;
			component["headClip"].speed = 0f;
			component["headClip"].normalizedTime = 0f;
			return gameObject;
		}
		return null;
	}

	public void Start()
	{
	}

	public void SetCharacterCustomization(CharacterCustomization cc)
	{
		characterCustomization = cc;
	}
}
