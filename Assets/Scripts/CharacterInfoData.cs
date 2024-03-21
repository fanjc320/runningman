using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CharacterInfoData
{
	[SerializeField]
	private string id;

	[SerializeField]
	private string name1loc;

	[SerializeField]
	private string chardesc1loc;

	[SerializeField]
	private string skilldesc1loc;

	[SerializeField]
	private string cid;

	[SerializeField]
	private string modelname;

	[SerializeField]
	private string iconpath;

	[SerializeField]
	private string nameimagepath;

	[SerializeField]
	private string chcoinimagepath;

	[SerializeField]
	private string skillimagepath;

	[SerializeField]
	private string[] unlockattrkeys = new string[0];

	[SerializeField]
	private string[] unlockattrvalues = new string[0];

	[SerializeField]
	private string[] businessattrkeys = new string[0];

	[SerializeField]
	private string[] businessattrvalues = new string[0];

	[SerializeField]
	private string[] skillattrkeys = new string[0];

	[SerializeField]
	private string[] skillattrvalues = new string[0];

	[SerializeField]
	private string comment;

	public Dictionary<string, string> UnlockAttribute;

	public Dictionary<string, string> SkillAttribute;

	public Dictionary<string, string> BusinessAttribute;

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
	public string Chardesc1loc
	{
		get
		{
			return chardesc1loc;
		}
		set
		{
			chardesc1loc = value;
		}
	}

	[ExposeProperty]
	public string Skilldesc1loc
	{
		get
		{
			return skilldesc1loc;
		}
		set
		{
			skilldesc1loc = value;
		}
	}

	[ExposeProperty]
	public string CID
	{
		get
		{
			return cid;
		}
		set
		{
			cid = value;
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
	public string Nameimagepath
	{
		get
		{
			return nameimagepath;
		}
		set
		{
			nameimagepath = value;
		}
	}

	[ExposeProperty]
	public string Chcoinimagepath
	{
		get
		{
			return chcoinimagepath;
		}
		set
		{
			chcoinimagepath = value;
		}
	}

	[ExposeProperty]
	public string Skillimagepath
	{
		get
		{
			return skillimagepath;
		}
		set
		{
			skillimagepath = value;
		}
	}

	[ExposeProperty]
	public string[] Unlockattrkeys
	{
		get
		{
			return unlockattrkeys;
		}
		set
		{
			unlockattrkeys = value;
		}
	}

	[ExposeProperty]
	public string[] Unlockattrvalues
	{
		get
		{
			return unlockattrvalues;
		}
		set
		{
			unlockattrvalues = value;
		}
	}

	[ExposeProperty]
	public string[] Businessattrkeys
	{
		get
		{
			return businessattrkeys;
		}
		set
		{
			businessattrkeys = value;
		}
	}

	[ExposeProperty]
	public string[] Businessattrvalues
	{
		get
		{
			return businessattrvalues;
		}
		set
		{
			businessattrvalues = value;
		}
	}

	[ExposeProperty]
	public string[] Skillattrkeys
	{
		get
		{
			return skillattrkeys;
		}
		set
		{
			skillattrkeys = value;
		}
	}

	[ExposeProperty]
	public string[] Skillattrvalues
	{
		get
		{
			return skillattrvalues;
		}
		set
		{
			skillattrvalues = value;
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
		UnlockAttribute = Enumerable.Range(0, unlockattrkeys.Length).ToDictionary((int i) => unlockattrkeys[i], (int i) => unlockattrvalues[i]);
		SkillAttribute = Enumerable.Range(0, skillattrkeys.Length).ToDictionary((int i) => skillattrkeys[i], (int i) => skillattrvalues[i]);
		BusinessAttribute = Enumerable.Range(0, businessattrkeys.Length).ToDictionary((int i) => businessattrkeys[i], (int i) => businessattrvalues[i]);
	}
}
