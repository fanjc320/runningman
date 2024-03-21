using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Bonusbox : ScriptableObject
{
	[HideInInspector]
	[SerializeField]
	public string sheetName = string.Empty;

	[HideInInspector]
	[SerializeField]
	public string worksheetName = string.Empty;

	public BonusboxData[] dataArray;

	private int probTotal;

	[ExposeProperty]
	public string SheetName
	{
		get
		{
			return sheetName;
		}
		set
		{
			sheetName = value;
		}
	}

	[ExposeProperty]
	public string WorksheetName
	{
		get
		{
			return worksheetName;
		}
		set
		{
			worksheetName = value;
		}
	}

	public int ProbTotal => probTotal;

	public BonusboxData this[int index] => dataArray[index];

	public BonusboxData this[string key] => (from s in dataArray
		where s.ID == key
		select s).First();

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new BonusboxData[0];
		}
	}

	public BonusboxData Dice(float randValue)
	{
		int num = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < dataArray.Length; i++)
		{
			if (!dataArray[i].Type.Equals("token"))
			{
				list.Add(i);
				num += dataArray[i].Probability;
				continue;
			}
			string key = DataContainer.Instance.TokenRelativeCharacterID[DataContainer.Instance.TokenTableRaw[dataArray[i].Pvalue].ID];
			if (!PlayerInfo.Instance.CharUnlocks[key])
			{
				list.Add(i);
				num += dataArray[i].Probability;
			}
		}
		float num2 = 0f;
		for (int j = 0; j < list.Count; j++)
		{
			if (randValue < (num2 += (float)dataArray[list[j]].Probability / (float)num))
			{
				return dataArray[list[j]];
			}
		}
		return null;
	}

	public void InitMappers()
	{
		probTotal = dataArray.Sum((BonusboxData s) => s.Probability);
	}
}
