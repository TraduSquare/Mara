using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mara.Lib.Platforms.PS3.IO
{
    public class NPD
    {
        private byte[] magic = { 0x4E, 0x50, 0x44, 0x00 };
        private long version;
        private long license;
        private long type;
        private byte[] content_id = new byte[0x30];
        private byte[] digest = new byte[0x10];
        private byte[] titleHash = new byte[0x10];
        private byte[] devHash = new byte[0x10];
        private Int64 activationTime;
        private Int64 expirantionTime;

        public byte[] Magic
        {
            get => magic;
            set => magic = value;
        }

        public long Version
        {
            get => version;
            set => version = value;
        }

        public long License
        {
            get => license;
            set => license = value;
        }

        public long Type
        {
            get => type;
            set => type = value;
        }

        public byte[] ContentId
        {
            get => content_id;
            set => content_id = value;
        }

        public byte[] Digest
        {
            get => digest;
            set => digest = value;
        }

        public byte[] TitleHash
        {
            get => titleHash;
            set => titleHash = value;
        }

        public byte[] DevHash
        {
            get => devHash;
            set => devHash = value;
        }

        public long ActivationTime
        {
            get => activationTime;
            set => activationTime = value;
        }

        public long ExpirantionTime
        {
            get => expirantionTime;
            set => expirantionTime = value;
        }
    }
}
