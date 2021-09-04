﻿using System.IO;
using System.Linq;

namespace Mara.Lib.Platforms.Generic
{
    public class Main : PatchProcess
    { 
        public Main(string oriFolder, string outFolder, string filePath) : base(oriFolder, outFolder, filePath)
        {
        }


        public override (int, string) ApplyTranslation()
        {
            var count = maraConfig.FilesInfo.ListOriFiles.Length;
            var files = maraConfig.FilesInfo;

            for (int i = 0; i < count; i++)
            {
                var excludeFile = false;

                if (files.ListExcludeFiles != null)
                    excludeFile = CheckExcludeFile(files.ListOriFiles[i]);

                var oriFile = $"{oriFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";

                if (excludeFile)
                    if (!File.Exists(oriFile))
                        continue;

                var result = ApplyXdelta(oriFile, xdelta, oriFile, files.ListMd5Files[i]);

                if (result.Item1 != 0)
                    return result;
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
