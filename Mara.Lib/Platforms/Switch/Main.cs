using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mara.Lib.Platforms.Switch
{
    public class Main
    {
        public HOS horizon;
        public PartitionFS NSP;
        public GameCard XCI;
        public NCA NCAS;
        public Main(string Keys, string GamePath, bool checkSignature)
        {
            this.horizon = new HOS(Keys, checkSignature);
            if (GamePath.Contains(".nsp"))
            {
                this.NSP = new PartitionFS(GamePath);
                this.NCAS = new NCA(horizon, this.NSP.MountPFS0(horizon));
            } 
            else if (GamePath.Contains(".xci"))
            {
                this.XCI = new GameCard(horizon, GamePath);
                this.NCAS = new NCA(horizon, this.XCI.MountGameCard(horizon));
            }
            else
            {
                throw new Exception("Unrecognized file.");
            }
        }
    }
}
