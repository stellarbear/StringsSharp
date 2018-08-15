using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace StringsSharp
{
    public class StringsSharp : IDisposable
    {
        private Regex _searchPattern;
        private readonly Encoding _encoding;
        private readonly int _minStringLength;
        private readonly int _maxStringLength;

        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        private StringsSharp(string charRange, int minStringLength, int maxStringLength)
        {
            if ((maxStringLength > minStringLength) || ((maxStringLength == 0) && (minStringLength >= 3)))
            {
                _minStringLength = minStringLength;
                _maxStringLength = maxStringLength;
            }
            else
            {
                _minStringLength = 3;
                _maxStringLength = 0;
            }

            _searchPattern = BuildRegexStringExtractionPattern(charRange, _minStringLength, _maxStringLength);
        }


        /// <summary>
        /// Default constructor. minStringLength & maxStringLength may be left blank .
        /// </summary>
        /// <param name="codepage">Set codepage. Ascii: 1251. UTF16: 1200</param>
        /// <param name="charRange">Set character range for encoding type. Ascii: "[\x20-\x7E]". UTF16: "[\u0020-\u007E]". </param>
        /// <param name="minStringLength">Opt. Minimum string length. Must be less than maximum</param>
        /// <param name="maxStringLength">Opt. Maximum string length. Must be greater than minimum. 0 value is equal unlimited string</param>
        public StringsSharp(int codepage, string charRange, int minStringLength = 3, int maxStringLength = 0) :
            this(charRange, minStringLength, maxStringLength)
        {
            _encoding = Encoding.GetEncoding(codepage);
        }


        public void Dispose()
        {
            _searchPattern = null;
        }
        /// <summary>
        /// Scan file for string entries.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown when file path is set incorrectly.</exception>
        /// <param name="filename">Filename to be scanned.</param>
        /// <param name="chunkSplitSizeInMb">Huge files will be split in chunks (>= 1, default == 256).</param>
        public IEnumerable<MatchCollection> Scan(string filename, int chunkSplitSizeInMb = 256)
        {
            if (File.Exists(filename))
            {
                return ScanImplementation(filename, chunkSplitSizeInMb);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private IEnumerable<MatchCollection> ScanImplementation(string filename, int chunkSplitSizeInMb)
        {
            using (MemoryMappedFile mappedFile = MemoryMappedFile.CreateFromFile(
                        File.Open(filename, FileMode.Open, FileAccess.Read),
                        "mapped",
                        0,
                        MemoryMappedFileAccess.Read,
                        null,
                        System.IO.HandleInheritability.None,
                        false))
            {
                long offset = 0;
                long chunkSize = (chunkSplitSizeInMb >= 1) ? chunkSplitSizeInMb * 1024 * 1024 : 1024 * 1024;
                long bytesRemain = (new FileInfo(filename)).Length;

                //  Scan mapped file by schunks
                while (bytesRemain > 0)
                {
                    chunkSize = (bytesRemain < chunkSize) ? bytesRemain : chunkSize;

                    using (MemoryMappedViewStream mappedFileChunkStream =
                        mappedFile.CreateViewStream(offset, chunkSize, MemoryMappedFileAccess.Read))
                    {
                        byte[] dataChunk = new byte[chunkSize];
                        mappedFileChunkStream.Read(dataChunk, 0, (int)chunkSize);
                        
                        yield return _searchPattern.Matches(
                            _encoding.GetString(dataChunk));
                    }

                    offset += chunkSize;
                    bytesRemain -= chunkSize;

                    //  If file has more than one chunk, we overlap chunks
                    if (bytesRemain > 0)
                    {
                        int overlap = (_maxStringLength == -1) ? 1024 * 2 /*1 Mb*/ : _maxStringLength;
                        bytesRemain += overlap;
                        offset -= overlap;
                    }
                }
            }
        }

        private static Regex BuildRegexStringExtractionPattern(string charRange, int minStringLength, int maxStringLength)
        {
            string maxString = (maxStringLength == 0) ? "" : maxStringLength.ToString();
            return new Regex($"{charRange}{$"{"{"}{minStringLength}{","}{maxString}{"}"}"}",
                RegexOptions.Compiled);
        }
    }
}
