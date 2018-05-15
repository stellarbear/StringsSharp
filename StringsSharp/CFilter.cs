using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json;

namespace StringsSharp
{
    public class CFilter
    {
        Dictionary<string, Regex> Filters;
        public CFilter(string ConfigurationFile)
        {
            if (File.Exists(ConfigurationFile))
            {
                Filters = ReadJson(ConfigurationFile).
                    ToDictionary(k => k.Key, k => new Regex(k.Value, 
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace));
            }
            else
                throw new System.IO.FileNotFoundException();
        }

        public Dictionary<string, List<string>> Scan(Dictionary<string, int> Input)
        {
            if (Filters != null)
            {
                if (Filters.Count > 0)
                {
                    Dictionary<string, List<string>> Result = new Dictionary<string, List<string>>();

                    foreach (KeyValuePair<string, Regex> Filter in Filters)
                    {
                        List<string> RegexResult = Input.Keys.Where(x => Filter.Value.IsMatch(x)).ToList();
                        if (RegexResult.Count > 0) Result.Add(Filter.Key, RegexResult);
                    }

                    return Result;
                }
            }

            return null;
        }

        //Newtonsoft.Json.JsonReaderException
        //  Read new format
        private Dictionary<string, string> ReadJson(string Filename)
        {
            Dictionary<string, string> Result = new Dictionary<string, string>();
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile(Filename));
        }
        //  Read entire file
        private string ReadFile(string Filename)
        {
            string Result = null;

            //  Read if exists
            if (File.Exists(Filename))
            {
                using (System.IO.StreamReader SR = new System.IO.StreamReader(Filename, Encoding.UTF8))
                {
                    Result = SR.ReadToEnd();
                }
            }

            return Result;
        }
    }
}
