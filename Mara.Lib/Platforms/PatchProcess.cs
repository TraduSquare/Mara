using System;
using System.IO;

namespace Mara.Lib.Platforms
{
    public class PatchProcess
    {
        protected MaraConfig maraConfig;
        public PatchProcess(MaraConfig config)
        {
            maraConfig = config;
            GenerateTempFolder();
            ExtractPatch();
        }

        public virtual (int, string) ApplyTranslation()
        {
            return (0, string.Empty);
        }


        protected (int, string) ApplyXdelta(string file, string xdelta, string result, string md5, bool copyOri = false)
        {
            if (!File.Exists(file + "_ori") && copyOri)
            {
                File.Copy(file, file + "_ori");
                file += "_ori";
            }
                
            else if (copyOri)
            {
                File.Delete(file);
                file += "_ori";
            }

            if (Common.Md5.CalculateMd5(file) != md5)
                return (1, $"The file \"{file}\" is not equal than the original file.");

            try
            {
                Common.Xdelta.Apply(File.Open(file, FileMode.Open), File.ReadAllBytes(xdelta), result);
            }
            catch (Exception e)
            {
                return (99, $"Error patching the file.\n\n{e.Message}");
            }

            return (0, string.Empty);
        }

        private void GenerateTempFolder()
        {
            maraConfig.TempFolder = Path.GetTempPath() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Directory.CreateDirectory(maraConfig.TempFolder);
        }

        private void ExtractPatch()
        {
            // Todo - Extract 7z
        }

    }
}
