using System;
using System.Collections.Generic;
using System.IO;
using K4os.Compression.LZ4;
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
                var size = LZ4Codec.Encode(owo.m_data, 0, owo.m_size, target, 0, target.Length, LZ4Level.L12_MAX);
                _writer.Write(size);
                var data_con = new byte[size];
                Array.Copy(target, data_con, size);
                _writer.Write(data_con);
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
        var m_OWOS = new List<OWO>();
        for (var i = 0; i < m_file.m_numEntrys; i++)
        {
            var o = new OWO();
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
                LZ4Codec.Decode(
                    o.m_data, 0, o.m_compressedsize,
                    target, 0, target.Length);
                o.m_data = target;
            }

            m_OWOS.Add(o);
        }

        var footer = _reader.ReadString();
        if (!footer.Equals(UWU.m_footer))
            throw new Exception("Firma del archivo incorrecta");

        m_file.m_entry = m_OWOS.ToArray();

        return m_file;
    }

    public static void WriteRomfs(string path)
    {
        var localFs = new LocalFileSystem(path);
        var builder = new RomFsBuilder(localFs);
        var romFs = builder.Build();

        romFs.GetSize(out var romFsSize).ThrowIfFailure();

        using (var outFile = new FileStream("output.owo", FileMode.Create, FileAccess.ReadWrite))
        {
            romFs.CopyToStream(outFile, romFsSize);
        }
    }

    public static OWO ReadOWO(string path)
    {
        var _stream = new DataStream(File.Open(path, FileMode.Open));
        var _reader = new DataReader(_stream);
        var o = new OWO();
        var owo_magic = _reader.ReadInt32();
        if (owo_magic != OWO.m_magic)
            throw new Exception("Sistema de archivo (OWO) no reconocido.");
        o.m_platform = (MaraPlatform)_reader.ReadInt32();
        o.m_size = _reader.ReadInt32();
        o.m_compressedsize = _reader.ReadInt32();
        o.m_data = _reader.ReadBytes(o.m_compressedsize);

        // Descomprimir si es necesario
        if (o.m_size > o.m_compressedsize)
        {
            var target = new byte[o.m_size]; // or source.Length * 255 to be safe
            LZ4Codec.Decode(
                o.m_data, 0, o.m_compressedsize,
                target, 0, target.Length);
            o.m_data = target;
        }

        return o;
    }

    public static void ExtractOWO(OWO m_file, string path)
    {
        var localFs = new StreamStorage(new MemoryStream(m_file.m_data), false);
        var romfs = new RomFsFileSystem(localFs);
        romfs.Extract(path);
    }

    public static OWO SearchOWO(string path, MaraPlatform platform)
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

        for (var i = 0; i < m_file.m_numEntrys; i++)
        {
            var o = new OWO();
            var owo_magic = _reader.ReadInt32();
            if (owo_magic != OWO.m_magic)
                throw new Exception("Sistema de archivo (OWO) no reconocido.");
            o.m_platform = (MaraPlatform)_reader.ReadInt32();
            o.m_size = _reader.ReadInt32();
            o.m_compressedsize = _reader.ReadInt32();
            o.m_data = _reader.ReadBytes(o.m_compressedsize);
            if (o.m_platform != platform)
            {
                _reader.Stream.Position += o.m_compressedsize;
                continue;
            }

            // Descomprimir si es necesario
            if (m_file.m_compressed == 1)
            {
                var target = new byte[o.m_size]; // or source.Length * 255 to be safe
                LZ4Codec.Decode(
                    o.m_data, 0, o.m_compressedsize,
                    target, 0, target.Length);
                o.m_data = target;
            }

            return o;
        }

        return null;
    }

    public static MaraPlatform[] GetPlatformsFromUWU(string path)
    {
        var m_file = new UWU();
        var m_platforms = new List<MaraPlatform>();
        var _stream = new DataStream(File.Open(path, FileMode.Open));
        var _reader = new DataReader(_stream);

        var magic = _reader.ReadInt32();
        if (magic != UWU.m_magic)
            throw new Exception("Sistema de archivo no reconocido.");

        m_file.m_version = _reader.ReadInt32();
        m_file.m_compressed = _reader.ReadByte();
        m_file.m_numEntrys = _reader.ReadInt32();

        for (var i = 0; i < m_file.m_numEntrys; i++)
        {
            var o = new OWO();
            var owo_magic = _reader.ReadInt32();
            if (owo_magic != OWO.m_magic)
                throw new Exception("Sistema de archivo (OWO) no reconocido.");
            o.m_platform = (MaraPlatform)_reader.ReadInt32();
            o.m_size = _reader.ReadInt32();
            o.m_compressedsize = _reader.ReadInt32();
            o.m_data = _reader.ReadBytes(o.m_compressedsize);
            _reader.Stream.Position += o.m_compressedsize;
            m_platforms.Add(o.m_platform);
        }

        return m_platforms.ToArray();
    }

    /* Code: https://stackoverflow.com/a/3967595 */
    public static void SplitFile(string inputFile, int chunkSize, string path)
    {
        File.Move(inputFile, inputFile.Replace('.', '-'));
        inputFile = inputFile.Replace('.', '-');
        const int BUFFER_SIZE = 20 * 2048;
        var buffer = new byte[BUFFER_SIZE];
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        using (Stream input = File.OpenRead(inputFile))
        {
            var index = 0;
            while (input.Position < input.Length)
            {
                using (Stream output = File.Create(path + "\\" + index))
                {
                    int remaining = chunkSize, bytesRead;
                    while (remaining > 0 && (bytesRead = input.Read(buffer, 0,
                               Math.Min(remaining, BUFFER_SIZE))) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                        remaining -= bytesRead;
                    }
                }

                index++;
            }
        }

        SetArchiveBit(path + "\\");
    }

    public static void SetArchiveBit(string path)
    {
        var dir = new DirectoryInfo(path);
        dir.Attributes |= FileAttributes.Archive;
    }
}