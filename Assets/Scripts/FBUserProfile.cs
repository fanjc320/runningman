using System;

public class FBUserProfile
{
	public enum UISelectType
	{
		InviteNonGameFriend,
		RequestAllFriend,
		RequestGameFriend
	}

	public string Id;

	public string ImageURL;

	public string UserName;

	public bool IsAppFriend = true;

	public bool[] UISelected = new bool[Enum.GetValues(typeof(UISelectType)).Length];
}
