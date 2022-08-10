using System;
using System.IO;
using System.Linq;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.Fs;

namespace Mara.Lib.Platforms.Switch;

public class GameCard
{
    private readonly Xci Gamecard;

    public GameCard(HOS hos, IStorage gamecardimage)
    {
        Gamecard = new Xci(hos.keys, gamecardimage);
    }

    public GameCard(HOS hos, string gamecardpath)
    {
        if (hos.CheckSignature)
        {
            if (CheckCert(gamecardpath))
            {
                IStorage tmp = new LocalStorage(gamecardpath, FileAccess.Read);
                Gamecard = new Xci(hos.keys, tmp);
            }
            else
            {
                throw new Exception("Invalid Cert Signature.");
            }
        }
        else
        {
            IStorage tmp = new LocalStorage(gamecardpath, FileAccess.Read);
            Gamecard = new Xci(hos.keys, tmp);
        }
    }

    public string MountGameCard(HOS hos)
    {
        var fs = hos.horizon.Fs;
        var mountname = "GameCard";
        if (hos.CheckSignature)
            if (Gamecard.Header.SignatureValidity == Validity.Invalid)
                throw new Exception("Invalid GameCard Signature.");
        using var HFS0 = new UniqueRef<IFileSystem>(Gamecard.OpenPartition(XciPartitionType.Secure));
        fs.Register(mountname.ToU8Span(), ref HFS0.Ref());

        return mountname + ":/";
    }

    private static bool CheckCert(string path)
    {
        using (var s = File.Open(path, FileMode.Open))
        {
            var Readerdec = new BinaryReader(s);
            var tmp = new byte[512];
            Readerdec.BaseStream.Position = 0x7000;
            tmp = Readerdec.ReadBytes(Signatures.GameCardInvalidCert.Length);
            if (tmp.SequenceEqual(Signatures.GameCardInvalidCert))
                return false;
            return true;
        }
    }
}