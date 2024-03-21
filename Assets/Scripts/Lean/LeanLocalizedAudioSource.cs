using UnityEngine;

namespace Lean
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("Lean/Localized Audio Source")]
	public class LeanLocalizedAudioSource : LeanLocalizedBehaviour
	{
		public bool AllowFallback;

		public AudioClip FallbackAudioClip;

		public override void UpdateTranslation(LeanTranslation translation)
		{
			AudioSource component = GetComponent<AudioSource>();
			if (translation != null)
			{
				component.clip = (translation.Object as AudioClip);
			}
			else if (AllowFallback)
			{
				component.clip = FallbackAudioClip;
			}
		}
	}
}
