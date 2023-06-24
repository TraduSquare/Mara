using System;
using System.IO;
using LibHac;
using LibHac.Common;
using LibHac.Fs.Fsa;
using Mara.Lib.Common;
using Mara.Lib.Common.IO;

namespace Mara.Lib.Platforms.Switch;

public class Main : PatchProcess
{
    private readonly bool BuildRomfs;
    private readonly bool NeedExefs;
    private readonly string titleid;
    public HOS horizon;
    public NCA NCAS;
    public PartitionFS NSP;
    public GameCard XCI;

    public Main(string oriFolder, string outFolder, string filePath, string Keys, string TitleID,
        string UpdateFile = null, bool extractExefs = false, bool checkSignature = true, bool buildromfs = true) : base(
        oriFolder, outFolder, filePath)
    {
        titleid = TitleID;
        horizon = new HOS(Keys, checkSignature);
        NeedExefs = extractExefs;
        BuildRomfs = buildromfs;
        if (oriFolder.Contains(".nsp"))
        {
            NSP = new PartitionFS(oriFolder);
            if (UpdateFile != null)
                NCAS = new NCA(horizon, NSP.MountPFS0(horizon, "base"),
                    new PartitionFS(UpdateFile).MountPFS0(horizon, "Update"));
            else
                NCAS = new NCA(horizon, NSP.MountPFS0(horizon, "base"));
        }
        else if (oriFolder.Contains(".xci"))
        {
            XCI = new GameCard(horizon, oriFolder);
            if (UpdateFile != null)
                NCAS = new NCA(horizon, XCI.MountGameCard(horizon),
                    new PartitionFS(UpdateFile).MountPFS0(horizon, "Update"));
            else
                NCAS = new NCA(horizon, XCI.MountGameCard(horizon));
        }
        else
        {
            throw new Exception("Unrecognized file.");
        }

        var rc = NCAS.MountProgram(horizon, titleid);
        if (rc.IsFailure())
            throw new Exception("Unable to mount the NCAS.");
    }
    
    public Main(string oriFolder, string outFolder, OWO filePath, string Keys, string TitleID,
        string UpdateFile = null, bool extractExefs = false, bool checkSignature = true, bool buildromfs = true) : base(
        oriFolder, outFolder, filePath)
    {
        titleid = TitleID;
        horizon = new HOS(Keys, checkSignature);
        NeedExefs = extractExefs;
        BuildRomfs = buildromfs;
        if (oriFolder.Contains(".nsp"))
        {
            NSP = new PartitionFS(oriFolder);
            if (UpdateFile != null)
                NCAS = new NCA(horizon, NSP.MountPFS0(horizon, "base"),
                    new PartitionFS(UpdateFile).MountPFS0(horizon, "Update"));
            else
                NCAS = new NCA(horizon, NSP.MountPFS0(horizon, "base"));
        }
        else if (oriFolder.Contains(".xci"))
        {
            XCI = new GameCard(horizon, oriFolder);
            if (UpdateFile != null)
                NCAS = new NCA(horizon, XCI.MountGameCard(horizon),
                    new PartitionFS(UpdateFile).MountPFS0(horizon, "Update"));
            else
                NCAS = new NCA(horizon, XCI.MountGameCard(horizon));
        }
        else
        {
            throw new Exception("Unrecognized file.");
        }

        var rc = NCAS.MountProgram(horizon, titleid);
        if (rc.IsFailure())
            throw new Exception("Unable to mount the NCAS.");
    }

