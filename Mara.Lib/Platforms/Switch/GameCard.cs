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
    public class GameCard
    {
        private Xci Gamecard;

        public GameCard(HOS hos, IStorage gamecardimage)
        {
            this.Gamecard = new Xci(hos.keys, gamecardimage);
        }

        public GameCard(HOS hos, string gamecardpath)
        {
            if (hos.CheckSignature == true)
            {
                if(CheckCert(gamecardpath) == true)
                {
                    IStorage tmp = new LocalStorage(gamecardpath, FileAccess.Read);
                    this.Gamecard = new Xci(hos.keys, tmp);
                }
                else
                {
                    throw new Exception("Invalid Cert Signature.");
                }
            }
            else
            {
                IStorage tmp = new LocalStorage(gamecardpath, FileAccess.Read);
                this.Gamecard = new Xci(hos.keys, tmp);
            }
        }

        public string MountGameCard(HOS hos)
        {
            FileSystemClient fs = hos.horizon.Fs;
            string mountname = "GameCard";
            if(hos.CheckSignature == true)
            {
                if (Gamecard.Header.SignatureValidity == Validity.Invalid)
                {
                    throw new Exception("Invalid GameCard Signature.");
                }
            }

            fs.Register(mountname.ToU8Span(), Gamecard.OpenPartition(XciPartitionType.Secure));

            return mountname + ":/";
        }

        private static bool CheckCert(string path)
        {
            using (FileStream s = File.Open(path, FileMode.Open))
            {
                BinaryReader Readerdec = new BinaryReader(s);
                byte[] tmp = new byte[512];
                Readerdec.BaseStream.Position = 0x7000;
                tmp = Readerdec.ReadBytes(Signatures.GameCardInvalidCert.Length);
                if (tmp.SequenceEqual(Signatures.GameCardInvalidCert) == true)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
