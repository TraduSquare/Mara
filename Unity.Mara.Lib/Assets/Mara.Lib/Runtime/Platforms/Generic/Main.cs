using System;
using System.IO;
using System.Linq;
using Mara.Lib.Common.IO;
using UnityEngine;

namespace Mara.Lib.Platforms.Generic
{
    public class Main : PatchProcess
    { 
        public Main(string oriFolder, string outFolder, string filePath) : base(oriFolder, outFolder, filePath)
        {
        }
        
        public Main(string oriFolder, string outFolder, OWO filePath) : base(oriFolder, outFolder, filePath)
        {
        }

        public override (int, string) ApplyTranslation()
        {
            var count = maraConfig.FilesInfo.ListXdeltaFiles.Length;
            var files = maraConfig.FilesInfo;

            for (int i = 0; i < count; i++)
            {
                var excludeFile = false;

                if (files.ListExcludeFiles != null)
                    excludeFile = CheckExcludeFile(files.ListOriFiles[i]);

                var oriFile = $"{oriFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i].Replace('\\', Path.DirectorySeparatorChar)}";
                var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i].Replace('\\', Path.DirectorySeparatorChar)}";

                if (excludeFile)
                    if (!File.Exists(oriFile))
                        continue;

                var result = ApplyXdelta(oriFile, xdelta, oriFile, files.ListMd5Files[i]);

                if (result.Item1 != 0)
                    return result;
            }
            
            // Copy Files
            for (var i = 0; i < files.ListCopyFiles.Length; i++)
            {
                var oriFile = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListCopyFiles[i]}";
                var outFile = $"{oriFolder}{Path.DirectorySeparatorChar}{files.ListCopyFiles[i]}";
                var folderFile = Path.GetDirectoryName(outFile);

                if (!Directory.Exists(folderFile))
                    Directory.CreateDirectory(folderFile);
                
                if(File.Exists(outFile))
                    File.Delete(outFile);
                
                File.Copy(oriFile, outFile);
            }
            
            return base.ApplyTranslation();
        }

        private bool CheckExcludeFile(string file)
        {
            foreach (var excludeFile in maraConfig.FilesInfo.ListExcludeFiles)
            {
                if (excludeFile == file)
                    return true;
            }

            return false;
        }

    }
}
