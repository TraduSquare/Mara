using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mara.Lib;
using Mara.Lib.Common;
using Mara.Lib.Configs;
using Newtonsoft.Json;

namespace Mara.Generator
{
    public class PatchGenerator
    {
        public string OriPath { get; }
        public string ModPath { get; }
        public string OutPath { get; }

        public PatchGenerator(string oriPassed, string modPassed, string outPassed)
        {
            OriPath = oriPassed;
            ModPath = modPassed;
            OutPath = outPassed;
        }

        public MaraConfig GeneratePatch(MaraConfig config)
        {
            var files = GetListModFiles();
            config.FilesInfo ??= new PatchFilesInfo();
            config.FilesInfo.ListOriFiles = files.Item1;
            config.FilesInfo.ListCopyFiles = files.Item2;
            config.FilesInfo.ListXdeltaFiles = GenerateXdeltas(files.Item1);
            config.FilesInfo.ListMd5Files = files.Item3;
            return config;
        }

        private (string[], string[], string[], string[]) GetListModFiles()
        {
            var listMOD = Directory.GetFiles(ModPath, "*.*", SearchOption.AllDirectories);
            var listOri = Directory.GetFiles(OriPath, "*.*", SearchOption.AllDirectories);
            Dictionary<string, string> modfiles = new Dictionary<string, string>();
            Dictionary<string, string> orifiles = new Dictionary<string, string>();
            List<string> list = new List<string>();
            List<string> Copylist = new List<string>();
            List<string> listMODmd5 = new List<string>();

            foreach (var filepath in listMOD)
            {
                modfiles.Add(filepath.Replace(ModPath + "\\", ""), GenerateMd5(filepath));
            }
            foreach (var filepath in listOri)
            {
                orifiles.Add(filepath.Replace(OriPath + "\\", ""), GenerateMd5(filepath));
            }

            foreach (var keypair in modfiles)
            {
                orifiles.TryGetValue(keypair.Key, out string md5);
                if (!string.IsNullOrEmpty(md5))
                {
                    if (!md5.Equals(keypair.Value))
                    {
                        list.Add(keypair.Key);
                        listMODmd5.Add(keypair.Value);
                    }
                }
                else
                {
                    Copylist.Add(keypair.Key);
                    string path = $"{OutPath}{Path.DirectorySeparatorChar}{keypair.Key}";
                    if (!File.Exists(path))
                    {
                        if (!Directory.Exists(Directory.GetParent(path).ToString()))
                            Directory.CreateDirectory(Directory.GetParent(path).ToString());
                        File.Copy($"{ModPath}{Path.DirectorySeparatorChar}{keypair.Key}", path);
                    }
                        
                }
            }

            /*List<string> list = new List<string>();
            List<string> Copylist = new List<string>();
            for (int i = 0; i < listMOD.Length; i++)
            {
                bool found = false;
                var a = listMOD[i].Replace(ModPath + "\\", "");
                for (int j = 0; j < listOri.Length; j++)
                {
                    var b = listOri[j].Replace(OriPath + "\\", "");
                    if (a.Contains(b))
                    {
                        found = true;
                        if (!listMODmd5[i]
                            .Equals(listOrimd5[j]))
                        {
                            list.Add(a);
                        }
                        break;
                    }
                }

                if (!found)
                {
                    Copylist.Add(a);
                }
            }*/

            return (list.ToArray(), Copylist.ToArray(), listMODmd5.ToArray(), listOri);
        }
        
        private string[] GetListCopyModFiles()
        {
            List<string> files = new List<string>();
            var list = Directory.GetFiles(ModPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = list[i].Replace(ModPath+"\\", "");
                if (!File.Exists($"{OriPath}{Path.DirectorySeparatorChar}{list[i]}"))
                {
                    files.Add(list[i]);
                }
            }

            return files.ToArray();
        }


        private string[] GenerateXdeltas(string[] files)
        {
            var xdelta3 = Path.GetTempFileName();
            var list = new List<string>();
            File.WriteAllBytes(xdelta3, Properties.Resources.xdelta3);

            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists($"{ModPath}{Path.DirectorySeparatorChar}{files[i]}") && File.Exists($"{OriPath}{Path.DirectorySeparatorChar}{files[i]}")) {
                    var xdeltaFile = $"{files[i]}.xdelta";
                    list.Add(xdeltaFile);

                    var dir = Path.GetDirectoryName($"{OutPath}{Path.DirectorySeparatorChar}{xdeltaFile}");

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var generateXdelta = new ProcessStartInfo();
                    {
                        string arguments =
                            $" -e -S -f -9 -s \"{OriPath}{Path.DirectorySeparatorChar}{files[i]}\" \"{ModPath}{Path.DirectorySeparatorChar}{files[i]}\" \"{OutPath}{Path.DirectorySeparatorChar}{xdeltaFile}\"";
                        generateXdelta.FileName = xdelta3;
                        generateXdelta.Arguments = arguments;
                        generateXdelta.UseShellExecute = false;
                        generateXdelta.CreateNoWindow = true;
                        generateXdelta.ErrorDialog = false;
                        generateXdelta.RedirectStandardOutput = true;
                        Process x = Process.Start(generateXdelta);
                        x.WaitForExit();
                    }
                }
            }

            File.Delete(xdelta3);

            return list.ToArray();
        }


        private string[] GenerateMd5(string[] files)
        {
            var m5 = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                m5.Add(Md5.CalculateMd5(files[i]));
            }

            return m5.ToArray();
        }
        
        private string GenerateMd5(string files)
        {
            return Md5.CalculateMd5(files);
        }
    }
}
