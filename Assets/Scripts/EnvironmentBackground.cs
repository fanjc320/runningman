using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBackground : MonoBehaviour
{
	private CharacterCamera characterCamera;

	public float leftLimit;

	public float rightLimit;

	private int nextMonumentSpawnIndex;

	private List<Transform> currentMonumentElements = new List<Transform>();

	private int nextBackgroundSpawnIndex;

	private List<Transform> currentBackgroundElements = new List<Transform>();

	public void ResetBackground(GameObject[] monumentPrefabs, GameObject[] backgroundPrefabs)
	{
		if (currentMonumentElements != null)
		{
			foreach (Transform currentMonumentElement in currentMonumentElements)
			{
				UnityEngine.Object.Destroy(currentMonumentElement.gameObject);
			}
			currentMonumentElements.Clear();
		}
		if (monumentPrefabs.Length != 0)
		{
			foreach (GameObject original in monumentPrefabs)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(original);
				gameObject.SetActive(value: false);
				Transform transform = gameObject.transform;
				transform.parent = base.gameObject.transform;
				transform.localPosition = new Vector3(leftLimit, 0f, 0f);
				transform.localRotation = Quaternion.identity;
				ToggleElement(transform, on: false);
				currentMonumentElements.Add(transform.transform);
			}
			int num = UnityEngine.Random.Range(0, currentMonumentElements.Count - 1);
			if (num == nextMonumentSpawnIndex)
			{
				if (num + 1 > currentMonumentElements.Count - 1)
				{
					nextMonumentSpawnIndex = 0;
				}
				else
				{
					nextMonumentSpawnIndex = num + 1;
				}
			}
			else
			{
				nextMonumentSpawnIndex = num;
			}
			if (currentMonumentElements.Count > 0)
			{
				ToggleElement(currentMonumentElements[nextMonumentSpawnIndex], on: true);
			}
		}
		if (currentBackgroundElements != null)
		{
			foreach (Transform currentBackgroundElement in currentBackgroundElements)
			{
				UnityEngine.Object.Destroy(currentBackgroundElement.gameObject);
			}
			currentBackgroundElements.Clear();
		}
		if (backgroundPrefabs.Length > 2)
		{
			foreach (GameObject original2 in backgroundPrefabs)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original2);
				gameObject2.SetActive(value: false);
				Transform transform2 = gameObject2.transform;
				transform2.parent = base.gameObject.transform;
				transform2.localPosition = new Vector3(leftLimit, 0f, 0f);
				transform2.localRotation = Quaternion.identity;
				ToggleElement(transform2, on: false);
				currentBackgroundElements.Add(transform2.transform);
			}
			int num2 = nextBackgroundSpawnIndex = UnityEngine.Random.Range(0, currentBackgroundElements.Count - 1);
			nextBackgroundSpawnIndex = PlaceElement(nextBackgroundSpawnIndex, currentBackgroundElements, leftLimit * 0.33f);
			nextBackgroundSpawnIndex = PlaceElement(nextBackgroundSpawnIndex, currentBackgroundElements, 0f);
			nextBackgroundSpawnIndex = PlaceElement(nextBackgroundSpawnIndex, currentBackgroundElements, rightLimit * 0.5f);
		}
	}

	private int PlaceElement(int spawnPosition, List<Transform> spawnList, float position)
	{
		spawnPosition = ((spawnPosition != spawnList.Count - 1) ? (spawnPosition + 1) : 0);
		ToggleElement(spawnList[spawnPosition], on: true);
		spawnList[spawnPosition].localPosition = new Vector3(position, 0f, 0f);
		return spawnPosition;
	}

	private IEnumerator Start()
	{
		while (true)
		{
			nextMonumentSpawnIndex = UpdateElementList(currentMonumentElements, nextMonumentSpawnIndex);
			nextBackgroundSpawnIndex = UpdateElementList(currentBackgroundElements, nextBackgroundSpawnIndex);
			yield return new WaitForSeconds(2f);
		}
	}

	private int UpdateElementList(List<Transform> elementList, int spawnPosition)
	{
		foreach (Transform element in elementList)
		{
			Vector3 localPosition = element.localPosition;
			if (localPosition.x > rightLimit)
			{
				ToggleElement(element, on: false);
				element.localPosition = new Vector3(leftLimit, 0f, 0f);
				element.GetComponent<MoveGameObject>().model.localScale = Vector3.zero;
				ToggleElement(elementList[spawnPosition], on: true);
				spawnPosition = ((spawnPosition != elementList.Count - 1) ? (spawnPosition + 1) : 0);
			}
		}
		return spawnPosition;
	}

	private void ToggleElement(Transform gO, bool on)
	{
		gO.gameObject.SetActive(on);
	}

	private void Awake()
	{
		characterCamera = CharacterCamera.Instance;
	}

	public void Update()
	{
		Transform transform = base.gameObject.transform;
		Vector3 position = characterCamera.transform.position;
		transform.position = new Vector3(0f, 0f, position.z);
	}
}
