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
        Dictionary<string, List<string>> ContentToPatch = new Dictionary<string, List<string>>();

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
                    var splitLine = maraConfig.FilesInfo.ListOriFiles[i].Split('\\');

                    if (splitLine[0] == content.Name)
                    {
                        if (ContentToPatch.ContainsKey(splitLine[0]))
                        {
                            if (!ContentToPatch[splitLine[0]].Contains(splitLine[1]))
                            {
                                ContentToPatch[splitLine[0]].Add(splitLine[1]);
                            }
                        }
                        else
                        {
                            ContentToPatch.Add(splitLine[0], new List<string> { splitLine[1] });
                        }

                        if (patchMode == PatchMode.Specific && splitLine[1] != "rom")
                        {
                            throw new FormatException($"Unsupported specific patching for {splitLine[1]}");
                        }
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
                        var contentChildNode = ContentNode.Children[contentToPatch.Key];
                        contentChildNode.TransformWith<Binary2Ncch>();

                        foreach (var node in Navigator.IterateNodes(contentChildNode))
                        {
                            node.Stream.WriteTo(tempFolder + Path.DirectorySeparatorChar + contentToPatch.Key + Path.DirectorySeparatorChar + node.Name);
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
                        var contentChildNode = ContentNode.Children[contentToPatch.Key];
                        contentChildNode.TransformWith<Ncch2Binary>();
                    }
                    break;
                case PatchMode.Specific:
                    foreach (var contentToPatch in ContentToPatch)
                    {
                        var contentNode = ContentNode.Children[contentToPatch.Key];
                        contentNode.TransformWith<Binary2Ncch>();

                        foreach (var node in Navigator.IterateNodes(contentNode))
                        {
                            switch(node.Name)
                            {
                                case "rom":
                                    node.TransformWith<BinaryIvfc2NodeContainer>();
                                    break;
                            }

                            foreach(var nodeChild in Navigator.IterateNodes(node))
                            {
                                var path = nodeChild.Path.Replace($"/root/content/{contentToPatch.Key}/", "").Replace('/', Path.DirectorySeparatorChar);

                                // Using node.Stream will return null for some reason
                                var nodeChildFromRoot = Navigator.SearchNode(Cia, nodeChild.Path);
                                if (!nodeChildFromRoot.IsContainer)
                                {
                                    nodeChildFromRoot.Stream.WriteTo(tempFolder + Path.DirectorySeparatorChar + contentToPatch.Key + Path.DirectorySeparatorChar + path);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < count; i++)
                    {
                        var file = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                        var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";

                        var result = ApplyXdelta(file, xdelta, file, files.ListMd5Files[i]);

                        if (result.Item1 != 0)
                            return result;

                        // We need to delete these since later we're going to make a new Node container with this folder
                        if (File.Exists($"{file}_ori"))
                        {
                            File.Delete($"{file}_ori");
                        }

                        if (File.Exists(xdelta))
                        {
                            File.Delete(xdelta);
                        }
                    }

                    foreach (var contentToPatch in ContentToPatch)
                    {
                        var contentPath = $"{tempFolder}{Path.DirectorySeparatorChar}{contentToPatch.Key}{Path.DirectorySeparatorChar}";

                        foreach (var contentChild in ContentToPatch[contentToPatch.Key])
                        {
                            var contentChildPath = $"{contentPath}{Path.DirectorySeparatorChar}{contentChild}";

                            switch (contentChild)
                            {
                                case "rom":
                                    ContentNode.Children[contentToPatch.Key].Add(NodeFactory.FromDirectory(contentChildPath, "*", contentChild, true));
                                    ContentNode.Children[contentToPatch.Key].Children[contentChild].TransformWith<NodeContainer2BinaryIvfc>();
                                    break;
                            }
                        }

                        ContentNode.Children[contentToPatch.Key].TransformWith<Ncch2Binary>();
                    }
                    break;
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
