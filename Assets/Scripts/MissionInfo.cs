using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class MissionInfo : ScriptableObject
{
	[HideInInspector]
	[SerializeField]
	public string sheetName = string.Empty;

	[HideInInspector]
	[SerializeField]
	public string worksheetName = string.Empty;

	public MissionInfoData[] dataArray;

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

	public MissionInfoData this[string key] => (from s in dataArray
		where s.ID == key
		select s).First();

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new MissionInfoData[0];
		}
	}

	public void InitMappers()
	{
		dataArray.All(delegate(MissionInfoData data)
		{
			data.InitMapper();
			return true;
		});
	}
}
