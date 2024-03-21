using UnityEngine;
using UnityEngine.UI;

namespace Lean
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("Lean/Localized Image")]
	public class LeanLocalizedImage : LeanLocalizedBehaviour
	{
		public bool AllowFallback;

		public Sprite FallbackSprite;

		public override void UpdateTranslation(LeanTranslation translation)
		{
			Image component = GetComponent<Image>();
			if (translation != null)
			{
				component.sprite = (translation.Object as Sprite);
			}
			else if (AllowFallback)
			{
				component.sprite = FallbackSprite;
			}
		}
	}
}
