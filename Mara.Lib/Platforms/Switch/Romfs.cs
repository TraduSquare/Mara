using LibHac;
using LibHac.Fs;
using LibHac.FsSystem;
using LibHac.FsSystem.RomFs;
using System;
using System.IO;

namespace Mara.Lib.Platforms.Switch
{
    public class Romfs
    {
        private IStorage romFs;
        private long romFssize;

        public Romfs(string folder)
        {
            var builder = new RomFsBuilder(new LocalFileSystem(folder));
            this.romFs = builder.Build();
            this.romFs.GetSize(out this.romFssize).ThrowIfFailure();
        }

        public Stream ToStream()
        {
            return romFs.AsStream();
        }

        public Result DumpToFile(string path, bool deletefolder = false)
        {
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                romFs.CopyToStream(file, romFssize);
            }
            return Result.Success;
        }
    }
}
