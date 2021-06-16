﻿using System;
using System.Collections.Generic;
using LibHac.Common;
using LibHac.Fs;
using LibHac.Fs.Fsa;
using LibHac.FsSystem;
using LibHac.FsSystem.NcaUtils;

namespace Mara.Lib.Platforms.Switch
{
    public class NCA
    {
        List<Nca> BaseNcas = new List<Nca>();
        List<Nca> UpdateNcas = new List<Nca>();
        public NCA(HOS hos, string basemountname, string updatemountname = null)
        {
            FileSystemClient fs = hos.horizon.Fs;
            foreach (DirectoryEntryEx entry in fs.EnumerateEntries(basemountname.ToString(), "*.nca", SearchOptions.Default))
            {
                fs.OpenFile(out FileHandle nca, entry.FullPath.ToU8Span(), OpenMode.Read);
                BaseNcas.Add(new Nca(hos.keys, new FileHandleStorage(fs, nca)));
            }
            if (updatemountname != null)
            {
                foreach (DirectoryEntryEx entry in fs.EnumerateEntries(updatemountname.ToString(), "*.nca", SearchOptions.Default))
                {
                    fs.OpenFile(out FileHandle nca, entry.FullPath.ToU8Span(), OpenMode.Read);
                    UpdateNcas.Add(new Nca(hos.keys, new FileHandleStorage(fs, nca)));
                }
            }
        }

        public void MountProgram(HOS hos, string titleid)
        {
            for(int i = 0; i< BaseNcas.Count; i++)
            {
                if(BaseNcas[i].Header.ContentType == NcaContentType.Program)
                {
                    if (BaseNcas[i].Header.TitleId.ToString("X16") == titleid)
                    {
                        if (hos.CheckSignature == true)
                        {
                            if (BaseNcas[i].VerifyHeaderSignature() == LibHac.Validity.Valid)
                            {
                                FileSystemClient fs = hos.horizon.Fs;
                                fs.Register("exefs".ToU8Span(), OpenFileSystemByType(NcaSectionType.Code, BaseNcas[i]));
                                fs.Register("romfs".ToU8Span(), OpenFileSystemByType(NcaSectionType.Data, BaseNcas[i]));
                            }
                            else
                            {
                                throw new Exception("Invalid Nca Header Signature.");
                            }
                        }
                        else
                        {
                            FileSystemClient fs = hos.horizon.Fs;
                            fs.Register("exefs".ToU8Span(), OpenFileSystemByType(NcaSectionType.Code, BaseNcas[i]));
                            fs.Register("romfs".ToU8Span(), OpenFileSystemByType(NcaSectionType.Data, BaseNcas[i]));
                        }
                    }
                    else
                    {
                        throw new Exception("Mismatch TitleID: The game doesnt match with the patch.");
                    }
                }
            }
        }

        private IFileSystem OpenFileSystemByType(NcaSectionType type, Nca nca)
        {
            return OpenFileSystem(Nca.GetSectionIndexFromType(type, nca.Header.ContentType), nca);
        }

        private IFileSystem OpenFileSystem(int index, Nca nca)
        {
            if(UpdateNcas.Count > 0)
            {
                for (int i = 0; i < UpdateNcas.Count; i++)
                {
                    if (UpdateNcas[i].Header.ContentType == NcaContentType.Program)
                    {
                        var titleid = nca.Header.TitleId.ToString("X16").ToCharArray();
                        titleid[13] = '8';
                        if (UpdateNcas[i].Header.TitleId.ToString("X16") == titleid.ToString())
                        {
                            // Si o si hay que verificar la firma del update ya que sino el resultado puede ser catastrófico
                            if (UpdateNcas[i].VerifyHeaderSignature() == LibHac.Validity.Valid)
                            {
                                nca.OpenFileSystemWithPatch(UpdateNcas[i], index, 0);
                            }
                        }
                        else
                        {
                            throw new Exception("Mismatch TitleID: The game doesnt match with the update.");
                        }
                    }
                }
            }
                
            return nca.OpenFileSystem(index, 0);
        }
    }
}
