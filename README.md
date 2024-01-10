# Universal declarative language
Parser for universal declarative language.
In human words the language supports just one kind of notation:
```
identifier "name" : type = value { children }
```
where only the `identifier` is mandatory and everything else is optional.  

Input file is parsed into tree structure of nodes where each node has:
- identifier
- optional string name
- optional string type
- optional value (int, float, bool or string)
- optional child nodes

Parser does the lexical and syntactic analysis. Semantics are up to you.

## Grammar in EBNF
```
letter = "A" | "B" | "C" | "D" | "E" | "F" | "G"
       | "H" | "I" | "J" | "K" | "L" | "M" | "N"
       | "O" | "P" | "Q" | "R" | "S" | "T" | "U"
       | "V" | "W" | "X" | "Y" | "Z" | "a" | "b"
       | "c" | "d" | "e" | "f" | "g" | "h" | "i"
       | "j" | "k" | "l" | "m" | "n" | "o" | "p"
       | "q" | "r" | "s" | "t" | "u" | "v" | "w"
       | "x" | "y" | "z";
       
digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;

white = " " | "\n" | "\t" | "\r" | "\f" | "\b";

character = letter | digit | white;

number = [ "-" ], digit, { digit }, [ ".", digit, { digit } ];
identifier = character, { character | digit } ;
bool = "true" | "false";
string = '"' { any character - '"' } '"';

name = string;
type = identifier;
value = bool | string | number;
block = "{" { node } "}";

node = identifier, [ name ], [':', type], ['=', value], [ block ];

grammar = node;
```
