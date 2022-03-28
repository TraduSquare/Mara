using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Mara.Lib.Configs
{
    public class PatchInfo
    {
        public int PatchId { get; set; }
        public string PatchVersion { get; set; }
        public string Changelog { get; set; }

        [Preserve]
        [JsonConstructor]
        public PatchInfo() { }
    }
}
