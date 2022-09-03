using System;
using System.Collections.Generic;
using System.IO;
using Yarhl.IO;

namespace Mara.Lib.Common.Steam;

/// <summary>
///     Extrae el listado de appsIDs de los juegos de steam
/// </summary>
internal class PackageInfo
{
    public DataReader _reader;
    public List<int> AppID = new();

    public PackageInfo(string path)
    {
        if (File.Exists(path))
            _reader = new DataReader(DataStreamFactory.FromFile(path, FileOpenMode.Read));
        else
            throw new Exception("Archivo no encontrado");

        while (true)
        {
            var p = Search(_reader.Stream, new byte[] { 0x61, 0x70, 0x70, 0x69, 0x64, 0x73, 0x00, 0x02, 0x30, 0x00 });
            if (p != -1)
                AppID.Add(_reader.ReadInt32());
            else
                break;
        }
    }

    private static long Search(Stream stream, byte[] pattern)
    {
        long start = -1;

        while (stream.Position < stream.Length)
        {
            if (stream.ReadByte() != pattern[0])
                continue;

            start = stream.Position - 1;

            for (var idx = 1; idx < pattern.Length; idx++)
                if (stream.ReadByte() != pattern[idx])
                {
                    start = -1;
                    break;
                }

            if (start > -1) return start;
        }

        return start;
    }
}