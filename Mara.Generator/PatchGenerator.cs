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
            config.FilesInfo.ListOriFiles = files;
            config.FilesInfo.ListXdeltaFiles = GenerateXdeltas(files);
            config.FilesInfo.ListMd5Files = GenerateMd5(files);
            return config;
        }

        private string[] GetListModFiles()
        {
            var list = Directory.GetFiles(ModPath, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = list[i].Replace(ModPath+"\\", "");
            }

            return list;
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

                if (File.Exists($"{ModPath}{Path.DirectorySeparatorChar}{files[i]}") && File.Exists($"{OriPath}{Path.DirectorySeparatorChar}{files[i]}"))
                    m5.Add(Md5.CalculateMd5($"{OriPath}{Path.DirectorySeparatorChar}{files[i]}"));
            }

            return m5.ToArray();
        }
    }
}
