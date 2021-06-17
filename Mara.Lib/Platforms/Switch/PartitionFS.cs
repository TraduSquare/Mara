using System;
using System.IO;
using System.Linq;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Mara.Lib.Platforms.Switch
{
    public class PartitionFS
    {
        private PartitionFileSystem PFS0;
        private Ticket tik;
        public PartitionFS(IStorage file)
        {
            this.PFS0 = new PartitionFileSystem(file);
        }

        public PartitionFS(string path)
        {
            this.PFS0 = new PartitionFileSystem(new LocalStorage(path, FileAccess.Read));
        }

        public string MountPFS0(HOS hos, string mountname)
        {
            bool tikfound = false;
            FileSystemClient fs = hos.horizon.Fs;
            fs.Register(mountname.ToU8Span(), this.PFS0);

            foreach (DirectoryEntryEx entry in fs.EnumerateEntries(mountname + ":/", "*.tik", SearchOptions.Default))
            {
                tikfound = true;
                fs.OpenFile(out FileHandle ticket, entry.FullPath.ToU8Span(), OpenMode.Read);
                tik = new Ticket(new FileHandleStorage(fs, ticket).AsStream());
                if (hos.CheckSignature == true)
                {
                    if (tik.Signature.SequenceEqual(Signatures.InvalidTikSig))
                    {
                        throw new Exception("Invalid ticket Signature.");
                    }
                }
                hos.keys = Signatures.AddKey(tik.RightsId, tik.GetTitleKey(hos.keys), hos.keys);
            }

            if (tikfound == false && hos.CheckSignature == true)
            {
                throw new Exception("ticket not found.");
            }

            return mountname + ":/";
        }

        public void ExtractPFS0(string output)
        {
            this.PFS0.Extract(output);
        }
    }
}
