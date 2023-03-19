using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mara.Lib.Common.IO;
using Mara.Lib.Platforms.PS3.IO;
using Utils = Mara.Lib.Platforms.PS3.Crypto.Utils;

namespace Mara.Lib.Platforms.PS3
{
    public class Main : PatchProcess
    {
        private byte[] klicense;
        private List<byte[]> m_CID = new();
        private List<string> m_decdlc = new();
        private Dictionary<string, (NPD, byte[])> m_dlcs = new();
        private string[] m_encdlc;
        private string m_GamePath;
        private Dictionary<string, byte[]> m_keys = new();
        
        private string Titleid { get; set; }

        public Main(string oriFolder, string outFolder, OWO filePath, string titleid, string[] RAPsfile, byte[] DEVKEY)
            : base(oriFolder, outFolder, filePath)
        {
            this.Titleid = titleid;
            m_GamePath = Path.Combine(oriFolder, "game", titleid);
            try
            {
                m_encdlc = Directory.GetFiles(Path.Combine(m_GamePath, "USRDIR", "DLC"), "*.EDAT", SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            klicense = DEVKEY;
        }

        public Main(string oriFolder, string outFolder, string filePath, string titleid, string[] RAPsfile,
            byte[] DEVKEY)
            : base(oriFolder, outFolder, filePath)
        {
            this.Titleid = titleid;
            m_GamePath = Path.Combine(oriFolder, "game", titleid);
            try
            {
                m_encdlc = Directory.GetFiles(Path.Combine(m_GamePath, "USRDIR", "DLC"), "*.EDAT", SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            klicense = DEVKEY;
        }

        public override (int, string) ApplyTranslation()
        {
            var count = maraConfig.FilesInfo.ListXdeltaFiles.Length;
            var files = maraConfig.FilesInfo;

            var MANUAL = Path.Combine(outFolder, "MANUAL");
            var game = Path.Combine(outFolder, "USRDIR");
            var dlc = Path.Combine(game, "DLC");

            //InitDirs(new[] { MANUAL, game, dlc });

            for (var i = 0; i < count; i++)
            {
                var oriFile = $"{m_GamePath}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";

                var outFile = $"{m_GamePath}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                var folderFile = Path.GetDirectoryName(outFile);
                if (!Directory.Exists(folderFile))
                    Directory.CreateDirectory(folderFile);

                (int, string) resultApplyXdelta;
                Console.WriteLine($"Parcheando: {oriFile}");
                if (checkDLC(oriFile))
                {
                    oriFile = $"{tempFolder}{Path.DirectorySeparatorChar}DAT{Path.DirectorySeparatorChar}{Path.GetFileName(files.ListOriFiles[i])}";

                    try
                    {
                        resultApplyXdelta = ApplyXdelta(oriFile, xdelta, outFile, files.ListMd5Files[i]);
                        var result = ProccessDLC(outFile);
                        // Ignorar si peta ya que son dlcs
                        /*if (result.Item1 != 0)
                            return result;*/
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    resultApplyXdelta = ApplyXdelta(oriFile, xdelta, outFile, files.ListMd5Files[i]);
                    if (resultApplyXdelta.Item1 != 0)
                        return resultApplyXdelta;
                }
            }

            // Copy Files
            for (var i = 0; i < files.ListCopyFiles.Length; i++)
            {
                var oriFile = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListCopyFiles[i]}";
                var outFile = $"{m_GamePath}{Path.DirectorySeparatorChar}{files.ListCopyFiles[i]}";
                var folderFile = Path.GetDirectoryName(outFile);
                
                Console.WriteLine($"Copiando: {oriFile}");
                
                if (!Directory.Exists(folderFile))
                    Directory.CreateDirectory(folderFile);
                
                if(File.Exists(outFile))
                    File.Delete(outFile);
                
                File.Copy(oriFile, outFile);
            }
            Console.WriteLine("Todo ejecutado correctamente!");
            return (0, "");
        }

        private bool checkDLC(string path)
        {
            var info = new DirectoryInfo(Path.GetDirectoryName(path));
            return path.Contains(".DAT") && info.Name.ToUpper() == "DLC";
        }

        private (int, string) ProccessDLC(string path)
        {
            string finaloutput = path.Replace(".DAT", ".EDAT");
            m_dlcs.TryGetValue(Path.GetFileName(finaloutput + "_ori"), out var data);
            try
            {
                if (data.Item1 != null)
                {
                    EDAT.encryptFile(path, finaloutput, klicense, data.Item2,
                        data.Item1.ContentId, Utils.StringToByteArray("3C"),
                        new[] { (byte)data.Item1.Type }, new[] { (byte)data.Item1.Version });

                    if (File.Exists(path))
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

        public void reloaddlcs(string[] RAPsfile)
        {
            foreach (var RAPfile in RAPsfile)
                if (RAPfile != "")
                    m_keys.Add(RAPfile.Replace(".rap", ""), RAP.GetKey(RAPfile));
    
            if (!Directory.Exists(Path.Combine(tempFolder, "DAT")))
                Directory.CreateDirectory(Path.Combine(tempFolder, "DAT"));
            
            if (m_encdlc != null && m_encdlc.Length > 0)
                foreach (var f in m_encdlc)
            foreach (var key in m_keys)
                try
                {
                    var a = EDAT.decryptFileWithResult(f,
                        Path.Combine(tempFolder, "DAT", Path.GetFileName(f)).Replace("EDAT_ori", "DAT"), this.klicense, key.Value);
                    
                    if (a != null)
                        m_dlcs.Add(Path.GetFileName(f), (a, key.Value));
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
        }

        public void UpdateFolders(string path1, string path2)
        {
            m_GamePath = Path.Combine(oriFolder, "game", Titleid);
            try
            {
                m_encdlc = Directory.GetFiles(Path.Combine(m_GamePath, "USRDIR", "DLC"), "*.EDAT", SearchOption.AllDirectories);
                if (m_encdlc != null && m_encdlc.Length > 0)
                    foreach (var mDlc in m_encdlc)
                    {
                        if(!File.Exists(mDlc.Replace(".EDAT", ".EDAT_ori")))
                            File.Move(mDlc,mDlc.Replace(".EDAT", ".EDAT_ori"));
                    }
            
                m_encdlc = Directory.GetFiles(Path.Combine(m_GamePath, "USRDIR", "DLC"), "*.EDAT_ori", SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
        }
    }
}
