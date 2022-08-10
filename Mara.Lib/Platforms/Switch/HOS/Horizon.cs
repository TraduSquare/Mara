using LibHac;
using LibHac.Common.Keys;
using LibHac.Tools.Fs;

namespace Mara.Lib.Platforms.Switch;

public class HOS
{
    public bool CheckSignature;
    public HorizonClient horizon;
    public KeySet keys;

    public HOS(string Keys, bool Sig = false)
    {
        keys = ExternalKeyReader.ReadKeyFile(Keys);
        keys.DeriveKeys();
        var tmp = HorizonFactory.CreateWithDefaultFsConfig(new HorizonConfiguration(), new InMemoryFileSystem(), keys);
        horizon = tmp.CreatePrivilegedHorizonClient();
        CheckSignature = Sig;
    }
}