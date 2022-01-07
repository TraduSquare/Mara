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
        Node Cia { get; set; }
        Node ContentNode { get; set; }
        List<string> ContentToPatch = new List<string>();

        public Main(string oriFile, string outFile, string patchPath, PatchMode patchMode) : base(oriFile, outFile, patchPath)
        {
            PatchMode = patchMode;

            try
            {
                Cia = NodeFactory.FromFile(oriFile, "root").TransformWith<BinaryCia2NodeContainer>();
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    throw new FormatException($"{Path.GetFileName(oriFile)} is an invalid CIA file.", ex);
                }
                else
                {
                    throw new Exception($"An error has ocurred while reading the file.", ex);
                }
            }

            ContentNode = Cia.Children["content"];

            foreach (var content in ContentNode.Children)
            {
                if (content.Tags.ContainsKey("LEMON_NCCH_ENCRYPTED"))
                {
                    throw new Exception("Encrypted (legit) CIA not supported");
                }

                for (int i = 0; i < maraConfig.FilesInfo.ListOriFiles.Length; i++)
                {
                    if (maraConfig.FilesInfo.ListOriFiles[i].StartsWith(content.Name))
                    {
                        ContentToPatch.Add(content.Name);
                    }
                }
            }
        }

        public override (int, string) ApplyTranslation()
        {
            var count = maraConfig.FilesInfo.ListOriFiles.Length;
            var files = maraConfig.FilesInfo;

            switch (PatchMode)
            {
                case PatchMode.General:
                    foreach (var contentToPatch in ContentToPatch)
                    {
                        var contentChildNode = ContentNode.Children[contentToPatch];
                        contentChildNode.TransformWith<Binary2Ncch>();

                        foreach (var node in Navigator.IterateNodes(contentChildNode))
                        {
                            node.Stream.WriteTo(tempFolder + Path.DirectorySeparatorChar + contentToPatch + Path.DirectorySeparatorChar + node.Name);
                        }
                    }

                    for (int i = 0; i < count; i++)
                    {
                        var file = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                        var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";

                        var result = ApplyXdelta(file, xdelta, file, files.ListMd5Files[i]);

                        if (result.Item1 != 0)
                            return result;

                        var content = files.ListOriFiles[i].Split('\\')[0];
                        ContentNode.Children[content].Add(NodeFactory.FromFile(file, Path.GetFileName(files.ListOriFiles[i])));
                    }

                    foreach (var contentToPatch in ContentToPatch)
                    {
                        var contentChildNode = ContentNode.Children[contentToPatch];
                        contentChildNode.TransformWith<Ncch2Binary>();
                    }
                    break;
                case PatchMode.Specific:
                    throw new NotImplementedException();
            }

            Cia.TransformWith<NodeContainer2BinaryCia>();

            if (File.Exists(outFolder))
            {
                File.Delete(outFolder);
            }

            Cia.Stream.WriteTo(outFolder);
            Cia.Stream?.Dispose();

            return base.ApplyTranslation();
        }
    }

    public enum PatchMode
    {
        General,
        Specific
    }
}
