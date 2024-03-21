using System;
using UnityEngine;

[Serializable]
public class GameAbilityParameterData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string name_loc;

	[SerializeField]
	private string desc_loc;

	[SerializeField]
	private string iconpath;

	[SerializeField]
	private string abilitytype;

	[SerializeField]
	private string activetype;

	[SerializeField]
	private string comment;

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
	public string Name_loc
	{
		get
		{
			return name_loc;
		}
		set
		{
			name_loc = value;
		}
	}

	[ExposeProperty]
	public string Desc_loc
	{
		get
		{
			return desc_loc;
		}
		set
		{
			desc_loc = value;
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
	public string Abilitytype
	{
		get
		{
			return abilitytype;
		}
		set
		{
			abilitytype = value;
		}
	}

	[ExposeProperty]
	public string Activetype
	{
		get
		{
			return activetype;
		}
		set
		{
			activetype = value;
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
}
