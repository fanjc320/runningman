using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeAssets : MonoBehaviour
{
	[Serializable]
	public class TextureMaterialPair
	{
		public Texture2D texture;

		public Material material;
	}

	[Serializable]
	public class Assets
	{
		public string theme;

		public GameObject[] environmentModels;

		public GameObject[] characterModels;

		public TextureMaterialPair[] textureAndMaterials;

		public GameObject[] monumentPrefabs;

		public GameObject[] backgroundPrefabs;

		public AudioClip[] sounds;

		public Vector4[] distort;

		public Color[] fogGradientTop;

		public Color[] fogGradientBottom;

		public Color[] fogSilhouetteColor;

		public Texture ToonRamp;

		public float RimPower;

		public float RimStrength;

		public Color RimColor;

		public float Outline;

		public Color OutlineColor;

		public float fogGradientOffset;

		public Material glowGold;

		public Color glowGoldColor;

		public float glowGoldFalloff = 200f;

		public Dictionary<Material, Texture> textureAndMaterialLinks = new Dictionary<Material, Texture>();

		public void CreateMaterialsAndTextureLink()
		{
			TextureMaterialPair[] array = textureAndMaterials;
			foreach (TextureMaterialPair textureMaterialPair in array)
			{
				textureAndMaterialLinks.Add(textureMaterialPair.material, textureMaterialPair.texture);
			}
		}
	}

	public Assets[] assets;

	public EnvironmentBackground background;

	public Material[] dynamicChangingMaterials;

	public HashSet<Material> dynamicChangingMaterialsSet = new HashSet<Material>();

	[HideInInspector]
	public Dictionary<string, Mesh> environmentModelMeshes = new Dictionary<string, Mesh>();

	[HideInInspector]
	public Dictionary<string, Mesh> characterModelMeshes = new Dictionary<string, Mesh>();

	[HideInInspector]
	public Dictionary<Material, Material> original2CloneMaterials = new Dictionary<Material, Material>();

	private Dictionary<Theme, Assets> theme2assets;

	private HashSet<Texture> allTextures;

	private CameraCulling cameraCulling;

	private int seqPropsIdx = -1;

	private Coroutine crtChangePropSequence;

	private Coroutine crtChangeColors;

	private Coroutine crtChangeDistort;

	public static ThemeAssets instance;

	public static ThemeAssets Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (UnityEngine.Object.FindObjectOfType(typeof(ThemeAssets)) as ThemeAssets);
				if (!(instance == null))
				{
				}
			}
			return instance;
		}
	}

	private void GetMeshes()
	{
		Assets[] array = this.assets;
		foreach (Assets assets in array)
		{
			GameObject[] environmentModels = assets.environmentModels;
			foreach (GameObject gameObject in environmentModels)
			{
				MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>(includeInactive: true);
				MeshFilter[] array2 = componentsInChildren;
				foreach (MeshFilter meshFilter in array2)
				{
					if (!environmentModelMeshes.ContainsKey(meshFilter.sharedMesh.name))
					{
						environmentModelMeshes.Add(meshFilter.sharedMesh.name, meshFilter.sharedMesh);
					}
				}
			}
			GameObject[] characterModels = assets.characterModels;
			foreach (GameObject gameObject2 in characterModels)
			{
				SkinnedMeshRenderer[] componentsInChildren2 = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
				SkinnedMeshRenderer[] array3 = componentsInChildren2;
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array3)
				{
					if (!characterModelMeshes.ContainsKey(skinnedMeshRenderer.sharedMesh.name))
					{
						characterModelMeshes.Add(skinnedMeshRenderer.sharedMesh.name, skinnedMeshRenderer.sharedMesh);
					}
				}
			}
		}
	}

	public Material ReplaceMaterial(Material originalMaterial)
	{
		Material result = null;
		Material value;
		if (dynamicChangingMaterialsSet.Contains(originalMaterial))
		{
			result = RegisterDynmaicMaterial(originalMaterial);
		}
		else if (original2CloneMaterials.TryGetValue(originalMaterial, out value))
		{
			result = value;
		}
		return result;
	}

	private void Awake()
	{
		cameraCulling = (UnityEngine.Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling);
		theme2assets = new Dictionary<Theme, Assets>();
		allTextures = new HashSet<Texture>();
		Material[] array = dynamicChangingMaterials;
		foreach (Material item in array)
		{
			dynamicChangingMaterialsSet.Add(item);
		}
		Assets[] array2 = this.assets;
		foreach (Assets assets in array2)
		{
			Theme theme = Theme.FindByName(assets.theme);
			if (theme == null)
			{
				continue;
			}
			theme2assets.Add(theme, assets);
			assets.CreateMaterialsAndTextureLink();
			TextureMaterialPair[] textureAndMaterials = assets.textureAndMaterials;
			foreach (TextureMaterialPair textureMaterialPair in textureAndMaterials)
			{
				if (!allTextures.Contains(textureMaterialPair.texture))
				{
					allTextures.Add(textureMaterialPair.texture);
				}
				if (!original2CloneMaterials.ContainsKey(textureMaterialPair.material))
				{
					Material value = new Material(textureMaterialPair.material);
					original2CloneMaterials.Add(textureMaterialPair.material, value);
				}
			}
			if (!original2CloneMaterials.ContainsKey(assets.glowGold))
			{
				Material value2 = new Material(assets.glowGold);
				original2CloneMaterials.Add(assets.glowGold, value2);
			}
		}
		if (!(background == null))
		{
			background.ResetBackground(this.assets[0].monumentPrefabs, this.assets[0].backgroundPrefabs);
		}
		GetMeshes();
	}

	public Material RegisterDynmaicMaterial(Material sharedMaterial)
	{
		Material material = null;
		Material material2 = new Material(sharedMaterial);
		material2.name = "replaced_" + sharedMaterial.name;
		Assets[] array = this.assets;
		foreach (Assets assets in array)
		{
			TextureMaterialPair[] textureAndMaterials = assets.textureAndMaterials;
			foreach (TextureMaterialPair textureMaterialPair in textureAndMaterials)
			{
				if (textureMaterialPair.material == sharedMaterial)
				{
					assets.textureAndMaterialLinks.Add(material2, textureMaterialPair.texture);
					if (!original2CloneMaterials.ContainsKey(material2))
					{
						material = new Material(material2);
						original2CloneMaterials.Add(material2, material);
					}
				}
			}
		}
		return material;
	}

	private void Start()
	{
		ThemeManager.Instance.OnChangeTheme += OnChangeTheme;
		ThemeManager.Instance.Theme = Theme.NORMAL;
	}

	private void OnDestroy()
	{
		ThemeManager.Instance.OnChangeTheme -= OnChangeTheme;
	}

	private void OnChangeTheme(Theme theme)
	{
		if (theme == null)
		{
			return;
		}
		Assets assets = theme2assets[theme];
		Shader.SetGlobalColor("_SkyGradientTopColor", assets.fogGradientTop[0]);
		Shader.SetGlobalColor("_SkyGradientBottomColor", assets.fogGradientBottom[0]);
		Shader.SetGlobalColor("_FogSilhouetteColor", assets.fogSilhouetteColor[0]);
		Shader.SetGlobalVector("_Distort", assets.distort[0]);
		Shader.SetGlobalFloat("_SkyGradientOffset", assets.fogGradientOffset);
		Shader.SetGlobalFloat("_RimPower", assets.RimPower);
		Shader.SetGlobalFloat("_RimStrength", assets.RimStrength);
		Shader.SetGlobalColor("_RimColor", assets.RimColor);
		Shader.SetGlobalTexture("_ToonTex", assets.ToonRamp);
		Shader.SetGlobalFloat("_Outline", assets.Outline);
		Shader.SetGlobalColor("_OutlineColor", assets.OutlineColor);
		foreach (KeyValuePair<Material, Material> original2CloneMaterial in original2CloneMaterials)
		{
			if (assets.textureAndMaterialLinks.TryGetValue(original2CloneMaterial.Key, out Texture value))
			{
				original2CloneMaterial.Value.SetTexture("_MainTex", value);
			}
		}
		if (original2CloneMaterials.TryGetValue(assets.glowGold, out Material value2))
		{
			value2.SetColor("_MainColor", assets.glowGoldColor);
			value2.SetFloat("_Falloff", assets.glowGoldFalloff);
			if (cameraCulling == null)
			{
				cameraCulling = (UnityEngine.Object.FindObjectOfType(typeof(CameraCulling)) as CameraCulling);
			}
			cameraCulling.TransparentFXCullingDistance = assets.glowGoldFalloff;
		}
		if (background != null)
		{
			background.ResetBackground(assets.monumentPrefabs, assets.backgroundPrefabs);
		}
	}

	private IEnumerator ChangePropSequence(Theme theme)
	{
		WaitForSeconds yieldOp = new WaitForSeconds(12f);
		while (true)
		{
			yield return yieldOp;
			if (crtChangeColors != null)
			{
				StopCoroutine(crtChangeColors);
			}
			if (crtChangeDistort != null)
			{
				StopCoroutine(crtChangeDistort);
			}
			crtChangeColors = null;
			crtChangeDistort = null;
			seqPropsIdx = (seqPropsIdx + 1) % 7;
			switch (seqPropsIdx)
			{
			case 0:
				crtChangeDistort = StartCoroutine(ChangeDistort(theme, 0, 1));
				break;
			case 1:
				crtChangeDistort = StartCoroutine(ChangeDistort(theme, 1, 2));
				break;
			case 2:
				crtChangeDistort = StartCoroutine(ChangeDistort(theme, 2, 3));
				break;
			case 3:
				crtChangeColors = StartCoroutine(ChangeColors(theme, 0, 1));
				break;
			case 4:
				crtChangeDistort = StartCoroutine(ChangeDistort(theme, 3, 4));
				break;
			case 5:
				crtChangeDistort = StartCoroutine(ChangeDistort(theme, 4, 0));
				break;
			case 6:
				crtChangeColors = StartCoroutine(ChangeColors(theme, 1, 0));
				break;
			}
		}
	}

	private IEnumerator ChangeColors(Theme theme, int fromIdx, int toIdx)
	{
		Assets themeAsset = theme2assets[theme];
		LeanTween.value(base.gameObject, delegate(float eval)
		{
			Shader.SetGlobalColor("_SkyGradientTopColor", Color.Lerp(themeAsset.fogGradientTop[fromIdx], themeAsset.fogGradientTop[toIdx], eval));
			Shader.SetGlobalColor("_SkyGradientBottomColor", Color.Lerp(themeAsset.fogGradientBottom[fromIdx], themeAsset.fogGradientBottom[toIdx], eval));
			Shader.SetGlobalColor("_FogSilhouetteColor", Color.Lerp(themeAsset.fogSilhouetteColor[fromIdx], themeAsset.fogSilhouetteColor[toIdx], eval));
		}, 0f, 1f, 6f);
		yield break;
	}

	private IEnumerator ChangeDistort(Theme theme, int fromIdx, int toIdx)
	{
		Assets themeAsset = theme2assets[theme];
		LeanTween.value(base.gameObject, delegate(float eval)
		{
			Shader.SetGlobalColor("_Distort", Vector4.Lerp(themeAsset.distort[fromIdx], themeAsset.distort[toIdx], eval));
		}, 0f, 1f, 6f);
		yield break;
	}
}
