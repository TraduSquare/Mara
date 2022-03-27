using System.IO;
using Unity.Mara.Lib.Configs;

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
        
        public MaraConfig() { }
    }
}
