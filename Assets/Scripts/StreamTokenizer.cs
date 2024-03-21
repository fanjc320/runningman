using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class StreamTokenizer : IEnumerable<Token>, IEnumerable
{
	private TextReader _reader;

	public StreamTokenizer(TextReader reader)
	{
		_reader = reader;
	}

	public IEnumerator<Token> GetEnumerator()
	{
		StringBuilder value = new StringBuilder();
		while (true)
		{
			string text;
			string line = text = _reader.ReadLine();
			if (text == null)
			{
				break;
			}
			string text2 = line;
			for (int i = 0; i < text2.Length; i++)
			{
				char c = text2[i];
				switch (c)
				{
				case '"':
				case '\'':
					if (value.Length > 0)
					{
						yield return new Token(TokenType.Value, value.ToString());
						value.Length = 0;
					}
					yield return new Token(TokenType.Quote, c.ToString());
					break;
				case ',':
					if (value.Length > 0)
					{
						yield return new Token(TokenType.Value, value.ToString());
						value.Length = 0;
					}
					yield return new Token(TokenType.Comma, c.ToString());
					break;
				default:
					value.Append(c);
					break;
				}
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
