using System;
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
        List<Nca> ncas = new List<Nca>();
        public NCA(HOS hos, string mountname)
        {
            FileSystemClient fs = hos.horizon.Fs;
            foreach (DirectoryEntryEx entry in fs.EnumerateEntries(mountname.ToString(), "*.nca", SearchOptions.Default))
            {
                fs.OpenFile(out FileHandle nca, entry.FullPath.ToU8Span(), OpenMode.Read);
                ncas.Add(new Nca(hos.keys, new FileHandleStorage(fs, nca)));
            }
        }

        public void MountProgram(HOS hos)
        {
            for(int i = 0; i<ncas.Count; i++)
            {
                if(ncas[i].Header.ContentType == NcaContentType.Program)
                {
                    if(hos.CheckSignature == true)
                    {
                        if(ncas[i].VerifyHeaderSignature() == LibHac.Validity.Valid)
                        {
                            FileSystemClient fs = hos.horizon.Fs;
                            fs.Register("exefs".ToU8Span(), OpenFileSystemByType(NcaSectionType.Code, ncas[i]));
                            fs.Register("romfs".ToU8Span(), OpenFileSystemByType(NcaSectionType.Data, ncas[i]));
                        }
                        else
                        {
                            throw new Exception("Invalid Nca Header Signature.");
                        }
                    }
                    else
                    {
                        FileSystemClient fs = hos.horizon.Fs;
                        fs.Register("exefs".ToU8Span(), OpenFileSystemByType(NcaSectionType.Code, ncas[i]));
                        fs.Register("romfs".ToU8Span(), OpenFileSystemByType(NcaSectionType.Data, ncas[i]));
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
            return nca.OpenFileSystem(index, 0);
        }
    }
}
