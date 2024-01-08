using System.Globalization;
using System.Text;

namespace UniversalDeclarativeLanguage;

internal sealed class UdlLexer
{
	private readonly Stream        _source;
	private          int           _line          = 1;
	private          int           _column        = 1;
	private readonly StringBuilder _stringBuilder = new();

	public string PositionString => $"line: {_line} column: {_column}";

	public UdlLexer(Stream source)
	{
		_source = source;
	}

	public UdlToken NextToken()
	{
		while (true)
		{
			if (!TryReadNextChar(out char ch))
			{
				return new UdlToken(UdlTokenType.Eof);
			}

			switch (ch)
			{
				case '{':
					return new UdlToken(UdlTokenType.LeftBrace);
				case '}':
					return new UdlToken(UdlTokenType.RightBrace);
				case ':':
					return new UdlToken(UdlTokenType.Colon);
				case '=':
					return new UdlToken(UdlTokenType.EqualSign);
				case '"':
					_stringBuilder.Clear();
					while (TryReadNextChar(out ch))
					{
						if (ch == '"')
						{
							return new UdlToken(UdlTokenType.String, _stringBuilder.ToString());
						}
						_stringBuilder.Append(ch);
					}
					return new UdlToken(UdlTokenType.Invalid);
				case '/':
					if (!TryReadNextChar(out ch))
					{
						return new UdlToken(UdlTokenType.Invalid);
					}

					if (ch == '/')
					{
						// single line comment - skip to next line or eof
						while (TryReadNextChar(out ch))
						{
							if (ch == '\n')
							{
								break;
							}
						}
						continue;
					}

					if (ch == '*')
					{
						// multiline comment - skip to */
						bool wasAsterisk = false;
						while (TryReadNextChar(out ch))
						{
							if (ch == '*')
							{
								wasAsterisk = true;
								continue;
							}
							
							if (ch == '/' && wasAsterisk)
							{
								break;
							}
						
							wasAsterisk = false;
						}
						continue;
					}
					return new UdlToken(UdlTokenType.Invalid);
			}

			if (char.IsLetter(ch))
			{
				_stringBuilder.Clear();
				_stringBuilder.Append(ch);
				while (TryReadNextChar(out ch))
				{
					if (char.IsLetter(ch) || char.IsNumber(ch) || ch == '_')
					{
						_stringBuilder.Append(ch);
						continue;
					}

					_source.Position -= 1;
					break;
				}

				string identifier = _stringBuilder.ToString();
				if (identifier == "true")
				{
					return new UdlToken(true);
				}

				if (identifier == "false")
				{
					return new UdlToken(false);
				}
				
				return new UdlToken(UdlTokenType.Identifier, identifier);
			}

			if (char.IsNumber(ch) || ch == '-')
			{
				_stringBuilder.Clear();
				_stringBuilder.Append(ch);
				bool wasDot = false;
				while (TryReadNextChar(out ch))
				{
					if (char.IsNumber(ch))
					{
						_stringBuilder.Append(ch);
						continue;
					}

					if (ch == '.')
					{
						if (wasDot)
						{
							return new UdlToken(UdlTokenType.Invalid);
						}

						wasDot = true;
						_stringBuilder.Append(ch);
						continue;
					}

					if (ch == '_' || char.IsLetter(ch))
					{
						return new UdlToken(UdlTokenType.Invalid);
					}

					_source.Position -= 1;
					break;
				}

				if (wasDot)
				{
					if (float.TryParse(_stringBuilder.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out float floatNumber))
					{
						return new UdlToken(floatNumber);
					}
					return new UdlToken(UdlTokenType.Invalid);
				}

				if (int.TryParse(_stringBuilder.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int intNumber))
				{
					return new UdlToken(intNumber);
				}
				return new UdlToken(UdlTokenType.Invalid);
			}
			
			// other characters are ignored by definition
		}
	}

	private bool TryReadNextChar(out char ch)
	{
		int b = _source.ReadByte();
		if (b == -1)
		{
			ch = '\0';
			return false;
		}

		ch = (char) b;
		if (ch == '\n')
		{
			++_line;
			_column = 1;
		}
		else
		{
			++_column;
		}

		return true;
	}
}