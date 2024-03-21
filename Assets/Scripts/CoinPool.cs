using Lean;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : MonoBehaviour
{
	public GameObject coinPrefab;

	public GameObject coinRedPrefab;

	public Mesh coinMesh;

	public Mesh coinRedMesh;

	public Material coinMat;

	public Material coinRedMat;

	public int numDeletedPerCleanup = 4;

	public float cleanupIntervalInSeconds = 5f;

	private Vector3 spawnPoint = -1000f * Vector3.up;

	private Vector3 spawnSpacing = -20f * Vector3.right;

	private List<Transform> coins = new List<Transform>();

	private HashSet<Transform> coinsAll = new HashSet<Transform>();

	public static CoinPool instance;

	private int numberOfActiveCoins;

	private int numberOfActiveCoins_high;

	private int numberOfActiveCoinsR;

	private int numberOfActiveCoins_highR;

	private List<PickupRotate> activeRotatePickups = new List<PickupRotate>();

	private List<PickupRotate> activeRotatePickupsR = new List<PickupRotate>();

	public List<PickupRotate> ActiveRotatingPickups => activeRotatePickups;

	public List<PickupRotate> ActiveRotatingPickupsR => activeRotatePickupsR;

	public static CoinPool Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(CoinPool)) as CoinPool));

	public void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		for (int i = 0; i < activeRotatePickups.Count; i++)
		{
			if (activeRotatePickups[i].enabled)
			{
				activeRotatePickups[i].PhasedRotate();
			}
		}
	}

	public void InvisibleAllCoins()
	{
		IEnumerator enumerator = coinsAll.GetEnumerator();
		while (enumerator.MoveNext())
		{
			LeanPool.Despawn(((Transform)enumerator.Current).gameObject);
		}
		coinsAll.Clear();
	}

	public Transform GetCoin()
	{
		GameObject gameObject = LeanPool.Spawn(coinPrefab);
		ChangeCoin(gameObject, GameStats.Instance.IsDoubleCoin);
		coinsAll.Add(gameObject.transform);
		return gameObject.transform;
	}

	public void ChangeCoin(GameObject coinGO, bool isRedCoin)
	{
		Mesh mesh = (!isRedCoin) ? coinMesh : coinRedMesh;
		Material material = (!isRedCoin) ? coinMat : coinRedMat;
		Coin component = coinGO.GetComponent<Coin>();
		if ((bool)component.glowRenderer)
		{
			component.glowRenderer.material = material;
		}
		component.coinMeshFilter.mesh = mesh;
	}

	public void NotifyDoubleCoinAttr(bool isRedCoin)
	{
		HashSet<Transform>.Enumerator enumerator = coinsAll.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ChangeCoin(enumerator.Current.gameObject, isRedCoin);
		}
	}

	public void Put(Transform coin)
	{
		Put(new Transform[1]
		{
			coin
		});
	}

	public void Put(IEnumerable<Transform> coins)
	{
		foreach (Transform coin in coins)
		{
			if (coinsAll.Remove(coin))
			{
				LeanPool.Despawn(coin.gameObject);
			}
		}
	}

	private void OnDestroy()
	{
		instance = null;
	}
}
