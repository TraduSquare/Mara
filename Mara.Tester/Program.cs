using System;
using System.IO;

namespace Mara.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MARA TESTER - A simple GUI to test platform patches. DO NOT USE FOR FINAL RELEASES!");
            if (args.Length != 4)
                PrintInfo();

            CheckPaths(args[1], args[2], args[3], args[0]);

            switch (args[0].ToUpper())
            {
                case "PC":
                    ImportPc(args[1], args[2], args[3]);
                    break;
                case "PSVITA":
                    ImportVita(args[1], args[2], args[3]);
                    break;
                case "3DS-GENERAL":
                    Import3dsGeneral(args[1], args[2], args[3]);
                    break;
                case "3DS-SPECIFIC":
                    Import3dsSpecific(args[1], args[2], args[3]);
                    break;
                default:
                    PrintInfo();
                    break;
            }
        }


        private static void ImportVita(string oriFolder, string outFolder, string zipPatch)
        {
            var mainVita = new Lib.Platforms.Vita.Main(oriFolder, outFolder, zipPatch);
            PrintResult(mainVita.ApplyTranslation());
        }

        private static void ImportPc(string oriFolder, string outFolder, string zipPatch)
        {
            var mainVita = new Lib.Platforms.Generic.Main(oriFolder, outFolder, zipPatch);
            PrintResult(mainVita.ApplyTranslation());
        }

        private static void Import3dsGeneral(string oriFolder, string outFolder, string zipPatch)
        {
            var main3DS = new Lib.Platforms._3ds.Main(oriFolder, outFolder, zipPatch, Lib.Platforms._3ds.PatchMode.General);
            PrintResult(main3DS.ApplyTranslation());
        }

        private static void Import3dsSpecific(string oriFolder, string outFolder, string zipPatch)
        {
            var main3DS = new Lib.Platforms._3ds.Main(oriFolder, outFolder, zipPatch, Lib.Platforms._3ds.PatchMode.Specific);
            PrintResult(main3DS.ApplyTranslation());
        }

        private static void PrintResult((int, string) result)
        {
            Console.WriteLine($"RESULT CODE: {result.Item1}\n" +
                              $"MESSAGE: {(string.IsNullOrWhiteSpace(result.Item2)?"OK!":result.Item2)}");
        }

        private static void PrintInfo()
        {
            Console.WriteLine("\nTEST PC PATCH");
            Console.WriteLine("Mara.Tester PC \"ORIGINAL FILES FOLDER\" \"OUT PATCHED FILES FOLDER\" \"ZIP DATA PATH\"");
            Console.WriteLine("\nTEST PS VITA PATCH");
            Console.WriteLine("Mara.Tester PSVITA \"ORIGINAL FILES FOLDER\" \"OUT PATCHED FILES FOLDER\" \"ZIP DATA PATH\"");
        }

        static void CheckPaths(string oriPath, string outPath, string zipPath, string patchPlatform)
        {
            bool isDirectory = true;

            switch (patchPlatform.ToUpper())
            {
                case "3DS-GENERAL":
                    isDirectory = false;
                    break;
                case "3DS-SPECIFIC":
                    isDirectory = false;
                    break;
            }

            // Check ori folder
            if (!CheckDirectoryOrFile(oriPath, isDirectory))
                return;

            // Check result folder
            if (!CheckDirectoryOrFile(outPath, isDirectory))
                return;

            // Check zip file
            if (!CheckDirectoryOrFile(zipPath, false))
                return;
        }

        private static bool CheckDirectoryOrFile(string path, bool isDirectory)
        {
            if (!isDirectory)
            {
                if (File.Exists(path))
                    return true;

                Console.WriteLine($"The file \"{path}\" doesn't exist!");
                return false;

            }

            if (Directory.Exists(path))
                return true;

            Console.WriteLine($"The directory \"{path}\" doesn't exist!");
            return false;
        }
    }
}
