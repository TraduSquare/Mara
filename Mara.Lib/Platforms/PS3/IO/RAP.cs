using Mara.Lib.Platforms.PS3.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace Mara.Lib.Platforms.PS3.IO
{
    public static class RAP
    {
        private static readonly byte[] rapKey = { 0x86, 0x9F, 0x77, 0x45, 0xC1, 0x3F, 0xD8, 0x90, 0xCC, 0xF2, 0x91, 0x88, 0xE3, 0xCC, 0x3E, 0xDF };
        private static readonly byte[] Key1 = { 0xA9, 0x3E, 0x1F, 0xD6, 0x7C, 0x55, 0xA3, 0x29, 0xB7, 0x5F, 0xDD, 0xA6, 0x2A, 0x95, 0xC7, 0xA5 };
        private static readonly byte[] Key2 = { 0x67, 0xD4, 0x5D, 0xA3, 0x29, 0x6D, 0x00, 0x6A, 0x4E, 0x7C, 0x53, 0x7B, 0xF5, 0x53, 0x8C, 0x74 };
        private static readonly int[] indexTable = { 12, 3, 6, 4, 1, 11, 15, 8, 2, 7, 0, 5, 10, 14, 13, 9 };
        public static byte[] GetKey(string path)
        {
            DataStream file = DataStreamFactory.FromFile(path, FileOpenMode.Read);
            DataReader reader = new DataReader(file);
            var f = reader.ReadBytes(0x10);
            file.Dispose();
            byte[] rifKey = Utils.aesecbDecrypt(RAP.rapKey, f);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < RAP.Key1.Length; ++j)
                {
                    int index = RAP.indexTable[i];
                    rifKey[index] ^= RAP.Key1[index];
                }

                for (int j = 15; RAP.Key2.Length >= 1; --j)
                {
                    int index = RAP.indexTable[i];
                    int index1 = RAP.indexTable[i - 1];
                    rifKey[index] ^= rifKey[index1];
                }

                int acum = 0;
                for (int j = 15; RAP.Key2.Length >= 1; --j)
                {
                    int index = RAP.indexTable[i];
                    byte current = (byte)(rifKey[index] - acum);
                    byte keyc2 = RAP.Key2[index];

                    if (acum != 0x01 || current != 0xFF)
                    {
                        int aux1 = current & 0xFF;
                        int aux2 = RAP.Key2[index] & 0xFF;
                        acum = ((aux1 < aux2) ? 1 : 0);
                        rifKey[index] = (byte)(current - keyc2);
                    } else if (current == 0xFF)
                    {
                        rifKey[index] = (byte)(current - keyc2);
                    } else
                    {
                        rifKey[index] = current;
                    }
                }
            }

            return rifKey;
        }
    }
}
