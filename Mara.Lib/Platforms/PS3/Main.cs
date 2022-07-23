using System;
using System.IO;
using Mara.Lib.Platforms.PS3.IO;

namespace Mara.Lib.Platforms.PS3;

public class Main : PatchProcess
{
    private string m_GamePath;
    private string[] m_dlc;
    private byte[] m_key = new byte[0x10];
    public Main(string oriFolder, string outFolder, string filePath, string titleid, string RAPfile, byte[] DEVKEY) 
        : base(oriFolder, outFolder, filePath)
    {
        m_GamePath = Path.Combine("game", titleid);
        m_dlc = Directory.GetFiles(Path.Combine(m_GamePath, "DLC"), "*.EDAT", SearchOption.AllDirectories);
        if (RAPfile != "")
            m_key = RAP.GetKey(RAPfile);
        
        foreach (var f in m_dlc)
        {
            EDAT.decryptFile(f,Path.Combine(tempFolder,"DAT").Replace("EDAT","DAT"), DEVKEY, m_key);
        }
    }

    public override (int, string) ApplyTranslation()
    {
        var count = maraConfig.FilesInfo.ListXdeltaFiles.Length;
        var files = maraConfig.FilesInfo;
        
        throw new NotImplementedException();
    }
}