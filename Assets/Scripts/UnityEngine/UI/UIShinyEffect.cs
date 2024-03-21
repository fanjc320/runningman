using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/UIShinyEffect", 25)]
	public class UIShinyEffect : BaseMeshEffect
	{
		private static Vector3[] kVertexRect = new Vector3[6]
		{
			new Vector3(1f, 1f, 1f),
			new Vector3(1f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 1f, 1f),
			new Vector3(1f, 1f, 1f)
		};

		private List<UIVertex> uiVertices = new List<UIVertex>();

		protected UIShinyEffect()
		{
		}

		private void ModifyVertices(List<UIVertex> verts)
		{
			int count = verts.Count;
			for (int i = 0; kVertexRect.Length > i; i++)
			{
				int num = i % kVertexRect.Length;
				UIVertex value = verts[i];
				value.normal = kVertexRect[num];
				value.uv1 = Vector2.zero;
				verts[i] = value;
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				uiVertices.Clear();
				vh.GetUIVertexStream(uiVertices);
				ModifyVertices(uiVertices);
				vh.AddUIVertexTriangleStream(uiVertices);
			}
		}
	}
}
