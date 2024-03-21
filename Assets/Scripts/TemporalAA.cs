using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TemporalAA : MonoBehaviour
{
	public Shader shader;

	private Material mat;

	private bool initialized;

	private bool usingRT1 = true;

	private RenderTexture rt1;

	private RenderTexture rt2;

	private Matrix4x4 prevViewProj;

	private int JitterIdx;

	private float[] JitterOffsetX = new float[2]
	{
		-0.5f,
		0f
	};

	private float[] JitterOffsetY = new float[2]
	{
		0f,
		0.5f
	};

	public Vector4 vParams = new Vector4(0.8f, 0f, 24f, 32f);

	private Matrix4x4 getViewProjMatrix()
	{
		return GL.GetGPUProjectionMatrix(GetComponent<Camera>().projectionMatrix, renderIntoTexture: false) * GetComponent<Camera>().worldToCameraMatrix;
	}

	private void Start()
	{
		mat = new Material(shader);
		mat.hideFlags = HideFlags.HideAndDontSave;
	}

	private void OnEnable()
	{
		prevViewProj = getViewProjMatrix();
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		initialized = false;
		usingRT1 = true;
	}

	private void OnDisable()
	{
		UnityEngine.Object.Destroy(rt1);
		rt1 = null;
		UnityEngine.Object.Destroy(rt2);
		rt2 = null;
	}

	private float Halton(int Index, int Base)
	{
		float num = 0f;
		float num2 = 1f / (float)Base;
		float num3 = num2;
		while (Index > 0)
		{
			num += (float)(Index % Base) * num3;
			Index /= Base;
			num3 *= num2;
		}
		return num;
	}

	private void Update()
	{
		JitterIdx = (JitterIdx + 1) % 8;
		Matrix4x4 projectionMatrix = GetComponent<Camera>().projectionMatrix;
		projectionMatrix[0, 2] += (Halton(JitterIdx + 1, 2) - 0.5f) / (float)Screen.width;
		projectionMatrix[1, 2] += (Halton(JitterIdx + 1, 3) - 0.5f) / (float)Screen.height;
		GetComponent<Camera>().projectionMatrix = projectionMatrix;
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (rt1 == null)
		{
			rt1 = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
			rt1.Create();
		}
		if (rt2 == null)
		{
			rt2 = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
			rt2.Create();
		}
		GetComponent<Camera>().ResetProjectionMatrix();
		Matrix4x4 viewProjMatrix = getViewProjMatrix();
		mat.SetMatrix("combinedVP", prevViewProj * Matrix4x4.Inverse(viewProjMatrix));
		prevViewProj = viewProjMatrix;
		RenderTexture renderTexture = null;
		RenderTexture renderTexture2 = null;
		if (usingRT1)
		{
			renderTexture2 = rt1;
			renderTexture = rt2;
			usingRT1 = false;
		}
		else
		{
			renderTexture2 = rt2;
			renderTexture = rt1;
			usingRT1 = true;
		}
		if (!initialized)
		{
			renderTexture = src;
			initialized = true;
		}
		mat.SetVector("texel", new Vector4(1f / (float)src.width, 1f / (float)src.height, 0f, 0f));
		mat.SetVector("vParams", new Vector4(vParams.x, 0f, 1f / vParams.z, 1f / vParams.w));
		mat.SetTexture("_MainTex1", renderTexture);
		renderTexture2.DiscardContents();
		Graphics.Blit(src, renderTexture2, mat);
		Graphics.Blit(renderTexture2, dest);
	}
}
