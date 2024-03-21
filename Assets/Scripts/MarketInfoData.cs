using System;
using UnityEngine;

[Serializable]
public class MarketInfoData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string shopinfoid;

	[SerializeField]
	private string type;

	[SerializeField]
	private string marketkey;

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
	public string Shopinfoid
	{
		get
		{
			return shopinfoid;
		}
		set
		{
			shopinfoid = value;
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
	public string Marketkey
	{
		get
		{
			return marketkey;
		}
		set
		{
			marketkey = value;
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
