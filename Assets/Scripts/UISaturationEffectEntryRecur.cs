using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UISaturationEffectEntryRecur : MonoBehaviour
{
	public Material Material;

	public Image[] ExceptImages;

	public Color Saturation;

	private Color prevSaturation;

	private UISaturationEffect[] effects;

	private void Awake()
	{
		Image[] componentsInChildren = base.transform.GetComponentsInChildren<Image>();
		effects = componentsInChildren.Except(ExceptImages).Select(delegate(Image image)
		{
			image.material = Material;
			return image.gameObject.AddComponent<UISaturationEffect>();
		}).ToArray();
		prevSaturation = Saturation;
	}

	private void LateUpdate()
	{
		if (prevSaturation != Saturation)
		{
			updateEffects();
			prevSaturation = Saturation;
		}
	}

	private void updateEffects()
	{
		for (int i = 0; effects.Length > i; i++)
		{
			effects[i].Saturation = Saturation;
		}
	}
}
