using System;
using System.Collections.Generic;
using System.IO;
using Yarhl.IO;

namespace Mara.Lib.Platforms.Vita
{
    // Optimized version from this
    // https://github.com/KuromeSan/Sfo.NET/blob/master/ParamSfo.cs
    public class Sfo
    {
        const int PSF_TYPE_BIN = 0;
        const int PSF_TYPE_STR = 2;
        const int PSF_TYPE_VAL = 4;

        public string Magic = "\0PSF";
        public uint Version { get; set; }
        public uint KeyOffset { get; set; }
        public uint ValueOffset { get; set; }
        public uint Count { get; set; }
        public Dictionary<string, object> SfoValues { get; set; }

        public Sfo(string sfoPath)
        {
            SfoValues = new Dictionary<string, object>();
            ReadSfo(sfoPath);
        }

        private void ReadSfo(string sfoPath)
        {
            var reader = new DataReader(DataStreamFactory.FromFile(sfoPath, FileOpenMode.Read));

            // Read Sfo Header
            var magic = reader.ReadString(4);
            Version = reader.ReadUInt32();
            KeyOffset = reader.ReadUInt32();
            ValueOffset = reader.ReadUInt32();
            Count = reader.ReadUInt32();

            if (Magic != magic)
                throw new InvalidDataException("Sfo Magic is Invalid.");

            for (var i = 0; i < Count; i++)
            {
                var nameOffset = reader.ReadUInt16();
                var alignment = reader.ReadByte();
                var type = reader.ReadByte();
                var valueSize = reader.ReadUInt32();
                var totalSize = reader.ReadUInt32();
                var dataOffset = reader.ReadUInt32();

                var keyLocation = Convert.ToInt32(KeyOffset + nameOffset);

                var currentPosition = reader.Stream.Position;
                reader.Stream.Position = keyLocation;

                var keyName = reader.ReadString();
                var valueLocation = Convert.ToInt32(ValueOffset + dataOffset);

                object Value = "Unknown Type";


                switch (type)
                {
                    case PSF_TYPE_STR:
                        reader.Stream.Position = valueLocation;
                        Value = reader.ReadString();
                        break;

                    case PSF_TYPE_VAL:
                        reader.Stream.Position = valueLocation + i;
                        Value = reader.ReadUInt32();
                        break;

                    case PSF_TYPE_BIN:
                        reader.Stream.Position = valueLocation + i;
                        Value = reader.ReadBytes(Convert.ToInt32(valueSize));
                        break;
                }

                SfoValues[keyName] = Value;

                reader.Stream.Position = currentPosition;
            }
        }
    }
}
