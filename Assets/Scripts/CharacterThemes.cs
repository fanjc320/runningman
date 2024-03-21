using System.Collections.Generic;

public class CharacterThemes
{
	public static readonly Dictionary<Characters.CharacterType, List<CharacterTheme>> characterCustomThemes = new Dictionary<Characters.CharacterType, List<CharacterTheme>>();

	public static CharacterTheme? GetThemeForCharacter(Characters.CharacterType charType, int index)
	{
		if (index == 0)
		{
			return null;
		}
		List<CharacterTheme> list = TryGetCustomThemesForChar(charType);
		if (list != null && list.Count >= index)
		{
			return list[index - 1];
		}
		return null;
	}

	public static List<CharacterTheme> TryGetCustomThemesForChar(Characters.CharacterType charType)
	{
		characterCustomThemes.TryGetValue(charType, out List<CharacterTheme> value);
		if (value == null)
		{
		}
		return value;
	}
}
