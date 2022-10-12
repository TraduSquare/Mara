using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mara.Lib.Common.IO;
using Mara.Lib.Platforms.PS3.IO;
using Utils = Mara.Lib.Platforms.PS3.Crypto.Utils;

namespace Mara.Lib.Platforms.PS3;

public class Main : PatchProcess
{
    private readonly List<string> m_decdlc = new();
    private readonly string[] m_encdlc;
    private readonly string m_GamePath;
    private readonly Dictionary<string, byte[]> m_keys = new();
    private readonly Dictionary<string, (NPD, byte[])> m_dlcs = new();
    private readonly byte[] klicense;
    private readonly List<byte[]> m_CID = new();

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
    
    public Main(string oriFolder, string outFolder, string filePath, string titleid, string[] RAPsfile, byte[] DEVKEY, string[] CIDs)
        : base(oriFolder, outFolder, filePath)
    {
        m_GamePath = Path.Combine(oriFolder, "game", titleid);
        m_encdlc = Directory.GetFiles(Path.Combine(m_GamePath, "USRDIR", "DLC"), "*.EDAT", SearchOption.AllDirectories);
        klicense = DEVKEY;
        
        foreach (var RAPfile in RAPsfile)
            if (RAPfile != "")
                m_keys.Add(RAPfile.Replace(".rap", ""), RAP.GetKey(RAPfile));

        foreach (var cid in CIDs)
        {
            m_CID.Add(Utils.StringToByteArray(BitConverter.ToString(Encoding.UTF8.GetBytes(cid))
                .Replace("-", "")));
        }
        
        /*if (!Directory.Exists(Path.Combine(tempFolder, "DAT")))
            Directory.CreateDirectory(Path.Combine(tempFolder, "DAT"));*/

        foreach (var f in m_encdlc)
        foreach (var key in m_keys)
            try
            {
                var result = EDAT.decryptFileWithResult(f, Path.Combine(Path.Combine(m_GamePath, "USRDIR", "DLC"), Path.GetFileName(f)).Replace(".EDAT", ".DAT"), DEVKEY,
                    key.Value);
                m_dlcs.Add(Path.GetFileName(f.Replace(".EDAT", ".DAT")), (result, key.Value));
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
            
            if(!checkDLC(oriFile))
                if (resultApplyXdelta.Item1 != 0)
                    return resultApplyXdelta;

            if (checkDLC(oriFile))
            {
                var result = ProccessDLC(outFile);
                if (result.Item1 != 0)
                    return result;
            }
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

    private bool checkDLC(string path)
    {
        var info = new DirectoryInfo(Path.GetDirectoryName(path));
        return path.Contains(".DAT") && info.Name.ToUpper() == "DLC";
    }

    private (int, string) ProccessDLC(string path)
    {
        m_dlcs.TryGetValue(Path.GetFileName(path), out (NPD, byte[])data);
        try
        {
            if (data.Item1 != null)
            {
                EDAT.encryptFile(path, path.Replace(".DAT", ".EDAT"), klicense, data.Item2, 
                    data.Item1.ContentId, Utils.StringToByteArray("3C"), 
                    new []{ (byte)data.Item1.Type }, new []{(byte)data.Item1.Version});
                
                if(File.Exists(path))
                    File.Delete(path);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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