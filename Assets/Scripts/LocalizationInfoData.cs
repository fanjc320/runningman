using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

[Serializable]
public class LocalizationInfoData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string commentscene;

	[SerializeField]
	private string commentdesc;

	[SerializeField]
	private string ko;

	[SerializeField]
	private string en;

	[SerializeField]
	private string zh;

	private Dictionary<string, PropertyInfo> locValueMap = new Dictionary<string, PropertyInfo>();

	[ExposeProperty]
	public string ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	[ExposeProperty]
	public string Commentscene
	{
		get
		{
			return commentscene;
		}
		set
		{
			commentscene = value;
		}
	}

	[ExposeProperty]
	public string Commentdesc
	{
		get
		{
			return commentdesc;
		}
		set
		{
			commentdesc = value;
		}
	}

	[ExposeProperty]
	public string Ko
	{
		get
		{
			return ko;
		}
		set
		{
			ko = value;
		}
	}

	[ExposeProperty]
	public string En
	{
		get
		{
			return en;
		}
		set
		{
			en = value;
		}
	}

	[ExposeProperty]
	public string Zh
	{
		get
		{
			return zh;
		}
		set
		{
			zh = value;
		}
	}

	public string this[string locKey] => locValueMap[locKey].GetValue(this, null) as string;

	public void InitMapper()
	{
		StringBuilder sbFieldName = new StringBuilder();
		DataContainer.LocaleIdentifier.All(delegate(string localeID)
		{
			sbFieldName.Append(localeID);
			sbFieldName[0] = localeID.ToUpper()[0];
			string name = sbFieldName.ToString();
			locValueMap[localeID] = GetType().GetProperty(name);
			return true;
		});
	}
}
