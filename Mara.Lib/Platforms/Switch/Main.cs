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
        private bool BuildRomfs;
        public Main(string oriFolder, string outFolder, string filePath, string Keys, string TitleID, string UpdateFile = null, bool extractExefs = false, bool checkSignature = true, bool buildromfs = true) : base(oriFolder, outFolder, filePath)
        {
            this.titleid = TitleID;
            this.horizon = new HOS(Keys, checkSignature);
            this.NeedExefs = extractExefs;
            this.BuildRomfs = buildromfs;
            if (oriFolder.Contains(".nsp"))
            {
                this.NSP = new PartitionFS(oriFolder);
                if (UpdateFile != null)
                    this.NCAS = new NCA(horizon, this.NSP.MountPFS0(horizon, "base"), new PartitionFS(UpdateFile).MountPFS0(horizon, "Update"));
                else
                    this.NCAS = new NCA(horizon, this.NSP.MountPFS0(horizon, "base"));
            }
            else if (oriFolder.Contains(".xci"))
            {
                this.XCI = new GameCard(horizon, oriFolder);
                if (UpdateFile != null)
                    this.NCAS = new NCA(horizon, this.XCI.MountGameCard(horizon), new PartitionFS(UpdateFile).MountPFS0(horizon, "Update"));
                else
                    this.NCAS = new NCA(horizon, this.XCI.MountGameCard(horizon));
            }
            else
                throw new Exception("Unrecognized file.");

            Result rc = NCAS.MountProgram(horizon, titleid);
            if (rc.IsFailure())
                throw new Exception("Unable to mount the NCAS.");
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
            if (NeedExefs == true)
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

            if (BuildRomfs)
            {
                Romfs romfs = new Romfs(Path.Combine(layeredOut, "romfs"));
                if (romfs.DumpToFile(Path.Combine(layeredOut, "romfs.bin")) != Result.Success)
                    throw new Exception("Failed to build the romfs.bin");
                int filesize = (int)new FileInfo(Path.Combine(layeredOut, "romfs.bin")).Length;
                if (filesize / 1024d / 1024d > 2048)
                {
                    Common.SplitFile.Split(Path.Combine(layeredOut, "romfs.bin"), filesize, Path.Combine(layeredOut, "romfs.bin"));
                    File.Delete(Path.Combine(layeredOut, "romfs-bin"));
                }

                Directory.Delete(Path.Combine(layeredOut, "romfs"), true);
            }

            return base.ApplyTranslation();
        }

        private void UnmountPartition(string partition)
        {
            horizon.horizon.Fs.Unmount(new U8Span(partition));
        }
    }
}
