using System.Collections.Generic;
using UnityEngine;

public class ThemeShiftMeshes : MonoBehaviour
{
	private HashSet<MeshFilter> meshFilters = new HashSet<MeshFilter>();

	private void Awake()
	{
		MeshFilter[] componentsInChildren = base.transform.GetComponentsInChildren<MeshFilter>(includeInactive: true);
		MeshFilter[] array = componentsInChildren;
		foreach (MeshFilter item in array)
		{
			meshFilters.Add(item);
		}
	}

	private void Start()
	{
		ThemeManager.Instance.OnChangeTheme += OnChangeTheme;
	}

	private void OnDestroy()
	{
		ThemeManager.Instance.OnChangeTheme -= OnChangeTheme;
	}

	public void OnChangeTheme(Theme theme)
	{
		if (theme == null)
		{
			return;
		}
		string name = ThemeManager.Instance.Theme.Name;
		if (meshFilters != null)
		{
			foreach (MeshFilter meshFilter in meshFilters)
			{
				if (meshFilter != null && !(meshFilter.sharedMesh == null) && meshFilter.sharedMesh.name != string.Empty)
				{
					string name2 = meshFilter.sharedMesh.name;
					string str = name2.Substring(name2.IndexOf("_") + 1);
					string key = name + "_" + str;
					if (ThemeAssets.Instance.environmentModelMeshes.TryGetValue(key, out Mesh value))
					{
						meshFilter.mesh = value;
					}
				}
			}
		}
		SkinnedMeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			string name3 = skinnedMeshRenderer.sharedMesh.name;
			string str2 = name3.Substring(name3.IndexOf("_") + 1);
			string key2 = name + "_" + str2;
			if (ThemeAssets.Instance.characterModelMeshes.TryGetValue(key2, out Mesh value2))
			{
				skinnedMeshRenderer.sharedMesh = value2;
			}
		}
	}
}
