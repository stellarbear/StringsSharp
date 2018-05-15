# StringsSharp
Extract strings from files

## Description
Small utility that will extract strings from PE and other files. Useful for malware analisis.

You can set up pagecode, character range, minimum and maximum extraction strings length. See example below.

Huge files are processed by splitting them into fixed size chunks. Every subsequent chunk overlaps previous one, i.e. strings located at chunks border won't be lost

It has the ability to filter the results. It is based on configuration file, containing regular expressions. See example below.

## Usage
```C#
//	Desired file
string Filename = "<filename>";
//	Find Ascii strings
Dictionary<string, int> AsciiResult = (new StringsSharp.CStringsSharp(
                                    new StringsSharp.CEncoding("Ascii", 1251, "[\x20-\x7E]", 5, 64))).
                                    Scan(Filename, 128);
//	Find UTF-16 strings
Dictionary<string, int> UnicodeResult = (new StringsSharp.CStringsSharp(
                                    new StringsSharp.CEncoding("Unicode", 1200, "[\u0020-\u007E]"))).
                                    Scan(Filename, 128);
			      
//	Configuration file      
string Config = "<config.json>";
//	Fast call filtering function
Dictionary<string, List<string>> FilteredAscii = (new StringsSharp.CFilter(Config)).Scan(AsciiResult);
Dictionary<string, List<string>> FilteredUnicode = (new StringsSharp.CFilter(Config)).Scan(UnicodeResult);
```

## Other
Build in vs 2017

Special thanks to (EricZimmerman)[https://github.com/EricZimmerman/bstrings].