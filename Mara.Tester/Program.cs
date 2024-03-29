﻿using System;
using System.Collections.Generic;
using System.IO;
using Mara.Lib.Common.IO;

namespace Mara.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MARA TESTER - A simple GUI to test platform patches. DO NOT USE FOR FINAL RELEASES!");
            /*if (args.Length != 4)
                PrintInfo();

            // Check ori folder
            if (!CheckDirectoryOrFile(args[1], true))
                return;

            // Check result folder
            if (!CheckDirectoryOrFile(args[2], true))
                return;

            // Check zip file
            if (!CheckDirectoryOrFile(args[3], false))
                return;*/

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
                case "PS3":
                    var raps = Directory.GetFiles(args[5], "*.rap", SearchOption.AllDirectories);
                    var devkey = StringToByteArrayFastest(args[6]);
                    ImportPs3(args[1], args[2], args[3], args[4], raps, devkey);
                    break;
                case "UWU":
                    var romfs = Directory.GetFiles(args[1], "*.owo", SearchOption.AllDirectories);
                    List<OWO> m_owos = new List<OWO>();
                    foreach (var romf in romfs)
                    {
                        OWO o = new OWO(romf);
                        m_owos.Add(o);
                    }
                    Mara.Lib.Common.IO.Utils.WriteUWU("output.uwu", m_owos.ToArray(), true);
                    break;
                case "OWOE":
                    var uwu = Utils.ReadUWU(args[1]);
                    int meme = 0;
                    foreach (var VARIABLE in uwu.m_entry)
                    {
                        File.WriteAllBytes($"file_{meme}.OWO", VARIABLE.m_data);
                        meme++;
                    }
                    break;
                case "OWO":
                    Utils.WriteRomfs(args[1]);
                    break;
                default:
                    PrintInfo();
                    break;
            }
        }
        
        private static byte[] StringToByteArrayFastest(string hex) {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }
        
        private static int GetHexVal(char hex) {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
        
        private static void ImportPs3(string oriFolder, string outFolder, string filePath, string titleid, string[] RAPsfile, byte[] DEVKEY)
        {
            Console.Write("PS3 MODE!");
            var mainPs3 = new Lib.Platforms.PS3.Main(oriFolder, outFolder, filePath, titleid, RAPsfile, DEVKEY, new []{ "" });
            PrintResult(mainPs3.ApplyTranslation());
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
