﻿using System.IO;
using Mara.Lib.Configs;

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
        public MaraPlatform Platform { get; set; }
    }

    public enum MaraPlatform
    {
        Nintendo3ds,
        Generic,
        Playstation3,
        Playstation4,
        NintendoSwitch,
        PlaystationVita,
        None
    }
}
