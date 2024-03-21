using System;
using UnityEngine;

public class NametagInfo : MonoBehaviour
{
	public static int mFreeNametagMax = 6;

	public static int mFreeNametagMin = 0;

	public static int mPurchageHaveNametag = 0;

	public static int mFreeHaveNametag = 0;

	public static int mHaveNametag = 0;

	public static float mNametagChargeTime = 480f;

	public static float mConnectPlayTime = Time.fixedTime + mNametagChargeTime;

	public static float mNametagLeftTime = 0f;

	public static DateTime lastnametagTime;

	public static float mDelayime = 0f;
}
