using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Compressors.LZMA;

namespace Mara.Lib.Common
{
    public class Lzma
    {
        public static void Create(string folderFiles, string outFile)
        {
            throw new NotSupportedException();

            using (var archive = ZipArchive.Create())
            {
                archive.AddAllFromDirectory(folderFiles);
                archive.SaveTo(outFile, CompressionType.LZMA);
            }
        }

        public static void Unpack(string file, string outFolder)
        {
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);

            using var archive = ZipArchive.Open(file);

            foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
            {
                entry.WriteToDirectory(outFolder, new ExtractionOptions()
                {
                    ExtractFullPath = true,
                    Overwrite = false,
                });
            }
        }
    }
}
