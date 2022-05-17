using System;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Text;
using Mara.Lib.Platforms.PS3.Crypto;
using Mara.Lib.Platforms.PS3.System;
using Yarhl.IO;

namespace Mara.Lib.Platforms.PS3.IO
{
    public static class EDAT
    {
        public static void decryptFile(string InPath, string OutPath, byte[] klicense, byte[] keyFromRif)
        {
            var file = DataStreamFactory.FromFile(InPath, FileOpenMode.Read);
            var reader = new DataReader(file);

            var npd = validateNPD(Path.GetFileName(InPath), klicense, reader);
            if (npd == null) throw new Exception("Invalid NPD");

            Console.WriteLine("NPD valid!");
            var data = getEDATData(reader);
            var rifkey = getKey(npd, data, klicense, keyFromRif);

            if (rifkey == null) throw new Exception("ERROR: Key for decryption is missing");
            Console.WriteLine($"DECRYPTION KEY: {BitConverter.ToString(rifkey)}");

            if (!checkHeader(rifkey, data, npd, reader)) throw new Exception("Error verifying header.");

            if (decryptData(reader, npd, data, rifkey))
            {
            }
        }

        private static bool decryptData(DataReader reader, NPD npd, EDAT_Data data, byte[] rifkey)
        {
            var numBlocks = (int) ((data.fileLen + data.blockSize - 1L) / data.blockSize);
            var metadataSectionSize = (data.flags & 0x1L) != 0x0L || (data.flags & 0x20L) != 0x0L ? 32 : 16;
            var baseOffset = 256;
            var keyIndex = 0;

            if (npd.Version == 4L) keyIndex = 1;

            for (var i = 0; i < numBlocks; ++i)
            {
                reader.Stream.Seek(baseOffset + i * metadataSectionSize, SeekOrigin.Begin);
                var expectedHash = new byte[20];
                var compressionEndBlock = 0;
                long offset;
                var len = 0;

                if ((data.flags & 0x1L) != 0x0L)
                {
                    var metadata = reader.ReadBytes(32);
                    var result = decryptMetadataSection(metadata);
                    Span<byte> a = result;
                    offset = BinaryPrimitives.ReadInt32BigEndian(a.Slice(8));
                    compressionEndBlock = BinaryPrimitives.ReadInt32BigEndian(a.Slice(12));
                    Array.Copy(metadata, 16, expectedHash, 0, 16);
                }
                else if ((data.flags & 0x20L) != 0x0L)
                {
                    reader.Stream.Seek(baseOffset + i * (metadataSectionSize + data.blockSize), SeekOrigin.Begin);
                    var metadata = reader.ReadBytes(32);

                    for (var j = 0; j < 16; j++)
                    {
                        expectedHash[j] = (byte) (metadata[j] ^ metadata[j + 16]);
                        Array.Copy(metadata, 16, expectedHash, 16, 4);
                    }

                    offset = baseOffset + i * (metadataSectionSize + data.blockSize) + metadataSectionSize;
                    len = (int) data.blockSize;
                    if (i == numBlocks - 1)
                    {
                        var a = new BigInteger(data.fileLen);
                        var b = new BigInteger(data.blockSize);
                        var remainder = a % b;
                        var aux = (int) remainder;
                        len = aux > 0 ? aux : len;
                    }
                }
                else
                {
                    expectedHash = reader.ReadBytes(expectedHash.Length);
                    offset = baseOffset + i * data.blockSize + numBlocks * metadataSectionSize;
                    len = (int) data.blockSize;
                    if (i == numBlocks - 1)
                    {
                        var a = new BigInteger(data.fileLen);
                        var b = new BigInteger(data.blockSize);
                        var remainder = a % b;
                        var aux2 = (int) remainder;
                        len = aux2 > 0 ? aux2 : len;
                    }
                }

                var realLen = len;
                len = (int) ((len + 15) & 0xFFFFFFF0);
                reader.Stream.Seek(offset, SeekOrigin.Begin);
                var encryptedData = new byte[len];
                var decryptedData = new byte[len];
                encryptedData = reader.ReadBytes(len);
                var key = new byte[16];
                var hash = new byte[16];
                var blockKey = calculateBlockKey(i, npd);
                key = Utils.aesecbEncrypt(rifkey, blockKey);

                if ((data.flags & 0x10L) != 0x0L)
                    hash = Utils.aesecbEncrypt(rifkey, key);
                else
                    Array.Copy(key, 0, hash, 0, key.Length);
            }

            var cryptoFlag = (data.flags & 0x2L) == 0x0L ? 2 : 1;
            int hashFlag;

            if ((data.flags & 0x10L) == 0x0L)
                hashFlag = 2;
            else if ((data.flags & 0x20L) == 0x0L)
                hashFlag = 4;
            else
                hashFlag = 1;
            if ((data.flags & 0x8L) != 0x0L)
            {
                cryptoFlag |= 0x10000000;
                hashFlag |= 0x10000000;
            }

            if ((data.flags & 0x80000000L) != 0x0L)
            {
                cryptoFlag |= 0x1000000;
                hashFlag |= 0x1000000;
            }

            var iv = npd.Version <= 1L ? new byte[16] : npd.Digest;
            throw new NotImplementedException();
        }

