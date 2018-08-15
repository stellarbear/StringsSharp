using StringsSharp;
using System;
using System.Text.RegularExpressions;

namespace StringsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = @"strings";
            string configurationFile = @"strings";

            try
            {
                //  Unicode
                using (StringsSharp.StringsSharp ss = new StringsSharp.StringsSharp(1200, "[\u0020-\u007E]", 4, 16))
                {
                    foreach (MatchCollection matches in ss.Scan(filename))
                    {
                        //  Process matches here
                    }
                }

                //  ASCII
                using (StringsSharp.StringsSharp ss = new StringsSharp.StringsSharp(1251, "[\x20-\x7E]"))
                {
                    using (StringsSharp.StringFilter sf = new StringFilter(configurationFile))
                    {
                        foreach (MatchCollection matches in ss.Scan(filename, 256))
                        {
                            foreach (Match match in matches)
                            {
                                if (sf.Scan(match.Value))
                                {
                                    //  Process string here
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //  Handle errors here
            }
        }
    }
}
