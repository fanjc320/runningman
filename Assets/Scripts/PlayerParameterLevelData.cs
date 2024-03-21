using System;
using UnityEngine;

[Serializable]
public class PlayerParameterLevelData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string infoid;

	[SerializeField]
	private int requiregold;

	[SerializeField]
	private float pvalue;

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
	public string Infoid
	{
		get
		{
			return infoid;
		}
		set
		{
			infoid = value;
		}
	}

	[ExposeProperty]
	public int Requiregold
	{
		get
		{
			return requiregold;
		}
		set
		{
			requiregold = value;
		}
	}

	[ExposeProperty]
	public float Pvalue
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
}