        private static byte[] calculateBlockKey(int blk, NPD npd)
        {
            var baseKey = npd.Version <= 1L ? new byte[16] : npd.DevHash;
            var result = new byte[16];
            Array.Copy(baseKey, 0, result, 0, 12);
            result[12] = (byte) ((blk >> 24) & 0xFF);
            result[13] = (byte) ((blk >> 16) & 0xFF);
            result[14] = (byte) ((blk >> 8) & 0xFF);
            result[15] = (byte) (blk & 0xFF);
            return result;
        }

        private static byte[] decryptMetadataSection(byte[] metadata)
        {
            // PAIN
            byte[] result =
            {
                (byte) (metadata[12] ^ metadata[8] ^ metadata[16]), (byte) (metadata[13] ^ metadata[9] ^ metadata[17]),
                (byte) (metadata[14] ^ metadata[10] ^ metadata[18]),
                (byte) (metadata[15] ^ metadata[11] ^ metadata[19]), (byte) (metadata[4] ^ metadata[8] ^ metadata[20]),
                (byte) (metadata[5] ^ metadata[9] ^ metadata[21]), (byte) (metadata[6] ^ metadata[10] ^ metadata[22]),
                (byte) (metadata[7] ^ metadata[11] ^ metadata[23]), (byte) (metadata[12] ^ metadata[0] ^ metadata[24]),
                (byte) (metadata[13] ^ metadata[1] ^ metadata[25]), (byte) (metadata[14] ^ metadata[2] ^ metadata[26]),
                (byte) (metadata[15] ^ metadata[3] ^ metadata[27]), (byte) (metadata[4] ^ metadata[0] ^ metadata[28]),
                (byte) (metadata[5] ^ metadata[1] ^ metadata[29]), (byte) (metadata[6] ^ metadata[2] ^ metadata[30]),
                (byte) (metadata[7] ^ metadata[3] ^ metadata[31])
            };
            return result;
        }

        private static bool checkHeader(byte[] rifkey, EDAT_Data data, NPD npd, DataReader reader)
        {
            reader.Stream.Seek(0, SeekOrigin.Begin);
            var header = new byte[160];
            var outBytes = new byte[160];
            var expectedHash = new byte[16];
            var keyIndex = 0;

            Console.WriteLine($"Checking NPD Version: {npd.Version}");
            Console.WriteLine($"EDATA Flag: 0x{data.flags}");

            switch (npd.Version)
            {
                case 0L:
                case 1L:
                {
                    if ((data.flags & 0x7FFFFFFEL) != 0x0L) return false;
                    break;
                }
                case 2L:
                {
                    if ((data.flags & 0x7EFFFFE0L) != 0x0L) return false;
                    break;
                }
                default:
                {
                    if (npd.Version != 3L && npd.Version != 4L)
                        throw new Exception($"ERROR: VERSION {npd.Version} DETECTED");
                    if ((data.flags & 0x7EFFFFC0L) != 0x0L) return false;
                    break;
                }
            }
            
            if ((data.flags & 0x20L) != 0x0L && (data.flags & 0x1L) != 0x0L) return false;

            header = reader.ReadBytes(header.Length);
            expectedHash = reader.ReadBytes(expectedHash.Length);

            // Comprobar Hash
            var hashFlag = (data.flags & 0x8L) == 0x0L ? 2 : 268435458;
            if ((data.flags & 0x80000000L) != 0x0L) hashFlag |= 0x1000000;
            if (npd.Version == 4L) keyIndex = 1;
            
            AppLoader a = new AppLoader();
            
            byte[] result = a.doAll(hashFlag, 1, header, 0, 0, header.Length, new byte[16], new byte[16], rifkey, expectedHash, 0, keyIndex);
            if (result == null) {
                Console.WriteLine("Error verifying header. Is rifKey valid?.");
                return false;
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

            var npd = reader.ReadBytes(128);
            long flags = reader.ReadInt32();

            if ((flags & 0x1000000L) != 0x0L) throw new Exception("SDAT not supported");

            if (!checkNPDHash1(filename, npd))
                return null;
            if (checkNPDHash2(devKLic, npd)) return null;

            return NPD.CreateNPD(npd);
        }

        private static bool checkNPDHash2(byte[] klicensee, byte[] npd)
        {
            var xoredKey = new byte[16];
            xoredKey = Utils.XOR(klicensee, EDAT_Keys.npdrm_omac_key2);
            var calculated = Utils.CMAC128(xoredKey, npd);
            var npdhash = new byte[16];
            Array.Copy(npd, 96, npdhash, 0, 16);
            if (npdhash.Equals(calculated))
                return true;
            return false;
        }

        private static bool checkNPDHash1(string filename, byte[] npd)
        {
            var fileBytes = Encoding.UTF8.GetBytes(filename);
            var data1 = new byte[48 + fileBytes.Length];
            Array.Copy(npd, 16, data1, 0, 48);
            Array.Copy(fileBytes, 0, data1, 48, fileBytes.Length);

            var hash1 = Utils.CMAC128(EDAT_Keys.npdrm_omac_key3, data1);
            return true;
        }
    }

    public class EDAT_Data
    {
        public long blockSize;
        public long fileLen;
        public long flags;

        public static EDAT_Data CreateEDAT_Data(DataReader edatreader)
        {
            var rc = new EDAT_Data();

            rc.flags = edatreader.ReadInt32();
            rc.blockSize = edatreader.ReadInt32();
            rc.fileLen = edatreader.ReadInt64();

            return rc;
        }
    }
}