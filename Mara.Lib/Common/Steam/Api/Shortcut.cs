using System;
using System.Collections.Generic;
using System.IO;
using VDFParser;
using VDFParser.Models;

namespace Mara.Lib.Common.Steam.Api;

public static class Shortcut
{
    public static void addGame(string AppName, string ExePath)
    {
        foreach (var f in Users.getUsersIds())
        {
            var vdfpath = Path.Combine(SteamUtils.GetSteamPath(), "userdata", $"{f}", "config", "shortcuts.vdf");
            var vdf = VDFParser.VDFParser.Parse(vdfpath);

            List<VDFEntry> m_entrys = new List<VDFEntry>(vdf);

            VDFEntry m_entry = new VDFEntry();

            AppId m_appid = new AppId(AppName, ExePath);
            
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
            m_entry.appid = (int)m_appid.getAppId();
            m_entry.Icon = "";
            m_entry.Tags = new []{""};
            
            m_entrys.Add(m_entry);
            var test = VDFSerializer.Serialize(m_entrys.ToArray());
            
            File.WriteAllBytes("test.vdf", test);
        }
    }
}