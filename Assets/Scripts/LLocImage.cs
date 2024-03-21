using Lean;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class LLocImage : LeanLocalizedBehaviour
{
	public bool NeedNativeSize = true;

	private Image image;

	private bool isCreatedVariables;

	public Image Image => image ?? (image = GetComponentsInChildren<Image>(includeInactive: true)[0]);

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public override void UpdateTranslation(LeanTranslation translation)
	{
		if (translation == null)
		{
			return;
		}
		string text = translation.Text;
		string language = translation.Language;
		Sprite[] source = Resources.LoadAll<Sprite>(translation.Text);
		Sprite sprite = (from spr in source
			where spr.name.Equals(PhraseName)
			select spr).FirstOrDefault();
		if (translation != null)
		{
			Image.sprite = sprite;
			if (NeedNativeSize)
			{
				Image.SetNativeSize();
			}
		}
	}
}
