using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class MarketInfo : ScriptableObject
{
	[HideInInspector]
	[SerializeField]
	public string sheetName = string.Empty;

	[HideInInspector]
	[SerializeField]
	public string worksheetName = string.Empty;

	public MarketInfoData[] dataArray;

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

	public MarketInfoData this[int index] => dataArray[index];

	public MarketInfoData this[string key] => (from s in dataArray
		where s.ID == key
		select s).First();

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new MarketInfoData[0];
		}
	}

	public void InitMappers()
	{
	}
}
