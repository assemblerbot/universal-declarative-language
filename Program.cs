// See https://aka.ms/new-console-template for more information

using UniversalDeclarativeLanguage;

internal static class Program
{
	private static void Main(string[] args)
	{
		DateTime start = DateTime.Now;
		
		using FileStream stream = new FileStream("Tests\\basic.udl", FileMode.Open);
		
		UdlParser parser = new();
		try
		{
			UdlNode? tree = parser.Parse(stream);
			Console.WriteLine(tree);
		}
		catch (UdlParserError e)
		{
			Console.WriteLine(e);
		}

		Console.WriteLine($"Finished in {(DateTime.Now - start).TotalMilliseconds}ms");
	}
}