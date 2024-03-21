using UnityEngine;

public class ThemeShiftTextures : MonoBehaviour
{
	private Renderer[] renderes;

	private void Start()
	{
	}

	private void RelinkMaterialWithThemeAssets()
	{
		renderes = base.gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
		Renderer[] array = renderes;
		foreach (Renderer renderer in array)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if (ThemeAssets.Instance != null && sharedMaterials[j] != null && ThemeAssets.Instance.original2CloneMaterials.TryGetValue(sharedMaterials[j], out Material _))
				{
					sharedMaterials[j] = ThemeAssets.Instance.ReplaceMaterial(sharedMaterials[j]);
				}
			}
			renderer.materials = sharedMaterials;
		}
	}
}
