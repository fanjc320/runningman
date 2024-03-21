using System;

public class ThemeManager
{
	public delegate void OnChangeThemeDelegate(Theme theme);

	private Theme theme;

	private DateTime _themeExpirationDate;

	public static ThemeManager instance;

	public Theme Theme
	{
		get
		{
			return theme;
		}
		set
		{
			theme = value;
			if (this.OnChangeTheme != null)
			{
				this.OnChangeTheme(theme);
			}
		}
	}

	public DateTime themeExpirationDate
	{
		get
		{
			return _themeExpirationDate;
		}
		set
		{
			_themeExpirationDate = value;
		}
	}

	public static ThemeManager Instance => instance ?? (instance = new ThemeManager());

	public event OnChangeThemeDelegate OnChangeTheme;

	private ThemeManager()
	{
	}

	public void ForceRefresh()
	{
		Theme = theme;
	}
}
