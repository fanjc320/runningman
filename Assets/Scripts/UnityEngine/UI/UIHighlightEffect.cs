using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/UIHighlightEffect", 27)]
	public class UIHighlightEffect : BaseMeshEffect
	{
		private Vector2 paramsUV1 = new Vector2(1f, 0f);

		private List<UIVertex> uiVertices = new List<UIVertex>();

		protected UIHighlightEffect()
		{
		}

		private void ModifyVertices(List<UIVertex> verts)
		{
			int count = verts.Count;
			for (int i = 0; count > i; i++)
			{
				UIVertex value = verts[i];
				value.normal = Vector3.zero;
				value.uv1 = paramsUV1;
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
