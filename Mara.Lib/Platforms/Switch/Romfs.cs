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

        public Romfs(string folder)
        {
            var builder = new RomFsBuilder(new LocalFileSystem(folder));
            this.romFs = builder.Build();
        }

        public Stream ToStream()
        {
            return romFs.AsStream();
        }
    }
}
