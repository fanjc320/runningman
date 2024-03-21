using System.Collections;
using UnityEngine;

public class QTest : MonoBehaviour
{
	private const float fpsMeasurePeriod = 0.5f;

	private int m_FpsAccumulator;

	private float m_FpsNextPeriod;

	private int m_CurrentFps;

	public static bool isDowngrade;

	private static QTest instance;

	private void Awake()
	{
		instance = this;
	}

	public static void CheckingPerformance()
	{
		instance.StartCoroutine(instance.StartCheckingPerformance());
	}

	private IEnumerator StartCheckingPerformance()
	{
		m_FpsNextPeriod = Time.realtimeSinceStartup + 0.5f;
		float procTime = 0f;
		while (procTime < 1f)
		{
			m_FpsAccumulator++;
			if (Time.realtimeSinceStartup > m_FpsNextPeriod)
			{
				m_CurrentFps = (int)((float)m_FpsAccumulator / 0.5f);
				m_FpsAccumulator = 0;
				m_FpsNextPeriod += 0.5f;
			}
			procTime += Time.deltaTime;
			yield return 0;
		}
		if (m_CurrentFps < 40)
		{
			isDowngrade = true;
			QualitySettings.SetQualityLevel(0, applyExpensiveChanges: true);
		}
		yield return 0;
	}
}
