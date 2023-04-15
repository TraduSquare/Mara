using System;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Text;
using Mara.Lib.Platforms.PS3.Crypto;
using Mara.Lib.Platforms.PS3.System;
using Yarhl.IO;

namespace Mara.Lib.Platforms.PS3.IO;

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

        if (decryptData(reader, npd, data, rifkey, OutPath)) Console.WriteLine("Done!");
    }
    
    public static NPD decryptFileWithResult(string InPath, string OutPath, byte[] klicense, byte[] keyFromRif)
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

        if (decryptData(reader, npd, data, rifkey, OutPath)) Console.WriteLine("Done!");
        
        return npd;
    }

    public static void encryptFile(string InPath, string OutPath, byte[] klicense, byte[] keyFromRif,
        byte[] ContentID, byte[] flags, byte[] type, byte[] version)
    {
        var encrypted = DataStreamFactory.FromFile(OutPath, FileOpenMode.ReadWrite);
        var decrypted = DataStreamFactory.FromFile(InPath, FileOpenMode.Read);
        var writer = new DataWriter(encrypted);
        var reader = new DataReader(decrypted);
        NPD npd;
        byte[] npd_bytes;

        (npd, npd_bytes) = writeValidNPD(Path.GetFileName(OutPath), klicense, reader, ContentID, type, version);

        writer.Write(npd_bytes);
        byte[] buffer = { 0, 0, 0, 0 };
        Array.Copy(flags, 0, buffer, 3, flags.Length);
        writer.Write(buffer);
        buffer[2] = 64;
        buffer[3] = 0;
        writer.Write(buffer);
        var len = reader.Stream.Length;
        var lenBuf = BitConverter.GetBytes(len);
        var rLenBuf = new byte[8];

        for (var i = 0; i < 8; ++i) rLenBuf[i] = 0;
        for (var i = 0; i < lenBuf.Length; ++i) rLenBuf[i] = lenBuf[i];

        writer.Write(Utils.ReverseBytes(rLenBuf));
        buffer[0] = 0;
        while (writer.Stream.Length < 256L) writer.Write(buffer[0]);
        writer.Write("a");
        var data = new EDAT_Data();
        data.flags = flags[0];
        data.blockSize = 16384L;
        data.fileLen = len;
        var rifkey = getKey(npd, data, klicense, keyFromRif);
        var hashFlag = (data.flags & 0x8L) == 0x0L ? 2 : 268435458;
        if ((data.flags & 0x80000000L) != 0x0L) hashFlag |= 0x1000000;

        encryptData(reader, writer, npd, data, rifkey);
        writer.Stream.Seek(144L);
        var aa = new AppLoader();

        var keyIndex = 0;
        if (npd.Version == 4L) keyIndex = 1;
        aa.doInit(hashFlag, 1, new byte[16], new byte[16], rifkey, keyIndex);
        var sectionSize = (data.flags & 0x1L) != 0x0L ? 32 : 16;
        var numBlocks = (int)((data.fileLen + data.blockSize - 1L) / data.blockSize);
        var readed = 0;
        var baseOffset = 256;
        var lenToRead = 0;

        var r = new DataReader(encrypted);
        for (long remaining = sectionSize * numBlocks; remaining > 0L; remaining -= lenToRead)
        {
            lenToRead = 15360L > remaining ? (int)remaining : 15360;
            r.Stream.Seek(baseOffset + readed);
            var content = new byte[lenToRead];
            var ooo = new byte[lenToRead];
            content = r.ReadBytes(content.Length);
            ooo = aa.doUpdate(content, 0, 0, lenToRead);
            readed += lenToRead;
        }

        var headerHash = new byte[16];
        headerHash = aa.doFinalButGetHash();
        writer.Stream.Seek(144L);
        writer.Write(headerHash);
        r.Stream.Seek(0);
        writer.Stream.Seek(0);

        var header = new byte[160];
        var headerODummy = new byte[160];
        header = r.ReadBytes(header.Length);
        var a = new AppLoaderReverse();
        var generatedHash = new byte[16];
        (_, generatedHash) = a.doAll(hashFlag, 1, header, 0, 0, header.Length, new byte[16], new byte[16],
            rifkey, generatedHash, 0, keyIndex);
        writer.Stream.Seek(160L);
        writer.Write(generatedHash);
        while (writer.Stream.Length < 256L) writer.Write(buffer[0]);
        encrypted.Close();
        decrypted.Close();
    }

    private static void encryptData(DataReader reader, DataWriter writer, NPD npd, EDAT_Data data, byte[] rifkey)
    {
        var numBlocks = (int)((data.fileLen + data.blockSize - 1L) / data.blockSize);
        var expectedHashForFile = new byte[numBlocks * 16];
        var encryptedDataForFile = new byte[(int)(reader.Stream.Length + 15L) & 0xFFFFFFF0];
        var EDATAVersion = new byte[16];

        switch (npd.Version)
        {
            case 2L:
                EDATAVersion = Utils.StringToByteArray("454441544120322E372E302E57000000");
                break;
            case 3L:
                EDATAVersion = Utils.StringToByteArray("454441544120332E332E302E57000000");
                break;
            case 4L:
                EDATAVersion = Utils.StringToByteArray("454441544120342E302E302E57000000");
                break;
        }

        var keyIndex = 0;
        if (npd.Version == 4L) keyIndex = 1;

        for (var i = 0; i < numBlocks; ++i)
        {
            var offset = i * data.blockSize;
            reader.Stream.Seek(offset);
            var len = (int)data.blockSize;

            if (i == numBlocks - 1)
                len = (int)(data.fileLen % data.blockSize);

            var realLen = len;
            len = (int)((len + 15) & 0xFFFFFFF0);
            Console.WriteLine($"Offset: {offset}, len: {len}, reallen: {realLen}");
            var encryptedData = new byte[len];
            var decryptedData = new byte[len];

            decryptedData = reader.ReadBytes(len);
            for (var ai = realLen; ai < len; ++ai) decryptedData[ai] = 0;

            var key = new byte[16];
            var hash = new byte[16];
            var blockKey = calculateBlockKey(i, npd);

            key = Utils.aesecbEncrypt(rifkey, blockKey);
            hash = key;
            if ((data.flags & 0x10L) != 0x0L) hash = Utils.aesecbEncrypt(rifkey, key);
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

            Console.WriteLine($"cryptoFlag: {cryptoFlag}");
            Console.WriteLine($"hashFlag: {hashFlag}");
            var a = new AppLoaderReverse();
            var iv = npd.Version <= 1L ? new byte[16] : npd.Digest;
            var generatedHash = new byte[20];
            var metadata = new byte[32];
            (encryptedData, generatedHash) = a.doAll(hashFlag, cryptoFlag, decryptedData, 0, 0, decryptedData.Length,
                key, iv, hash,
                generatedHash, 0, keyIndex);

            Array.Copy(encryptedData, 0, encryptedDataForFile, offset, len);
            Array.Copy(generatedHash, 0, expectedHashForFile, i * 16, 16);
        }

        writer.Stream.Seek(256L);
        writer.Write(expectedHashForFile);
        writer.Write(encryptedDataForFile);
        writer.Write(EDATAVersion);
    }

    private static (NPD npd, byte[] npd_bytes) writeValidNPD(string? FileName, byte[] klicense, DataReader reader,
        byte[] contentId, byte[] type, byte[] version)
    {
        var npd = new byte[128];
        npd[0] = 78;
        npd[1] = 80;
        npd[2] = 68;
        npd[4] = npd[3] = 0;
        npd[6] = npd[5] = 0;
        npd[7] = version[0];
        npd[8] = 0;
        npd[10] = npd[9] = 0;
        npd[11] = 3;
        npd[12] = 0;
        npd[14] = npd[13] = 0;
        npd[15] = type[0];
        for (var i = 0; i < 48; ++i) npd[16 + i] = contentId[i];

        byte[] fake_iv =
        {
            0x6D, 0x65, 0x67, 0x61, 0x66, 0x6C, 0x61, 0x6E,
            0x6C, 0x61, 0x63, 0x68, 0x75, 0x70, 0x61, 0x00
        };

        /*byte[] iv = {100, 117, 116, 115, 101, 110, 117, 114, 66, 102, 79, 121, 114, 111, 108, 71};

        iv = Utils.ReverseBytes(iv);*/

        Array.Copy(fake_iv, 0, npd, 64, fake_iv.Length);
        byte[] hash;
        (hash, npd) = createNPDHash1(FileName, npd);
        Array.Copy(hash, 0, npd, 80, 16);

        byte[] devhash;

        (devhash, npd) = createNPDHash2(klicense, npd);
        Array.Copy(devhash, 0, npd, 96, 16);
        for (var j = 0; j < 16; ++j) npd[112 + j] = 0;
        var _NPD = NPD.CreateNPD(npd);
        return (_NPD, npd);
    }

    private static (byte[], byte[]) createNPDHash2(byte[] klicense, byte[] npd)
    {
        var xoredKey = new byte[16];
        xoredKey = Utils.XOR(klicense, EDAT_Keys.npdrm_omac_key2);
        var calculated = Utils.CMAC128(xoredKey, npd, 96);
        Array.Copy(calculated, 0, npd, 96, 16);
        return (calculated, npd);
    }

    private static (byte[], byte[]) createNPDHash1(string? fileName, byte[] npd)
    {
        var fileBytes = Utils.charsToByte(fileName.ToCharArray());
        var data1 = new byte[48 + fileBytes.Length];

        Array.Copy(npd, 16, data1, 0, 48);
        Array.Copy(fileBytes, 0, data1, 48, fileBytes.Length);

        var hash1 = Utils.CMAC128(EDAT_Keys.npdrm_omac_key3, data1);

        Array.Copy(hash1, 0, npd, 80, 16);

        return (hash1, npd);
    }

    private static bool decryptData(DataReader reader, NPD npd, EDAT_Data data, byte[] rifkey,
        string OutPath = "decrypted.edat")
    {
        var numBlocks = (int)((data.fileLen + data.blockSize - 1L) / data.blockSize);
        var metadataSectionSize = (data.flags & 0x1L) != 0x0L || (data.flags & 0x20L) != 0x0L ? 32 : 16;
        var baseOffset = 256;
        var keyIndex = 0;

        if (npd.Version == 4L) keyIndex = 1;
        var decrypted = DataStreamFactory.FromFile(OutPath, FileOpenMode.Write);
        var writer = new DataWriter(decrypted);
        for (var i = 0; i < numBlocks; ++i)
        {
            reader.Stream.Seek(baseOffset + i * metadataSectionSize);
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
                reader.Stream.Seek(baseOffset + i * (metadataSectionSize + data.blockSize));
                var metadata = reader.ReadBytes(32);

                for (var j = 0; j < 16; j++)
                {
                    expectedHash[j] = (byte)(metadata[j] ^ metadata[j + 16]);
                    Array.Copy(metadata, 16, expectedHash, 16, 4);
                }

                offset = baseOffset + i * (metadataSectionSize + data.blockSize) + metadataSectionSize;
                len = (int)data.blockSize;
                if (i == numBlocks - 1)
                {
                    var a = new BigInteger(data.fileLen);
                    var b = new BigInteger(data.blockSize);
                    var remainder = a % b;
                    var aux = (int)remainder;
                    len = aux > 0 ? aux : len;
                }
            }
            else
            {
                expectedHash = reader.ReadBytes(expectedHash.Length);
                offset = baseOffset + i * data.blockSize + numBlocks * metadataSectionSize;
                len = (int)data.blockSize;
                if (i == numBlocks - 1)
                {
                    var a = new BigInteger(data.fileLen);
                    var b = new BigInteger(data.blockSize);
                    var remainder = a % b;
                    var aux2 = (int)remainder;
                    len = aux2 > 0 ? aux2 : len;
                }
            }

            var realLen = len;
            len = (int)((len + 15) & 0xFFFFFFF0);
            Console.WriteLine(
                $"Offset: {offset}, len: {len}, reallen: {realLen}, endCompress: {compressionEndBlock}");
            reader.Stream.Seek(offset);
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
            var app = new AppLoader();

            decryptedData = app.doAll(hashFlag, cryptoFlag, encryptedData, 0, 0, encryptedData.Length, key, iv,
                hash, expectedHash, 0, keyIndex);

            if (decryptedData == null) throw new Exception($"Error decrypting block {i}");
            writer.Write(decryptedData);
        }

        decrypted.Close();
        return true;
    }

    private static byte[] calculateBlockKey(int blk, NPD npd)
    {
        var baseKey = npd.Version <= 1L ? new byte[16] : npd.DevHash;
        var result = new byte[16];
        Array.Copy(baseKey, 0, result, 0, 12);
        result[12] = (byte)((blk >> 24) & 0xFF);
        result[13] = (byte)((blk >> 16) & 0xFF);
        result[14] = (byte)((blk >> 8) & 0xFF);
        result[15] = (byte)(blk & 0xFF);
        return result;
    }

    private static byte[] decryptMetadataSection(byte[] metadata)
    {
        // PAIN
        byte[] result =
        {
            (byte)(metadata[12] ^ metadata[8] ^ metadata[16]), (byte)(metadata[13] ^ metadata[9] ^ metadata[17]),
            (byte)(metadata[14] ^ metadata[10] ^ metadata[18]),
            (byte)(metadata[15] ^ metadata[11] ^ metadata[19]), (byte)(metadata[4] ^ metadata[8] ^ metadata[20]),
            (byte)(metadata[5] ^ metadata[9] ^ metadata[21]), (byte)(metadata[6] ^ metadata[10] ^ metadata[22]),
            (byte)(metadata[7] ^ metadata[11] ^ metadata[23]), (byte)(metadata[12] ^ metadata[0] ^ metadata[24]),
            (byte)(metadata[13] ^ metadata[1] ^ metadata[25]), (byte)(metadata[14] ^ metadata[2] ^ metadata[26]),
            (byte)(metadata[15] ^ metadata[3] ^ metadata[27]), (byte)(metadata[4] ^ metadata[0] ^ metadata[28]),
            (byte)(metadata[5] ^ metadata[1] ^ metadata[29]), (byte)(metadata[6] ^ metadata[2] ^ metadata[30]),
            (byte)(metadata[7] ^ metadata[3] ^ metadata[31])
        };
        return result;
    }

    private static bool checkHeader(byte[] rifkey, EDAT_Data data, NPD npd, DataReader reader)
    {
        reader.Stream.Seek(0);
        var header = new byte[160];
        var outBytes = new byte[160];
        var expectedHash = new byte[16];
        var keyIndex = 0;

        if (npd.Version == 4L) keyIndex = 1;

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

        var a = new AppLoader();

        var result = a.doAll(hashFlag, 1, header, 0, 0, header.Length, new byte[16], new byte[16], rifkey,
            expectedHash, 0, keyIndex);
        if (result == null)
        {
            Console.WriteLine("Error verifying header. Is rifKey valid?.");
            return false;
        }

        if ((data.flags & 0x20L) == 0x0L)
        {
            Console.WriteLine("Checking metadata hash:");
            a = new AppLoader();
            a.doInit(hashFlag, 1, new byte[16], new byte[16], rifkey, keyIndex);
            var sectionSize = (data.flags & 0x1L) != 0x0L ? 32 : 16;
            var numBlocks = (int)((data.fileLen + data.blockSize - 1L) / data.blockSize);
            var readed = 0;
            var baseOffset = 256;
            var lenToRead = 0;

            for (long re = sectionSize * numBlocks; re > 0L; re -= lenToRead)
            {
                lenToRead = 15360L > re ? (int)re : 15360;
                reader.Stream.Seek(baseOffset + readed);
                var content = new byte[lenToRead];
                content = reader.ReadBytes(lenToRead);
                var meme = a.doUpdate(content, 0, 0, lenToRead);
                readed += lenToRead;
            }
        }

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
        reader.Stream.Seek(128L);
        return EDAT_Data.CreateEDAT_Data(reader);
    }

    public static NPD validateNPD(string filename, byte[] devKLic, DataReader reader)
    {
        reader.Stream.Seek(0);

        var npd = reader.ReadBytes(128);
        var flags = Utils.bit32hex(reader.ReadBytes(4), 0);

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

        rc.flags = Utils.bit32hex(edatreader.ReadBytes(4), 0);
        rc.blockSize = Utils.bit32hex(edatreader.ReadBytes(4), 0);
        rc.fileLen = Utils.bit64hex(edatreader.ReadBytes(8), 0);

        return rc;
    }
}