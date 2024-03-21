using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Saturation", 26)]
	public class UISaturationEffect : BaseMeshEffect
	{
		private Vector2 paramsUV1 = new Vector2(0f, 1f);

		[SerializeField]
		private Color m_Saturation = new Color(0f, 0f, 0f, 0f);

		private List<UIVertex> uiVertices = new List<UIVertex>();

		public Color Saturation
		{
			get
			{
				return m_Saturation;
			}
			set
			{
				m_Saturation = value;
				if (null != base.graphic)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		private void ModifyVertices(List<UIVertex> verts)
		{
			if (IsActive())
			{
				int count = verts.Count;
				for (int i = 0; verts.Count > i; i++)
				{
					UIVertex value = verts[i];
					value.normal.x = m_Saturation.r;
					value.normal.y = m_Saturation.g;
					value.normal.z = 0f;
					value.uv1 = paramsUV1;
					verts[i] = value;
				}
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
