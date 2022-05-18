using System;
using System.Security.Cryptography;
using Mara.Lib.Platforms.PS3.Crypto;

namespace Mara.Lib.Platforms.PS3.System
{
    public class HMAC : IHash
    {
        private HMACSHA1 mac;
        private byte[] result;

        public override void doInit(byte[] hash)
        {
            mac = new HMACSHA1(hash);
        }

        public override byte[] doUpdate(byte[] data)
        {
            return result = mac.ComputeHash(data);
        }

        public override bool doFinal(byte[] p0, int p1)
        {
            return p0.Equals(p1);
        }

        public override bool doFinalButGetHash(byte[] p0)
        {
            return true;
        }
    }

    public class CMAC : IHash
    {
        public byte[] generateHash;
        private byte[] K1;
        private byte[] K2;
        private byte[] key;
        private byte[] nonProcessed;
        private byte[] previous;

        public override void doInit(byte[] key)
        {
            this.key = key;
            K1 = new byte[16];
            K2 = new byte[16];
            (K1, K2) = Utils.calculateSubkey(key);
            nonProcessed = null;
            previous = new byte[16];
        }

        public override byte[] doUpdate(byte[] data)
        {
            byte[] finaldata;
            if (nonProcessed != null)
            {
                var totalLen = data.Length + nonProcessed.Length;
                finaldata = new byte[totalLen];
                Array.Copy(nonProcessed, 0, finaldata, 0, totalLen);
                Array.Copy(data, 0, finaldata, nonProcessed.Length, 0);
            }
            else
            {
                finaldata = new byte[data.Length];
                Array.Copy(data, 0, finaldata, 0, data.Length);
            }

            byte[] aux;
            for (var i = 0; i < finaldata.Length - 16; i++)
            {
                aux = new byte[16];
                Array.Copy(finaldata, i, aux, 0, aux.Length);
                aux = Utils.XOR(aux, previous);
                previous = Utils.aesecbEncrypt(key, aux);
            }

            return finaldata;
        }

        public override bool doFinal(byte[] p0, int p1)
        {
            var aux = new byte[16];
            Array.Copy(nonProcessed, 0, aux, 0, nonProcessed.Length);
            if (nonProcessed.Length == 16)
            {
                aux = Utils.XOR(aux, K1);
            }
            else
            {
                aux[nonProcessed.Length] -= 0x80;
                aux = Utils.XOR(aux, K2);
            }

            aux = Utils.XOR(aux, previous);
            generateHash = Utils.aesecbEncrypt(key, aux);
            throw new NotImplementedException();
        }

        public override bool doFinalButGetHash(byte[] p0)
        {
            throw new NotImplementedException();
        }
    }
}