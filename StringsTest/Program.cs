using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StringsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string Filename = "<filename>";

            StringsSharp.CStringsSharp.GetVersion();
            Dictionary<string, int> AsciiResult = (new StringsSharp.CStringsSharp(
                                               new StringsSharp.CEncoding("Ascii", 1251, "[\x20-\x7E]", 5))).
                                                Scan(Filename, 128);
            Dictionary<string, int> UnicodeResult = (new StringsSharp.CStringsSharp(
                                               new StringsSharp.CEncoding("Unicode", 1200, "[\u0020-\u007E]"))).
                                                Scan(Filename, 128);
            
            string Config = "<config.json>";
            Dictionary<string, List<string>> FilteredAscii = (new StringsSharp.CFilter(Config)).Scan(AsciiResult);
            Dictionary<string, List<string>> FilteredUnicode = (new StringsSharp.CFilter(Config)).Scan(UnicodeResult);
        }
    }
}
