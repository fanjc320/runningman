using UnityEngine;

public class EditorLoadFromLazyLoadScene : MonoBehaviour
{
	private void Start()
	{
		if (!Game.HasLoaded)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene");
		}
	}

	private void Update()
	{
	}
}
