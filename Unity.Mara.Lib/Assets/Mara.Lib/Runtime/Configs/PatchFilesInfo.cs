using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Unity.Mara.Lib.Configs
{
    public class PatchFilesInfo
    {
        public string[] ListOriFiles { get; set; }
        public string[] ListXdeltaFiles { get; set; }
        public string[] ListMd5Files { get; set; }
        public string[] ListExcludeFiles { get; set; }

        [Preserve]
        [JsonConstructor]
        public PatchFilesInfo(string[] listOriFiles, string[] listXdeltaFiles, string[] listMd5Files, string[] listExcludeFiles)
        {
            ListOriFiles = listOriFiles;
            ListXdeltaFiles = listXdeltaFiles;
            ListMd5Files = listMd5Files;
            ListExcludeFiles = listExcludeFiles;
        }
        
        public PatchFilesInfo() { }
    }
}
