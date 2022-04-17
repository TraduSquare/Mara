using Mara.Lib.Platforms.PS3.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace Mara.Lib.Platforms.PS3.IO
{
    public static class EDAT
    {
        public static void decryptFile(string InPath, string OutPath, byte[] klicense, byte[] keyFromRif)
        {
            DataStream file = DataStreamFactory.FromFile(InPath, FileOpenMode.Read);
            DataReader reader = new DataReader(file);

            NPD npd = validateNPD(Path.GetFileName(InPath), klicense, reader);
            if (npd == null)
            {
                throw new Exception("Invalid NPD");
            }
            else
            {
                Console.WriteLine("NPD valid!");
                EDAT_Data data = getEDATData(reader);
                byte[] rifkey = getKey(npd, data, klicense, keyFromRif);
                
                if (rifkey == null) {
                    throw new Exception("ERROR: Key for decryption is missing");
                }
                Console.WriteLine($"DECRYPTION KEY: {BitConverter.ToString(rifkey)}");

                if (!checkHeader(rifkey, data, npd, reader))
                {
                    throw new Exception("Error verifying header.");
                }

                if (decryptData(reader, npd, data, rifkey))
                {
                    
                }
            }
        }

        private static bool decryptData(DataReader reader, NPD npd, EDAT_Data data, byte[] rifkey)
        {
            int numBlocks = (int) ((data.fileLen + data.blockSize - 1L) / data.blockSize);
            int metadataSectionSize = ((data.flags & 0x1L) != 0x0L || (data.flags & 0x20L) != 0x0L) ? 32 : 16;
            int baseOffset = 256;
            int keyIndex = 0;
            
            if (npd.Version == 4L) {
                keyIndex = 1;
            }

            for (int i = 0; i < numBlocks; ++i)
            {
                reader.Stream.Seek(baseOffset + i * metadataSectionSize, SeekOrigin.Begin);
                byte[] expectedHash = new byte[20];
                int compressionEndBlock = 0;
                long offset;
                int len;

                if ((data.flags & 0x1L) != 0x0L)
                {
                    
                }
            }
            
            throw new NotImplementedException();
        }

        private static bool checkHeader(byte[] rifkey, EDAT_Data data, NPD npd, DataReader reader)
        {
            reader.Stream.Seek(0, SeekOrigin.Begin);
            byte[] header = new byte[160];
            byte[] outBytes = new byte[160];
            byte[] expectedHash = new byte[16];
            
            Console.WriteLine($"Checking NPD Version: {npd.Version}");
            Console.WriteLine($"EDATA Flag: 0x{data.flags}");
            
            if (npd.Version == 0L || npd.Version == 1L) {
                if ((data.flags & 0x7FFFFFFEL) != 0x0L) {
                    return false;
                }
            } else if (npd.Version == 2L) {
                if ((data.flags & 0x7EFFFFE0L) != 0x0L) {
                    return false;
                }
            } else {
                if (npd.Version != 3L && npd.Version != 4L)
                {
                    throw new Exception($"ERROR: VERSION {npd.Version} DETECTED");
                }
                if ((data.flags & 0x7EFFFFC0L) != 0x0L) {
                    return false;
                }
            }
            
            if ((data.flags & 0x20L) != 0x0L && (data.flags & 0x1L) != 0x0L) {
                return false;
            }

            header = reader.ReadBytes(header.Length);
            expectedHash = reader.ReadBytes(expectedHash.Length);
            
            // Comprobar Hash
            int hashFlag = ((data.flags & 0x8L) == 0x0L) ? 2 : 268435458;
            if ((data.flags & 0x80000000L) != 0x0L) {
                hashFlag |= 0x1000000;
            }
            int keyIndex = 0;
            if (npd.Version == 4L) {
                keyIndex = 1;
            }
            // TODO: Cosas que no entiendo
            return true;
        }

        private static byte[] getKey(NPD npd, EDAT_Data data, byte[] klicense, byte[] keyFromRif)
        {
            byte[] result = null;
            
            if ((data.flags & 0x1000000L) != 0x0L)
            {
                result = new byte[16];
                result = Utils.XOR(npd.DevHash, EDAT_Keys.SDATKEY);
            } 
            else if (npd.License == 3L)
            {
                result = klicense;
            } 
            else if (npd.License == 2L)
            {
                result = keyFromRif;
            }

            return result;
        }

        private static EDAT_Data getEDATData(DataReader reader)
        {
            reader.Stream.Seek(128L, SeekOrigin.Begin);
            return EDAT_Data.CreateEDAT_Data(reader);
        }

        public static NPD validateNPD(string filename, byte[] devKLic, DataReader reader)
        {
            reader.Stream.Seek(0, SeekOrigin.Begin);

            byte[] npd = reader.ReadBytes(128);
            long flags = reader.ReadInt32();

            if ((flags & 0x1000000L) != 0x0L)
            {
                throw new Exception("SDAT not supported");
            } 
            else
            {
                if (!checkNPDHash1(filename, npd)) {
                    return null;
                } 
                else if (checkNPDHash2(devKLic, npd))
                {
                    return null;
                }
            }
            
            return NPD.CreateNPD(npd);
        }

        private static bool checkNPDHash2(byte[] klicensee, byte[] npd)
        {
            byte[] xoredKey = new byte[16];
            xoredKey = Utils.XOR(klicensee, EDAT_Keys.npdrm_omac_key2);
            byte[] calculated = Utils.CMAC128(xoredKey, npd);
            byte[] npdhash = new byte[16];
            Array.Copy(npd, 96, npdhash, 0, 16);
            if (npdhash.Equals(calculated))
                return true;
            else
                return false;
        }

        private static bool checkNPDHash1(string filename, byte[] npd)
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(filename);
            byte[] data1 = new byte[48 + fileBytes.Length];
            Array.Copy(npd, 16, data1, 0, 48);
            Array.Copy(fileBytes, 0, data1, 48, fileBytes.Length);

            byte[] hash1 = Utils.CMAC128(EDAT_Keys.npdrm_omac_key3, data1);
            return true;
        }
    }

    public class EDAT_Data
    {
        public long flags;
        public long blockSize;
        public Int64 fileLen;

        public static EDAT_Data CreateEDAT_Data(DataReader edatreader)
        {
            EDAT_Data rc = new EDAT_Data();

            rc.flags = edatreader.ReadInt32();
            rc.blockSize = edatreader.ReadInt32();
            rc.fileLen = edatreader.ReadInt64();

            return rc;
        }
    }
}
