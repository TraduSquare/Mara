using LibHac;
using LibHac.Common.Keys;
using LibHac.Fs;

namespace Mara.Lib.Platforms.Switch
{
    internal class HOS
    {
        public KeySet keys;
        public HorizonClient horizon;

        public HOS(string Keys)
        {
            this.keys = ExternalKeyReader.ReadKeyFile(Keys);
            Horizon tmp = HorizonFactory.CreateWithDefaultFsConfig(new HorizonConfiguration(), new InMemoryFileSystem(), this.keys);
            this.horizon = tmp.CreatePrivilegedHorizonClient();
        }
    }
}
