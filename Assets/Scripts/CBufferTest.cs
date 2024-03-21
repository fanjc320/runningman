using UnityEngine;
using UnityEngine.Rendering;

public class CBufferTest : MonoBehaviour
{
	private CommandBuffer cBuffer;

	private Renderer thisRenderer;

	private Material thisMaterial;

	public void OnEnable()
	{
		Cleanup();
	}

	public void OnDisable()
	{
		Cleanup();
	}

	private void Cleanup()
	{
		if (cBuffer != null && null != Camera.main)
		{
			Camera.main.RemoveCommandBuffer(CameraEvent.AfterEverything, cBuffer);
			cBuffer = null;
			thisRenderer = null;
			thisMaterial = null;
		}
	}

	public void OnWillRenderObject()
	{
		if (!(null == Camera.current) && cBuffer == null)
		{
			if (null == thisRenderer)
			{
				thisRenderer = GetComponentsInChildren<Renderer>(includeInactive: true)[0];
			}
			cBuffer = new CommandBuffer();
			int nameID = Shader.PropertyToID("_GrabBlurTexture");
			cBuffer.GetTemporaryRT(nameID, -1, -1, 24, FilterMode.Bilinear);
			cBuffer.Blit(BuiltinRenderTextureType.CurrentActive, nameID);
			cBuffer.SetGlobalTexture("_GrabBlurTexture", nameID);
			cBuffer.DrawRenderer(thisRenderer, thisRenderer.material);
			Camera.main.AddCommandBuffer(CameraEvent.AfterEverything, cBuffer);
			thisRenderer.enabled = true;
			thisRenderer.enabled = false;
		}
	}
}
