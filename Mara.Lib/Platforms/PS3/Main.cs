using System;
using System.Collections.Generic;
using System.IO;
using Mara.Lib.Platforms.PS3.IO;

namespace Mara.Lib.Platforms.PS3;

public class Main : PatchProcess
{
    private readonly List<string> m_decdlc = new();
    private readonly string[] m_encdlc;
    private readonly string m_GamePath;
    private readonly byte[] m_key = new byte[0x10];

    public Main(string oriFolder, string outFolder, string filePath, string titleid, string RAPfile, byte[] DEVKEY)
        : base(oriFolder, outFolder, filePath)
    {
        m_GamePath = Path.Combine("game", titleid);
        m_encdlc = Directory.GetFiles(Path.Combine(m_GamePath, "DLC"), "*.EDAT", SearchOption.AllDirectories);
        if (RAPfile != "")
            m_key = RAP.GetKey(RAPfile);

        foreach (var f in m_encdlc)
        {
            m_decdlc.Add(Path.Combine(tempFolder, "DAT").Replace("EDAT", "DAT"));
            EDAT.decryptFile(f, Path.Combine(tempFolder, "DAT").Replace("EDAT", "DAT"), DEVKEY, m_key);
        }
    }

    public override (int, string) ApplyTranslation()
    {
        var count = maraConfig.FilesInfo.ListXdeltaFiles.Length;
        var files = maraConfig.FilesInfo;

        var MANUAL = Path.Combine(outFolder, "MANUAL");
        var game = Path.Combine(outFolder, "USRDIR");

        InitDirs(MANUAL, game);

        for (var i = 0; i < count; i++)
        {
            var oriFile = $"{oriFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
            var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";

            var outFile = $"{game}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
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
            var outFile = $"{game}{Path.DirectorySeparatorChar}{files.ListCopyFiles[i]}";
            var folderFile = Path.GetDirectoryName(outFile);
            if (!Directory.Exists(folderFile))
                Directory.CreateDirectory(folderFile);

            File.Copy(oriFile, outFile);
        }

        throw new NotImplementedException();
    }

    private void InitDirs(string p1, string p2)
    {
        if (!Directory.Exists(p1))
            Directory.CreateDirectory(p1);
        if (!Directory.Exists(p2))
            Directory.CreateDirectory(p2);
    }
}