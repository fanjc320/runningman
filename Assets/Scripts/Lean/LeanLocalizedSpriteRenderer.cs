using UnityEngine;

namespace Lean
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(SpriteRenderer))]
	[AddComponentMenu("Lean/Localized Sprite Renderer")]
	public class LeanLocalizedSpriteRenderer : LeanLocalizedBehaviour
	{
		public bool AllowFallback;

		public Sprite FallbackSprite;

		public override void UpdateTranslation(LeanTranslation translation)
		{
			SpriteRenderer component = GetComponent<SpriteRenderer>();
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
