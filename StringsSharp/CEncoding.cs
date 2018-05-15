using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringsSharp
{
    /// <summary>
    /// CStringsSharp subclass. 
    /// </summary>
    public class CEncoding
    {
        int Codepage;
        Regex SearchPattern;
        string CharRange, Name;
        int MinLength = 3, MaxLength = -1;
        Dictionary<string, int> Strings =
            new Dictionary<string, int>();

        /// <summary>
        /// Encoding ctor()
        /// </summary>
        /// See <see href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd317756(v=vs.85).aspx"/> for codes
        /// <param name="Name">Probe naming</param>
        /// <param name="Codepage">Set codepage. Ascii: 1251. UTF16: 1200</param>
        /// <param name="CharRange">Set character range for encoding type. Ascii: "[\x20-\x7E]". UTF16: "[\u0020-\u007E]". </param>
        /// <param name="Min">Opt. Minimum string length.</param>
        /// <param name="Max">Opt. Maximum string length.</param>
        public CEncoding(string Name, int Codepage, string CharRange, int Min = 3, int Max = -1)
        {
            this.Name = Name;
            this.Codepage = Codepage;
            this.CharRange = CharRange;
            UpdateLimits(Min, Max);
        }

        /// <summary>
        /// Set string limitations.
        /// </summary>
        /// <param name="Min">Opt. Minimum string length.</param>
        /// <param name="Max">Opt. Maximum string length.</param>
        public void UpdateLimits(int Min = 3, int Max = -1)
        {
            if ((Max > Min) || ((Max == -1) && (Min >= 3)))
            {
                MinLength = Min;
                MaxLength = Max;
            }

            string MaxString = (MaxLength == -1) ? "" : MaxLength.ToString();
            SearchPattern = new Regex($"{CharRange}{$"{"{"}{MinLength}{","}{MaxString}{"}"}"}",
                RegexOptions.Compiled);
        }

        internal Dictionary<string, int> GetMatches()
        {
            return Strings;
        }
        internal void Scan(byte[] Data)
        {
            foreach (Match ScanMatch in SearchPattern.Matches(
                                        Encoding.GetEncoding(Codepage).GetString(Data)))
            {
                if (Strings.ContainsKey(ScanMatch.Value))
                    Strings[ScanMatch.Value]++;
                else
                    Strings.Add(ScanMatch.Value, 1);
            }
        }
        internal void ClearMatches()
        {
            Strings.Clear();
        }
        internal int GetMaxLength()
        {
            return (MaxLength == -1) ? 1024 * 2 : MaxLength;
        }
        internal string GetName()
        {
            return Name;
        }
    }
}
