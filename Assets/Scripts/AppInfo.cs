using UnityEngine;

public class AppInfo : MonoBehaviour
{
	[SerializeField]
	public eBuildType buildType_;

	public static AppInfo instance_;

	public static AppInfo Instance
	{
		get
		{
			if (null == instance_)
			{
				instance_ = (UnityEngine.Object.FindObjectOfType(typeof(AppInfo)) as AppInfo);
				if (!(null == instance_))
				{
				}
			}
			return instance_;
		}
	}

	private void Start()
	{
		GameSystem.Instance.Initialize();
	}

	private void Update()
	{
	}
}
