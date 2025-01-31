using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMPro
{
	[Serializable]
	public class TextMeshProFont : ScriptableObject
	{
		public enum FontAssetTypes
		{
			None,
			SDF,
			Bitmap
		}

		public FontAssetTypes fontAssetType;

		[SerializeField]
		private FaceInfo m_fontInfo;

		public int fontHashCode;

		[SerializeField]
		public Texture2D atlas;

		[SerializeField]
		public Material material;

		public int materialHashCode;

		[SerializeField]
		private List<GlyphInfo> m_glyphInfoList;

		private Dictionary<int, GlyphInfo> m_characterDictionary;

		private Dictionary<int, KerningPair> m_kerningDictionary;

		[SerializeField]
		private KerningTable m_kerningInfo;

		[SerializeField]
		private KerningPair m_kerningPair;

		[SerializeField]
		private LineBreakingTable m_lineBreakingInfo;

		[SerializeField]
		public FontCreationSetting fontCreationSettings;

		[SerializeField]
		public bool propertiesChanged;

		private int[] m_characterSet;

		public float NormalStyle;

		public float BoldStyle = 0.75f;

		public float boldSpacing = 7f;

		public byte ItalicStyle = 35;

		public byte TabSize = 10;

		private byte m_oldTabSize;

		public FaceInfo fontInfo => m_fontInfo;

		public Dictionary<int, GlyphInfo> characterDictionary => m_characterDictionary;

		public Dictionary<int, KerningPair> kerningDictionary => m_kerningDictionary;

		public KerningTable kerningInfo => m_kerningInfo;

		public LineBreakingTable lineBreakingInfo => m_lineBreakingInfo;

		private void OnEnable()
		{
			if (m_characterDictionary == null)
			{
				ReadFontDefinition();
			}
		}

		private void OnDisable()
		{
		}

		private void OnValidate()
		{
			if (m_oldTabSize != TabSize)
			{
				m_oldTabSize = TabSize;
				ReadFontDefinition();
			}
		}

		public void AddFaceInfo(FaceInfo faceInfo)
		{
			m_fontInfo = faceInfo;
		}

		public void AddGlyphInfo(GlyphInfo[] glyphInfo)
		{
			m_glyphInfoList = new List<GlyphInfo>();
			int num = glyphInfo.Length;
			m_fontInfo.CharacterCount = num;
			m_characterSet = new int[num];
			for (int i = 0; i < num; i++)
			{
				GlyphInfo glyphInfo2 = new GlyphInfo();
				glyphInfo2.id = glyphInfo[i].id;
				glyphInfo2.x = glyphInfo[i].x;
				glyphInfo2.y = glyphInfo[i].y;
				glyphInfo2.width = glyphInfo[i].width;
				glyphInfo2.height = glyphInfo[i].height;
				glyphInfo2.xOffset = glyphInfo[i].xOffset;
				glyphInfo2.yOffset = glyphInfo[i].yOffset + m_fontInfo.Padding;
				glyphInfo2.xAdvance = glyphInfo[i].xAdvance;
				m_glyphInfoList.Add(glyphInfo2);
				m_characterSet[i] = glyphInfo2.id;
			}
			m_glyphInfoList = (from s in m_glyphInfoList
				orderby s.id
				select s).ToList();
		}

		public void AddKerningInfo(KerningTable kerningTable)
		{
			m_kerningInfo = kerningTable;
		}

		public void ReadFontDefinition()
		{
			if (m_fontInfo == null)
			{
				return;
			}
			m_characterDictionary = new Dictionary<int, GlyphInfo>();
			foreach (GlyphInfo glyphInfo2 in m_glyphInfoList)
			{
				if (!m_characterDictionary.ContainsKey(glyphInfo2.id))
				{
					m_characterDictionary.Add(glyphInfo2.id, glyphInfo2);
				}
			}
			GlyphInfo glyphInfo = new GlyphInfo();
			if (m_characterDictionary.ContainsKey(32))
			{
				m_characterDictionary[32].width = m_characterDictionary[32].xAdvance;
				m_characterDictionary[32].height = m_fontInfo.Ascender - m_fontInfo.Descender;
				m_characterDictionary[32].yOffset = m_fontInfo.Ascender;
			}
			else
			{
				glyphInfo = new GlyphInfo();
				glyphInfo.id = 32;
				glyphInfo.x = 0f;
				glyphInfo.y = 0f;
				glyphInfo.width = m_fontInfo.Ascender / 5f;
				glyphInfo.height = m_fontInfo.Ascender - m_fontInfo.Descender;
				glyphInfo.xOffset = 0f;
				glyphInfo.yOffset = m_fontInfo.Ascender;
				glyphInfo.xAdvance = m_fontInfo.PointSize / 4f;
				m_characterDictionary.Add(32, glyphInfo);
			}
			if (!m_characterDictionary.ContainsKey(160))
			{
				glyphInfo = GlyphInfo.Clone(m_characterDictionary[32]);
				m_characterDictionary.Add(160, glyphInfo);
			}
			if (!m_characterDictionary.ContainsKey(10))
			{
				glyphInfo = new GlyphInfo();
				glyphInfo.id = 10;
				glyphInfo.x = 0f;
				glyphInfo.y = 0f;
				glyphInfo.width = 10f;
				glyphInfo.height = m_characterDictionary[32].height;
				glyphInfo.xOffset = 0f;
				glyphInfo.yOffset = m_characterDictionary[32].yOffset;
				glyphInfo.xAdvance = 0f;
				m_characterDictionary.Add(10, glyphInfo);
				if (!m_characterDictionary.ContainsKey(13))
				{
					m_characterDictionary.Add(13, glyphInfo);
				}
			}
			if (!m_characterDictionary.ContainsKey(9))
			{
				glyphInfo = new GlyphInfo();
				glyphInfo.id = 9;
				glyphInfo.x = m_characterDictionary[32].x;
				glyphInfo.y = m_characterDictionary[32].y;
				glyphInfo.width = m_characterDictionary[32].width * (float)(int)TabSize + (m_characterDictionary[32].xAdvance - m_characterDictionary[32].width) * (float)(TabSize - 1);
				glyphInfo.height = m_characterDictionary[32].height;
				glyphInfo.xOffset = m_characterDictionary[32].xOffset;
				glyphInfo.yOffset = m_characterDictionary[32].yOffset;
				glyphInfo.xAdvance = m_characterDictionary[32].xAdvance * (float)(int)TabSize;
				m_characterDictionary.Add(9, glyphInfo);
			}
			m_fontInfo.TabWidth = m_characterDictionary[9].xAdvance;
			m_kerningDictionary = new Dictionary<int, KerningPair>();
			List<KerningPair> kerningPairs = m_kerningInfo.kerningPairs;
			for (int i = 0; i < kerningPairs.Count; i++)
			{
				KerningPair kerningPair = kerningPairs[i];
				KerningPairKey kerningPairKey = new KerningPairKey(kerningPair.AscII_Left, kerningPair.AscII_Right);
				if (!m_kerningDictionary.ContainsKey(kerningPairKey.key))
				{
					m_kerningDictionary.Add(kerningPairKey.key, kerningPair);
				}
			}
			m_lineBreakingInfo = new LineBreakingTable();
			TextAsset textAsset = Resources.Load("LineBreaking Leading Characters", typeof(TextAsset)) as TextAsset;
			if (textAsset != null)
			{
				m_lineBreakingInfo.leadingCharacters = GetCharacters(textAsset);
			}
			TextAsset textAsset2 = Resources.Load("LineBreaking Following Characters", typeof(TextAsset)) as TextAsset;
			if (textAsset2 != null)
			{
				m_lineBreakingInfo.followingCharacters = GetCharacters(textAsset2);
			}
			fontHashCode = TMP_TextUtilities.GetSimpleHashCode(base.name);
			materialHashCode = TMP_TextUtilities.GetSimpleHashCode(material.name);
		}

		private Dictionary<int, char> GetCharacters(TextAsset file)
		{
			Dictionary<int, char> dictionary = new Dictionary<int, char>();
			string text = file.text;
			foreach (char c in text)
			{
				if (!dictionary.ContainsKey(c))
				{
					dictionary.Add(c, c);
				}
			}
			return dictionary;
		}
	}
}
