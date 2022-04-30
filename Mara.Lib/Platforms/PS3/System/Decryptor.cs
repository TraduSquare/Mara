using System;
using System.Security.Cryptography;

namespace Mara.Lib.Platforms.PS3.System
{
    public class NoCrypt : IDecryptor
    {
        public void doInit(byte[] Key, byte[] iv)
        {
            throw new NotImplementedException();
        }

        public byte[] doUpdate(byte[] i, int p1, int p3, int p4)
        {
            var array = new byte[i.Length];
            Array.Copy(i, p1, array, p3, p4);
            return array;
        }
    }

    public class AESCBC128Decrypt : IDecryptor
    {
        private ICryptoTransform c;

        public void doInit(byte[] Key, byte[] iv)
        {
            var c = new AesManaged();
            c.Key = Key;
            c.IV = iv;
            c.Mode = CipherMode.CBC;
            c.Padding = PaddingMode.None;
            this.c = c.CreateDecryptor();
        }

        public byte[] doUpdate(byte[] p0, int p1, int p3, int p4)
        {
            return c.TransformFinalBlock(p0, p1, p4);
        }
    }
}