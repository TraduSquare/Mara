using System;

namespace Mara.Lib.Platforms.PS3.System
{
    public abstract class IHash
    {
        private int hashLen;
        public byte[] result;
        public abstract void doInit(byte[] hash);
        public abstract byte[] doUpdate(byte[] data);
        public abstract bool doFinal(byte[] p0, int p1);
        public abstract bool doFinalButGetHash(byte[] p0);

        public virtual void setHashLen(int len)
        {
            if (len == 16 || len == 20)
            {
                hashLen = len;
                return;
            }

            throw new Exception("Hash len must be 0x10 or 0x14");
        }
    }
}