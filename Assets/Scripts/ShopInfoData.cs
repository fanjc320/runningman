using System;
using UnityEngine;

[Serializable]
public class ShopInfoData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string name1loc;

	[SerializeField]
	private string type;

	[SerializeField]
	private int reward;

	[SerializeField]
	private string costtype;

	[SerializeField]
	private int cost;

	[SerializeField]
	private string bonus;

	[SerializeField]
	private string comment;

	public string costString;

	public string currencyCode;

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
	public int Reward
	{
		get
		{
			return reward;
		}
		set
		{
			reward = value;
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
	public string Bonus
	{
		get
		{
			return bonus;
		}
		set
		{
			bonus = value;
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
