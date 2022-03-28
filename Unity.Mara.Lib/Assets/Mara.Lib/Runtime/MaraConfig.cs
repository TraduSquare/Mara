using System.IO;
using Unity.Mara.Lib.Configs;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Mara.Lib
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
        public MaraConfig() { }
    }
}
