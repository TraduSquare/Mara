using System;
using System.Collections.Generic;
using System.IO;

namespace Mara.Lib.Common.Steam
{

    internal class LibraryFolders
    {
        public List<string> SteamLibraries = new();

        public LibraryFolders(string path)
        {
            if (File.Exists(path))
            {
                var txt = File.ReadAllLines(path);

                foreach (var s in txt)
                    if (s.Contains("\"path\""))
                        SteamLibraries.Add(s.Replace("		\"path\"		\"", "").Replace("\"", ""));
            }
            else
            {
                throw new Exception("Archivo no encontrado");
            }
        }
    }
}