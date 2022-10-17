using System;
using System.IO;
using Mara.Lib.Common;
using Mara.Lib.Common.IO;
using Mara.Lib.Configs;
using Newtonsoft.Json;

namespace Mara.Lib.Platforms
{
    public class PatchProcess
    {
        public MaraConfig maraConfig  { get; set; }
        public string tempFolder  { get; set; }
        protected string oriFolder;
        protected string outFolder;
        protected string filePath;
        public OWO filePack { get; set; }

        public PatchProcess(string oriFolder, string outFolder, string filePath)
        {
            this.oriFolder = oriFolder;
            this.outFolder = outFolder;
            this.filePath = filePath;
            GenerateTempFolder();
            ExtractPatch();
        }
        
        public PatchProcess(string oriFolder, string outFolder, OWO file)
        {
            this.oriFolder = oriFolder;
            this.outFolder = outFolder;
            filePack = file;
            GenerateTempFolder();
            ExtractPatch();
        }

        public virtual (int, string) ApplyTranslation()
        {
            Directory.Delete(tempFolder, true);
            return (0, string.Empty);
        }


        protected (int, string) ApplyXdelta(string file, string xdelta, string result, string md5)
        {
            if (!File.Exists(file + "_ori"))
                File.Move(file, file + "_ori");

            if (File.Exists(file))
                File.Delete(file);


            file += "_ori";

            if (Md5.CalculateMd5(file) != md5)
                return (1, $"The file \"{file}\" is not equal than the original file.");

            var outdir = Path.GetDirectoryName(result);
            if (!Directory.Exists(outdir))
                Directory.CreateDirectory(outdir);

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

        protected (int, string) CopyFiles(string inPath, string result)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(result)))
                    Directory.CreateDirectory(Path.GetDirectoryName(result));
            }
            catch (Exception ex)
            {
                return (-1, "Error al intentar crear una carpeta.");
            }

            File.Copy(inPath, result);
            
            return (0, string.Empty);
        }

        private void GenerateTempFolder()
        {
            tempFolder = Path.GetTempPath() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Directory.CreateDirectory(tempFolder);
        }

        private void ExtractPatch()
        {
            if (filePath == null && filePack != null)
                Utils.ExtractOWO(filePack, tempFolder);
            else
                Lzma.Unpack(filePath, tempFolder);
            maraConfig = JsonConvert.DeserializeObject<MaraConfig>(File.ReadAllText($"{tempFolder}{Path.DirectorySeparatorChar}data.json").Replace('\\', Path.DirectorySeparatorChar));
        }

        private void DeleteTempFolder()
        {
            Directory.Delete(tempFolder, true);
        }

        public PatchInfo GetInfo()
        {
            return maraConfig.Info;
        }

        public void UpdateFolders(string newOriFolder, string newOutFolder)
        {
            this.oriFolder = newOriFolder;
            this.outFolder = newOutFolder;
        }
    }
}