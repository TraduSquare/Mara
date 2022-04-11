using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

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
    }
}
