using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/UIEmptyEffect", 28)]
	public class UIEmptyEffect : BaseMeshEffect
	{
		private List<UIVertex> uiVertices = new List<UIVertex>();

		protected UIEmptyEffect()
		{
		}

		private void ModifyVertices(List<UIVertex> verts)
		{
			int count = verts.Count;
			for (int i = 0; count > i; i++)
			{
				UIVertex value = verts[i];
				value.normal = Vector3.zero;
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
