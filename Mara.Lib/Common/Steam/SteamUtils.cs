using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Mara.Lib.Common.Steam;

public class SteamUtils
{
    private readonly PackageInfo packageinfo;
    private readonly LibraryFolders SteamGameLibraries;

    public SteamUtils()
    {
        // deberia ser igual en todos lados desconozco si en Deck es diferente
        packageinfo = new PackageInfo(Path.Combine(GetSteamPath(), "appcache", "packageinfo.vdf"));
        SteamGameLibraries = new LibraryFolders(Path.Combine(GetSteamPath(), "config", "libraryfolders.vdf"));
    }

    /// <summary>
    ///     Comprueba si el usuario tiene el appid
    /// </summary>
    /// <param name="appid">Appid de la store, puedes obtener el appid en steamDB https://steamdb.info/</param>
    /// <returns></returns>
    public bool IsGameOwned(int appid)
    {
        if (packageinfo.AppID.Contains(appid))
            return true;
        return false;
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

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam") ??
                      RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                          .OpenSubKey("SOFTWARE\\Valve\\Steam");

            if (key != null && key.GetValue("SteamPath") is string steamPath) return steamPath;
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
            var tmpPath = Path.Combine(library, "steamapps", "common", FolderName);
            if (Directory.Exists(tmpPath)) return tmpPath;
        }

        return null;
    }

    public List<string> GetSteamLibraries()
    {
        return SteamGameLibraries.SteamLibraries;
    }
    
    /// <summary>
    /// Request to steam install a game
    /// </summary>
    /// <param name="appID">The game appID</param>
    public static void InstallGame(long appID)
    {
        Internet.OpenUrl($"steam://install/{appID}");
    }
    
    /// <summary>
    /// Request to steam uninstall a game
    /// </summary>
    /// <param name="appID">The game appID</param>
    public static void UninstallGame(long appID)
    {
        Internet.OpenUrl($"steam://uninstall/{appID}");
    }
    
    /// <summary>
    /// Request to steam validate the game files. this makes uninstall mods.
    /// </summary>
    /// <param name="appID">The game appID</param>
    public static void ValidateGameFiles(long appID)
    {
        Internet.OpenUrl($"steam://validate/{appID}");
    }
}