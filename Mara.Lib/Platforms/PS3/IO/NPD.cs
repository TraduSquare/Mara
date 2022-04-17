using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

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

        public static NPD CreateNPD(byte[] npd)
        {
            NPD n = new NPD();
            DataReader reader = new DataReader(new MemoryStream(npd));
            n.Magic = reader.ReadBytes(4);
            n.Version = reader.ReadInt32();
            n.License = reader.ReadInt32();
            n.Type = reader.ReadInt32();
            n.ContentId = reader.ReadBytes(48);
            n.Digest = reader.ReadBytes(16);
            n.TitleHash = reader.ReadBytes(16);
            n.DevHash = reader.ReadBytes(16);
            n.ActivationTime = reader.ReadInt64();
            n.ExpirantionTime = reader.ReadInt64();

            if (n.Validate())
                return n;
            else
                return null;
        }

        private bool Validate()
        {
            if (magic[0] == 78 && magic[1] == 80 && magic[2] == 68 && magic[3] == 0 && ActivationTime.Equals(0) && ExpirantionTime == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
