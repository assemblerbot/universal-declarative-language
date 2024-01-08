using System.Diagnostics;
using System.Text;

namespace UniversalDeclarativeLanguage;

public sealed class UdlParser
{
	private UdlLexer _lexer = null!;
	private UdlToken _token;

	public UdlNode? ParseUTF8(string source)
	{
		return Parse(new MemoryStream(Encoding.UTF8.GetBytes(source)));
	}

	public UdlNode? Parse(Stream source)
	{
		_lexer = new(source);

		_token = _lexer.NextToken();
		if (_token.Type == UdlTokenType.Eof)
		{
			return null;
		}

		UdlNode node = Node();
		if (_token.Type != UdlTokenType.Eof)
		{
			throw new UdlParserError($"End of file expected at {_lexer.PositionString}");
		}
		
		return node;
	}

	private UdlNode Node()
	{
		string         keyword = Keyword();
		string?        name    = Name();
		string?        type    = Type();
		object?        value   = Value();
		List<UdlNode>? block   = Block();

		return new UdlNode(keyword, name, type, value, block);
	}

	private string Keyword()
	{
		if (_token.Type != UdlTokenType.Identifier)
		{
			throw new UdlParserError($"Missing identifier at {_lexer.PositionString}");
		}
		
		string keyword = _token.Value!;
		_token = _lexer.NextToken();
		return keyword;
	}

	private string? Name()
	{
		if (_token.Type != UdlTokenType.String)
		{
			return null;
		}

		string name = _token.Value!;
		_token = _lexer.NextToken();
		return name;
	}

	private string? Type()
	{
		if (_token.Type != UdlTokenType.Colon)
		{
			return null;
		}

		_token = _lexer.NextToken();
		if (_token.Type == UdlTokenType.Eof)
		{
			throw new UdlParserError($"Missing identifier at {_lexer.PositionString}");
		}

		string type = _token.Value!;
		_token = _lexer.NextToken();
		return type;
	}

	private object? Value()
	{
		if (_token.Type != UdlTokenType.EqualSign)
		{
			return null;
		}

		_token = _lexer.NextToken();
		if (_token.Type == UdlTokenType.Eof)
		{
			throw new UdlParserError($"Missing value at {_lexer.PositionString}");
		}

		object? value = _token.Type switch
		{
			UdlTokenType.String      => _token.Value,
			UdlTokenType.IntNumber   => _token.IntValue,
			UdlTokenType.FloatNumber => _token.FloatValue,
			UdlTokenType.Bool        => _token.BoolValue,
			_ => throw new UdlParserError($"Invalid value at {_lexer.PositionString}")
		};

		_token = _lexer.NextToken();
		return value;
	}

	private List<UdlNode>? Block()
	{
		if (_token.Type != UdlTokenType.LeftBrace)
		{
			return null;
		}
		_token = _lexer.NextToken();

		if (_token.Type == UdlTokenType.Eof)
		{
			throw new UdlParserError($"Missing }} at {_lexer.PositionString}");
		}

		List<UdlNode> block = new();
		while (_token.Type != UdlTokenType.RightBrace)
		{
			block.Add(Node());
		}
		_token = _lexer.NextToken();

		return block;
	}
}