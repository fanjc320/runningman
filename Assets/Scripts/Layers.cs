using UnityEngine;

public class Layers
{
	public readonly int Character = FindLayer("Character");

	public readonly int Enemy = FindLayer("Enemy");

	public readonly int Default = FindLayer("Default");

	public readonly int HitBounceOnly = FindLayer("HitBounceOnly");

	public readonly int Hit = FindLayer("Hit");

	public readonly int KeepOnHoverboard = FindLayer("KeepOnHoverboard");

	public static Layers _instance;

	public static Layers Instance
	{
		get
		{
			if (_instance == null)
			{
				Init();
			}
			return _instance;
		}
	}

	private Layers()
	{
	}

	private static int FindLayer(string name)
	{
		int num = LayerMask.NameToLayer(name);
		if (num == -1)
		{
		}
		return num;
	}

	public static void Init()
	{
		_instance = new Layers();
	}
}
