namespace Mara.Lib.Platforms.PS3.System
{
    public interface IHash
    {
        public void setHashLen(int p0);

        public void doInit(byte[] p0);

        public void doUpdate(byte[] p0, int p1, int p2);

        public bool doFinal(byte[] p0, int p1);

        public bool doFinalButGetHash(byte[] p0);
    }
}