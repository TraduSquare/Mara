using System.IO;
using LibHac;
using LibHac.Fs;
using LibHac.FsSystem;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.RomFs;

namespace Mara.Lib.Platforms.Switch;

public class Romfs
{
    private readonly IStorage romFs;
    private readonly long romFssize;

    public Romfs(string folder)
    {
        var builder = new RomFsBuilder(new LocalFileSystem(folder));
        romFs = builder.Build();
        romFs.GetSize(out romFssize).ThrowIfFailure();
    }

    public Stream ToStream()
    {
        return romFs.AsStream();
    }

    public Result DumpToFile(string path, bool deletefolder = false)
    {
        using (var file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
        {
            romFs.CopyToStream(file, romFssize);
        }

        return Result.Success;
    }
}