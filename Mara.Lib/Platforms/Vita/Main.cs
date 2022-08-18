using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mara.Lib.Common.IO;

namespace Mara.Lib.Platforms.Vita
{
    public class Main : PatchProcess
    {
        private string titleIdSelected;
        public Main(string oriFolder, string outFolder, string filePath) : base(oriFolder, outFolder, filePath)
        {
        }
        
        public Main(string oriFolder, string outFolder, OWO filePath) : base(oriFolder, outFolder, filePath)
        {
        }


        private bool CheckTitleId()
        {
            var sfo = new Sfo($"{oriFolder}{Path.DirectorySeparatorChar}GAME{Path.DirectorySeparatorChar}sce_sys{Path.DirectorySeparatorChar}param.sfo");
            var titleId = File.ReadAllLines($"{tempFolder}{Path.DirectorySeparatorChar}titleids.txt");
            foreach (var id in titleId)
            {
                if ((string) sfo.SfoValues["TITLE_ID"] == id)
                {
                    titleIdSelected = id;
                    return true;
                }
            }

            return false;
        }

        public override (int, string) ApplyTranslation()
        {
            var check = CheckGame();

            if (check.Item1 != 0)
                return check;

            var count = maraConfig.FilesInfo.ListOriFiles.Length;
            var files = maraConfig.FilesInfo;

            for (int i = 0; i < count; i++)
            {
                var oriFile = $"{oriFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";
                var outFile = files.ListOriFiles[i];
                if (outFile.StartsWith("GAME"))
                    outFile = $"{outFolder}{Path.DirectorySeparatorChar}rePatch{Path.DirectorySeparatorChar}{titleIdSelected}{outFile.Substring(4)}";
                else
                {
                    // Check if the DLC exist!
                    if (File.Exists(oriFile) || File.Exists(oriFile+"_ori"))
                        outFile = $"{outFolder}{Path.DirectorySeparatorChar}reAddcont{Path.DirectorySeparatorChar}{titleIdSelected}{outFile.Substring(3)}";
                    else
                        continue;
                }
                
                var result = ApplyXdelta(oriFile, xdelta, outFile, files.ListMd5Files[i]);

                if (result.Item1 != 0)
                    return result;
            }

            return base.ApplyTranslation();
        }

        private (int, string) CheckGame()
        {
            // Check if the GAME folder exists
            if (!Directory.Exists(oriFolder + Path.DirectorySeparatorChar + "GAME"))
                return (5, "The GAME folder doesn't exist!");

            // Check the current game
            if (!CheckTitleId())
                return (6, "Game invalid!");

            return (0, "");
        }
    }
}
