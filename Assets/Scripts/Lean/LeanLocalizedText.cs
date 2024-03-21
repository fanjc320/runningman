using UnityEngine;
using UnityEngine.UI;

namespace Lean
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Text))]
	[AddComponentMenu("Lean/Localized Text")]
	public class LeanLocalizedText : LeanLocalizedBehaviour
	{
		public bool AllowFallback;

		public string FallbackText;

		public override void UpdateTranslation(LeanTranslation translation)
		{
			Text component = GetComponent<Text>();
			if (translation != null)
			{
				component.text = translation.Text;
			}
			else if (AllowFallback)
			{
				component.text = FallbackText;
			}
		}
	}
}
