using System;
using System.Collections.Generic;
using UnityEngine;

public class LateUpdater : MonoBehaviour
{
	private static LateUpdater instance;

	private Queue<Action> actionQueue = new Queue<Action>(32);

	public static LateUpdater Instance
	{
		get
		{
			if (null != instance)
			{
				return instance;
			}
			GameObject gameObject = new GameObject(typeof(LateUpdater).Name);
			return gameObject.AddComponent<LateUpdater>();
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void LateUpdate()
	{
		while (0 < actionQueue.Count)
		{
			Action action = actionQueue.Dequeue();
			action();
		}
	}

	public void AddAction(Action action)
	{
		actionQueue.Enqueue(action);
	}
}
