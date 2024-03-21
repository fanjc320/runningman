using UnityEngine;

public class ThemeToggler : MonoBehaviour
{
	private Theme theme;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.T))
		{
			if (theme == Theme.NORMAL)
			{
				theme = Theme.NORMAL;
				ThemeManager.Instance.Theme = theme;
			}
			else
			{
				theme = Theme.NORMAL;
				ThemeManager.Instance.Theme = theme;
			}
		}
	}
}
