using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
	[Serializable]
	public class Characters
	{
		public string name;

		public CustomSets[] customSets;
	}

	[Serializable]
	public class CustomSets
	{
		public Material material;

		public TwoGradientToTexture gradient;

		public Mesh skinnedMesh;

		public Material customSkinnedMaterial;

		public Renderer blinkingEyes;
	}

	public Characters[] characters;

	private Texture2D gradient;

	[HideInInspector]
	public Dictionary<string, CustomSets[]> customSetLookupTable = new Dictionary<string, CustomSets[]>();

	[SerializeField]
	private SkinnedMeshRenderer customSkinnedMeshRenderer;

	public void Initialize()
	{
		if (characters.Length != 0)
		{
			for (int i = 0; i < characters.Length; i++)
			{
				customSetLookupTable.Add(characters[i].name, characters[i].customSets);
			}
		}
		CharacterModelPreviewFactory instance = CharacterModelPreviewFactory.Instance;
		instance.SetCharacterCustomization(this);
	}

	public void Customize(string name, int customizedVersion, SkinnedMeshRenderer model)
	{
		if (customSkinnedMeshRenderer != null)
		{
			customSkinnedMeshRenderer.enabled = false;
		}
		if (!customSetLookupTable.TryGetValue(name, out CustomSets[] value))
		{
			return;
		}
		if (value[customizedVersion].material != null)
		{
			model.material = value[customizedVersion].material;
		}
		if (value[customizedVersion].blinkingEyes != null)
		{
			value[customizedVersion].blinkingEyes.material = value[customizedVersion].material;
		}
		Material material = null;
		if (value[customizedVersion].gradient != null)
		{
			TwoGradientToTexture twoGradientToTexture = UnityEngine.Object.Instantiate(value[customizedVersion].gradient);
			gradient = twoGradientToTexture.Initialize();
			material = model.material;
			material.SetTexture("_CustomColorsTex", gradient);
			model.material = material;
			UnityEngine.Object.Destroy(twoGradientToTexture.gameObject);
		}
		if (!(value[customizedVersion].skinnedMesh != null))
		{
			return;
		}
		customSkinnedMeshRenderer.sharedMesh = value[customizedVersion].skinnedMesh;
		customSkinnedMeshRenderer.enabled = true;
		if (!(value[customizedVersion].customSkinnedMaterial != null))
		{
			return;
		}
		if (value[customizedVersion].customSkinnedMaterial == value[customizedVersion].material)
		{
			if (material != null)
			{
				customSkinnedMeshRenderer.material = material;
			}
			else
			{
				customSkinnedMeshRenderer.material = value[customizedVersion].customSkinnedMaterial;
			}
		}
		else
		{
			customSkinnedMeshRenderer.material = value[customizedVersion].customSkinnedMaterial;
		}
	}
}
