using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Mara.Lib.Platforms.PS3.Crypto
{
    public static class Utils
    {
        public static byte[] aesecbDecrypt(byte[] key, byte[] data)
        {
            var rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.None;
            var cTransform = rDel.CreateDecryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] aesecbEncrypt(byte[] key, byte[] data)
        {
            var rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.None;
            var cTransform = rDel.CreateEncryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] aescbcDecrypt(byte[] key, byte[] iv, byte[] data)
        {
            var rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.IV = iv;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.None;
            var cTransform = rDel.CreateDecryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] aescbcEncrypt(byte[] key, byte[] iv, byte[] data)
        {
            var rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.IV = iv;
            rDel.Mode = CipherMode.CBC;
            rDel.Padding = PaddingMode.None;
            var cTransform = rDel.CreateEncryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] CMAC128(byte[] key, byte[] data, int len = 0, int inoffset = 0)
        {
            var K1 = new byte[16];
            var K2 = new byte[16];

            (K1, K2) = calculateSubkey(key);

            var input = new byte[16];
            var previous = new byte[16];
            int remaining;
            var currentOffset = inoffset;

            if (len == 0)
                len = data.Length;
            for (remaining = len; remaining > 16; remaining -= 16)
            {
                Array.Copy(data, input, 16);
                input = XOR(input, previous);
                previous = aesecbEncrypt(key, input);
                currentOffset += 16;
            }

            input = new byte[16];
            Array.Copy(data, currentOffset, input, 0, remaining);
            if (remaining == 16)
            {
                input = XOR(input, previous);
                input = XOR(input, K1);
            }
            else
            {
                input[remaining] = 0x80;
                input = XOR(input, previous);
                input = XOR(input, K2);
            }

            previous = aesecbEncrypt(key, input);
            return previous;
        }

        public static (byte[], byte[]) calculateSubkey(byte[] key)
        {
            var K1 = new byte[16];
            var K2 = new byte[16];
            var zero = new byte[16];
            var L = new byte[16];

            L = aesecbEncrypt(key, zero);

            var aux = new BigInteger(L);

            if ((L[0] & 0x80) != 0x0)
            {
                aux = aux << 1;
                aux = new BigInteger(XOR(aux.ToByteArray(), 130L));
            }
            else
            {
                aux = aux << 1;
            }

            var aux2 = aux.ToByteArray();
            if (aux2.Length >= 16)
            {
                Array.Copy(aux2, aux2.Length - 16, K1, 0, 16);
            }
            else
            {
                // revisar
                Array.Copy(zero, K1, zero.Length);
                Array.Copy(aux2, 0, K1, 16 - aux2.Length, aux2.Length);
            }

            aux = new BigInteger(K1);
            if ((K1[0] & 0x80) != 0x0)
            {
                aux = aux << 1;
                aux = new BigInteger(XOR(aux.ToByteArray(), 130L));
            }
            else
            {
                aux = aux << 1;
            }

            aux2 = aux.ToByteArray();
            if (aux2.Length >= 16)
            {
                Array.Copy(aux2, aux2.Length - 16, K2, 0, 16);
            }
            else
            {
                // revisar
                Array.Copy(zero, K2, zero.Length);
                Array.Copy(aux2, 0, K2, 16 - aux2.Length, aux2.Length);
            }

            return (K1, K2);
        }

        public static byte[] XOR(byte[] inputA, byte[] inputB)
        {
            var output = new byte[inputB.Length];

            for (var i = 0; i < inputB.Length; ++i) output[i] = (byte) (inputA[i] ^ inputB[i]);

            return output;
        }

        public static byte[] XOR(byte[] input, long number)
        {
            var array = new byte[input.Length];

            for (var i = 0; i < input.Length; ++i) array[i] = (byte) (input[i] ^ number);

            return array;
        }

        public static long bit32hex(byte[] buffer, int initOffset)
        {
            var result = 0L;
            for (var i = initOffset; i < initOffset + 4; ++i) result = result * 256L + (buffer[i] & 0xFF);
            return result;
        }

        public static long bit64hex(byte[] buffer, int initOffset)
        {
            var result = 0L;
            for (var i = initOffset; i < initOffset + 8; ++i) result = result * 256L + (buffer[i] & 0xFF);
            return result;
        }
        
        public static byte[] ReverseBytes(byte[] raw) {
            int len = raw.Length;
            byte[] final = new byte[len];
            for (int i = 0; i < len; ++i) {
                final[len - i - 1] = raw[i];
            }
            return final;
        }
    }
}