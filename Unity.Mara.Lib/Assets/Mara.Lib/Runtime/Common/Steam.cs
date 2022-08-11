using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;
using Microsoft.Win32;

#if UNITY_EDITOR && UNITY_STANDALONE

namespace Mara.Lib.Common
{
    /// <summary>
    /// Extrae el listado de appsIDs de los juegos de steam
    /// </summary>
    internal class PackageInfo
    {
        public List<int> AppID = new List<int>();
        public DataReader _reader;
        public PackageInfo(string path)
        {
            if (File.Exists(path))
                _reader = new DataReader(DataStreamFactory.FromFile(path, FileOpenMode.Read));
            else
                throw new Exception("Archivo no encontrado");

            while (true)
            {
                var p = Search(_reader.Stream, new byte[] { 0x61, 0x70, 0x70, 0x69, 0x64, 0x73, 0x00, 0x02, 0x30, 0x00 });
                if (p != -1)
                    AppID.Add(_reader.ReadInt32());
                else
                {
                    break;
                }
            }
        }

        static long Search(Stream stream, byte[] pattern)
        {
            long start = -1;

            while (stream.Position < stream.Length)
            {
                if (stream.ReadByte() != pattern[0])
                    continue;

                start = stream.Position - 1;

                for (int idx = 1; idx < pattern.Length; idx++)
                {
                    if (stream.ReadByte() != pattern[idx])
                    {
                        start = -1;
                        break;
                    }
                }

                if (start > -1)
                {
                    return start;
                }
            }

            return start;
        }
    }

    internal class LibraryFolders
    {
        public List<string> SteamLibraries = new List<string>();
        public LibraryFolders(string path)
        {
            if (File.Exists(path))
            {
                var txt = File.ReadAllLines(path);

                foreach (var s in txt)
                {
                    if (s.Contains("\"path\""))
                    {
                        SteamLibraries.Add(s.Replace("		\"path\"		\"", "").Replace("\"", ""));
                    }
                }
            }
            else
            {
                throw new Exception("Archivo no encontrado");
            }
        }
    }

    public class Steam
    {
        private PackageInfo packageinfo;
        private LibraryFolders SteamGameLibraries;
        public Steam()
        {
            // deberia ser igual en todos lados desconozco si en Deck es diferente
            packageinfo = new PackageInfo(Path.Combine(GetSteamPath(), "appcache", "packageinfo.vdf"));
            SteamGameLibraries = new LibraryFolders(Path.Combine(GetSteamPath(), "config", "libraryfolders.vdf"));
        }

        /// <summary>
        /// Comprueba si el usuario tiene el appid
        /// </summary>
        /// <param name="appid">Appid de la store, puedes obtener el appid en steamDB https://steamdb.info/</param>
        /// <returns></returns>
        public bool IsGameOwned(int appid)
        {
            if (packageinfo.AppID.Contains(appid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetSteamPath()
        {
#if !UNITY_EDITOR && UNITY_STANDALONE
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.LinuxPlayer)
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
#endif
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var paths = new[] { ".steam", ".steam/steam", ".steam/root", ".local/share/Steam" };

                return paths
                    .Select(path => Path.Join(home, path))
                    .FirstOrDefault(steamPath => Directory.Exists(Path.Join(steamPath, "appcache")));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam") ??
                          RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                              .OpenSubKey("SOFTWARE\\Valve\\Steam");

                if (key != null && key.GetValue("SteamPath") is string steamPath)
                {
                    return steamPath;
                }
            }

            throw new PlatformNotSupportedException();
        }

        public static string GetPackageInfoPath()
        {
            return Path.Combine(GetSteamPath(), "appcache", "packageinfo.vdf");
        }

        public static string GetLibraryFoldersPath()
        {
            return Path.Combine(GetSteamPath(), "config", "libraryfolders.vdf");
        }

        public string GetGameFolderPath(string FolderName)
        {
            foreach (var library in GetSteamLibraries())
            {
                string tmpPath = Path.Combine(library, "steamapps", "common", FolderName);
                if (Directory.Exists(tmpPath))
                {
                    return tmpPath;
                }
            }
            return null;
        }

        public List<string> GetSteamLibraries()
        {
            return SteamGameLibraries.SteamLibraries;
        }
    }
}
#endif