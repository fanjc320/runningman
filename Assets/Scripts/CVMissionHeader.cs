public class CVMissionHeader : CVMission
{
	public LLocImage HeaderImageText;

	public override void SetData(CVMissionData newData)
	{
		base.SetData(newData);
		HeaderImageText.SetPhraseName((data as CVMissionDataHeader).LocImageText);
	}
}
