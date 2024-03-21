using EnhancedUI.EnhancedScroller;

public class CVMission : EnhancedScrollerCellView
{
	protected CVMissionData data;

	public virtual void SetData(CVMissionData newData)
	{
		data = newData;
	}
}
