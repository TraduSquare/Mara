using System.Collections.Generic;
using System.IO;
using VDFParser;
using VDFParser.Models;

namespace Mara.Lib.Common.Steam.Api;

public static class Shortcut
{
    public static void addGameAllUsers(string AppName, string ExePath, string icon = "", string grid = "",
        string gridHero = "", string gridLogo = "", string gridp = "")
    {
        foreach (var f in Users.getUsersIds())
        {
            var vdfpath = Path.Combine(SteamUtils.GetSteamPath(), "userdata", $"{f}", "config", "shortcuts.vdf");
            var vdf = VDFParser.VDFParser.Parse(vdfpath);

            var m_entrys = new List<VDFEntry>(vdf);

            var m_entry = new VDFEntry();

            var m_appid = new AppId(AppName, ExePath);

            m_entry.AllowDesktopConfig = 0x1;
            m_entry.AllowOverlay = 0x1;
            m_entry.AppName = AppName;
            m_entry.Devkit = 0x0;
            m_entry.DevkitGameID = "";
            m_entry.DevkitOverrideAppID = 0x0;
            m_entry.Exe = $"\"{ExePath}\"";
            m_entry.IsHidden = 0x0;
            m_entry.LastPlayTime = 0;
            m_entry.LaunchOptions = "";
            m_entry.OpenVR = 0x0;
            m_entry.ShortcutPath = "";
            m_entry.StartDir = $"\"{Path.GetDirectoryName(ExePath)}\"";
            m_entry.appid = (int)m_appid.getShortAppId();
            m_entry.Icon = icon;
            m_entry.Tags = new[] { "" };

            m_entrys.Add(m_entry);
            var vdfbytes = VDFSerializer.Serialize(m_entrys.ToArray());

            if (File.Exists(grid))
                File.Copy(grid,
                    $"{Path.Combine(SteamUtils.GetSteamPath(), "userdata", $"{f}", "config", "grid", $"{m_appid.getShortcutId()}.png")}");

            if (File.Exists(gridHero))
                File.Copy(gridHero,
                    $"{Path.Combine(SteamUtils.GetSteamPath(), "userdata", $"{f}", "config", "grid", $"{m_appid.getShortcutId()}_hero.png")}");

            if (File.Exists(gridLogo))
                File.Copy(gridLogo,
                    $"{Path.Combine(SteamUtils.GetSteamPath(), "userdata", $"{f}", "config", "grid", $"{m_appid.getShortcutId()}_logo.png")}");

            if (File.Exists(gridp))
                File.Copy(gridp,
                    $"{Path.Combine(SteamUtils.GetSteamPath(), "userdata", $"{f}", "config", "grid", $"{m_appid.getShortcutId()}p.png")}");

            File.WriteAllBytes(vdfpath, vdfbytes);
        }
    }
}