    public override (int, string) ApplyTranslation()
    {
        var count = maraConfig.FilesInfo.ListOriFiles.Length;
        var fileTemp = $"{tempFolder}{Path.DirectorySeparatorChar}files";
        var exefsdir = $"{tempFolder}{Path.DirectorySeparatorChar}exefs";
        var romfsdir = $"{tempFolder}{Path.DirectorySeparatorChar}romfs";
        var romfs_romdir = $"{tempFolder}{Path.DirectorySeparatorChar}romfs_rom";
        var layeredOut =
            $"{Path.GetDirectoryName(oriFolder)}{Path.DirectorySeparatorChar}LayeredFS{Path.DirectorySeparatorChar}{titleid}";

        /* Init Dirs */
        if (!Directory.Exists(fileTemp))
            Directory.CreateDirectory(fileTemp);
        if (NeedExefs)
            if (!Directory.Exists(exefsdir))
                Directory.CreateDirectory(exefsdir);
        if (!Directory.Exists(romfsdir))
            Directory.CreateDirectory(romfsdir);
        if (!Directory.Exists(romfsdir))
            Directory.CreateDirectory(romfsdir);
        if (!Directory.Exists(layeredOut))
            Directory.CreateDirectory(layeredOut);
        if (!Directory.Exists(romfs_romdir))
            Directory.CreateDirectory(romfs_romdir);

        var files = maraConfig.FilesInfo;
        Result result;
        if (NeedExefs)
        {
            result = FSUtils.MountFolder(horizon.horizon.Fs, exefsdir, "OutExefs");
            if (result.IsFailure()) return (2, $"Error mounting exeFs\n{result.Description}");
        }

        result = FSUtils.MountFolder(horizon.horizon.Fs, romfsdir, "OutRomfs");
        if (result.IsFailure()) return (3, $"Error mounting romFs\n{result.Description}");
        foreach (var file in files.ListOriFiles)
        {
            // Supongamos que en array de archivos tiene un archivo que está en exefs/main hay
            // que limpiar el "exefs/" ya que esa ruta no existe en el sistem de archivos montado e igual con el romfs.
            if (file.Contains("exefs") && NeedExefs)
                result = FSUtils.CopyFile(horizon.horizon.Fs, "exefs:/" + file.Replace("exefs", ""),
                    "OutExefs:/" + file.Substring(6).Replace("\\", "/"));
            else if (file.Contains("romfs"))
                result = FSUtils.CopyFile(horizon.horizon.Fs, "romfs:/" + file.Substring(6).Replace("\\", "/"),
                    "OutRomfs:/" + file.Substring(6).Replace("\\", "/"));
            if (result.IsFailure()) return (4, $"Error copying switch Files\n{result.Description}");
        }


        for (var i = 0; i < count; i++)
        {
            var oriFile = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
            var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";
            string outFile;

            if (BuildRomfs)
                outFile =
                    $"{tempFolder}{Path.DirectorySeparatorChar}romfs_rom{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
            else
                outFile = $"{layeredOut}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";

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
        
        foreach (var t in files.ListCopyFiles)
        {
            var oriFile = $"{tempFolder}{Path.DirectorySeparatorChar}{t}";
            var outFile = $"{oriFolder}{Path.DirectorySeparatorChar}{t}";
            var folderFile = Path.GetDirectoryName(outFile);

            if (!Directory.Exists(folderFile))
                Directory.CreateDirectory(folderFile);
                
            if(File.Exists(outFile))
                File.Delete(outFile);
                
            File.Copy(oriFile, outFile);
        }

        if (BuildRomfs)
        {
            var romfs = new Romfs(Path.Combine(romfs_romdir, "romfs"));
            if (romfs.DumpToFile(Path.Combine(layeredOut, "romfs.bin")) != Result.Success)
                throw new Exception("Failed to build the romfs.bin");
            var filesize = (int)new FileInfo(Path.Combine(layeredOut, "romfs.bin")).Length;
            if (filesize / 1024d / 1024d > 2048)
            {
                Utils.SplitFile(Path.Combine(layeredOut, "romfs.bin"), filesize, Path.Combine(layeredOut, "romfs.bin"));
                File.Delete(Path.Combine(layeredOut, "romfs-bin"));
            }
        }

        return base.ApplyTranslation();
    }

    private void UnmountPartition(string partition)
    {
        horizon.horizon.Fs.Unmount(new U8Span(partition));
    }
}