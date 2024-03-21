using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StartItemInfoData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string name1loc;

	[SerializeField]
	private string desc1loc;

	[SerializeField]
	private string costtype;

	[SerializeField]
	private int cost;

	[SerializeField]
	private bool enabled;

	[SerializeField]
	private string iconpath;

	[SerializeField]
	private string descnameimagepath;

	[SerializeField]
	private string comment;

	private static Dictionary<string, int> costTypeToInt = new Dictionary<string, int>
	{
		{
			"gold",
			0
		},
		{
			"gem",
			1
		}
	};

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
	public string Costtype
	{
		get
		{
			return costtype;
		}
		set
		{
			costtype = value;
		}
	}

	[ExposeProperty]
	public int Cost
	{
		get
		{
			return cost;
		}
		set
		{
			cost = value;
		}
	}

	[ExposeProperty]
	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	[ExposeProperty]
	public string Iconpath
	{
		get
		{
			return iconpath;
		}
		set
		{
			iconpath = value;
		}
	}

	[ExposeProperty]
	public string Descnameimagepath
	{
		get
		{
			return descnameimagepath;
		}
		set
		{
			descnameimagepath = value;
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

	public int CostTypeToInt => costTypeToInt[costtype];
}
