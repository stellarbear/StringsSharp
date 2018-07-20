# StringsSharp
Extract strings from files. [Nuget package](https://www.nuget.org/packages/StringsSharp) is available. Sample are files included.

## Description
Small utility that will extract strings from PE and other files. Useful for malware analisis.

You can set up pagecode, character range, minimum and maximum extraction strings length. See example below.

Huge files are processed by splitting them into fixed size chunks. Every subsequent chunk overlaps previous one, i.e. strings located at chunks border won't be lost

It has the ability to filter the results. It is based on configuration file, containing regular expressions.

## Usage
```C#
//	Desired file
string filename = "<filename>";

//  Unicode. Char range: [\u0020-\u007E]. Min string length: 4. Max string length: 16
using (StringsSharp.StringsSharp ss = new StringsSharp.StringsSharp(1200, "[\u0020-\u007E]", 4, 16))
{
	//	Default chunk size is used
    foreach (string extractedString in ss.Scan(filename))
    {
        //  Process string here
    }
}

//  ASCII. Char range: [\x20-\x7E]. Min and string length are set to default
using (StringsSharp.StringsSharp ss = new StringsSharp.StringsSharp(1251, "[\x20-\x7E]"))
{
    using (StringsSharp.StringFilter sf = new StringFilter(configurationFile))
    {
		//	Chunk size is set to 256
        foreach (string extractedString in ss.Scan(filename, 256))
        {
			//	Result filtration in action
            foreach (string filteredString in sf.Scan(extractedString))
            {
                //  Process string here
            }
        }
    }
}
```

## Other
Build in vs 2017

Special thanks to [EricZimmerman](https://github.com/EricZimmerman/bstrings).
