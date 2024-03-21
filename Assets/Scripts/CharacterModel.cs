using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
	public SkinnedMeshRenderer model;

	public Transform meshHoverboard;

	public MeshRenderer meshBlobShadow;

	public Animation characterAnimation;

	public MeshRenderer shadow;

	public Transform spineTransform;

	public Transform shoulderTransform;

	public Transform jetpackCloudPositionL;

	public Transform jetpackCloudPositionR;

	public Transform feverCloudPosition;

	private SkinnedMeshRenderer[] models;

	private GameObject currentMenuBoard;

	private Dictionary<string, SkinnedMeshRenderer> modelLookupTable;

	private string[] modelNames;

	private Color overlayColor = Color.black;

	private bool blinking;

	public float blinkFrequency = 1.5f;

	private Animation _animationComponent;

	public string[] ModelNames => modelNames;

	public Color OverlayColor
	{
		get
		{
			return overlayColor;
		}
		set
		{
			overlayColor = value;
			for (int i = 0; i < models.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = models[i];
				skinnedMeshRenderer.sharedMaterial.SetColor("_OverlayColor", overlayColor);
			}
		}
	}

	public void ChangeModelFromPlayerInfo()
	{
		CharacterInfoData characterInfoData = DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.SelectedCharID];
		string modelname = characterInfoData.Modelname;
		CheckForCharactersToBeInSyncWithStaticData();
		ChangeCharacterModel(modelname);
	}

	public void Awake()
	{
		models = GetComponentsInChildren<SkinnedMeshRenderer>();
		modelNames = new string[models.Length];
		modelLookupTable = new Dictionary<string, SkinnedMeshRenderer>();
		for (int i = 0; i < models.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = models[i];
			string name = skinnedMeshRenderer.gameObject.name;
			modelNames[i] = name;
			modelLookupTable.Add(name, skinnedMeshRenderer);
		}
		if (!PlayerInfo.Instance.CharUnlocks[PlayerInfo.Instance.SelectedCharID])
		{
			PlayerInfo.Instance.SelectedCharID = "1";
		}
		CharacterInfoData characterInfoData = DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.SelectedCharID];
		string modelname = characterInfoData.Modelname;
		CheckForCharactersToBeInSyncWithStaticData();
		ChangeCharacterModel(modelname);
	}

	public void ChangeCharacterModel(string name)
	{
		StopIdleAnimations();
		if (modelLookupTable.TryGetValue(name, out SkinnedMeshRenderer value))
		{
			for (int i = 0; i < models.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = models[i];
				skinnedMeshRenderer.enabled = (skinnedMeshRenderer == value);
			}
		}
		if (value != null)
		{
			model = value;
		}
	}

	public Animation GetAnimation()
	{
		if (_animationComponent == null)
		{
			_animationComponent = GetComponentInChildren<Animation>();
		}
		return _animationComponent;
	}

	public GameObject GetHoverboardGameObject()
	{
		return null;
	}

	public void SetNewHoverboard(GameObject newBoard)
	{
		if (currentMenuBoard != null)
		{
			UnityEngine.Object.Destroy(currentMenuBoard);
		}
		currentMenuBoard = UnityEngine.Object.Instantiate(newBoard, meshHoverboard.transform.position, meshHoverboard.transform.rotation);
		currentMenuBoard.transform.parent = meshHoverboard.transform;
		MeshRenderer[] componentsInChildren = currentMenuBoard.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = true;
			meshRenderer.gameObject.layer = newBoard.layer;
		}
	}

	public void HideAllPowerups()
	{
	}

	public void HideBlobShadow()
	{
		meshBlobShadow.enabled = false;
	}

	public void StartBlink()
	{
		blinking = true;
		StartCoroutine(Blink());
	}

	private IEnumerator Blink()
	{
		while (blinking)
		{
			OverlayColor = pMath.Square(Time.time * blinkFrequency) * Color.white;
			yield return null;
		}
		OverlayColor = Color.black;
	}

	public void StopBlink()
	{
		blinking = false;
	}

	public void ResetBlink()
	{
		OverlayColor = Color.black;
	}

	public void StartIdleAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartIdleAnimations();
			}
		}
	}

	public void StartIdlePopupAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartIdlePopupAnimations();
			}
		}
	}

	public void StopIdleAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StopIdleAnimations();
			}
		}
	}

	public void PauseIdleAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.PauseIdleAnimations();
			}
		}
	}

	public void ResumeIdleAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.ResumeIdleAnimations();
			}
		}
	}

	public void StartUnlockAnimations()
	{
		if (!(model == null))
		{
			AvatarAnimations component = model.GetComponent<AvatarAnimations>();
			if (component != null)
			{
				component.StartUnlockAnimations();
			}
		}
	}

	private void CheckForCharactersToBeInSyncWithStaticData()
	{
	}
}
