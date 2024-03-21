using System;
using UnityEngine;

[Serializable]
public class TokenInfoData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string name1loc;

	[SerializeField]
	private string desc1loc;

	[SerializeField]
	private string iconpath;

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
