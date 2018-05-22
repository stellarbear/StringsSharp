using Alphaleonis.Win32.Filesystem;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StringsSharp
{
    /// <summary>
    /// CStringsSharp main class. 
    /// </summary>
    public class CStringsSharp : IDisposable
    {
        CEncoding Encoding;

        /// <summary>
        /// Get library version.
        /// </summary>
        public static Version GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Default ctor(). Sets string encoding which will be searched in files.
        /// </summary>
        /// <param name="Encoding">String encoding.</param>
        public CStringsSharp(CEncoding Encoding) { this.Encoding = Encoding; }
        
        /// <summary>
        /// Scan file for string entries.
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when file path is set incorrectly.</exception>
        /// <param name="Filename">Filename to be scanned.</param>
        /// <param name="ChunkMBSize">Huge files will be split in chunks (>= 1, default == 256).</param>
        public Dictionary<string, int> Scan(string Filename, int ChunkMBSize = 256)
        {
            if (File.Exists(Filename))
            {
                MemoryMappedFile MappedFile = MemoryMappedFile.CreateFromFile(
                       File.Open(File.GetFileSystemEntryInfo(Filename).LongFullPath,
                                   System.IO.FileMode.Open,
                                   System.IO.FileAccess.Read),
                       "mapped",
                       0,
                       MemoryMappedFileAccess.Read,
                       null,
                       System.IO.HandleInheritability.None,
                       false);

                long Offset = 0;
                long ChunkSize = (ChunkMBSize >= 1) ? ChunkMBSize * 1024 * 1024 : 1024 * 1024;
                long BytesRemain = (new FileInfo(Filename)).Length;

                //  Scan mapped file by schunks
                while (BytesRemain > 0)
                {
                    ChunkSize = (BytesRemain < ChunkSize) ? BytesRemain : ChunkSize;

                    using (MemoryMappedViewStream MappedFileChunkStream = MappedFile.CreateViewStream(Offset, ChunkSize, MemoryMappedFileAccess.Read))
                    {
                        byte[] DataChunk = new byte[ChunkSize];
                        MappedFileChunkStream.Read(DataChunk, 0, (int)ChunkSize);

                        Encoding.Scan(DataChunk);
                    }

                    Offset += ChunkSize;
                    BytesRemain -= ChunkSize;

                    //  If file has more than one chunk, we overlap chunks
                    if (BytesRemain > 0)
                    {
                        Offset -= Encoding.GetMaxLength();
                        BytesRemain += Encoding.GetMaxLength();
                    }
                }

                //  Free resources
                MappedFile.Dispose();
            }
            else
                throw new System.IO.FileNotFoundException();

            return Encoding.GetMatches();
        }

        /// <summary>
        /// Disposable extension. 
        /// </summary>
        public void Dispose()
        {
            Encoding.ClearMatches();
            Encoding = null;
        }
    }
}
