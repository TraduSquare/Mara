using UnityEngine;
using UnityEditor;
using LibHac.FsSystem.RomFs;
using LibHac.FsSystem;
using LibHac.Fs;
using System.IO;
using K4os.Compression.LZ4.Streams;

namespace Unity.Mara.Lib
{
    public class SetupEditor
    {
        [MenuItem("Mara/Build Romfs")]
        private static void BuildRomfs()
        {
            string path = EditorUtility.OpenFolderPanel("Selecciona Carpeta", "", "");
            string output = EditorUtility.SaveFilePanel("Selecciona donde guarda el parche", "", "", "romfs");

            var localFs = new LocalFileSystem(path);
            RomFsBuilder builder = new RomFsBuilder(localFs);
            IStorage romFs = builder.Build();

            romFs.GetSize(out long romFsSize).ThrowIfFailure();
            using (var target = LZ4Stream.Encode(File.Create(output + ".lz4"), K4os.Compression.LZ4.LZ4Level.L12_MAX))
            {
                romFs.CopyToStream(target, romFsSize);
            }
        }
    }
}