using System;
using System.Collections.Generic;
using System.IO;
using K4os.Compression.LZ4;
using LibHac.Fs;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.RomFs;
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
        m_file.m_entry = entrys;

        _writer.Write(UWU.m_magic);
        _writer.Write(m_file.m_version);
        _writer.Write(m_file.m_compressed);
        _writer.Write(m_file.m_numEntrys);
        foreach (var owo in m_file.m_entry)
        {
            _writer.Write(OWO.m_magic);
            _writer.Write((int)owo.m_platform);
            if (compress)
            {
                var target = new byte[LZ4Codec.MaximumOutputSize(owo.m_size)];
                _writer.Write(owo.m_size);
                _writer.Write(LZ4Codec.Encode(owo.m_data, 0, owo.m_size, target, 0, target.Length, LZ4Level.L12_MAX));
                File.WriteAllBytes("ad", owo.m_data);
                File.WriteAllBytes("dsa", target);
                _writer.Write(target);
            }
            else
            {
                _writer.Write(owo.m_size);
                _writer.Write(owo.m_compressedsize);
                _writer.Write(owo.m_data);
            }
        }

        _writer.Write(UWU.m_footer);
        _writer.Write(0);
    }

    public static UWU ReadUWU(string path)
    {
        var m_file = new UWU();
        var _stream = new DataStream(File.Open(path, FileMode.Open));
        var _reader = new DataReader(_stream);

        var magic = _reader.ReadInt32();
        if (magic != UWU.m_magic)
            throw new Exception("Sistema de archivo no reconocido.");

        m_file.m_version = _reader.ReadInt32();
        m_file.m_compressed = _reader.ReadByte();
        m_file.m_numEntrys = _reader.ReadInt32();
        List<OWO> m_OWOS = new List<OWO>();
        for (var i = 0; i < m_file.m_numEntrys; i++)
        {
            OWO o = new OWO();
            var owo_magic = _reader.ReadInt32();
            if (owo_magic != OWO.m_magic)
                throw new Exception("Sistema de archivo (OWO) no reconocido.");
            o.m_platform = (MaraPlatform)_reader.ReadInt32();
            o.m_size = _reader.ReadInt32();
            o.m_compressedsize = _reader.ReadInt32();
            o.m_data = _reader.ReadBytes(o.m_compressedsize);
            
            // Descomprimir si es necesario
            if (m_file.m_compressed == 1)
            {
                var target = new byte[o.m_size]; // or source.Length * 255 to be safe
                var decoded = LZ4Codec.Decode(
                    o.m_data, 0, o.m_compressedsize,
                    target, 0, target.Length);
                o.m_data = target;
            }
            m_OWOS.Add(o);
        }

        /*var footer = _reader.ReadString();
        if (!footer.Equals(UWU.m_footer))
            throw new Exception("Firma del archivo incorrecta");*/
        
        m_file.m_entry = m_OWOS.ToArray();
        
        return m_file;
    }

    public static void WriteOWO(string path)
    {
        var localFs = new LocalFileSystem(path);
        RomFsBuilder builder = new RomFsBuilder(localFs);
        IStorage romFs = builder.Build();

        romFs.GetSize(out long romFsSize).ThrowIfFailure();

        using (var outFile = new FileStream("output.owo", FileMode.Create, FileAccess.ReadWrite))
        {
            romFs.CopyToStream(outFile, romFsSize);
        }
    }
}