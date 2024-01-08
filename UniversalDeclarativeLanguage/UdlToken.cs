using System.Globalization;

namespace UniversalDeclarativeLanguage;

public readonly struct UdlToken
{
	public readonly UdlTokenType Type = UdlTokenType.Invalid;
	
	public readonly string? Value = null;
	public readonly int     IntValue;
	public readonly float   FloatValue;
	public readonly bool    BoolValue;

	public UdlToken(UdlTokenType type)
	{
		Type = type;
	}

	public UdlToken(UdlTokenType type, string value)
	{
		Type  = type;
		Value = value;
	}

	public UdlToken(int value)
	{
		Type       = UdlTokenType.IntNumber;
		Value      = value.ToString();
		IntValue   = value;
		FloatValue = value;
	}

	public UdlToken(float value)
	{
		Type       = UdlTokenType.FloatNumber;
		Value      = value.ToString(CultureInfo.InvariantCulture);
		IntValue   = (int)value;
		FloatValue = value;
	}

	public UdlToken(bool value)
	{
		Type      = UdlTokenType.Bool;
		BoolValue = value;
	}
}