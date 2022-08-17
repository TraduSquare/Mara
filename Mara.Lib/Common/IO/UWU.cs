using System;
using System.IO;

namespace Mara.Lib.Common.IO;

public struct UWU
{
    public readonly int m_magic = 0x30555755;
    public int m_version { get; set; }
    public int m_numEntrys { get; set; }
    public byte m_compressed { get; set; }
    public OWOTable m_table;
    public readonly string m_footer = "MegaFlanLaChupaEnEsteArchivo";

    public UWU()
    {
        m_table = new OWOTable();
        m_version = 1;
        m_numEntrys = 0;
        m_compressed = 0;
    }
}

public struct OWOTable
{
    public OWO[] m_entry;

    public OWOTable()
    {
        m_entry = new OWO[(int)MaraPlatform.None];
    }
}

public struct OWO
{
    public readonly int m_magic = 0x304F574F;
    public MaraPlatform m_platform = MaraPlatform.Generic;
    public int m_size;
    public byte[] m_data;

    public OWO(string path, MaraPlatform platform = MaraPlatform.Generic)
    {
        var data = File.ReadAllBytes(path);
        m_magic = 0x304F574F;
        m_size = data.Length;
        m_data = data;
        m_platform = platform;
    }
}