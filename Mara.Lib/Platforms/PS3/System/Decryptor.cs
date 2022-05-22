using System;
using System.Security.Cryptography;

namespace Mara.Lib.Platforms.PS3.System;

public class NoCrypt : IDecryptor
{
    public override void doInit(byte[] Key, byte[] iv)
    {
    }

    public override byte[] doUpdate(byte[] data, int inputOffset, int p3, int inputCount)
    {
        var array = new byte[data.Length];
        Array.Copy(data, inputOffset, array, p3, inputCount);
        return array;
    }
}

public class AESCBC128Decrypt : IDecryptor
{
    private ICryptoTransform c;

    public override void doInit(byte[] Key, byte[] iv)
    {
        var c = new AesManaged();
        c.Key = Key;
        c.IV = iv;
        c.Mode = CipherMode.CBC;
        c.Padding = PaddingMode.None;
        this.c = c.CreateDecryptor();
    }

    public override byte[] doUpdate(byte[] data, int inputOffset, int p3, int inputCount)
    {
        return c.TransformFinalBlock(data, inputOffset, inputCount);
    }
}