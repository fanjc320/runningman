using AudienceNetwork;
using UnityEngine;
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
	public NativeAd nativeAd;

	public GameObject targetAdObject;

	public Button targetButton;

	private bool adLoaded;

	private void Start()
	{
		adLoaded = false;
		LoadAd();
	}

	private void OnDestroy()
	{
		if ((bool)nativeAd)
		{
			nativeAd.Dispose();
		}
	}

	public bool IsAdLoaded()
	{
		return adLoaded;
	}

	public void LoadAd()
	{
		NativeAd nativeAd = new NativeAd("YOUR_PLACEMENT_ID");
		this.nativeAd = nativeAd;
		if ((bool)targetAdObject)
		{
			if ((bool)targetButton)
			{
				nativeAd.RegisterGameObjectForImpression(targetAdObject, new Button[1]
				{
					targetButton
				});
			}
			else
			{
				nativeAd.RegisterGameObjectForImpression(targetAdObject, new Button[0]);
			}
		}
		else
		{
			nativeAd.RegisterGameObjectForImpression(base.gameObject, new Button[0]);
		}
		nativeAd.NativeAdDidLoad = delegate
		{
			adLoaded = true;
			StartCoroutine(nativeAd.LoadCoverImage(nativeAd.CoverImageURL));
			StartCoroutine(nativeAd.LoadIconImage(nativeAd.IconImageURL));
		};
		nativeAd.NativeAdDidFailWithError = delegate
		{
		};
		nativeAd.NativeAdWillLogImpression = delegate
		{
		};
		nativeAd.NativeAdDidClick = delegate
		{
		};
		nativeAd.LoadAd();
	}
}
