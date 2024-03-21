using System;
using System.Collections.Generic;
using UnityEngine;

public class LateUpdaterLastOrder : MonoBehaviour
{
	private Queue<Action> actionQueue = new Queue<Action>(32);

	private void Awake()
	{
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
