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
        }

        public static NPD validateNPD(string filename, byte[] devKLic, DataReader reader)
        {
            reader.Stream.Seek(0, SeekOrigin.Begin);

            byte[] npd = reader.ReadBytes(128);
            long flags = reader.ReadInt32();

            if ((flags & 0x1000000L) != 0x0L)
            {
                // SDAT DEBE CASCAR
            } 
            else
            {

            }

            return null;
        }

        private bool checkNPDHash1(string filename, byte[] npd)
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(filename);
            byte[] data1 = new byte[48 + fileBytes.Length];
            Array.Copy(npd, 16, data1, 0, 48);
            Array.Copy(fileBytes, 0, data1, 48, fileBytes.Length);

            byte[] hash1 = Utils.CMAC128(null, data1);
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
