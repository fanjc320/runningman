using System;
using System.Collections.Generic;

namespace Lean
{
	[Serializable]
	public class LeanPhrase
	{
		public string Name;

		public List<LeanTranslation> Translations = new List<LeanTranslation>();

		public LeanTranslation FindTranslation(string language)
		{
			return Translations.Find((LeanTranslation t) => t.Language == language);
		}

		public LeanTranslation AddTranslation(string language)
		{
			LeanTranslation leanTranslation = FindTranslation(language);
			if (leanTranslation == null)
			{
				leanTranslation = new LeanTranslation();
				leanTranslation.Language = language;
				Translations.Add(leanTranslation);
			}
			return leanTranslation;
		}
	}
}
