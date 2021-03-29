using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mara.Lib.Platforms.Switch
{
    public class Main : PatchProcess
    {
        public HOS horizon;
        public PartitionFS NSP;
        public GameCard XCI;
        public NCA NCAS;
        private string titleid;
        public Main(MaraConfig config, string Keys, string GamePath, string TitleID, bool checkSignature) : base(config)
        {
            this.titleid = TitleID;
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

        public override (int, string) ApplyTranslation()
        {
            throw new NotImplementedException();
        }
    }
}
