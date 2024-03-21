using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class LocalizationInfo : ScriptableObject
{
	[HideInInspector]
	[SerializeField]
	public string sheetName = string.Empty;

	[HideInInspector]
	[SerializeField]
	public string worksheetName = string.Empty;

	public LocalizationInfoData[] dataArray;

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

	public LocalizationInfoData this[int index] => dataArray[index];

	public LocalizationInfoData this[string key] => (from s in dataArray
		where s.ID == key
		select s).First();

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new LocalizationInfoData[0];
		}
	}

	public void InitMappers()
	{
		dataArray.All(delegate(LocalizationInfoData data)
		{
			data.InitMapper();
			return true;
		});
	}
}
