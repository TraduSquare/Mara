using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Yarhl.IO;

namespace Mara.Lib.Common
{
    /// <summary>
    /// Extrae el listado de appsIDs de los juegos de steam
    /// </summary>
    internal class PackageInfo
    {
        public List<int> AppID = new List<int>();
        public DataReader _reader;
        public PackageInfo(string path)
        {
            if (File.Exists(path))
                _reader = new DataReader(DataStreamFactory.FromFile(path, FileOpenMode.Read));
            else
                throw new Exception("Archivo no encontrado");

            while (true)
            {
                var p = Search(_reader.Stream, new byte[] { 0x61, 0x70, 0x70, 0x69, 0x64, 0x73, 0x00, 0x02, 0x30, 0x00 });
                if (p != -1)
                    AppID.Add(_reader.ReadInt32());
                else
                {
                    break;
                }
            }
        }

        static long Search(Stream stream, byte[] pattern)
        {
            long start = -1;

            while (stream.Position < stream.Length)
            {
                if (stream.ReadByte() != pattern[0])
                    continue;

                start = stream.Position - 1;

                for (int idx = 1; idx < pattern.Length; idx++)
                {
                    if (stream.ReadByte() != pattern[idx])
                    {
                        start = -1;
                        break;
                    }
                }

                if (start > -1)
                {
                    return start;
                }
            }

            return start;
        }
    }

    public class Steam
    {
        PackageInfo packageinfo;

        public Steam()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // deberia ser igual en todos lados desconozco si en Deck es diferente
                packageinfo = new PackageInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".steam{Path.PathSeparator}" +
                    $"debian-installation{Path.PathSeparator}appcache{Path.PathSeparator}packageinfo.vdf"));
            } 
            else
            {
                // No se si esto puede variar
                packageinfo = new PackageInfo("C:\\Program Files (x86)\\Steam\\appcache\\packageinfo.vdf");
            }
        }

        /// <summary>
        /// Comprueba si el usuario tiene el appid
        /// </summary>
        /// <param name="appid">Appid de la store, puedes obtener el appid en steamDB https://steamdb.info/</param>
        /// <returns></returns>
        public bool IsGameOwned(int appid)
        {
            if (packageinfo.AppID.Contains(appid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
