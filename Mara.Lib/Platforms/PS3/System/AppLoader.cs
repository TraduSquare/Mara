using System;

namespace Mara.Lib.Platforms.PS3.System
{
    public class AppLoader
    {
        private bool cryptoDebug;
        private IDecryptor dec;
        private IHash hash;
        private bool hashDebug;

        public AppLoader()
        {
            hashDebug = false;
            cryptoDebug = false;
        }

        public byte[] doAll(int hashFlag, int cryptoFlag, byte[] i, int inOffset, int outOffset, int len,
            byte[] key, byte[] iv, byte[] hash, byte[] expectedHash, int hashOffset, int keyIndex)
        {
            return null;
        }

        public void doInit(int hashFlag, int cryptoFlag, byte[] key, byte[] iv, byte[] hashKey, int keyIndex)
        {
            var calculatedKey = new byte[key.Length];
            var calculatedIV = new byte[iv.Length];
            var calculatedHash = new byte[hashKey.Length];
        }

        private void getCryptoKeys(int cryptoFlag, byte[] calculatedKey, byte[] calculatedIV, byte[] key, byte[] iv,
            int keyIndex)
        {
            var mode = (int) (cryptoFlag & 0xF0000000);
        }

        private void setDecryptor(int cryptoFlag)
        {
            int aux = cryptoFlag & 0xFF;
            switch (aux)
            {
                case 1:
                    this.dec = new NoCrypt();
                    break;
                case 2:
                    this.dec = new AESCBC128Decrypt();
                    break;
                default:
                    throw new Exception("No soportado");
            }
        }
    }
}