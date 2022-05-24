using System.IO;
using Mara.Lib.Configs;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Mara.Lib
{
    public class MaraConfig
    {
        public GuiConfig GuiConfig { get; set; }
        public PatchFilesInfo FilesInfo { get; set; }
        public PatchInfo Info { get; set; }
        public string OutFolder { get; set; }
        public string TempFolder { get; set; }
        public string FilePath { get; set; }

        [Preserve]
        [JsonConstructor]
        public MaraConfig(GuiConfig guiConfig, PatchFilesInfo filesInfo, PatchInfo info, string outFolder, string tempFolder, string filePath)
        {
            GuiConfig = guiConfig;
            FilesInfo = filesInfo;
            Info = info;
            OutFolder = outFolder;
            TempFolder = tempFolder;
            FilePath = filePath;
        }
        
        public MaraConfig() { }
    }
}
