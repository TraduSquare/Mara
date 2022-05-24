namespace Mara.Lib.Platforms.PS3.System
{
    public abstract class IDecryptor
    {
        public abstract void doInit(byte[] Key, byte[] iv);
        public abstract byte[] doUpdate(byte[] data, int inputOffset, int p3, int inputCount);
    }
}