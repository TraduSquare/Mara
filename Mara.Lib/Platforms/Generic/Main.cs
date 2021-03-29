using System.IO;

namespace Mara.Lib.Platforms.Generic
{
    public class Main : PatchProcess
    {
        public Main(MaraConfig config) : base(config)
        { }

        public override (int, string) ApplyTranslation()
        {
            var count = maraConfig.FilesInfo.ListOriFiles.Length;
            var dirTemp = maraConfig.TempFolder;
            var fileTemp = $"{dirTemp}{Path.DirectorySeparatorChar}files";

            if (Directory.Exists(fileTemp))
                Directory.CreateDirectory(fileTemp);

            var files = maraConfig.FilesInfo;

            for (int i = 0; i < count; i++)
            {
                var result = ApplyXdelta($"{maraConfig.FilePath}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}",
                    $"{dirTemp}{Path.DirectorySeparatorChar}{files.ListXdeltaFiles[i]}",
                    $"{maraConfig.FilePath}{Path.DirectorySeparatorChar}{files.ListOriFiles[i]}",
                    files.ListXdeltaFiles[i], true);
                if (result.Item1 != 0)
                    return result;
            }

            return (0, string.Empty);
        }
    }
}
