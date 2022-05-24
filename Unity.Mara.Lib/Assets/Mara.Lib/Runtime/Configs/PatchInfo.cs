using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Mara.Lib.Configs
{
    public class PatchInfo
    {
        public int PatchId { get; set; }
        public string PatchVersion { get; set; }
        public string Changelog { get; set; }

        [Preserve]
        [JsonConstructor]
        public PatchInfo(int patchId, string patchVersion, string changelog)
        {
            PatchId = patchId;
            PatchVersion = patchVersion;
            Changelog = changelog;
        }
        
        public PatchInfo() { }
    }
}
