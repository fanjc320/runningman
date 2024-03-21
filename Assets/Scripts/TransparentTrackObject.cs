using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TransparentTrackObject : MonoBehaviour
{
	private static int kBlockerNameHash = "Blocker_Roll".ToString().GetHashCode();

	public Material TransMat;

	public void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.layer != 10 && collider.gameObject.layer != 13)
		{
			return;
		}
		Transform transform = null;
		try
		{
			TrackObject componentInParent = collider.GetComponentInParent<TrackObject>();
			transform = ((!(null != componentInParent)) ? collider.transform : componentInParent.transform);
		}
		catch (Exception)
		{
		}
		if (transform.gameObject.name.GetHashCode() == kBlockerNameHash && !(null == transform))
		{
			Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
			Material[] storedMaterials = (from s in componentsInChildren
				select s.material).ToArray();
			for (int i = 0; componentsInChildren.Length > i; i++)
			{
				componentsInChildren[i].material = new Material(TransMat);
			}
			StartCoroutine(crtTransparent(componentsInChildren, storedMaterials));
		}
	}

	private IEnumerator crtTransparent(Renderer[] storedRenderers, Material[] storedMaterials)
	{
		float elapsedTime = 0f - Time.deltaTime;
		while (0.3334f > elapsedTime)
		{
			elapsedTime += Time.deltaTime;
			for (int i = 0; storedRenderers.Length > i; i++)
			{
				storedRenderers[i].material.SetFloat("_Alpha", Mathf.Lerp(1f, 0.6f, Mathf.Clamp01(elapsedTime / 0.3334f)));
			}
			yield return 0;
		}
		yield return new WaitForSeconds(1f);
		for (int j = 0; storedRenderers.Length > j; j++)
		{
			storedRenderers[j].material = storedMaterials[j];
		}
	}
}
