using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mara.Lib.Platforms.Switch
{
    public class Main
    {
        HOS horizon;
        PartitionFS NSP;
        public Main(string Keys, string GamePath)
        {
            this.horizon = new HOS(Keys);
            if (GamePath.Contains(".nsp"))
            {
                this.NSP = new PartitionFS(GamePath);
            } 
            else if (GamePath.Contains(".xci"))
            {
                throw new NotImplementedException("GameCards not implemented yet.");
            }
            else
            {
                throw new Exception("Unrecognized file.");
            }
        }
    }
}
