namespace Mara.Lib.Platforms.PS3.System
{
    public interface IDecryptor
    {
        public void doInit(byte[] Key, byte[] iv);
        public byte[] doUpdate(byte[] p0, int p1, int p3, int p4);
    }
}