using System;
using System.IO;
using System.Linq;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.Es;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem;

namespace Mara.Lib.Platforms.Switch;

public class PartitionFS
{
    private readonly PartitionFileSystem PFS0;
    private Ticket tik;

    public PartitionFS(IStorage file)
    {
        PFS0 = new PartitionFileSystem(file);
    }

    public PartitionFS(string path)
    {
        PFS0 = new PartitionFileSystem(new LocalStorage(path, FileAccess.Read));
    }

    public string MountPFS0(HOS hos, string mountname)
    {
        var tikfound = false;
        var fs = hos.horizon.Fs;
        using var PFS0_FS = new UniqueRef<IFileSystem>(PFS0);
        fs.Register(mountname.ToU8Span(), ref PFS0_FS.Ref);

        foreach (var entry in fs.EnumerateEntries(mountname + ":/", "*.tik", SearchOptions.Default))
        {
            tikfound = true;
            fs.OpenFile(out var ticket, entry.FullPath.ToU8Span(), OpenMode.Read);
            tik = new Ticket(new FileHandleStorage(fs, ticket).AsStream());
            if (hos.CheckSignature)
            {
                if (tik.Signature.SequenceEqual(Signatures.InvalidTikSig))
                    throw new Exception("Invalid ticket Signature.");
                // Si no contiene esa secuencia es un juego base
                if (!BitConverter.ToString(tik.RightsId).Replace("-", "").Contains("80000000"))
                    if (Signatures.CheckDeviceID(tik.DeviceId) != Result.Success)
                        throw new Exception("Invalid ticket.");
            }

            hos.keys = Signatures.AddKey(tik.RightsId, tik.GetTitleKey(hos.keys), hos.keys);
        }

        if (tikfound == false && hos.CheckSignature) throw new Exception("ticket not found.");

        return mountname + ":/";
    }

    public void ExtractPFS0(string output)
    {
        PFS0.Extract(output);
    }
}