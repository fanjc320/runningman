public class Token
{
	public string Value
	{
		get;
		private set;
	}

	public TokenType Type
	{
		get;
		private set;
	}

	public Token(TokenType type, string value)
	{
		Value = value;
		Type = type;
	}
}
