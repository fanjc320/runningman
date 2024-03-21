using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CsvParser : IEnumerable<string>, IEnumerable
{
	private StreamTokenizer _tokenizer;

	public CsvParser(Stream data)
	{
		_tokenizer = new StreamTokenizer(new StreamReader(data));
	}

	public CsvParser(string data)
	{
		_tokenizer = new StreamTokenizer(new StringReader(data));
	}

	public IEnumerator<string> GetEnumerator()
	{
		bool inQuote = false;
		StringBuilder result = new StringBuilder();
		foreach (Token token in _tokenizer)
		{
			switch (token.Type)
			{
			case TokenType.Comma:
				if (inQuote)
				{
					result.Append(token.Value);
				}
				else
				{
					yield return result.ToString();
					result.Length = 0;
				}
				break;
			case TokenType.Quote:
				inQuote = !inQuote;
				break;
			case TokenType.Value:
				result.Append(token.Value);
				break;
			default:
				throw new InvalidOperationException("Unknown token type: " + token.Type);
			}
		}
		if (result.Length > 0)
		{
			yield return result.ToString();
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
