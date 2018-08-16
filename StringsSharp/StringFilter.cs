using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace StringsSharp
{
    public class StringFilter : IDisposable
    {
        Dictionary<string, Regex> _filters;
        public StringFilter(string configurationFile)
        {
            if (File.Exists(configurationFile))
            {
                try
                {
                    Dictionary<string, string> jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile(configurationFile));
                    _filters = jsonData.
                        ToDictionary(k => k.Key, k => 
                            new Regex(k.Value, RegexOptions.Compiled | 
                                               RegexOptions.IgnoreCase | 
                                               RegexOptions.IgnorePatternWhitespace));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
                throw new FileNotFoundException($"File does not exist: {configurationFile}");
        }
        public void Dispose()
        {
            _filters?.Clear();
            _filters = null;
        }

        public IEnumerable<string> Scan(string inputString)
        {
            if (_filters != null)
            {
                if (_filters.Count > 0)
                {
                    foreach (KeyValuePair<string, Regex> filter in _filters)
                    {
                        if (filter.Value.IsMatch((inputString)))
                        {
                            yield return filter.Key;
                        }
                    }
                }
            }
        }

        public bool ScanQuick(string inputString)
        {
            if (_filters != null)
            {
                if (_filters.Count > 0)
                {
                    foreach (KeyValuePair<string, Regex> filter in _filters)
                    {
                        if (filter.Value.IsMatch((inputString)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private string ReadFile(string filename)
        {
            string result = null;

            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename, Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }
    }
}
