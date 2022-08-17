using System.IO;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;
using Yarhl.IO;

namespace Mara.Lib.Common.IO;

public class Utils
{
    public static void WriteUWU(string path, OWO[] entrys, bool compress = true)
    {
        var m_file = new UWU();
        var _stream = new DataStream(File.Create(path));
        var _writer = new DataWriter(_stream);

        if (compress)
            m_file.m_compressed = 1;
        else
            m_file.m_compressed = 0;

        m_file.m_numEntrys = entrys.Length;
        m_file.m_table.m_entry = entrys;

        _writer.Write(m_file.m_magic);
        _writer.Write(m_file.m_version);
        _writer.Write(m_file.m_compressed);
        _writer.Write(m_file.m_numEntrys);
        foreach (var owo in m_file.m_table.m_entry)
        {
            _writer.Write(owo.m_magic);
            _writer.Write((int)owo.m_platform);
            if (compress)
            {
                var target = new byte[LZ4Codec.MaximumOutputSize(owo.m_size)];
                _writer.Write(target.Length);
                LZ4Codec.Encode(owo.m_data, 0, owo.m_size, target, 0, target.Length);
                _writer.Write(target);
            }
            else
            {
                _writer.Write(owo.m_size);
                _writer.Write(owo.m_data);
            }
        }

        _writer.Write(m_file.m_footer);
        _writer.Write(0);
    }
}