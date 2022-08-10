using System;
using System.Buffers;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.Fs;
using Path = System.IO.Path;

namespace Mara.Lib.Platforms.Switch;

internal class FSUtils
{
    public static Result MountFolder(FileSystemClient fs, string path, string mountname)
    {
        using var LocalFS = new UniqueRef<IFileSystem>(new LocalFileSystem(path));
        return fs.Register(mountname.ToU8Span(), ref LocalFS.Ref());
    }

    public static Result CopyFile(FileSystemClient fs, string srcPath, string dstPath)
    {
        U8Span sourcePath = srcPath.ToU8String();
        U8Span destPath = dstPath.ToU8String();
        var rc = fs.OpenFile(out var sourceHandle, sourcePath, OpenMode.Read);
        if (rc.IsFailure()) return rc;
        fs.EnsureDirectoryExists(Path.GetDirectoryName(dstPath));
        try
        {
            rc = fs.GetFileSize(out var oriFileSize, sourceHandle);
            if (rc.IsFailure()) return rc;
            rc = fs.CreateOrOverwriteFile(dstPath, oriFileSize);
            if (rc.IsFailure()) return rc;
            rc = fs.OpenFile(out var destHandle, destPath, OpenMode.All);
            if (rc.IsFailure()) return rc;

            try
            {
                const int maxBufferSize = 1024 * 1024;

                rc = fs.GetFileSize(out var fileSize, sourceHandle);
                if (rc.IsFailure()) return rc;

                var bufferSize = (int)Math.Min(maxBufferSize, fileSize);

                var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    for (long offset = 0; offset < fileSize; offset += bufferSize)
                    {
                        var toRead = (int)Math.Min(fileSize - offset, bufferSize);
                        var buf = buffer.AsSpan(0, toRead);

                        rc = fs.ReadFile(out var _, sourceHandle, offset, buf);
                        if (rc.IsFailure()) return rc;

                        rc = fs.WriteFile(destHandle, offset, buf, WriteOption.None);
                        if (rc.IsFailure()) return rc;
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }

                rc = fs.FlushFile(destHandle);
                if (rc.IsFailure()) return rc;
            }
            finally
            {
                if (destHandle.IsValid)
                    fs.CloseFile(destHandle);
            }
        }
        finally
        {
            if (sourceHandle.IsValid)
                fs.CloseFile(sourceHandle);
        }

        return Result.Success;
    }
}