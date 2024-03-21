using AudienceNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdViewTest : MonoBehaviour
{
	private AdView adView;

	private void Awake()
	{
		AdView adView = this.adView = new AdView("YOUR_PLACEMENT_ID", AdSize.BANNER_HEIGHT_50);
		this.adView.Register(base.gameObject);
		this.adView.AdViewDidLoad = delegate
		{
			this.adView.Show(100.0);
		};
		adView.AdViewDidFailWithError = delegate
		{
		};
		adView.AdViewWillLogImpression = delegate
		{
		};
		adView.AdViewDidClick = delegate
		{
		};
		adView.LoadAd();
	}

	private void OnDestroy()
	{
		if ((bool)adView)
		{
			adView.Dispose();
		}
	}

	public void NextScene()
	{
		SceneManager.LoadScene("NativeAdScene");
	}
}
