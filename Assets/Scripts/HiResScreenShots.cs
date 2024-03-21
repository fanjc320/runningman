using System;
using System.IO;
using UnityEngine;

public class HiResScreenShots : MonoBehaviour
{
	public int resWidth = 2550;

	public int resHeight = 3300;

	private bool takeHiResShot;

	public static string ScreenShotName(int width, int height)
	{
		return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png", Application.dataPath, width, height, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}

	public void TakeHiResShot()
	{
		takeHiResShot = true;
	}

	private void LateUpdate()
	{
		takeHiResShot |= UnityEngine.Input.GetKeyDown("k");
		if (takeHiResShot)
		{
			RenderTexture renderTexture = new RenderTexture(resWidth, resHeight, 24);
			GetComponent<Camera>().targetTexture = renderTexture;
			Texture2D texture2D = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, mipChain: false);
			GetComponent<Camera>().Render();
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, resWidth, resHeight), 0, 0);
			GetComponent<Camera>().targetTexture = null;
			RenderTexture.active = null;
			UnityEngine.Object.Destroy(renderTexture);
			byte[] bytes = texture2D.EncodeToPNG();
			string path = ScreenShotName(resWidth, resHeight);
			File.WriteAllBytes(path, bytes);
			takeHiResShot = false;
		}
	}
}
