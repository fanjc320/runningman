using System;
using UnityEngine;

[Serializable]
public class BonusboxData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string name1loc;

	[SerializeField]
	private string type;

	[SerializeField]
	private string pvalue;

	[SerializeField]
	private int probability;

	[SerializeField]
	private string modelname;

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
	public string Pvalue
	{
		get
		{
			return pvalue;
		}
		set
		{
			pvalue = value;
		}
	}

	[ExposeProperty]
	public int Probability
	{
		get
		{
			return probability;
		}
		set
		{
			probability = value;
		}
	}

	[ExposeProperty]
	public string Modelname
	{
		get
		{
			return modelname;
		}
		set
		{
			modelname = value;
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
