using UnityEngine;

internal class CharacterRenderingEffects : MonoBehaviour
{
	[SerializeField]
	private GameObject jetpackParticles;

	public ParticleFollow jetpackParticleCloudL;

	public ParticleFollow jetpackParticleCloudR;

	public GameObject JetpackParticles => jetpackParticles;

	public void Initialize(CharacterModel characterModel)
	{
	}

	public void SetRightAndLeftParticlesActive(bool active)
	{
		jetpackParticles.SetActive(active);
	}
}
