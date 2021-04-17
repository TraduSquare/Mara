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
        private bool NeedExefs;
        public Main(MaraConfig config, string Keys, string GamePath, string TitleID, bool extractExefs = false, bool checkSignature = true) : base(config)
        {
            this.titleid = TitleID;
            this.horizon = new HOS(Keys, checkSignature);
            this.NeedExefs = extractExefs;
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
            var count = maraConfig.FilesInfo.ListOriFiles.Length;
            var dirTemp = maraConfig.TempFolder;
            var fileTemp = $"{dirTemp}{Path.DirectorySeparatorChar}files";
            var exefsdir = $"{dirTemp}{Path.DirectorySeparatorChar}romfs";
            var romfsdir = $"{dirTemp}{Path.DirectorySeparatorChar}exefs";
            var layeredOut = $"{maraConfig.FilePath}{Path.DirectorySeparatorChar}LayeredFS{Path.DirectorySeparatorChar}{this.titleid}";

            /* Init Dirs */
            if (Directory.Exists(fileTemp))
                Directory.CreateDirectory(fileTemp);
            if(NeedExefs == true)
                if (Directory.Exists(exefsdir))
                    Directory.CreateDirectory(exefsdir);
            if (Directory.Exists(romfsdir))
                Directory.CreateDirectory(romfsdir);
            if (Directory.Exists(layeredOut))
                Directory.CreateDirectory(layeredOut);

            var files = maraConfig.FilesInfo;

            /* if(NeedExefs == true)
                FSUtils.MountFolder(horizon.horizon.Fs, exefsdir, "OutExefs");
            FSUtils.MountFolder(horizon.horizon.Fs, romfsdir, "OutRomfs");

            foreach(string file in files.ListOriFiles)
            {
                // Supongamos que en array de archivos tiene un archivo que está en exefs/main hay
                // que limpiar el "exefs/" ya que esa ruta no existe en el sistem de archivos montado e igual con el romfs.
                if (file.Contains("exefs") == true && NeedExefs == true)
                {
                    FSUtils.CopyFile(horizon.horizon.Fs, "exefs:/" + file.Replace("exefs/", ""), "OutExefs:/" + file.Replace("exefs/", ""));
                } 
                else if (file.Contains("romfs"))
                {
                    FSUtils.CopyFile(horizon.horizon.Fs, "romfs:/" + file.Replace("romfs/",""), "OutRomfs:/" + file.Replace("romfs/",""));
                }
            } */

            for (int i = 0; i < count; i++)
            {
                var result = ApplyXdelta($"", $"{dirTemp}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}", 
                    $"{maraConfig.FilePath}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}",
                    files.ListXdeltaFiles[i], true);
            }

            return (0, string.Empty);
        }
    }
}
