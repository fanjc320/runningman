using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Flop : UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IEventSystemHandler
{
	public float Offset = 64f;

	public Transform LookAt;

	private float first;

	private float last;

	private float dragStartTime;

	private float dragTime;

	public float accel;

	private float moveDelta;

	public AudioSource tic;

	public AudioClip foundClip;

	public float DampRatio = 1f;

	public float DampMinimum = 0.5f;

	public Transform targetX;

	private Transform[] sortedByName;

	private List<Transform> thisChildren = new List<Transform>();

	private Transform lastT;

	public Color OutlineColor;

	private Color tmpAlphaColor = Color.white;

	public float OffsetAcc = 1f;

	public float DragRatio = 0.5f;

	private bool dragBegan;

	public event Action<bool, float, string> OnUpdate;

	public event Action<bool> OnDragStateChanged;

	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			float num = (float)i * Offset;
			base.transform.GetChild(i).GetComponent<FlopOffsetX>().OffsetX = num;
			Drag(num, base.transform.GetChild(i));
		}
		sortedByName = (from s in Enumerable.Range(0, base.transform.childCount)
			select base.transform.GetChild(s) into s
			orderby int.Parse(s.name)
			select s).ToArray();
		Drag(0f);
	}

	private void Update()
	{
		bool arg = false;
		float arg2 = 0f;
		string arg3 = string.Empty;
		float num = Mathf.Abs(accel);
		if (num > 50f)
		{
			Drag(accel * Time.deltaTime);
			accel = Mathf.Lerp(accel, 0f, Time.deltaTime * 3f);
		}
		else if (targetX != null)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Vector3 localPosition = targetX.localPosition;
				Drag((0f - localPosition.x) * Time.deltaTime * 3f);
			}
			Order();
			Vector3 localPosition2 = targetX.localPosition;
			if (Mathf.Abs(localPosition2.x) < 0.5f)
			{
				arg = true;
				arg3 = base.transform.GetChild(base.transform.childCount - 1).gameObject.name;
				targetX = null;
			}
		}
		else if (num < 50f && num > 0f)
		{
			accel = 0f;
			float num2 = float.MaxValue;
			Transform transform = null;
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform2 = (Transform)enumerator.Current;
					Vector3 localPosition3 = transform2.localPosition;
					if (Mathf.Abs(localPosition3.x) < num2)
					{
						transform = transform2;
						Vector3 localPosition4 = transform2.localPosition;
						num2 = Mathf.Abs(localPosition4.x);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			targetX = transform;
		}
		if (this.OnUpdate != null)
		{
			this.OnUpdate(arg, arg2, arg3);
		}
	}

	public void Drag(PointerEventData e)
	{
		Vector2 delta = e.delta;
		Drag(delta.x * DragRatio);
	}

	public void Drag(float delta)
	{
		float num = (float)base.transform.childCount * Offset / 2f;
		for (int i = 0; sortedByName.Length > i; i++)
		{
			float num2 = sortedByName[i].GetComponent<FlopOffsetX>().OffsetX + delta;
			if (Mathf.Abs(num2) > num)
			{
				num2 = ((!(num2 < 0f)) ? (first = 0f - num + (num2 - num)) : ((last += Offset) + delta));
			}
			Drag(num2, sortedByName[i]);
		}
		Order();
	}

	private void Order()
	{
		thisChildren.Clear();
		for (int i = 0; base.transform.childCount > i; i++)
		{
			thisChildren.Add(base.transform.GetChild(i));
		}
		thisChildren.Sort(delegate(Transform lhs, Transform rhs)
		{
			Vector3 localPosition = lhs.localPosition;
			ref float z = ref localPosition.z;
			Vector3 localPosition2 = rhs.localPosition;
			return z.CompareTo(localPosition2.z);
		});
		first = float.MaxValue;
		last = float.MinValue;
		int count = thisChildren.Count;
		for (int j = 0; j < count; j++)
		{
			thisChildren[j].SetSiblingIndex(j);
		}
		for (int k = 0; sortedByName.Length > k; k++)
		{
			if (sortedByName[k].GetComponent<FlopOffsetX>().OffsetX < first)
			{
				first = sortedByName[k].GetComponent<FlopOffsetX>().OffsetX;
			}
			if (sortedByName[k].GetComponent<FlopOffsetX>().OffsetX > last)
			{
				last = sortedByName[k].GetComponent<FlopOffsetX>().OffsetX;
			}
		}
	}

	private void Drag(float x, Transform t)
	{
		float num = Offset * (float)base.transform.childCount / 2f;
		float num2 = Mathf.Abs(x);
		float num3 = num2 / num;
		float num4 = 1f - DampMinimum;
		float num5 = 1f - num3;
		float num6 = DampMinimum + num4 * Mathf.Pow(Mathf.Max(0f, num5), 10.5f);
		t.GetComponent<FlopOffsetX>().OffsetX = x;
		float x2 = (!(0f <= x)) ? ((0f - num2) * (num5 + OffsetAcc)) : (num2 * (num5 + OffsetAcc));
		Vector3 localPosition = t.localPosition;
		t.localPosition = new Vector3(x2, localPosition.y, num5);
		t.localScale = Vector3.one * num6;
		if (0f >= num6)
		{
			t.gameObject.SetActive(value: false);
			return;
		}
		if (num2 < 0.5f)
		{
			t.GetComponent<CircleOutline>().effectColor = OutlineColor;
		}
		else
		{
			t.GetComponent<CircleOutline>().effectColor = Color.clear;
		}
		tmpAlphaColor.a = num5;
		t.GetComponent<Image>().color = tmpAlphaColor;
		t.gameObject.SetActive(value: true);
	}

	public void OnDrag(PointerEventData e)
	{
		float num = moveDelta;
		Vector2 delta = e.delta;
		moveDelta = num + delta.x * DragRatio * 3f;
		Drag(e);
	}

	public void OnBeginDrag(PointerEventData e)
	{
		accel = 0f;
		dragStartTime = Time.time;
		moveDelta = 0f;
		targetX = null;
		dragBegan = true;
		if (this.OnDragStateChanged != null)
		{
			this.OnDragStateChanged(dragBegan);
		}
	}

	public void OnEndDrag(PointerEventData e)
	{
		dragTime = Time.time - dragStartTime;
		accel = moveDelta / dragTime;
		dragBegan = false;
		if (this.OnDragStateChanged != null)
		{
			this.OnDragStateChanged(dragBegan);
		}
	}

	public void OnPointerDown(PointerEventData e)
	{
		moveDelta = 0f;
		accel = 0f;
		targetX = null;
	}
}
