using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sources
{
	public class TextWithIcon : Text
	{
		private Image icon;

		private Vector3 iconPosition;

		private List<Image> icons;

		private List<Vector2> positions = new List<Vector2>();

		private float _fontHeight;

		private float _fontWidth;

		public float ImageScale;

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			base.OnPopulateMesh(toFill);
			List<UIVertex> list = new List<UIVertex>();
			toFill.GetUIVertexStream(list);
			positions.Clear();
			List<int> indexes = getIndexes(this);
			icons = GetComponentsInChildren<Image>().ToList();
			for (int i = 0; i < indexes.Count; i++)
			{
				Vector3[] array = new Vector3[4];
				int num = indexes[i] * 4;
				int num2 = num + 4;
				int num3 = 0;
				for (int j = num; j < num2; j++)
				{
					ref Vector3 reference = ref array[num3];
					UIVertex uIVertex = list[j];
					reference = uIVertex.position;
					num3++;
					if (num3 == 4)
					{
						num3 = 0;
					}
				}
				positions.Add(CenterOfVectors(array));
				_fontHeight = Vector3.Distance(array[0], array[3]);
				_fontWidth = Vector3.Distance(array[0], array[1]);
			}
		}

		private void Update()
		{
			for (int i = 0; i < icons.Count; i++)
			{
				icons[i].rectTransform.anchoredPosition = positions[i];
				icons[i].rectTransform.sizeDelta = new Vector2(_fontWidth * ImageScale, _fontHeight * ImageScale);
			}
		}

		private Vector3 CenterOfVectors(Vector3[] vectors)
		{
			Vector3 vector = Vector3.zero;
			if (vectors == null || vectors.Length == 0)
			{
				return vector;
			}
			foreach (Vector3 b in vectors)
			{
				vector += b;
			}
			return vector / vectors.Length;
		}

		private List<int> getIndexes(Text textObject)
		{
			List<int> list = new List<int>();
			IEnumerator enumerator = Regex.Matches(textObject.text, "\\$").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Match match = (Match)enumerator.Current;
					list.Add(match.Index);
				}
				return list;
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
