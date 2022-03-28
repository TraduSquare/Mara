using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Mara.Lib.Configs
{
    public class GuiConfig
    {
        public string Credits { get; set; }
        public string MainBackgroundImage { get; set; }
        public string GameTitle { get; set; }

        [Preserve]
        [JsonConstructor]
        public GuiConfig() { }
    }
}
