using SceneGate.Lemon.Containers.Converters;
using SceneGate.Lemon.Titles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileSystem;

namespace Mara.Lib.Platforms._3ds
{
    class Main : PatchProcess
    {
        public enum PatchMode {
            General,
            Specific
        }

        public PatchMode GetPatchMode { get; set; }

        public Main(MaraConfig config, string GamePath, PatchMode patchMode) : base(config)
        {
            GetPatchMode = patchMode;

            if (GamePath.Contains(".cia"))
            {                
                var data = NodeFactory.FromFile(GamePath, "root")
                    .TransformWith<BinaryCia2NodeContainer>();

                string titleId = data.Children["title"]
                    .TransformWith<Binary2TitleMetadata>()
                    .GetFormatAs<TitleMetadata>()
                    .TitleId.ToString("X16");

                //Add region check via MaraConfig using titleId

                var programNode = data.Children["content"].Children["program"];
                if (programNode.Tags.ContainsKey("LEMON_NCCH_ENCRYPTED"))
                {
                    throw new Exception("Encrypted (legit) CIA not supported");
                }

                programNode.TransformWith<Binary2Ncch>();
                programNode.Children["rom"].TransformWith<BinaryIvfc2NodeContainer>();
                programNode.Children["system"].TransformWith<BinaryExeFs2NodeContainer>();                
            }
            else
            {
                throw new Exception("Unrecognized file.");
            }
        }

        public override (int, string) ApplyTranslation()
        {
            switch (GetPatchMode)
            {
                case PatchMode.General: //Generic ApplyTranslation method as a test model
                    var count = maraConfig.FilesInfo.ListOriFiles.Length;
                    var dirTemp = maraConfig.TempFolder;
                    var fileTemp = $"{dirTemp}{Path.DirectorySeparatorChar}files";

                    if (Directory.Exists(fileTemp))
                        Directory.CreateDirectory(fileTemp);

                    var files = maraConfig.FilesInfo;

                    for (int i = 0; i < count; i++)
                    {
                        var result = ApplyXdelta($"{maraConfig.FilePath}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}",
                            $"{dirTemp}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}",
                            $"{maraConfig.FilePath}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}",
                            files.ListXdeltaFiles[i], true);
                        if (result.Item1 != 0)
                            return result;
                    }
                    break;
                case PatchMode.Specific:
                    throw new NotImplementedException();
            }
            return (0, string.Empty);
        }
    }
}
