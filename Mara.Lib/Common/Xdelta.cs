using System.IO;
using PleOps.XdeltaSharp.Decoder;

namespace Mara.Lib.Common
{
    public class Xdelta
    {
        public static void Apply(FileStream file, byte[] xdeltaFile, string outfile)
        {
            var outStream = new FileStream(outfile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            var xdelta = new MemoryStream(xdeltaFile);

            var decoder = new Decoder(file, xdelta, outStream);
            decoder.Run();
            outStream.Close();
            file.Close();
        }

        public static void ApplyFileStream(string file, string xdeltaFile, string outfile)
        {
            using var fileStream = new FileStream(file, FileMode.Open);
            using var xdeltaStream = new FileStream(xdeltaFile, FileMode.Open);
            using var outStream = new FileStream(outfile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

            var decoder = new Decoder(fileStream, xdeltaStream, outStream);
            decoder.Run();
            fileStream.Close();
            xdeltaStream.Close();
            outStream.Close();
        }
    }
}
