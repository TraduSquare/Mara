using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Numerics;

namespace Mara.Lib.Platforms.PS3.Crypto
{
    public static class Utils
    {
        public static byte[] aesecbDecrypt(byte[] key, byte[] data)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.None;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length); ;
        }

        public static byte[] aesecbEncrypt(byte[] key, byte[] data)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.None;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            return cTransform.TransformFinalBlock(data, 0, data.Length); ;
        }

        public static byte[] CMAC128(byte[] key, byte[] data)
        {
            byte[] K1 = new byte[16];
            byte[] K2 = new byte[16];

            (K1, K2) = calculateSubkey(key);

            byte[] input = new byte[16];
            byte[] previous = new byte[16];
            int remaining;
            int currentOffset = 0;

            for (remaining = data.Length; remaining > 16; remaining -= 16)
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
            } else
            {
                input[remaining] = 0x80;
                input = XOR(input, previous);
                input = XOR(input, K2);
            }
            previous = aesecbEncrypt(key, input);
            return previous;
        }

        private static (byte[], byte[]) calculateSubkey(byte[] key)
        {
            byte[] K1 = new byte[16];
            byte[] K2 = new byte[16];
            byte[] zero = new byte[16];
            byte[] L = new byte[16];

            L = aesecbEncrypt(key, zero);

            BigInteger aux = new BigInteger(L);

            if ((L[0] & 0x80) != 0x0)
            {
                aux = aux << 1;
                aux = new BigInteger(XOR(aux.ToByteArray(), 130L));
            } else
            {
                aux = aux << 1;
            }

            byte[] aux2 = aux.ToByteArray();
            if (aux2.Length >= 16)
            {
                Array.Copy(aux2, aux2.Length - 16, K1, 0, 16);
            } else
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
            byte[] output = new byte[inputB.Length];

            for (int i = 0; i < inputB.Length; ++i)
            {
                output[i] = (byte)(inputA[i] ^ inputB[i]);
            }

            return output;
        }

        public static byte[] XOR(byte[] input, long number)
        {
            byte[] array = new byte[input.Length];

            for (int i = 0; i < input.Length; ++i)
            {
                array[i] = (byte)(input[i] ^ number);
            }

            return array;
        }
    }
}
