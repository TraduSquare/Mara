using SceneGate.Lemon.Containers.Converters;
using SceneGate.Lemon.Titles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace Mara.Lib.Platforms._3ds
{
    public class Main : PatchProcess
    {
        public PatchMode PatchMode { get; set; }
        bool UseFileStream { get; set; }
        Node Cia { get; set; }
        Node ContentNode { get; set; }
        Dictionary<string, List<string>> ContentToPatch = new Dictionary<string, List<string>>();

        public Main(string oriFile, string outFile, string patchPath, PatchMode patchMode, bool useFileStream = false) : base(oriFile, outFile, patchPath)
        {
            PatchMode = patchMode;
            UseFileStream = useFileStream;

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

                        if (patchMode == PatchMode.Specific)
                        {
                            switch(splitLine[1])
                            {
                                case "rom":
                                    break;
                                case "system":
                                    break;
                                default:
                                    throw new FormatException($"Unsupported specific patching for {splitLine[1]}");
                            }
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

                        var result = ApplyXdelta(file, xdelta, file, files.ListMd5Files[i], UseFileStream);

                        if (result.Item1 != 0)
                            return result;

                        var content = files.ListOriFiles[i].Split('\\')[0];
                        ContentNode.Children[content].Add(NodeFactory.FromFile(file, Path.GetFileName(files.ListOriFiles[i])));
                    }

                    foreach (var contentToPatch in ContentToPatch)
                    {
                        var contentChildNode = ContentNode.Children[contentToPatch.Key];
                        var contentChildPath = $"{tempFolder}{Path.DirectorySeparatorChar}{contentToPatch.Key}";
                        contentChildNode.TransformWith<Ncch2Binary>();

                        // Doing this reduces the amount of memory used.
                        Directory.Delete(contentChildPath, true);
                        contentChildNode.Stream.WriteTo(contentChildPath);

                        ContentNode.Add(NodeFactory.FromFile(contentChildPath, contentToPatch.Key));
                    }
                    break;
                case PatchMode.Specific:
                    foreach (var contentToPatch in ContentToPatch)
                    {
                        var contentNode = ContentNode.Children[contentToPatch.Key];
                        contentNode.TransformWith<Binary2Ncch>();

                        foreach (var node in Navigator.IterateNodes(contentNode))
                        {
                            if (contentToPatch.Value.Contains(node.Name))
                            {
                                switch (node.Name)
                                {
                                    case "rom":
                                        node.TransformWith<BinaryIvfc2NodeContainer>();
                                        break;
                                    case "system":
                                        node.TransformWith<BinaryExeFs2NodeContainer>();
                                        break;
                                    default:
                                        continue;
                                }

                                foreach (var nodeChild in Navigator.IterateNodes(node))
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
                    }

                    for (int i = 0; i < count; i++)
                    {
                        var file = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}";
                        var xdelta = $"{tempFolder}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}";

                        var result = ApplyXdelta(file, xdelta, file, files.ListMd5Files[i], UseFileStream);

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
                        var contentPath = $"{tempFolder}{Path.DirectorySeparatorChar}{contentToPatch.Key}";

                        foreach (var contentChild in ContentToPatch[contentToPatch.Key])
                        {
                            var contentChildPath = $"{contentPath}{Path.DirectorySeparatorChar}{contentChild}";
                            ContentNode.Children[contentToPatch.Key].Add(NodeFactory.FromDirectory(contentChildPath, "*", contentChild, true));

                            switch (contentChild)
                            {
                                case "rom":
                                    ContentNode.Children[contentToPatch.Key].Children[contentChild].TransformWith<NodeContainer2BinaryIvfc>();
                                    break;
                                case "system":
                                    IConverter<NodeContainerFormat, BinaryFormat> binaryConverter = new BinaryExeFs2NodeContainer();
                                    var convertedNode = binaryConverter.Convert(ContentNode.Children[contentToPatch.Key].Children[contentChild].GetFormatAs<NodeContainerFormat>());
                                    ContentNode.Children[contentToPatch.Key].Children[contentChild].ChangeFormat(convertedNode);
                                    break;
                            }
                        }

                        ContentNode.Children[contentToPatch.Key].TransformWith<Ncch2Binary>();

                        // Doing this reduces the amount of memory used.
                        Directory.Delete(contentPath, true);
                        ContentNode.Children[contentToPatch.Key].Stream.WriteTo(contentPath);
                        ContentNode.Children[contentToPatch.Key].Dispose();

                        ContentNode.Add(NodeFactory.FromFile(contentPath, contentToPatch.Key));
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
