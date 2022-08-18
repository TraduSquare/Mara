using System;
using System.Collections.Generic;
using System.IO;
using Mara.Lib.Common.IO;
using Mara.Lib.Platforms.PS3.IO;

namespace Mara.Lib.Platforms.PS3;

public class Main : PatchProcess
{
    private readonly List<string> m_decdlc = new();
    private readonly string[] m_encdlc;
    private readonly string m_GamePath;
    private readonly Dictionary<string, byte[]> m_keys = new();

    public Main(string oriFolder, string outFolder, OWO filePath, string titleid, string[] RAPsfile, byte[] DEVKEY)
        : base(oriFolder, outFolder, filePath)
    {
        m_GamePath = Path.Combine(oriFolder, "game", titleid);
        m_encdlc = Directory.GetFiles(Path.Combine(m_GamePath, "USRDIR", "DLC"), "*.EDAT", SearchOption.AllDirectories);
        foreach (var RAPfile in RAPsfile)
            if (RAPfile != "")
                m_keys.Add(RAPfile.Replace(".rap", ""), RAP.GetKey(RAPfile));

        if (!Directory.Exists(Path.Combine(tempFolder, "DAT")))
            Directory.CreateDirectory(Path.Combine(tempFolder, "DAT"));

        foreach (var f in m_encdlc)
        foreach (var key in m_keys)
            try
            {
                EDAT.decryptFile(f, Path.Combine(tempFolder, "DAT", Path.GetFileName(f)).Replace("EDAT", "DAT"), DEVKEY,
                    key.Value);
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }
    
    public Main(string oriFolder, string outFolder, string filePath, string titleid, string[] RAPsfile, byte[] DEVKEY)
        : base(oriFolder, outFolder, filePath)
    {
        m_GamePath = Path.Combine(oriFolder, "game", titleid);
        m_encdlc = Directory.GetFiles(Path.Combine(m_GamePath, "USRDIR", "DLC"), "*.EDAT", SearchOption.AllDirectories);
        foreach (var RAPfile in RAPsfile)
            if (RAPfile != "")
                m_keys.Add(RAPfile.Replace(".rap", ""), RAP.GetKey(RAPfile));

        if (!Directory.Exists(Path.Combine(tempFolder, "DAT")))
            Directory.CreateDirectory(Path.Combine(tempFolder, "DAT"));

        foreach (var f in m_encdlc)
        foreach (var key in m_keys)
            try
            {
                EDAT.decryptFile(f, Path.Combine(tempFolder, "DAT", Path.GetFileName(f)).Replace("EDAT", "DAT"), DEVKEY,
                    key.Value);
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }

    public override (int, string) ApplyTranslation()
    {
        var count = maraConfig.FilesInfo.ListXdeltaFiles.Length;
        var files = maraConfig.FilesInfo;

        var MANUAL = Path.Combine(outFolder, "MANUAL");
        var game = Path.Combine(outFolder, "USRDIR");
        var dlc = Path.Combine(game, "DLC");

        InitDirs(new[] { MANUAL, game, dlc });

        for (var i = 0; i < count; i++)
        {
            var oriFile = $"{m_GamePath}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
            var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";

            var outFile = $"{outFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
            var folderFile = Path.GetDirectoryName(outFile);
            if (!Directory.Exists(folderFile))
                Directory.CreateDirectory(folderFile);

            var resultApplyXdelta = ApplyXdelta(oriFile, xdelta, outFile, files.ListMd5Files[i]);

            if (resultApplyXdelta.Item1 != 0)
                return resultApplyXdelta;
        }

        // Copy Files
        for (var i = 0; i < files.ListCopyFiles.Length; i++)
        {
            var oriFile = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListCopyFiles[i]}";
            var outFile = $"{outFolder}{Path.DirectorySeparatorChar}{files.ListCopyFiles[i]}";
            var folderFile = Path.GetDirectoryName(outFile);
            if (!Directory.Exists(folderFile))
                Directory.CreateDirectory(folderFile);

            File.Copy(oriFile, outFile);
        }

        return (0, "");
    }

    private void InitDirs(string[] Folders)
    {
        foreach (var dir in Folders)
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
    }
}