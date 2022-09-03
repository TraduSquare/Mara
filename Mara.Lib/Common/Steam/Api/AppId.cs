using System;
using System.IO.Hashing;
using System.Text;

namespace Mara.Lib.Common.Steam.Api;

public class AppId
{
    private readonly long m_appid;

    public AppId(string AppName, string ExePath)
    {
        var m = Crc32.Hash(Encoding.Default.GetBytes(ExePath + AppName));
        m_appid = BitConverter.ToInt32(m) << 32;
    }

    public long getShortcutId()
    {
        return (m_appid >> 32) - 0x100000000;
    }

    public long getAppId()
    {
        return m_appid;
    }

    public long getShortAppId()
    {
        return m_appid >> 32;
    }
}