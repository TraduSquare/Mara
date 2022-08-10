using System.IO;
using Mara.Lib.Platforms.PS3.Crypto;
using Yarhl.IO;

namespace Mara.Lib.Platforms.PS3.IO;

public class NPD
{
    public byte[] Magic { get; set; } = { 0x4E, 0x50, 0x44, 0x00 };

    public long Version { get; set; }

    public long License { get; set; }

    public long Type { get; set; }

    public byte[] ContentId { get; set; } = new byte[0x30];

    public byte[] Digest { get; set; } = new byte[0x10];

    public byte[] TitleHash { get; set; } = new byte[0x10];

    public byte[] DevHash { get; set; } = new byte[0x10];

    public long ActivationTime { get; set; }

    public long ExpirantionTime { get; set; }

    public static NPD CreateNPD(byte[] npd)
    {
        var n = new NPD();
        var reader = new DataReader(new MemoryStream(npd));
        n.Magic = reader.ReadBytes(4);
        n.Version = Utils.bit32hex(reader.ReadBytes(4), 0);
        n.License = Utils.bit32hex(reader.ReadBytes(4), 0);
        n.Type = Utils.bit32hex(reader.ReadBytes(4), 0);
        n.ContentId = reader.ReadBytes(48);
        n.Digest = reader.ReadBytes(16);
        n.TitleHash = reader.ReadBytes(16);
        n.DevHash = reader.ReadBytes(16);
        n.ActivationTime = Utils.bit64hex(reader.ReadBytes(8), 0);
        n.ExpirantionTime = Utils.bit64hex(reader.ReadBytes(8), 0);

        if (!n.Validate())
            return null;
        return n;
    }

    private bool Validate()
    {
        if (Magic[0] == 78 && Magic[1] == 80 && Magic[2] == 68 && Magic[3] == 0 && ActivationTime.Equals(0) &&
            ExpirantionTime == 0)
            return true;
        return false;
    }
}