using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mara.Lib.Platforms.Vita
{
    public class Main : PatchProcess
    {
        public Main(string oriFolder, string outFolder, string filePath, string[] mediaId, bool dlc) : base(oriFolder, outFolder, filePath)
        {
        }
    }
}
