using System.Collections.Generic;
using System.IO;

namespace Mara.Lib.Common.Steam.Api;

public class Users
{
    public uint m_userid { get; set; }
    public string m_username { get; set; }


    public static uint[] getUsersIds()
    {
        List<uint> m_users = new List<uint>();
        foreach (var f in Directory.GetDirectories(Path.Combine(SteamUtils.GetSteamPath(), "userdata")))
        {
            m_users.Add(uint.Parse(Path.GetFileName(f)));
        }
        
        return m_users.ToArray();
    }
    
    public static uint[] getUsers()
    {
        List<uint> m_users = new List<uint>();
        foreach (var f in Directory.GetDirectories(Path.Combine(SteamUtils.GetSteamPath(), "userdata")))
        {
            m_users.Add(uint.Parse(Path.GetDirectoryName(f)));
        }
        
        return m_users.ToArray();
    }
}