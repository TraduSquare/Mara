using System;
using System.Buffers;
using System.IO;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;

namespace Mara.Lib.Platforms.Switch
{
    class FSUtils
    {
        public static void MountFolder(FileSystemClient fs, string path, string mountname)
        {
            fs.Register(mountname.ToU8Span(), new LocalFileSystem(path));
        }

        public static Result CopyFile(FileSystemClient fs, string srcPath, string dstPath)
        {
            U8Span sourcePath = srcPath.ToU8String();
            U8Span destPath = dstPath.ToU8String();
            Result rc = fs.OpenFile(out FileHandle sourceHandle, sourcePath, OpenMode.Read);
            if (rc.IsFailure()) return rc;
            fs.EnsureDirectoryExists(Path.GetDirectoryName(dstPath));
            try
            {
                rc = fs.OpenFile(out FileHandle destHandle, destPath, OpenMode.Write | OpenMode.AllowAppend);
                if (rc.IsFailure()) return rc;

                try
                {
                    const int maxBufferSize = 1024 * 1024;

                    rc = fs.GetFileSize(out long fileSize, sourceHandle);
                    if (rc.IsFailure()) return rc;

                    int bufferSize = (int)Math.Min(maxBufferSize, fileSize);

                    byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                    try
                    {
                        for (long offset = 0; offset < fileSize; offset += bufferSize)
                        {
                            int toRead = (int)Math.Min(fileSize - offset, bufferSize);
                            Span<byte> buf = buffer.AsSpan(0, toRead);

                            rc = fs.ReadFile(out long _, sourceHandle, offset, buf);
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
}
