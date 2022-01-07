using SceneGate.Lemon.Containers.Converters;
using SceneGate.Lemon.Titles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace Mara.Lib.Platforms._3ds
{
    public class Main : PatchProcess
    {
        public PatchMode PatchMode { get; set; }
        Node cia { get; set; }
        Node programNode { get; set; }

        public Main(string oriFile, string outFile, string patchPath, PatchMode patchMode) : base(oriFile, outFile, patchPath)
        {
            PatchMode = patchMode;

            try
            {
                cia = NodeFactory.FromFile(oriFile, "root").TransformWith<BinaryCia2NodeContainer>();
            }
            catch (Exception ex)
            {
                throw new FormatException($"{Path.GetFileName(oriFile)} is an invalid CIA file.", ex);
            }

            programNode = cia.Children["content"].Children["program"];
            if (programNode.Tags.ContainsKey("LEMON_NCCH_ENCRYPTED"))
            {
                throw new Exception("Encrypted (legit) CIA not supported");
            }
        }

        public override (int, string) ApplyTranslation()
        {
            var count = maraConfig.FilesInfo.ListOriFiles.Length;
            var files = maraConfig.FilesInfo;

            programNode.TransformWith<Binary2Ncch>();

            switch (PatchMode)
            {
                case PatchMode.General:
                    foreach (var node in Navigator.IterateNodes(programNode))
                    {
                        node.Stream.WriteTo(tempFolder + Path.DirectorySeparatorChar + node.Name);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        var file = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                        var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";

                        var result = ApplyXdelta(file, xdelta, file, files.ListMd5Files[i]);

                        if (result.Item1 != 0)
                            return result;

                        programNode.Add(NodeFactory.FromFile(file, files.ListOriFiles[i]));
                    }
                    break;
                case PatchMode.Specific:
                    throw new NotImplementedException();
                    programNode.Children["rom"].TransformWith<BinaryIvfc2NodeContainer>();
                    programNode.Children["system"].TransformWith<BinaryExeFs2NodeContainer>();
            }

            programNode.TransformWith<Ncch2Binary>();
            cia.TransformWith<NodeContainer2BinaryCia>();

            if (File.Exists(outFolder))
            {
                File.Delete(outFolder);
            }

            cia.Stream.WriteTo(outFolder);
            cia.Stream?.Dispose();

            return base.ApplyTranslation();
        }
    }

    public enum PatchMode
    {
        General,
        Specific
    }
}
