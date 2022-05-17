using System.Security.Cryptography;
using NotImplementedException = System.NotImplementedException;

namespace Mara.Lib.Platforms.PS3.System
{
    public class HMAC : IHash
    {
        private HMACSHA1 mac;
        private byte[] result;
        
        public override void doInit(byte[] hash)
        {
            mac = new HMACSHA1(hash);
            throw new NotImplementedException();
        }

        public override byte[] doUpdate(byte[] data)
        {
            return result = mac.ComputeHash(data);
        }

        public override bool doFinal(byte[] p0, int p1)
        {
            return p0.Equals(p1);
        }

        public override bool doFinalButGetHash(byte[] p0)
        {
            return true;
        }
    }
    
    public class CMAC : IHash
    {
        
        public override void doInit(byte[] hash)
        {
            throw new NotImplementedException();
        }

        public override byte[] doUpdate(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override bool doFinal(byte[] p0, int p1)
        {
            throw new NotImplementedException();
        }

        public override bool doFinalButGetHash(byte[] p0)
        {
            throw new NotImplementedException();
        }
    }
}