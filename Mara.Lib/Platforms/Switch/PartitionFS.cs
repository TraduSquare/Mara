using System;
using System.IO;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Mara.Lib.Platforms.Switch
{
    class PartitionFS
    {
        private PartitionFileSystem PFS0;

        public PartitionFS(IStorage file)
        {
            this.PFS0 = new PartitionFileSystem(file);
        }

        public PartitionFS(string path)
        {
            using (IStorage file = new LocalStorage(path, FileAccess.Read))
            {
                this.PFS0 = new PartitionFileSystem(file);
            }
        }

        public string MountPFS0(HOS hos)
        {
            FileSystemClient fs = hos.horizon.Fs;
            string mountname = "PFS0";
            fs.Register(mountname.ToU8Span(), this.PFS0);
            return mountname + ":/";
        }

        public void ExtractPFS0(string output)
        {
            this.PFS0.Extract(output);
        }
    }
}
