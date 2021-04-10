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
        public Main(MaraConfig config, string GamePath) : base(config)
        {
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
            throw new NotImplementedException();
        }
    }
}
