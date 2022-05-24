using System;
using Mara.Lib.Platforms.PS3.Crypto;

namespace Mara.Lib.Platforms.PS3.System
{
    public class AppLoaderReverse
    {
        private IDecryptor dec;
        private IHash hash;

        public void doInit(int hashFlag, int cryptoFlag, byte[] key, byte[] iv, byte[] hashKey, int keyIndex)
        {
            var calculatedKey = new byte[key.Length];
            var calculatedIV = new byte[iv.Length];
            var calculatedHash = new byte[hashKey.Length];
            (calculatedKey, calculatedIV) = getCryptoKeys(cryptoFlag, key, iv, keyIndex);
            calculatedHash = getHashKeys(hashFlag, hashKey, keyIndex);
            setDecryptor(cryptoFlag);
            setHash(hashFlag);
            Console.WriteLine($"ERK: {BitConverter.ToString(calculatedKey).Replace("-", "")} " +
                              $"\nIV: {BitConverter.ToString(calculatedIV).Replace("-", "")}" +
                              $"\nHASH: {BitConverter.ToString(calculatedHash).Replace("-", "")}");
            dec.doInit(calculatedKey, calculatedIV);
            hash.doInit(calculatedHash);
        }

        public (byte[], byte[]) doUpdate(byte[] i, int inOffset, int outOffset, int len)
        {
            var encrypted = dec.doUpdate(i, inOffset, outOffset, len);
            var hashg = hash.doUpdate(encrypted);
            return (encrypted, hashg);
        }

        public (byte[], byte[]) doAll(int hashFlag, int cryptoFlag, byte[] data, int inOffset, int outOffset, int len,
            byte[] key, byte[] iv, byte[] hash, byte[] expectedHash, int hashOffset, int keyIndex)
        {
            byte[] _hash, encrypted;
            doInit(hashFlag, cryptoFlag, key, iv, hash, keyIndex);
            (encrypted, _hash) = doUpdate(data, inOffset, outOffset, len);
            _hash = doFinal();
            return (encrypted, _hash);
        }

        private byte[] doFinal()
        {
            return hash.doFinalButGetHash();
        }

        private void setDecryptor(int cryptoFlag)
        {
            var aux = cryptoFlag & 0xFF;
            switch (aux)
            {
                case 1:
                    dec = new NoCrypt();
                    break;
                case 2:
                    dec = new AESCBC128Encryptor();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void setHash(int hashFlag)
        {
            var aux = hashFlag & 0xFF;
            switch (aux)
            {
                case 1:
                    hash = new HMAC();
                    hash.setHashLen(20);
                    break;
                case 2:
                    hash = new CMAC();
                    hash.setHashLen(16);
                    break;
                case 4:
                    hash = new HMAC();
                    hash.setHashLen(16);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private (byte[], byte[]) getCryptoKeys(int cryptoFlag, byte[] key, byte[] iv,
            int keyIndex)
        {
            var mode = (int) (cryptoFlag & 0xF0000000);
            switch (mode)
            {
                case 268435456:
                    var dec = Utils.aescbcDecrypt(EDAT_Keys.EDATKEY[keyIndex], EDAT_Keys.EDATIV[keyIndex], key);
                    return (dec, iv);
                    break;
                case 536870912:
                    return (EDAT_Keys.EDATKEY[keyIndex], EDAT_Keys.EDATIV[keyIndex]);
                    break;
                case 0:
                    Console.WriteLine("MODE: Unencrypted ERK");
                    return (key, iv);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private byte[] getHashKeys(int hashFlag, byte[] hash, int keyIndex)
        {
            var mode = (int) (hashFlag & 0xF0000000);
            switch (mode)
            {
                case 268435456:
                    var calculatedHash =
                        Utils.aescbcDecrypt(EDAT_Keys.EDATKEY[keyIndex], EDAT_Keys.EDATIV[keyIndex], hash);
                    return calculatedHash;
                case 536870912:
                    return EDAT_Keys.EDATHASH[keyIndex];
                case 0:
                    return hash;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}