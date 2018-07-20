using System;
using StringsSharp;

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
                    foreach (string extractedString in ss.Scan(filename))
                    {
                        //  Process string here
                    }
                }

                //  ASCII
                using (StringsSharp.StringsSharp ss = new StringsSharp.StringsSharp(1251, "[\x20-\x7E]"))
                {
                    using (StringsSharp.StringFilter sf = new StringFilter(configurationFile))
                    {
                        foreach (string extractedString in ss.Scan(filename, 256))
                        {
                            foreach (string filteredString in sf.Scan(extractedString))
                            {
                                //  Process string here
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
