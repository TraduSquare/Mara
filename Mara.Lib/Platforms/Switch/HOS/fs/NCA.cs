using System;
using System.Collections.Generic;
using LibHac;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.Tools.Fs;
using LibHac.Tools.FsSystem;
using LibHac.Tools.FsSystem.NcaUtils;

namespace Mara.Lib.Platforms.Switch;

public class NCA
{
    private readonly List<Nca> BaseNcas = new();
    private readonly List<Nca> UpdateNcas = new();

    public NCA(HOS hos, string basemountname, string updatemountname = null)
    {
        var fs = hos.horizon.Fs;
        foreach (var entry in fs.EnumerateEntries(basemountname, "*.nca", SearchOptions.Default))
        {
            fs.OpenFile(out var nca, entry.FullPath.ToU8Span(), OpenMode.Read);
            BaseNcas.Add(new Nca(hos.keys, new Nca(hos.keys, new FileHandleStorage(fs, nca)).OpenDecryptedNca()));
        }

        if (updatemountname != null)
            foreach (var entry in fs.EnumerateEntries(updatemountname, "*.nca", SearchOptions.Default))
            {
                fs.OpenFile(out var nca, entry.FullPath.ToU8Span(), OpenMode.Read);
                UpdateNcas.Add(new Nca(hos.keys, new Nca(hos.keys, new FileHandleStorage(fs, nca)).OpenDecryptedNca()));
            }
    }

    public Result MountProgram(HOS hos, string titleid)
    {
        for (var i = 0; i < BaseNcas.Count; i++)
            if (BaseNcas[i].Header.ContentType == NcaContentType.Program)
            {
                if (BaseNcas[i].Header.TitleId.ToString("X16") == titleid)
                {
                    if (hos.CheckSignature)
                    {
                        if (BaseNcas[i].VerifyHeaderSignature() == Validity.Valid)
                        {
                            var fs = hos.horizon.Fs;

                            using var ExeFS =
                                new UniqueRef<IFileSystem>(OpenFileSystemByType(NcaSectionType.Code, BaseNcas[i]));
                            using var RomFS =
                                new UniqueRef<IFileSystem>(OpenFileSystemByType(NcaSectionType.Data, BaseNcas[i]));

                            fs.Register("exefs".ToU8Span(), ref ExeFS.Ref);
                            fs.Register("romfs".ToU8Span(), ref RomFS.Ref);

                            return Result.Success;
                        }

                        throw new Exception("Invalid Nca Header Signature.");
                    }

                    {
                        var fs = hos.horizon.Fs;

                        using var ExeFS =
                            new UniqueRef<IFileSystem>(OpenFileSystemByType(NcaSectionType.Code, BaseNcas[i]));
                        using var RomFS =
                            new UniqueRef<IFileSystem>(OpenFileSystemByType(NcaSectionType.Data, BaseNcas[i]));

                        fs.Register("exefs".ToU8Span(), ref ExeFS.Ref);
                        fs.Register("romfs".ToU8Span(), ref RomFS.Ref);

                        return Result.Success;
                    }
                }

                throw new Exception("Mismatch TitleID: The game doesnt match with the patch.");
            }

        throw new Exception("NCA: The Program NCA not found.");
    }

    private IFileSystem OpenFileSystemByType(NcaSectionType type, Nca nca)
    {
        return OpenFileSystem(Nca.GetSectionIndexFromType(type, nca.Header.ContentType), nca);
    }

    private IFileSystem OpenFileSystem(int index, Nca nca)
    {
        if (UpdateNcas.Count > 0)
        {
            for (var i = 0; i < UpdateNcas.Count; i++)
                if (UpdateNcas[i].Header.ContentType == NcaContentType.Program)
                {
                    if (UpdateNcas[i].Header.TitleId.ToString("X16") == nca.Header.TitleId.ToString("X16"))
                        try
                        {
                            return nca.OpenFileSystemWithPatch(UpdateNcas[i], index, 0);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                "Partición del tipo: {0} no encontrado en la update, procediendo a abrir la partición {0} del nca base.",
                                index);
                            return nca.OpenFileSystem(index, 0);
                        }

                    throw new Exception("Mismatch TitleID: The game doesnt match with the update.");
                }

            throw new Exception("NCA: The Program NCA not found.");
        }

        return nca.OpenFileSystem(index, 0);
    }
}