using System;
using System.IO;

namespace Mara.Lib.Common.IO
{

    public class UWU
    {
        public static readonly int m_magic = 0x30555755;
        public int m_version { get; set; }
        public int m_numEntrys { get; set; }
        public byte m_compressed { get; set; }
        public OWO[] m_entry;
        public static readonly string m_footer = "MegaFlanLaChupaEnEsteArchivo";

        public UWU()
        {
            m_entry = Array.Empty<OWO>();
            m_version = 2;
            m_numEntrys = 0;
            m_compressed = 0;
        }
    }

    public class OWO
    {
        public static readonly int m_magic = 0x304F574F;
        public MaraConfig.MaraPlatform m_platform = MaraConfig.MaraPlatform.Generic;
        public int m_size;
        public int m_compressedsize;
        public byte[] m_data;

        public OWO()
        {
            
        }
        
        public OWO(string path, MaraConfig.MaraPlatform platform = MaraConfig.MaraPlatform.Generic)
        {
            var data = File.ReadAllBytes(path);
            m_size = data.Length;
            m_compressedsize = data.Length;
            m_data = data;
            m_platform = platform;
        }
    }
}