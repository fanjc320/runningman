using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MissionInfoData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string name1loc;

	[SerializeField]
	private string desc1loc;

	[SerializeField]
	private string missionnameimagepath;

	[SerializeField]
	private string type;

	[SerializeField]
	private string goaltype;

	[SerializeField]
	private string goalvalue;

	[SerializeField]
	private string[] goalconditionalkeys = new string[0];

	[SerializeField]
	private string[] goalconditionalvalues = new string[0];

	[SerializeField]
	private string[] presentkeys = new string[0];

	[SerializeField]
	private int[] presentvalues = new int[0];

	[SerializeField]
	private string platform;

	[SerializeField]
	private string comment;

	public Dictionary<string, string> GoalConditions;

	public Dictionary<string, int> PresentAttribute;

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
	public string Name1loc
	{
		get
		{
			return name1loc;
		}
		set
		{
			name1loc = value;
		}
	}

	[ExposeProperty]
	public string Desc1loc
	{
		get
		{
			return desc1loc;
		}
		set
		{
			desc1loc = value;
		}
	}

	[ExposeProperty]
	public string Missionnameimagepath
	{
		get
		{
			return missionnameimagepath;
		}
		set
		{
			missionnameimagepath = value;
		}
	}

	[ExposeProperty]
	public string Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	[ExposeProperty]
	public string Goaltype
	{
		get
		{
			return goaltype;
		}
		set
		{
			goaltype = value;
		}
	}

	[ExposeProperty]
	public string Goalvalue
	{
		get
		{
			return goalvalue;
		}
		set
		{
			goalvalue = value;
		}
	}

	[ExposeProperty]
	public string[] Goalconditionalkeys
	{
		get
		{
			return goalconditionalkeys;
		}
		set
		{
			goalconditionalkeys = value;
		}
	}

	[ExposeProperty]
	public string[] Goalconditionalvalues
	{
		get
		{
			return goalconditionalvalues;
		}
		set
		{
			goalconditionalvalues = value;
		}
	}

	[ExposeProperty]
	public string[] Presentkeys
	{
		get
		{
			return presentkeys;
		}
		set
		{
			presentkeys = value;
		}
	}

	[ExposeProperty]
	public int[] Presentvalues
	{
		get
		{
			return presentvalues;
		}
		set
		{
			presentvalues = value;
		}
	}

	[ExposeProperty]
	public string Platform
	{
		get
		{
			return platform;
		}
		set
		{
			platform = value;
		}
	}

	[ExposeProperty]
	public string Comment
	{
		get
		{
			return comment;
		}
		set
		{
			comment = value;
		}
	}

	public void InitMapper()
	{
		GoalConditions = Enumerable.Range(0, goalconditionalkeys.Length).ToDictionary((int i) => goalconditionalkeys[i], (int i) => goalconditionalvalues[i]);
		PresentAttribute = Enumerable.Range(0, presentkeys.Length).ToDictionary((int i) => presentkeys[i], (int i) => presentvalues[i]);
	}
}
