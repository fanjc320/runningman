using System;
using UnityEngine;

namespace Lean
{
	public abstract class LeanLocalizedBehaviour : MonoBehaviour
	{
		[LeanPhraseName]
		public string PhraseName;

		public abstract void UpdateTranslation(LeanTranslation translation);

		public void SetPhraseName(string newPhraseName)
		{
			if (PhraseName != newPhraseName)
			{
				PhraseName = newPhraseName;
				UpdateLocalization();
			}
		}

		public void UpdateLocalization()
		{
			UpdateTranslation(LeanLocalization.GetTranslation(PhraseName));
		}

		protected virtual void OnEnable()
		{
			LeanLocalization.OnLocalizationChanged = (Action)Delegate.Combine(LeanLocalization.OnLocalizationChanged, new Action(UpdateLocalization));
			UpdateLocalization();
		}

		protected virtual void OnDisable()
		{
			LeanLocalization.OnLocalizationChanged = (Action)Delegate.Remove(LeanLocalization.OnLocalizationChanged, new Action(UpdateLocalization));
		}
	}
}
