using System;

public class Theme
{
	public static Theme NORMAL = new Theme("normal");

	private string name;

	public string Name => name;

	public TimeSpan TimeToExpire
	{
		get
		{
			DateTime d = ThemeManager.Instance.themeExpirationDate;
			if (d == DateTime.MinValue)
			{
				d = DateTime.UtcNow;
			}
			return d - DateTime.UtcNow;
		}
	}

	private Theme(string name)
	{
		this.name = name;
	}

	public static Theme FindByName(string name)
	{
		if (name == NORMAL.name)
		{
			return NORMAL;
		}
		return null;
	}

	public override string ToString()
	{
		return name;
	}
}
