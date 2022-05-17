using Mara.Lib.Platforms.PS3.Crypto;
using Yarhl.IO;

namespace Mara.Lib.Platforms.PS3.IO
{
    public static class RAP
    {
        private static readonly byte[] rapKey =
            {0x86, 0x9F, 0x77, 0x45, 0xC1, 0x3F, 0xD8, 0x90, 0xCC, 0xF2, 0x91, 0x88, 0xE3, 0xCC, 0x3E, 0xDF};

        private static readonly byte[] Key1 =
            {0xA9, 0x3E, 0x1F, 0xD6, 0x7C, 0x55, 0xA3, 0x29, 0xB7, 0x5F, 0xDD, 0xA6, 0x2A, 0x95, 0xC7, 0xA5};

        private static readonly byte[] Key2 =
            {0x67, 0xD4, 0x5D, 0xA3, 0x29, 0x6D, 0x00, 0x6A, 0x4E, 0x7C, 0x53, 0x7B, 0xF5, 0x53, 0x8C, 0x74};

        private static readonly int[] indexTable = {12, 3, 6, 4, 1, 11, 15, 8, 2, 7, 0, 5, 10, 14, 13, 9};

        public static byte[] GetKey(string path)
        {
            var file = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            var reader = new DataReader(file);
            var f = reader.ReadBytes(0x10);
            file.Dispose();
            var rifKey = Utils.aesecbDecrypt(rapKey, f);

            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 16; ++j)
                {
                    var index = indexTable[j];
                    rifKey[index] ^= Key1[index];
                }

                for (var j = 15; j >= 1; --j)
                {
                    var index = indexTable[j];
                    var index1 = indexTable[j - 1];
                    rifKey[index] ^= rifKey[index1];
                }

                var acum = 0;
                for (var j = 0; j < 16; ++j)
                {
                    var index = indexTable[j];
                    var current = (byte) (rifKey[index] - acum);
                    var keyc2 = Key2[index];

                    if (acum != 1 || current != 255)
                    {
                        var aux1 = current & 0xFF;
                        var aux2 = Key2[index] & 0xFF;
                        acum = ((aux1 < aux2) ? 1 : 0);
                        rifKey[index] = (byte) (current - keyc2);
                    }
                    else if (current == 255)
                    {
                        rifKey[index] = (byte) (current - keyc2);
                    }
                    else
                    {
                        rifKey[index] = current;
                    }
                }
            }

            return rifKey;
        }
    }
}