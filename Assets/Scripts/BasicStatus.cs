using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class BasicStatus : ScriptableObject
{
	[HideInInspector]
	[SerializeField]
	public string sheetName = string.Empty;

	[HideInInspector]
	[SerializeField]
	public string worksheetName = string.Empty;

	public BasicStatusData[] dataArray;

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

	public BasicStatusData this[int index] => dataArray[index];

	public BasicStatusData this[string key] => (from s in dataArray
		where s.ID == key
		select s).First();

	private void OnEnable()
	{
		if (dataArray == null)
		{
			dataArray = new BasicStatusData[0];
		}
	}

	public void InitMappers()
	{
	}
}
