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

            // Check ori folder
            if (!CheckDirectoryOrFile(args[1], true))
                return;

            // Check result folder
            if (!CheckDirectoryOrFile(args[2], true))
                return;

            // Check zip file
            if (!CheckDirectoryOrFile(args[3], false))
                return;

            switch (args[0].ToUpper())
            {
                case "PC":
                    ImportPc(args[1], args[2], args[3]);
                    break;
                case "PSVITA":
                    ImportVita(args[1], args[2], args[3]);
                    break;
                case "Switch":
                    ImportSwitch(args[1], args[2], args[3], args[4], args[5]);
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

        private static void ImportSwitch(string oriFolder, string outFolder, string zipPatch, string keyset, string TitleID, string UpdateFile = null)
        {
            var mainSwitch = new Lib.Platforms.Switch.Main(oriFolder, outFolder, zipPatch, keyset, TitleID, UpdateFile);
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
