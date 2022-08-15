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

        public override byte[] doFinal()
        {
            return result;
        }

        public override byte[] doFinalButGetHash()
        {
            throw new NotImplementedException();
        }
    }

    public class CMAC : IHash
    {
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
            (K1, K2) = Utils.calculateSubkeyCMAC(key);
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
            int i;
            for (i = 0; i < finaldata.Length - 16; i += 16)
            {
                aux = new byte[16];
                Array.Copy(finaldata, i, aux, 0, aux.Length);
                aux = Utils.XOR(aux, previous);
                previous = Utils.aesecbEncrypt(key, aux);
            }

            nonProcessed = new byte[finaldata.Length - i];
            Array.Copy(finaldata, i, nonProcessed, 0, nonProcessed.Length);
            return finaldata;
        }

        public override byte[] doFinal()
        {
            byte[] generateHash;
            var aux = new byte[16];
            Array.Copy(nonProcessed, 0, aux, 0, nonProcessed.Length);
            if (nonProcessed.Length == 16)
            {
                aux = Utils.XOR(aux, K1);
            }
            else
            {
                aux[nonProcessed.Length] = 0x80;
                aux = Utils.XOR(aux, K2);
            }

            aux = Utils.XOR(aux, previous);
            generateHash = Utils.aesecbEncrypt(key, aux);
            return generateHash;
        }

        public override byte[] doFinalButGetHash()
        {
            byte[] generateHash;
            var aux = new byte[16];
            Array.Copy(nonProcessed, 0, aux, 0, nonProcessed.Length);
            if (nonProcessed.Length == 16)
            {
                aux = Utils.XOR(aux, K1);
            }
            else
            {
                aux[nonProcessed.Length] = 0x80;
                aux = Utils.XOR(aux, K2);
            }

            aux = Utils.XOR(aux, previous);
            generateHash = Utils.aesecbEncrypt(key, aux);
            return generateHash;
        }
    }
}