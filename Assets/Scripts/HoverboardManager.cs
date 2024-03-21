internal class HoverboardManager
{
	public delegate void OnHoverboardChangeDelegate(Hoverboards.BoardType hoverboard);

	private Hoverboards.BoardType hoverboard;

	public static HoverboardManager instance;

	public Hoverboards.BoardType Hoverboard
	{
		get
		{
			return hoverboard;
		}
		set
		{
			hoverboard = value;
		}
	}

	public static HoverboardManager Instance => instance ?? (instance = new HoverboardManager());
}
