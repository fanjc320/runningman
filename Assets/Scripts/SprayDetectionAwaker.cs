using UnityEngine;

public class SprayDetectionAwaker : MonoBehaviour
{
	private RunnerAnimPlayer runnerAnimPlayer;

	private void Start()
	{
		runnerAnimPlayer = UtilRMan.FindObject<RunnerAnimPlayer>();
	}

	private void CheckScreen(string screenName)
	{
		runnerAnimPlayer.PlayOrMutePaintSound(doPlay: false);
	}
}
