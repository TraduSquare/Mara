using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

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
        public Main(string oriFolder, string outFolder, string filePath, string Keys, string TitleID, bool extractExefs = false, bool checkSignature = true) : base(oriFolder, outFolder, filePath)
        {
            this.titleid = TitleID;
            this.horizon = new HOS(Keys, checkSignature);
            this.NeedExefs = extractExefs;
            if (oriFolder.Contains(".nsp"))
            {
                this.NSP = new PartitionFS(oriFolder);
                this.NCAS = new NCA(horizon, this.NSP.MountPFS0(horizon));
            } 
            else if (oriFolder.Contains(".xci"))
            {
                this.XCI = new GameCard(horizon, oriFolder);
                this.NCAS = new NCA(horizon, this.XCI.MountGameCard(horizon));
            }
            else
                throw new Exception("Unrecognized file.");

            NCAS.MountProgram(horizon, titleid);

        }

        public override (int, string) ApplyTranslation()
        {
            var count = maraConfig.FilesInfo.ListOriFiles.Length;
            var fileTemp = $"{tempFolder}{Path.DirectorySeparatorChar}files";
            var exefsdir = $"{tempFolder}{Path.DirectorySeparatorChar}exefs";
            var romfsdir = $"{tempFolder}{Path.DirectorySeparatorChar}romfs";
            var layeredOut = $"{Path.GetDirectoryName(oriFolder)}{Path.DirectorySeparatorChar}LayeredFS{Path.DirectorySeparatorChar}{this.titleid}";

            /* Init Dirs */
            if (!Directory.Exists(fileTemp))
                Directory.CreateDirectory(fileTemp);
            if(NeedExefs == true)
                if (!Directory.Exists(exefsdir))
                    Directory.CreateDirectory(exefsdir);
            if (!Directory.Exists(romfsdir))
                Directory.CreateDirectory(romfsdir);
            if (!Directory.Exists(romfsdir))
                Directory.CreateDirectory(romfsdir);
            if (!Directory.Exists(layeredOut))
                Directory.CreateDirectory(layeredOut);

            var files = maraConfig.FilesInfo;
            Result result;
            if (NeedExefs)
            {
                result = FSUtils.MountFolder(horizon.horizon.Fs, exefsdir, "OutExefs");
                if (result.IsFailure()) return (2, $"Error mounting exeFs\n{result.Description}");
            }
                
            result = FSUtils.MountFolder(horizon.horizon.Fs, romfsdir, "OutRomfs");
            if (result.IsFailure()) return (3, $"Error mounting romFs\n{result.Description}");
            foreach (string file in files.ListOriFiles)
            {
                // Supongamos que en array de archivos tiene un archivo que está en exefs/main hay
                // que limpiar el "exefs/" ya que esa ruta no existe en el sistem de archivos montado e igual con el romfs.
                if (file.Contains("exefs") == true && NeedExefs == true)
                {
                    result = FSUtils.CopyFile(horizon.horizon.Fs, "exefs:/" + file.Replace("exefs", ""), "OutExefs:/" + file.Substring(6).Replace("\\", "/"));
                } 
                else if (file.Contains("romfs"))
                {
                    result = FSUtils.CopyFile(horizon.horizon.Fs, "romfs:/" + file.Substring(6).Replace("\\", "/"), "OutRomfs:/" + file.Substring(6).Replace("\\", "/"));
                }
                if (result.IsFailure()) return (4, $"Error copying switch Files\n{result.Description}");
            }

            
            for (int i = 0; i < count; i++)
            {
                var oriFile = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";
                var outFile = $"{layeredOut}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";

                var folderFile = Path.GetDirectoryName(outFile);
                if (!Directory.Exists(folderFile))
                    Directory.CreateDirectory(folderFile);

                var resultApplyXdelta = ApplyXdelta(oriFile, xdelta, outFile, files.ListMd5Files[i]);

                if (resultApplyXdelta.Item1 != 0)
                    return resultApplyXdelta;
            }

            UnmountPartition("romfs");
            UnmountPartition("OutRomfs");
            if (NeedExefs)
            {
                UnmountPartition("exefs");
                UnmountPartition("OutExefs");
            }

            return base.ApplyTranslation();
        }

        private void UnmountPartition(string partition)
        {
            horizon.horizon.Fs.Unmount(new U8Span(partition));
        }
    }
}
